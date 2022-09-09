using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Dto.ProductPhoto;
using Dipterv.Shared.Dto.SpecialOffer;

namespace Dipterv.Shared.Dto
{
    public class ProductDetailsDto : ListProductDto
    {
        // Reviews
        public List<ProductReviewDto> Reviews { get; set; } = new();
        public ProductReviewDto? UserReview { get; set; } = default;
        public bool CanDeleteReview { get; set; } = false;
        public bool CanWriteReview { get; set; } = false;

        // SpecialOffer
        public List<SpecialOfferDto> AvailableSpecialOffers { get; set; } = new();

        // ShoppingCart
        public decimal ItemTotal { get; set; } = 0;

        // Images
        public byte[] LargePhoto { get; set; } = new byte[0];
        public List<ProductPhotoDto> Photos { get; set; } = new();
    }
}
