using Dipterv.Shared.Automapper;
using Dipterv.Shared.Dto.SpecialOffer;
using System;

namespace Dipterv.Shared.Dto
{
    public partial class ProductDto
    {
        // Base data
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public decimal ListPrice { get; set; }
        public string Size { get; set; }
        public DateTime SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public int? ProductSubcategoryId { get; set; }
        public int? ProductCategoryId { get; set; }
        public bool CanBuyProduct => SellStartDate <= DateTime.Now.Date && (!SellEndDate.HasValue || SellEndDate.Value > DateTime.Now.Date);

        // Review
        public List<int> ProductReviewIds { get; set; } = new();
        
        // Images
        public List<int> ProductPhotoIds { get; set; } = new();
        public int? PrimaryProductPhotoId { get; set; } = default;

        // SpecialOffer
        public List<int> SpecialOfferIds { get; set; } = new();

        // Shopping cart
        public List<int> ShoppingCartItemIds { get; set; } = new();
    }
}
