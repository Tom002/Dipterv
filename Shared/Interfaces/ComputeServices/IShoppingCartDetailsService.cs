using Dipterv.Shared.Dto.ShoppingCart;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.Authentication;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface IShoppingCartDetailsService
    {
        [ComputeMethod]
        Task<ShoppingCartDto> GetShoppingCartForCustomer(Session session, CancellationToken cancellationToken = default);

        [CommandHandler]
        Task<ShoppingCartItemDto> AddProductToCart(AddShoppingCartItemCommand addShoppingCartItemCommand, CancellationToken cancellationToken = default);

        [CommandHandler]
        Task RemoveProductFromCart(RemoveShoppingCartItemCommand command, CancellationToken cancellationToken = default);

        [CommandHandler]
        Task<ShoppingCartItemDto> UpdateProductQuantityInCart(UpdateShoppingCartItemCommand command, CancellationToken cancellationToken = default);
    }
}
