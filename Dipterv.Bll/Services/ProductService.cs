using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dipterv.Bll.Extensions;
using Dipterv.Bll.Helper;
using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto;
using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Dto.ProductCategory;
using Dipterv.Shared.Dto.ProductPhoto;
using Dipterv.Shared.Helper;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.ComputeServices;
using Dipterv.Shared.Paging;
using Microsoft.EntityFrameworkCore;
using Stl.Async;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.EntityFramework;
using System.Reactive;

namespace Dipterv.Bll.Services
{
    public class ProductService : DbServiceBase<FusionDbContext>, IProductService
    {
        private readonly IMapper _mapper;
        private readonly IDbEntityResolver<int, Product> _productResolver;
        private readonly IDbEntityResolver<int, ProductPhoto> _productPhotoResolver;
        private readonly IDateService _dateService;
        private readonly IAuth _authService;

        public ProductService(
            IServiceProvider services,
            IMapper mapper,
            IDbEntityResolver<int, Product> productResolver,
            IDbEntityResolver<int, ProductPhoto> productPhotoResolver,
            IDateService dateService,
            IAuth authService)
            : base(services)
        {
            _mapper = mapper;
            _productResolver = productResolver;
            _productPhotoResolver = productPhotoResolver;
            _dateService = dateService;
            _authService = authService;
        }

        [ComputeMethod]
        public virtual async Task<ProductDto?> TryGetProduct(int productId, CancellationToken cancellationToken)
        {
            var product = await _productResolver.Get(productId, cancellationToken);
            if (product is Product)
            {
                return _mapper.Map<ProductDto>(product);
            }
            
            return default;
        }

        [ComputeMethod]
        public virtual async Task<List<ProductDto?>> TryGetManyProducts(List<int> productIdList, CancellationToken cancellationToken)
        {
            var products = await Task.WhenAll(productIdList.Select(async productId =>
            {
                return await TryGetProduct(productId, cancellationToken);
            }));
            return products.ToList();
        }

        [ComputeMethod]
        public virtual async Task<ProductPhotoDto?> TryGetProductPhoto(int productPhotoId, CancellationToken cancellationToken)
        {
            var productPhoto = await _productPhotoResolver.Get(productPhotoId, cancellationToken);
            if (productPhoto is ProductPhoto)
            {
                return _mapper.Map<ProductPhotoDto>(productPhoto);
            }

            return default;
        }

        [ComputeMethod]
        public virtual async Task<List<ProductPhotoDto?>> TryGetManyProductPhotos(List<int> productPhotoIdList, CancellationToken cancellationToken)
        {
            var productPhotos = await Task.WhenAll(productPhotoIdList.Select(async productPhotoId =>
            {
                return await TryGetProductPhoto(productPhotoId, cancellationToken);
            }));
            return productPhotos.ToList();
        }

        [ComputeMethod]
        public virtual async Task<List<ProductCategoryDto>> GetCategories(CancellationToken cancellationToken)
        {
            await using var dbContext = CreateDbContext();

            return await dbContext.ProductCategories
                .ProjectTo<ProductCategoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }


        [ComputeMethod]
        public virtual async Task<List<ProductDto>> GetAll(string searchTerm, CancellationToken cancellationToken)
        {
            await using var dbContext = CreateDbContext();

            var productIdList = await GetProductIdListForSearchTerm(searchTerm, cancellationToken);

            var products = await Task.WhenAll(productIdList.Select(async productId =>
            {
                return await TryGetProduct(productId, cancellationToken);
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
                _ = TryGetProduct(command.ProductId, cancellationToken);
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
                _ = TryGetProduct(command.ProductId, cancellationToken);
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
