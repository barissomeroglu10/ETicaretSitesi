using Microsoft.AspNetCore.Mvc;
using ETicaretSitesi.Services;
using System.Security.Claims;

namespace ETicaretSitesi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<RecommendationController> _logger;

        public RecommendationController(IRecommendationService recommendationService, ILogger<RecommendationController> logger)
        {
            _recommendationService = recommendationService;
            _logger = logger;
        }

        [HttpGet("for-user/{userId}")]
        public async Task<IActionResult> GetRecommendationsForUser(int userId, [FromQuery] int count = 5)
        {
            try
            {
                var recommendations = await _recommendationService.GetRecommendationsForUserAsync(userId, count);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı önerileri alınırken hata oluştu");
                return StatusCode(500, "Öneriler alınırken hata oluştu");
            }
        }

        [HttpGet("for-product/{productId}")]
        public async Task<IActionResult> GetRecommendationsForProduct(int productId, [FromQuery] int count = 5)
        {
            try
            {
                var recommendations = await _recommendationService.GetRecommendationsForProductAsync(productId, count);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün önerileri alınırken hata oluştu");
                return StatusCode(500, "Öneriler alınırken hata oluştu");
            }
        }

        [HttpPost("train")]
        public async Task<IActionResult> TrainModel()
        {
            try
            {
                await _recommendationService.TrainModelAsync();
                return Ok(new { message = "Model başarıyla eğitildi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Model eğitimi sırasında hata oluştu");
                return StatusCode(500, "Model eğitimi sırasında hata oluştu");
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetModelStatus()
        {
            try
            {
                var isTrained = await _recommendationService.IsModelTrainedAsync();
                return Ok(new { isTrained = isTrained });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Model durumu kontrol edilirken hata oluştu");
                return StatusCode(500, "Model durumu kontrol edilirken hata oluştu");
            }
        }

        [HttpGet("my-recommendations")]
        public async Task<IActionResult> GetMyRecommendations([FromQuery] int count = 5)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("Giriş yapmanız gerekiyor");
                }

                var recommendations = await _recommendationService.GetRecommendationsForUserAsync(userId, count);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kişisel öneriler alınırken hata oluştu");
                return StatusCode(500, "Öneriler alınırken hata oluştu");
            }
        }
    }
} 