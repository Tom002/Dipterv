using Dipterv.Dal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Dal.Extensions
{
    public static class ProductExtensions
    {
        public static Expression<Func<Product, double>> GetAverageReviewExpression()
             => src => (src.ProductReviews.Average(pr => (double?)pr.Rating)) ?? 0;

        public static Expression<Func<Product, int>> GetReviewCountExpression()
             => src => src.ProductReviews.Count();

        public static Expression<Func<Product, int>> GetCurrentStockExpression()
             => src => (src.ProductInventories.Sum(pi => pi.Quantity));

        public static Expression<Func<Product, IEnumerable<SpecialOffer>>> MapProductToSpecialOffers => src => src.SpecialOfferProducts.Select(sp => sp.SpecialOffer);

        public static Expression<Func<Product, decimal>> BestAvailableListPriceExpression()
             => src => src.SpecialOfferProducts.Select(sop => sop.SpecialOffer).FirstOrDefault(SpecialOfferExtensions.IsSpecialOfferValidFunc()) != null
                 ? (src.SpecialOfferProducts.Select(sop => sop.SpecialOffer)
                    .Where(SpecialOfferExtensions.IsSpecialOfferValidFunc())
                    .OrderByDescending(so => so.DiscountPct)
                    .First().DiscountPct / 100) * src.ListPrice
                 : src.ListPrice;

        public static Expression<Func<Product, bool>> CanBuyProductExpression()
             => src => src.SellStartDate.Date <= DateTime.Now.Date &&
                       (!src.SellEndDate.HasValue || src.SellEndDate.Value.Date >= DateTime.Now.Date);

        public static Func<Product, bool> CanBuyProductFunc()
             => CanBuyProductExpression().Compile();
    }
}
