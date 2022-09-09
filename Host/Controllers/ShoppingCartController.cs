using Dipterv.Shared.Dto.ShoppingCart;
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
    public class ShoppingCartController : ControllerBase, IShoppingCartService
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet, Publish]
        public Task<ShoppingCartItemDto> GetCartItem(int cartItemId, CancellationToken cancellationToken = default) =>
            _shoppingCartService.GetCartItem(cartItemId, cancellationToken);

        [HttpGet, Publish]
        public Task<ShoppingCartItemDto> GetCartItemByShoppingCartKeyAndProductId([FromQuery] string shoppingCartKey, [FromQuery] int productId, CancellationToken cancellationToken = default) =>
            _shoppingCartService.GetCartItemByShoppingCartKeyAndProductId(shoppingCartKey, productId, cancellationToken);

        [HttpGet, Publish]
        public Task<int?> GetCartItemIdByShoppingCartKeyAndProductId(string shoppingCartKey, int productId, CancellationToken cancellationToken = default) =>
            _shoppingCartService.GetCartItemIdByShoppingCartKeyAndProductId(shoppingCartKey, productId, cancellationToken);

        [HttpGet, Publish]
        public Task<List<int>> GetCartItemIdsForCustomer(string shoppingCartKey, CancellationToken cancellationToken = default) =>
            _shoppingCartService.GetCartItemIdsForCustomer(shoppingCartKey, cancellationToken);

        [HttpGet, Publish]
        public Task<List<ShoppingCartItemDto>> GetCartItemsByProductId(int productId, CancellationToken cancellationToken = default) =>
            _shoppingCartService.GetCartItemsByProductId(productId, cancellationToken);

        [HttpGet, Publish]
        public Task<List<ShoppingCartItemDto>> GetCartItemsForCustomer(string shoppingCartKey, CancellationToken cancellationToken = default) =>
            _shoppingCartService.GetCartItemsForCustomer(shoppingCartKey, cancellationToken);

        [HttpGet, Publish]
        public Task<List<ShoppingCartItemDto>> GetManyCartItems(List<int> cartItemIdList, CancellationToken cancellationToken = default) =>
            _shoppingCartService.GetManyCartItems(cartItemIdList, cancellationToken);
    }
}
