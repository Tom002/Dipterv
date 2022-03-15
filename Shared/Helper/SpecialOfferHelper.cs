using Dipterv.Shared.Dto.SpecialOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Helper
{
    public static class SpecialOfferHelper
    {
        public static SpecialOfferDto? GetBestAvalaibleOffer(List<SpecialOfferDto> allOffers, int quantity)
        {
            return allOffers
                .Where(o => o.MinQty <= quantity)
                .Where(o => !o.MaxQty.HasValue || o.MaxQty.Value >= quantity)
                .OrderByDescending(o => o.DiscountPct)
                .FirstOrDefault();
        }
    }
}
