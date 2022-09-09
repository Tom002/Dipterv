using Dipterv.Dal.Extensions;
using Dipterv.Shared.Dto;
using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Dto.ProductInventory;
using Dipterv.Shared.Dto.ShoppingCart;
using Dipterv.Shared.Dto.SpecialOffer;

namespace Dipterv.Bll.Helper
{
    public static class ProductHelper
    {
        public static void FillSpecialOfferData(ListProductDto product, List<SpecialOfferDto> specialOffers)
        {
            product.BestAvailableSpecialOffer = SpecialOfferExtensions.GetBestAvailableSpecialOfferDtoForQuantity(quantity: 1).Compile().Invoke(specialOffers);
            if (product.BestAvailableSpecialOffer != null)
            {
                product.OriginalPrice = product.ListPrice;
                product.ListPrice = ((100 - product.BestAvailableSpecialOffer.DiscountPct) / 100) * product.OriginalPrice.Value;
            }
        }

        public static void FillReviewData(ListProductDto product, List<ProductReviewDto> productReviews)
        {
            product.AverageRating = (productReviews.Average(pr => (double?)pr.Rating)) ?? 0;
        }

        public static void FillReviewData(ProductDetailsDto product, List<ProductReviewDto> productReviews)
        {
            product.AverageRating = (productReviews.Any() ? productReviews.Average(pr => pr.Rating) : 0);
            product.Reviews = productReviews;
        }

        public static void FillInventoryData(ListProductDto product, List<ProductInventoryDto> productInventories)
        {
            product.CurrentStock = productInventories.Sum(pi => pi.Quantity);
        }

        public static void FillShoppingCartData(ListProductDto product, List<ShoppingCartItemDto> allShoppingCartItems, int currentStock, string? customerShoppingCartKey = default)
        {
            product.TotalQuantityInActiveShoppingCarts = allShoppingCartItems.Sum(s => s.Quantity);
            product.ReservedQuantityInShoppingCarts = allShoppingCartItems.Sum(s => s.ReservedQuantity);

            if (customerShoppingCartKey != null)
            {
                var customerShoppingCartItems = allShoppingCartItems.Where(sci => sci.ShoppingCartId == customerShoppingCartKey).ToList();
                var productShoppingCartItem = customerShoppingCartItems.FirstOrDefault(s => s.ProductId == product.ProductId);

                if(productShoppingCartItem != null)
                {
                    product.IsInShoppingCart = true;
                    product.QuantityInShoppingCart = productShoppingCartItem.Quantity;
                    product.CustomerShoppingCartItemId = productShoppingCartItem.ShoppingCartItemId;
                    product.CanOrderProduct = (currentStock - product.ReservedQuantityInShoppingCarts) >= productShoppingCartItem.Quantity;
                }
                else
                {
                    product.IsInShoppingCart = false;
                }
            }
        }
    }
}
