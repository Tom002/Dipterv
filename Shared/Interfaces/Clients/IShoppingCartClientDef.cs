using Dipterv.Shared.Dto.ShoppingCart;
using RestEase;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("shoppingCart")]
    public interface IShoppingCartClientDef
    {
        [Get("getCartItemByShoppingCartKeyAndProductId")]
        Task<ShoppingCartItemDto?> GetCartItemByShoppingCartKeyAndProductId(string shoppingCartKey, int productId, CancellationToken cancellationToken = default);

        [Get("getCartItemsForCustomer")]
        Task<List<ShoppingCartItemDto>> GetCartItemsForCustomer(string shoppingCartKey, CancellationToken cancellationToken = default);

        [Get("getCartItemIdsForCustomer")]
        Task<List<int>> GetCartItemIdsForCustomer(string shoppingCartKey, CancellationToken cancellationToken = default);

        [Get("getCartItem")]
        Task<ShoppingCartItemDto?> GetCartItem(int cartItemId, CancellationToken cancellationToken = default);

        [Get("getCartItemIdByShoppingCartKeyAndProductId")]
        Task<int?> GetCartItemIdByShoppingCartKeyAndProductId(string shoppingCartKey, int productId, CancellationToken cancellationToken = default);
    }
}
