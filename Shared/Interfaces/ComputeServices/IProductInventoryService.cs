using Dipterv.Shared.Dto.ProductInventory;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface IProductInventoryService
    {
        [ComputeMethod]
        public Task<int> ProductGetTotalStock(int productId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<List<ProductInventoryDto>> ProductGetInvetories(int productId, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task AddProductInventory(AddProductInventoryCommand command, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task EditProductInventory(EditProductInventoryCommand command, CancellationToken cancellationToken = default);
    }
}
