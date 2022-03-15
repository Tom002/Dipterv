using AutoMapper;
using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto;
using Dipterv.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.EntityFramework;

namespace Dipterv.Bll.Services
{
    public class ProductReviewService : DbServiceBase<FusionDbContext>, IProductReviewService
    {
        private readonly IMapper _mapper;
        private readonly IDbEntityResolver<int, ProductReview> _productReviewResolver;
        private readonly IAuth _authService;

        public ProductReviewService(
            IServiceProvider services,
            IMapper mapper,
            IDbEntityResolver<int, ProductReview> productReviewResolver,
            IAuth authService)
            : base(services)
        {
            _mapper = mapper;
            _productReviewResolver = productReviewResolver;
            _authService = authService;
        }

        [ComputeMethod]
        public virtual async Task<ProductReviewDto?> TryGet(int productReviewId, CancellationToken cancellationToken)
        {
            await using var dbContext = CreateDbContext();
            var product = await _productReviewResolver.Get(productReviewId, cancellationToken);
            if (product is ProductReview)
                return _mapper.Map<ProductReviewDto>(product);
            return default;
        }

        [ComputeMethod]
        public virtual async Task<List<int>> GetReviewIdsForProduct(int productId, CancellationToken cancellationToken)
        {
            await using var dbContext = CreateDbContext();
            return await dbContext.ProductReviews.AsQueryable()
                .Where(r => r.ProductId == productId)
                .Select(r => r.ProductReviewId)
                .ToListAsync();
        }

        [CommandHandler]
        public virtual async Task Edit(UpdateProductReviewCommand command, CancellationToken cancellationToken)
        {
            if (Computed.IsInvalidating())
            {
                _ = TryGet(command.ProductReviewId, cancellationToken);
                return;
            }

            using var dbContext = CreateDbContext(readWrite: true);
            var productReview = await dbContext.ProductReviews.AsQueryable().SingleAsync(p => p.ProductReviewId == command.ProductReviewId, cancellationToken);
            if (productReview is ProductReview)
            {
                _mapper.Map(command, productReview);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        [CommandHandler]
        public virtual async Task Add(AddProductReviewCommand command, CancellationToken cancellationToken)
        {
            if (Computed.IsInvalidating())
            {
                _ = GetReviewIdsForProduct(command.ProductId, cancellationToken);
                return;
            }

            using var dbContext = CreateDbContext(readWrite: true);
            var reviewToAdd = _mapper.Map<ProductReview>(command);
            dbContext.ProductReviews.Add(reviewToAdd);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        [CommandHandler]
        public virtual async Task Delete(DeleteProductReviewCommand command, CancellationToken cancellationToken)
        {
            if (Computed.IsInvalidating())
            {
                _ = GetReviewIdsForProduct(command.ProductId, cancellationToken);
                _ = TryGet(command.ProductReviewId, cancellationToken);
                return;
            }

            using var dbContext = CreateDbContext(readWrite: true);

            var user = await _authService.GetUser(command.Session);
            if(user.IsAuthenticated && user.Claims.ContainsKey("comment_delete"))
            {
                var productReview = await dbContext.ProductReviews.AsQueryable()
                .Where(r => r.ProductId == command.ProductId)
                .Where(r => r.ProductReviewId == command.ProductReviewId)
                .SingleAsync();
                dbContext.Remove(productReview);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            throw new Exception("Unauthorized");
        }
    }
}

