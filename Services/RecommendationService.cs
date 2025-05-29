using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using ETicaretSitesi.Models;
using ETicaretSitesi.Models.ML;
using ETicaretSitesi.Data;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesi.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ETicaretDbContext _context;
        private readonly MLContext _mlContext;
        private readonly string _modelPath;
        private ITransformer? _model;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(ETicaretDbContext context, ILogger<RecommendationService> logger)
        {
            _context = context;
            _logger = logger;
            _mlContext = new MLContext(seed: 0);
            _modelPath = Path.Combine(Directory.GetCurrentDirectory(), "recommendation_model.zip");
        }

        public async Task<bool> IsModelTrainedAsync()
        {
            return File.Exists(_modelPath) || _model != null;
        }

        public async Task TrainModelAsync()
        {
            try
            {
                _logger.LogInformation("Model eğitimi başlatılıyor...");

                // Sipariş verilerini al
                var orderData = await GetTrainingDataAsync();
                
                if (!orderData.Any())
                {
                    _logger.LogWarning("Eğitim için yeterli veri bulunamadı.");
                    return;
                }

                _logger.LogInformation($"{orderData.Count} adet eğitim verisi bulundu.");

                var dataView = _mlContext.Data.LoadFromEnumerable(orderData);

                // Veri işleme pipeline'ı
                var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductRecommendationInput.UserId))
                    .Append(_mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductRecommendationInput.ProductId)))
                    .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                        labelColumnName: nameof(ProductRecommendationInput.Rating),
                        matrixColumnIndexColumnName: nameof(ProductRecommendationInput.UserId),
                        matrixRowIndexColumnName: nameof(ProductRecommendationInput.ProductId),
                        numberOfIterations: 20,
                        approximationRank: 100));

                // Model eğitimi
                _logger.LogInformation("Model eğitiliyor...");
                _model = pipeline.Fit(dataView);

                // Modeli kaydet
                _mlContext.Model.Save(_model, dataView.Schema, _modelPath);
                _logger.LogInformation($"Model başarıyla eğitildi ve kaydedildi: {_modelPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Model eğitimi sırasında hata oluştu");
                throw;
            }
        }

        private async Task<List<ProductRecommendationInput>> GetTrainingDataAsync()
        {
            var orders = await _context.Siparisler
                .Include(s => s.SiparisUrunleri)
                .Where(s => s.OdemeDurumu == OdemeDurumu.Odendi)
                .ToListAsync();

            var trainingData = new List<ProductRecommendationInput>();

            foreach (var order in orders)
            {
                foreach (var orderItem in order.SiparisUrunleri)
                {
                    trainingData.Add(new ProductRecommendationInput
                    {
                        UserId = (uint)order.KullaniciId,
                        ProductId = (uint)orderItem.UrunId,
                        Rating = Math.Min(5.0f, orderItem.Adet * 1.0f) // Adeti rating'e çevir (max 5)
                    });
                }
            }

            // Eğer çok az veri varsa, rastgele veri ekle (demo için)
            if (trainingData.Count < 50)
            {
                await AddSyntheticDataAsync(trainingData);
            }

            return trainingData;
        }

        private async Task AddSyntheticDataAsync(List<ProductRecommendationInput> trainingData)
        {
            _logger.LogInformation("Yetersiz veri için sentetik veri ekleniyor...");

            var products = await _context.Urunler.Where(u => u.AktifMi).Take(10).ToListAsync();
            var users = await _context.Kullanicilar.Take(5).ToListAsync();

            var random = new Random();
            
            // Her kullanıcı için rastgele ürün alımları oluştur
            for (int userId = 1; userId <= Math.Max(5, users.Count); userId++)
            {
                var productCount = random.Next(3, 8); // Her kullanıcı 3-7 ürün alsın
                var selectedProducts = products.OrderBy(x => random.Next()).Take(productCount);

                foreach (var product in selectedProducts)
                {
                    trainingData.Add(new ProductRecommendationInput
                    {
                        UserId = (uint)userId,
                        ProductId = (uint)product.Id,
                        Rating = random.Next(1, 6) // 1-5 arası rating
                    });
                }
            }
        }

        public async Task<List<Urun>> GetRecommendationsForUserAsync(int userId, int count = 5)
        {
            try
            {
                await LoadModelAsync();
                
                if (_model == null)
                {
                    // Model yoksa popüler ürünleri döndür
                    return await GetPopularProductsAsync(count);
                }

                var products = await _context.Urunler.Where(u => u.AktifMi).ToListAsync();
                var recommendations = new List<(int ProductId, float Score)>();

                var predictionEngine = _mlContext.Model.CreatePredictionEngine<ProductRecommendationInput, ProductRecommendationOutput>(_model);

                foreach (var product in products)
                {
                    var prediction = predictionEngine.Predict(new ProductRecommendationInput
                    {
                        UserId = (uint)userId,
                        ProductId = (uint)product.Id,
                        Rating = 0
                    });

                    recommendations.Add((product.Id, prediction.Score));
                }

                // En yüksek skorlu ürünleri al
                var topProductIds = recommendations
                    .OrderByDescending(r => r.Score)
                    .Take(count)
                    .Select(r => r.ProductId)
                    .ToList();

                return await _context.Urunler
                    .Where(u => topProductIds.Contains(u.Id) && u.AktifMi)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı önerileri alınırken hata oluştu");
                return await GetPopularProductsAsync(count);
            }
        }

        public async Task<List<Urun>> GetRecommendationsForProductAsync(int productId, int count = 5)
        {
            try
            {
                // Bu ürünü alan kullanıcıları bul
                var usersWhoBoughtThisProduct = await _context.SiparisUrunleri
                    .Include(su => su.Siparis)
                    .Where(su => su.UrunId == productId && su.Siparis.OdemeDurumu == OdemeDurumu.Odendi)
                    .Select(su => su.Siparis.KullaniciId)
                    .Distinct()
                    .ToListAsync();

                if (!usersWhoBoughtThisProduct.Any())
                {
                    return await GetPopularProductsAsync(count);
                }

                // Bu kullanıcıların aldığı diğer ürünleri bul
                var relatedProducts = await _context.SiparisUrunleri
                    .Include(su => su.Siparis)
                    .Include(su => su.Urun)
                    .Where(su => usersWhoBoughtThisProduct.Contains(su.Siparis.KullaniciId) 
                              && su.UrunId != productId 
                              && su.Urun.AktifMi
                              && su.Siparis.OdemeDurumu == OdemeDurumu.Odendi)
                    .GroupBy(su => su.UrunId)
                    .Select(g => new { ProductId = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(count)
                    .ToListAsync();

                var productIds = relatedProducts.Select(rp => rp.ProductId).ToList();
                
                return await _context.Urunler
                    .Where(u => productIds.Contains(u.Id) && u.AktifMi)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün önerileri alınırken hata oluştu");
                return await GetPopularProductsAsync(count);
            }
        }

        private async Task<List<Urun>> GetPopularProductsAsync(int count)
        {
            // En çok satan ürünleri döndür
            var popularProducts = await _context.SiparisUrunleri
                .Include(su => su.Urun)
                .Where(su => su.Urun.AktifMi)
                .GroupBy(su => su.UrunId)
                .Select(g => new { ProductId = g.Key, TotalSold = g.Sum(x => x.Adet) })
                .OrderByDescending(x => x.TotalSold)
                .Take(count)
                .ToListAsync();

            var productIds = popularProducts.Select(p => p.ProductId).ToList();

            if (!productIds.Any())
            {
                // Hiç satış yoksa rastgele aktif ürünleri döndür
                return await _context.Urunler
                    .Where(u => u.AktifMi)
                    .OrderBy(u => Guid.NewGuid())
                    .Take(count)
                    .ToListAsync();
            }

            return await _context.Urunler
                .Where(u => productIds.Contains(u.Id) && u.AktifMi)
                .ToListAsync();
        }

        private async Task LoadModelAsync()
        {
            if (_model == null && File.Exists(_modelPath))
            {
                try
                {
                    _model = _mlContext.Model.Load(_modelPath, out var schema);
                    _logger.LogInformation("Model başarıyla yüklendi.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Model yüklenirken hata oluştu");
                    // Model dosyası bozuksa sil
                    if (File.Exists(_modelPath))
                    {
                        File.Delete(_modelPath);
                    }
                }
            }
        }
    }
} 