using Dipterv.Dal.Model;
using Dipterv.Shared.Dto.SpecialOffer;
using System.Linq.Expressions;

namespace Dipterv.Dal.Extensions
{
    public static class SpecialOfferExtensions
    {
        public static Expression<Func<IEnumerable<SpecialOffer>, SpecialOffer?>> GetBestAvailableSpecialOfferWithoutQuantityRequirement
            => src => src
             .Where(s => s.StartDate.Date <= DateTime.Now.Date)
             .Where(s => s.EndDate.Date >= DateTime.Now.Date)
             .Where(s => s.MinQty <= 1)
             .OrderByDescending(s => s.DiscountPct)
             .FirstOrDefault();


        public static Func<SpecialOffer, bool> IsSpecialOfferValidFunc()
            => IsSpecialOfferValid().Compile();

        public static Expression<Func<SpecialOffer, bool>> IsSpecialOfferValid()
            => src => src.StartDate.Date <= DateTime.Now.Date &&
                      src.EndDate.Date >= DateTime.Now.Date;

        public static Func<SpecialOffer, bool> IsSpecialOfferValidForQuantityFunc(int quantity)
            => IsSpecialOfferValidForQuantity(quantity).Compile();

        public static Expression<Func<SpecialOffer, bool>> IsSpecialOfferValidForQuantity(int quantity)
            => IsSpecialOfferValid().And(src => src.MinQty <= quantity && (!src.MaxQty.HasValue || src.MaxQty.Value >= quantity));

        public static Expression<Func<IEnumerable<SpecialOffer>, SpecialOffer?>> GetBestAvailableSpecialOfferForQuantity(int quantity)
            => src => src
             .Where(s => s.StartDate.Date <= DateTime.Now.Date)
             .Where(s => s.EndDate.Date >= DateTime.Now.Date)
             .Where(s => s.MinQty <= quantity)
             .Where(s => s.MinQty <= quantity)
             .OrderByDescending(s => s.DiscountPct)
             .FirstOrDefault();

        public static Expression<Func<IEnumerable<SpecialOfferDto>, SpecialOfferDto?>> GetBestAvailableSpecialOfferDtoForQuantity(int quantity)
            => src => src
             .Where(s => s.StartDate.Date <= DateTime.Now.Date)
             .Where(s => s.EndDate.Date >= DateTime.Now.Date)
             .Where(s => s.MinQty <= quantity)
             .Where(s => s.MinQty <= quantity)
             .OrderByDescending(s => s.DiscountPct)
             .FirstOrDefault();
    }
}
