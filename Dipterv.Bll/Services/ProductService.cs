using AutoMapper;
using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.EntityFrameworkCore;
using Stl.Async;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.EntityFramework;
using System.Reactive;

namespace Dipterv.Bll.Services
{
    public class ProductService : DbServiceBase<FusionDbContext>, IProductService
    {
        private readonly IMapper _mapper;
        private readonly IDbEntityResolver<int, Product> _productResolver;
        private readonly IProductReviewService _productReviewService;
        private readonly IDateService _dateService;

        public ProductService(
            IServiceProvider services,
            IMapper mapper,
            IDbEntityResolver<int, Product> productResolver,
            IProductReviewService productReviewService,
            IDateService dateService)
            : base(services)
        {
            _mapper = mapper;
            _productResolver = productResolver;
            _productReviewService = productReviewService;
            _dateService = dateService;
        }

        [ComputeMethod]
        public virtual async Task<ProductDto?> TryGet(int productId, CancellationToken cancellationToken)
        {
            await using var dbContext = CreateDbContext();
            var currentDate = await _dateService.GetCurrentDate();

            var product = await _productResolver.Get(productId, cancellationToken);
            if (product is Product)
            {
                var productDto = _mapper.Map<ProductDto>(product);
                productDto.CanBuyProduct = product.SellStartDate <= currentDate && (!product.SellEndDate.HasValue || product.SellEndDate.Value > currentDate);
                return productDto;
            }
            return default;
        }

        [ComputeMethod]
        public virtual async Task<ProductWithReviewsDto?> TryGetWithReviews(int productId, CancellationToken cancellationToken)
        {
            ProductDto product = await TryGet(productId, cancellationToken);
            if (product is ProductDto)
            {
                var productReviewIds = await _productReviewService.GetReviewIdsForProduct(productId, cancellationToken);

                var reviews = await Task.WhenAll(productReviewIds.Select(async reviewId =>
                {
                    return await _productReviewService.TryGet(reviewId, cancellationToken);
                }));

                var productWithReviews = _mapper.Map<ProductDto, ProductWithReviewsDto>(product);
                productWithReviews.Reviews = reviews.ToList();

                return productWithReviews;
            }
            return default;
        }

        [ComputeMethod]
        public virtual async Task<List<ProductDto>> GetAll(string searchTerm, CancellationToken cancellationToken)
        {
            await using var dbContext = CreateDbContext();

            var productIdList = await GetProductIdListForSearchTerm(searchTerm, cancellationToken);

            var products = await Task.WhenAll(productIdList.Select(async productId =>
            {
                return await TryGet(productId, cancellationToken);
            }));

            return products.ToList();
        }

        public virtual async Task<List<int>> GetProductIdListForSearchTerm(string searchTerm, CancellationToken cancellationToken)
        {
            await PseudoGetAllAnySearchTerm();

            await using var dbContext = CreateDbContext();
            var productIdListQuery = dbContext.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                productIdListQuery = productIdListQuery.Where(p => p.Name.Contains(searchTerm));
            }
            return await productIdListQuery.Select(p => p.ProductId).ToListAsync();
        }

        [CommandHandler]
        public virtual async Task Edit(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            if (Computed.IsInvalidating())
            {
                _ = TryGet(command.ProductId, cancellationToken);
                return;
            }

            using var dbContext = CreateDbContext(readWrite: true);
            var product = await dbContext.Products.AsQueryable().SingleOrDefaultAsync(p => p.ProductId == command.ProductId, cancellationToken);
            if (product is Product)
            {
                _mapper.Map(command, product);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        [CommandHandler]
        public virtual async Task Add(AddProductCommand command, CancellationToken cancellationToken)
        {
            if (Computed.IsInvalidating())
            {
                _ = PseudoGetAllAnySearchTerm();
                return;
            }

            using var dbContext = CreateDbContext(readWrite: true);
            var product = _mapper.Map<Product>(command);
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        [CommandHandler]
        public virtual async Task Delete(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            if (Computed.IsInvalidating())
            {
                _ = TryGet(command.ProductId, cancellationToken);
                _ = PseudoGetAllAnySearchTerm();

                // TODO: Invalidate removed reviews

                return;
            }

            using var dbContext = CreateDbContext(readWrite: true);
            var product = await dbContext.Products.AsQueryable()
                .Include(p => p.ProductReviews)
                .Include(p => p.ProductCostHistories)
                .Include(p => p.SpecialOfferProducts).ThenInclude(s => s.SalesOrderDetails)
                .SingleAsync(p => p.ProductId == command.ProductId, cancellationToken);

            dbContext.SalesOrderDetails.RemoveRange(product.SpecialOfferProducts.SelectMany(s => s.SalesOrderDetails));
            dbContext.SpecialOfferProducts.RemoveRange(product.SpecialOfferProducts);
            dbContext.ProductReviews.RemoveRange(product.ProductReviews);
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }

        [ComputeMethod]
        protected virtual Task<Unit> PseudoGetAllAnySearchTerm() => TaskExt.UnitTask;
    }
}
