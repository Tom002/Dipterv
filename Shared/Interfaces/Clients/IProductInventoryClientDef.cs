using Dipterv.Shared.Dto.ProductInventory;
using RestEase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("productInventory")]
    public interface IProductInventoryClientDef
    {
        [Get("productGetTotalStock")]
        public Task<int> ProductGetTotalStock(int productId, CancellationToken cancellationToken = default);

        [Post("addProductInventory")]
        public Task AddProductInventory([Body] AddProductInventoryCommand command, CancellationToken cancellationToken = default);

        [Put("editProductInventory")]
        public Task EditProductInventory([Body] EditProductInventoryCommand command, CancellationToken cancellationToken = default);

        [Get("productGetInvetories")]
        public Task<List<ProductInventoryDto>> ProductGetInvetories(int productId, CancellationToken cancellationToken = default);
    }
}
