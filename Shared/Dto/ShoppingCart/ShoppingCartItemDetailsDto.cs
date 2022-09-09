using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Dto.SpecialOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Dto.ShoppingCart
{
    public class ShoppingCartItemDetailsDto : ShoppingCartItemDto
    {
        public List<SpecialOfferDto> AvailableSpecialOffers { get; set; } = new();

        public ListProductDto Product { get; set; } = new();
    }
}
