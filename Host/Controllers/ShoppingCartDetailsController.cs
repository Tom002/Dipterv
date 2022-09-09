using Dipterv.Shared.Dto.ShoppingCart;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Authentication;
using Stl.Fusion.Server;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController, JsonifyErrors]
    public class ShoppingCartDetailsController : ControllerBase, IShoppingCartDetailsService
    {
        private readonly IShoppingCartDetailsService _shoppingCartDetailsService;
        private readonly ISessionResolver _sessionResolver;

        public ShoppingCartDetailsController(IShoppingCartDetailsService shoppingCartDetailsService, ISessionResolver sessionResolver)
        {
            _shoppingCartDetailsService = shoppingCartDetailsService;
            _sessionResolver = sessionResolver;
        }

        [HttpPost]
        public Task<ShoppingCartItemDto> AddProductToCart([FromBody] AddShoppingCartItemCommand command, CancellationToken cancellationToken = default) =>
            _shoppingCartDetailsService.AddProductToCart(new AddShoppingCartItemCommand(_sessionResolver.Session, command.ProductId, command.Quantity), cancellationToken);

        [HttpGet, Publish]
        public Task<ShoppingCartDto> GetShoppingCartForCustomer([FromQuery] Session session, CancellationToken cancellationToken = default) =>
            _shoppingCartDetailsService.GetShoppingCartForCustomer(_sessionResolver.Session, cancellationToken);

        [HttpDelete]
        public Task RemoveProductFromCart([FromBody] RemoveShoppingCartItemCommand command, CancellationToken cancellationToken = default) =>
            _shoppingCartDetailsService.RemoveProductFromCart(new RemoveShoppingCartItemCommand(_sessionResolver.Session, command.CartItemId, command.ProductId, command.Quantity), cancellationToken);

        [HttpPut]
        public Task<ShoppingCartItemDto> UpdateProductQuantityInCart([FromBody] UpdateShoppingCartItemCommand command, CancellationToken cancellationToken = default) =>
            _shoppingCartDetailsService.UpdateProductQuantityInCart(new UpdateShoppingCartItemCommand(_sessionResolver.Session, command.CartItemId, command.Quantity, command.ProductId), cancellationToken);
    }
}
