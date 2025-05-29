using ETicaretSitesi.Models;

namespace ETicaretSitesi.Services
{
    public interface IRecommendationService
    {
        Task<List<Urun>> GetRecommendationsForUserAsync(int userId, int count = 5);
        Task<List<Urun>> GetRecommendationsForProductAsync(int productId, int count = 5);
        Task TrainModelAsync();
        Task<bool> IsModelTrainedAsync();
    }
} 