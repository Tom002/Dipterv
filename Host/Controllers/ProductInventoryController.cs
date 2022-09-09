using Dipterv.Shared.Dto.ProductInventory;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Server;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController, JsonifyErrors]
    public class ProductInventoryController : ControllerBase, IProductInventoryService
    {
        private readonly IProductInventoryService _productInventoryService;

        public ProductInventoryController(IProductInventoryService productInventoryService)
        {
            _productInventoryService = productInventoryService;
        }

        [HttpPost]
        public Task AddProductInventory([FromBody] AddProductInventoryCommand command, CancellationToken cancellationToken = default)
            => _productInventoryService.AddProductInventory(command, cancellationToken);

        [HttpPut]
        public Task EditProductInventory([FromBody] EditProductInventoryCommand command, CancellationToken cancellationToken = default)
            => _productInventoryService.EditProductInventory(command, cancellationToken);

        [HttpGet, Publish]
        public Task<List<ProductInventoryDto>> GetInventoriesForProduct(int productId, CancellationToken cancellationToken = default)
            => _productInventoryService.GetInventoriesForProduct(productId, cancellationToken);

        [HttpGet, Publish]
        public Task<List<ProductInventoryDto>> GetInventoriesForProductIdList(List<int> productIdList, CancellationToken cancellationToken = default)
            => _productInventoryService.GetInventoriesForProductIdList(productIdList, cancellationToken);

        [HttpGet, Publish]
        public Task<int> ProductGetTotalStock(int productId, CancellationToken cancellationToken = default)
            => _productInventoryService.ProductGetTotalStock(productId, cancellationToken);
    }
}
