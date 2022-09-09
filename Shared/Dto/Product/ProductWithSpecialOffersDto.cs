using Dipterv.Shared.Dto.SpecialOffer;

namespace Dipterv.Shared.Dto.Product
{
    public class ProductWithSpecialOffersDto : ProductDto
    {
        public List<SpecialOfferDto> SpecialOffers { get; set; } = new();
    }
}
