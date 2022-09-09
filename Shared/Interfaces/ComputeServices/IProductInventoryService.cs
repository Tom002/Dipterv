using Dipterv.Shared.Dto.ProductInventory;
using Stl.CommandR.Configuration;
using Stl.Fusion;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface IProductInventoryService
    {
        [ComputeMethod]
        public Task<int> ProductGetTotalStock(int productId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<List<ProductInventoryDto>> GetInventoriesForProduct(int productId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<List<ProductInventoryDto>> GetInventoriesForProductIdList(List<int> productIdList, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task AddProductInventory(AddProductInventoryCommand command, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task EditProductInventory(EditProductInventoryCommand command, CancellationToken cancellationToken = default);
    }
}
