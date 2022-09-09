using Dipterv.Shared.Dto.SpecialOffer;

namespace Dipterv.Shared.Dto.Product
{
    public class ListProductDto : ProductDto
    {
        // Reviews
        public int NumberOfRatings => ProductReviewIds.Where(pri => pri != null).Count();
        public double? AverageRating { get; set; }

        // Inventory
        public int CurrentStock { get; set; } = 0;

        // Photo
        public byte[]? ThumbnailImage { get; set; } = default;

        // Shopping cart
        public bool CanOrderProduct { get; set; } = false;
        public bool IsInShoppingCart { get; set; } = false;
        public int? CustomerShoppingCartItemId { get; set; } = default;
        public int? QuantityInShoppingCart { get; set; } = default;
        public int TotalQuantityInActiveShoppingCarts { get; set; } = 0;
        public int ReservedQuantityInShoppingCarts { get; set; } = 0;

        // SpecialOffer
        public decimal? OriginalPrice { get; set; }
        public SpecialOfferDto? BestAvailableSpecialOffer { get; set; }
    }
}
