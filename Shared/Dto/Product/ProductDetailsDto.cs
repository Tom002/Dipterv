using Dipterv.Shared.Dto.ProductInventory;
using Dipterv.Shared.Dto.SpecialOffer;
using System.Collections.Generic;

namespace Dipterv.Shared.Dto
{
    public class ProductDetailsDto : ProductWithReviewsDto
    {
        public SpecialOfferDto SelectedSpecialOffer { get; set; } = null;
        public int CurrentStock { get; set; } = 0;
        public bool CanSendOrder { get; set; } = false;
        public decimal OrderPrice { get; set; } = 0;
        public bool CanWriteReview { get; set; } = false;
        public bool CanDeleteReview { get; set; } = false;
    }
}
