using Microsoft.ML.Data;

namespace ETicaretSitesi.Models.ML
{
    public class ProductRecommendationInput
    {
        [KeyType(count: 10000)]
        public uint UserId { get; set; }

        [KeyType(count: 10000)]
        public uint ProductId { get; set; }

        public float Rating { get; set; }
    }
} 