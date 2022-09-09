using AutoMapper;
using Dipterv.Bll.Helper;
using Dipterv.Shared.Dto;
using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Helper;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.ComputeServices;
using Dipterv.Shared.Paging;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.Authentication;

namespace Dipterv.Bll.Services
{
    public class ProductDetailsService : IProductDetailsService
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductReviewService _productReviewService;
        private readonly ISpecialOfferService _specialOfferService;
        private readonly IProductInventoryService _productInventoryService;
        private readonly IAuth _authService;
        private readonly IMapper _mapper;

        public ProductDetailsService(
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IProductReviewService productReviewService,
            ISpecialOfferService specialOfferService,
            IProductInventoryService productInventoryService,
            IAuth authService,
            IMapper mapper)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _productReviewService = productReviewService;
            _specialOfferService = specialOfferService;
            _productInventoryService = productInventoryService;
            _authService = authService;
            _mapper = mapper;
        }



        [ComputeMethod]
        public virtual async Task<ProductDetailsDto?> GetProductDetails(int productId)
        {
            var averageRating = await GetAverageRating(productId);
            var totalInventory = await GetTotalInventoryForProduct(productId);
            // ...
            return new ProductDetailsDto();
        }

        [ComputeMethod]
        public virtual async Task<float> GetProductBaseData(int productId)
        {
            throw new NotImplementedException();
        }


        [ComputeMethod]
        public virtual async Task<float> GetAverageRating(int productId)
        {
            throw new NotImplementedException();
        }

        [ComputeMethod]
        public virtual async Task<int> GetTotalInventoryForProduct(int productId)
        {
            throw new NotImplementedException();
        }

        [ComputeMethod]
        public virtual async Task<int> GetProductInventoryAtLocation(int productId, int locationId)
        {
            throw new NotImplementedException();
        }

        [CommandHandler]
        public virtual async Task UpdateProductInventory(int productId, int locationId, int stock)
        {
            // ...
            // Ezen a blokkon belül hívott fv-ek invalidálásra kerülnek
            if (Computed.IsInvalidating())
            {
                _ = GetProductInventoryAtLocation(productId, locationId);
            }
            // ...
        }



        [ComputeMethod]
        public virtual async Task<ProductDetailsDto?> TryGetWithDetails(Session session, int productId, CancellationToken cancellationToken)
        {
            ProductDto? product = await _productService.TryGetProduct(productId, cancellationToken);
            if (product is ProductDto)
            {
                var productDetails = _mapper.Map<ProductDto, ProductDetailsDto>(product);

                var reviews = await _productReviewService.TryGetMany(product.ProductReviewIds.ToList(), cancellationToken);
                var specialOffers = await _specialOfferService.TryGetMany(product.SpecialOfferIds.ToList(), cancellationToken);
                var inventories = await _productInventoryService.GetInventoriesForProduct(product.ProductId, cancellationToken);
                var productImages = await _productService.TryGetManyProductPhotos(product.ProductPhotoIds.ToList(), cancellationToken);
                var shoppingCartItems = await _shoppingCartService.GetManyCartItems(product.ShoppingCartItemIds.ToList(), cancellationToken);

                string shoppingCartKey = null;
                var user = await _authService.GetUser(session, cancellationToken);
                if (user.IsAuthenticated && user.Claims.ContainsKey("customer_id"))
                {
                    var customerId = int.Parse(user.Claims["customer_id"]);
                    shoppingCartKey = ShoppingCartHelper.GetCustomerShoppingCartKey(customerId);

                    ProductHelper.FillReviewData(productDetails, reviews);
                    if (reviews.Any())
                    {
                        productDetails.UserReview = reviews.FirstOrDefault(r => r.ReviewerName == user.Name);
                    }
                }
                else
                {
                    shoppingCartKey = ShoppingCartHelper.GetGuestShoppingCartKey(session.Id);
                }

                
                productDetails.CanWriteReview = user.IsAuthenticated && productDetails.UserReview == null;

                ProductHelper.FillSpecialOfferData(productDetails, specialOffers);
                ProductHelper.FillInventoryData(productDetails, inventories);
                ProductHelper.FillShoppingCartData(productDetails, shoppingCartItems, productDetails.CurrentStock, shoppingCartKey);

                if(productDetails.IsInShoppingCart && productDetails.QuantityInShoppingCart.HasValue)
                {
                    productDetails.ItemTotal = productDetails.ListPrice * productDetails.QuantityInShoppingCart.Value;
                }

                if (productImages.Any() && product.PrimaryProductPhotoId.HasValue)
                {
                    var correspondingProductImage = productImages.First(p => p.ProductPhotoId == product.PrimaryProductPhotoId.Value);
                    productDetails.ThumbnailImage = correspondingProductImage.ThumbNailPhoto;
                    productDetails.LargePhoto = correspondingProductImage.LargePhoto;

                    productDetails.Photos = productImages;
                }
                return productDetails;
            }
            return default;
        }


    }
}
