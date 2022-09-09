using AutoMapper;
using Dipterv.Bll.Extensions;
using Dipterv.Bll.Helper;
using Dipterv.Dal.DbContext;
using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Helper;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.ComputeServices;
using Dipterv.Shared.Paging;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.EntityFramework;

namespace Dipterv.Bll.Services
{
    public class ProductSearchService : DbServiceBase<FusionDbContext>, IProductSearchService
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly IProductReviewService _productReviewService;
        private readonly IProductInventoryService _productInventoryService;
        private readonly ISpecialOfferService _specialOfferService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IDateService _dateService;
        private readonly IAuth _authService;

        public ProductSearchService(
            IServiceProvider services,
            IMapper mapper,
            IProductService productService,
            IProductReviewService productReviewService,
            IProductInventoryService productInventoryService,
            ISpecialOfferService specialOfferService,
            IShoppingCartService shoppingCartService,
            IDateService dateService,
            IAuth authService)
            : base(services)
        {
            _mapper = mapper;
            _productReviewService = productReviewService;
            _productService = productService;
            _productInventoryService = productInventoryService;
            _specialOfferService = specialOfferService;
            _shoppingCartService = shoppingCartService;
            _dateService = dateService;
            _authService = authService;
        }


        [ComputeMethod]
        public virtual async Task<PageResponse<ListProductDto>> Search(ProductSearchDto searchDto, CancellationToken cancellationToken)
        {
            var result = new List<ListProductDto>();

            string shoppingCartKey = null;
            var user = await _authService.GetUser(searchDto.Session, cancellationToken);
            if (user != null && user.IsAuthenticated && user.Claims.ContainsKey("customer_id"))
            {
                var customerId = int.Parse(user.Claims["customer_id"]);
                shoppingCartKey = ShoppingCartHelper.GetCustomerShoppingCartKey(customerId);
            }
            else
            {
                shoppingCartKey = ShoppingCartHelper.GetGuestShoppingCartKey(searchDto.Session.Id);
            }

            var productIdList = await GetProductIdListForFilter(searchDto.Filter, searchDto.PageRequest, cancellationToken);
            var products = await _productService.TryGetManyProducts(productIdList.Results, cancellationToken);

            var reviews = await _productReviewService.TryGetMany(products.SelectMany(p => p.ProductReviewIds).ToList(), cancellationToken);
            var specialOffers = await _specialOfferService.TryGetMany(products.SelectMany(p => p.SpecialOfferIds).ToList(), cancellationToken);
            var inventories = await _productInventoryService.GetInventoriesForProductIdList(products.Select(p => p.ProductId).ToList(), cancellationToken);
            var shoppingCartItems = await _shoppingCartService.GetManyCartItems(products.SelectMany(p => p.ShoppingCartItemIds).ToList(), cancellationToken);

            var primaryImageIds = products.Where(p => p.PrimaryProductPhotoId.HasValue).ToDictionary(p => p.ProductId, p => p.PrimaryProductPhotoId.Value);
            var primaryProductImages = await _productService.TryGetManyProductPhotos(primaryImageIds.Values.ToList(), cancellationToken);

            foreach (var product in products)
            {
                var productDto = _mapper.Map<ListProductDto>(product);

                var correspondingReviews = reviews.Where(r => r.ProductId == productDto.ProductId).ToList();
                ProductHelper.FillReviewData(productDto, correspondingReviews);

                var correspondingSpecialOffers = specialOffers.Where(r => r.ValidForProductIds.Contains(productDto.ProductId)).ToList();
                ProductHelper.FillSpecialOfferData(productDto, correspondingSpecialOffers);

                var correspondingInventories = inventories.Where(r => r.ProductId == productDto.ProductId).ToList();
                ProductHelper.FillInventoryData(productDto, correspondingInventories);

                var correspondingShoppingCartItems = shoppingCartItems.Where(r => r.ProductId == product.ProductId).ToList();
                ProductHelper.FillShoppingCartData(productDto, correspondingShoppingCartItems, productDto.CurrentStock, shoppingCartKey);

                if (primaryImageIds.TryGetValue(product.ProductId, out int primaryImageId))
                {
                    var correspondingProductImage = primaryProductImages.First(p => p.ProductPhotoId == primaryImageId);
                    productDto.ThumbnailImage = correspondingProductImage.ThumbNailPhoto;
                }

                result.Add(productDto);
            }

            return new PageResponse<ListProductDto>(result, productIdList.CurrentPage, productIdList.TotalCount);
        }

        [ComputeMethod]
        public virtual async Task<PageResponse<int>> GetProductIdListForFilter(FilterProductDto filter, ProductPageRequest pageRequest, CancellationToken cancellationToken)
        {
            // await PseudoGetAllAnySearchTerm();

            await using var dbContext = CreateDbContext();
            var productIdListQuery = dbContext.Products.AsQueryable();

            productIdListQuery = productIdListQuery
               .Where(!string.IsNullOrEmpty(filter.ProductName), p => p.Name.Contains(filter.ProductName))
               .Where(filter.OnlyShowDiscountedProducts, p => p.SpecialOfferProducts.Select(sop => sop.SpecialOffer).FirstOrDefault(so => so.StartDate.Date <= DateTime.Now.Date && so.EndDate.Date >= DateTime.Now.Date && so.MinQty <= 1) != null)
               .Where(filter.ProductCategoryId.HasValue, p => p.ProductSubcategory.ProductCategoryId == filter.ProductCategoryId.Value)
               .Where(filter.ProductSubcategoryId.HasValue, p => p.ProductSubcategory.ProductSubcategoryId == filter.ProductSubcategoryId.Value)
               .Where(filter.MinRating > 0, p => (p.ProductReviews.Average(pr => (double?)pr.Rating) ?? 0) > filter.MinRating)
               .Where(filter.MinPrice.HasValue, p =>
                    // Elérhető-e akció
                    p.SpecialOfferProducts.Select(sop => sop.SpecialOffer).FirstOrDefault(so => so.StartDate.Date <= DateTime.Now.Date && so.EndDate.Date >= DateTime.Now.Date && so.MinQty <= 1) != null
                    // Ha igen akkor az akciós árat használjuk
                    ? (((p.SpecialOfferProducts
                            .Select(sop => sop.SpecialOffer)
                            .Where(so => so.StartDate.Date <= DateTime.Now.Date && so.EndDate.Date >= DateTime.Now.Date && so.MinQty <= 1)
                            .OrderByDescending(so => so.DiscountPct)
                            .First().DiscountPct / 100) * p.ListPrice) >= filter.MinPrice.Value)
                    : p.ListPrice >= filter.MinPrice.Value)
                .Where(filter.MaxPrice.HasValue, p =>
                    // Elérhető-e akció
                    p.SpecialOfferProducts.Select(sop => sop.SpecialOffer).FirstOrDefault(so => so.StartDate.Date <= DateTime.Now.Date && so.EndDate.Date >= DateTime.Now.Date && so.MinQty <= 1) != null
                    // Ha igen akkor az akciós árat használjuk
                    ? (((p.SpecialOfferProducts
                            .Select(sop => sop.SpecialOffer)
                            .Where(so => so.StartDate.Date <= DateTime.Now.Date && so.EndDate.Date >= DateTime.Now.Date && so.MinQty <= 1)
                            .OrderByDescending(so => so.DiscountPct)
                            .First().DiscountPct / 100) * p.ListPrice) <= filter.MaxPrice.Value)
                    : p.ListPrice <= filter.MaxPrice.Value)
               .Where(filter.OnlyShowProductsInStock, p => (p.ProductInventories.Sum(pi => (double?)pi.Quantity) ?? 0) > 0)
               .Where(filter.OnlyShowProductsAvalaible, p => p.SellStartDate.Date <= DateTime.Now.Date && (!p.SellEndDate.HasValue || p.SellEndDate.Value.Date >= DateTime.Now.Date));

            switch (pageRequest.Order)
            {
                case ProductOrderByEnum.AverageReviews:
                    productIdListQuery = productIdListQuery.OrderByDescending(p => (p.ProductReviews.Average(pr => (double?)pr.Rating) ?? 0));
                    break;
                case ProductOrderByEnum.PriceHighToLow:
                    productIdListQuery = productIdListQuery.OrderByDescending(p =>
                        // Elérhető-e akció
                        p.SpecialOfferProducts.Select(sop => sop.SpecialOffer).FirstOrDefault(so => so.StartDate.Date <= DateTime.Now.Date && so.EndDate.Date >= DateTime.Now.Date && so.MinQty <= 1) != null
                        // Ha igen akkor az akciós árat használjuk
                        ? ((p.SpecialOfferProducts
                                .Select(sop => sop.SpecialOffer)
                                .Where(so => so.StartDate.Date <= DateTime.Now.Date && so.EndDate.Date >= DateTime.Now.Date && so.MinQty <= 1)
                                .OrderByDescending(so => so.DiscountPct)
                                .First().DiscountPct / 100) * p.ListPrice)
                        : p.ListPrice);
                    break;
                case ProductOrderByEnum.PriceLowToHigh:
                    productIdListQuery = productIdListQuery.OrderBy(p =>
                        // Elérhető-e akció
                        p.SpecialOfferProducts.Select(sop => sop.SpecialOffer).FirstOrDefault(so => so.StartDate.Date <= DateTime.Now.Date && so.EndDate.Date >= DateTime.Now.Date && so.MinQty <= 1) != null
                        // Ha igen akkor az akciós árat használjuk
                        ? ((p.SpecialOfferProducts
                                .Select(sop => sop.SpecialOffer)
                                .Where(so => so.StartDate.Date <= DateTime.Now.Date && so.EndDate.Date >= DateTime.Now.Date && so.MinQty <= 1)
                                .OrderByDescending(so => so.DiscountPct)
                                .First().DiscountPct / 100) * p.ListPrice)
                        : p.ListPrice);
                    break;
                case ProductOrderByEnum.NewestArrivals:
                    productIdListQuery = productIdListQuery.OrderByDescending(p => p.SellStartDate);
                    break;
            }

            return await productIdListQuery.Select(p => p.ProductId).ToPagedListAsync(pageRequest);
        }
    }
}
