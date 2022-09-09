using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto.ProductInventory;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.EntityFrameworkCore;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.EntityFramework;

namespace Dipterv.Bll.Services
{
    public class ProductInventoryService : DbServiceBase<FusionDbContext>, IProductInventoryService
    {
        private readonly IMapper _mapper;
        public ProductInventoryService(
            IServiceProvider services,
            IMapper mapper)
                : base(services)
        {
            _mapper = mapper;
        }

        [ComputeMethod]
        public virtual async Task<int> ProductGetTotalStock(int productId, CancellationToken cancellationToken = default)
        {
            using var dbContext = CreateDbContext(readWrite: false);

            return await dbContext.ProductInventories
                .Where(x => x.ProductId == productId)
                .SumAsync(x => x.Quantity);
        }

        [ComputeMethod]
        public virtual async Task<List<ProductInventoryDto>> GetInventoriesForProduct(int productId, CancellationToken cancellationToken = default)
        {
            using var dbContext = CreateDbContext(readWrite: true);

            return await dbContext.ProductInventories.AsQueryable()
                .Where(p => p.ProductId == productId)
                .ProjectTo<ProductInventoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        [ComputeMethod]
        public virtual async Task<List<ProductInventoryDto>> GetInventoriesForProductIdList(List<int> productIdList, CancellationToken cancellationToken = default)
        {
            using var dbContext = CreateDbContext(readWrite: true);

            var productInventories = await Task.WhenAll(productIdList.Select(async productId =>
            {
                return await GetInventoriesForProduct(productId, cancellationToken);
            }));

            return productInventories.SelectMany(pi => pi).ToList();
        }

        [CommandHandler]
        public virtual async Task AddProductInventory(AddProductInventoryCommand command, CancellationToken cancellationToken = default)
        {
            if(Computed.IsInvalidating())
            {
                _ = ProductGetTotalStock(command.ProductId, cancellationToken);
                _ = GetInventoriesForProduct(command.ProductId, cancellationToken);
            }

            await using var dbContext = CreateDbContext(readWrite: true);

            var productInventory = _mapper.Map<ProductInventory>(command);
            dbContext.ProductInventories.Add(productInventory);
            await dbContext.SaveChangesAsync();
        }

        [CommandHandler]
        public virtual async Task EditProductInventory(EditProductInventoryCommand command, CancellationToken cancellationToken = default)
        {
            if (Computed.IsInvalidating())
            {
                _ = ProductGetTotalStock(command.ProductId, cancellationToken);
                _ = GetInventoriesForProduct(command.ProductId, cancellationToken);
            }

            using var dbContext = CreateDbContext(readWrite: true);

            var inventory = await dbContext.ProductInventories
                .AsQueryable()
                .SingleOrDefaultAsync(p => p.ProductId == command.ProductId && p.LocationId == command.LocationId, cancellationToken);
            if(inventory is ProductInventory)
            {
                _mapper.Map(command, inventory);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
