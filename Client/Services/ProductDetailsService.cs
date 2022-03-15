using Dipterv.Client.Interfaces;
using Dipterv.Shared.Dto;
using Dipterv.Shared.Dto.SpecialOffer;
using Dipterv.Shared.Helper;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.ComputeServices;
using Stl.Fusion;
using Stl.Fusion.Authentication;

namespace Dipterv.Client.Services
{
    public class ProductDetailsService : IProductDetailsService
    {
        private readonly IProductService _productService;
        private readonly IProductReviewService _productReviewService;
        private readonly IProductInventoryService _productInventoryService;
        private readonly ISpecialOfferService _specialOfferService;
        private readonly IAuth _authService;

        public ProductDetailsService(
            IProductService productService,
            IProductReviewService productReviewService,
            IProductInventoryService productInventoryService,
            ISpecialOfferService specialOfferService,
            IAuth authService)
        {
            _productService = productService;
            _productReviewService = productReviewService;
            _productInventoryService = productInventoryService;
            _specialOfferService = specialOfferService;
            _authService = authService;
        }

        [ComputeMethod]
        public virtual async Task<ProductDetailsDto> GetProductDetails(Session session, int productId, int orderQuantity, CancellationToken cancellationToken = default)
        {
            var product = await _productService.TryGetWithReviews(productId, cancellationToken);
            var totalStock = await _productInventoryService.ProductGetTotalStock(productId, cancellationToken);
            var specialOffers = await _specialOfferService.GetActiveSpecialOffersForProduct(productId, cancellationToken);
            var selectedSpecialOffer = SpecialOfferHelper.GetBestAvalaibleOffer(specialOffers, orderQuantity);
            var user = await _authService.GetUser(session);

            var orderprice = orderQuantity * product.ListPrice;
            if (selectedSpecialOffer is SpecialOfferDto)
            {
                orderprice = orderprice * ((100 - selectedSpecialOffer.DiscountPct) / 100);
            }

            return new ProductDetailsDto
            {
                ListPrice = product.ListPrice,
                Name = product.Name,
                ProductId = product.ProductId,
                ProductNumber = product.ProductNumber,
                Reviews = product.Reviews,
                Size = product.Size,
                StandardCost = product.StandardCost,
                SellStartDate = product.SellStartDate,
                SellEndDate = product.SellEndDate,
                CurrentStock = totalStock,
                CanBuyProduct = product.CanBuyProduct,
                CanSendOrder = product.CanBuyProduct && orderQuantity <= totalStock,
                SelectedSpecialOffer = selectedSpecialOffer,
                OrderPrice = orderprice,

                CanDeleteReview = user.Claims.ContainsKey("comment_delete"),
                CanWriteReview = user.IsAuthenticated
            };
        }
    }
}
