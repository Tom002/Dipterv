using Dipterv.Shared.Dto.ShoppingCart;
using RestEase;
using Stl.Fusion.Authentication;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("shoppingCartDetails")]
    public interface IShoppingCartDetailsClientDef
    {
        [Get("getShoppingCartForCustomer")]
        Task<ShoppingCartDto> GetShoppingCartForCustomer([Query] Session session, CancellationToken cancellationToken = default);

        [Post("addProductToCart")]
        Task<ShoppingCartItemDto> AddProductToCart([Body] AddShoppingCartItemCommand addShoppingCartItemCommand, CancellationToken cancellationToken = default);

        [Delete("removeProductFromCart")]
        Task RemoveProductFromCart([Body] RemoveShoppingCartItemCommand command, CancellationToken cancellationToken = default);

        [Put("updateProductQuantityInCart")]
        Task<ShoppingCartItemDto> UpdateProductQuantityInCart([Body] UpdateShoppingCartItemCommand command, CancellationToken cancellationToken = default);
    }
}
