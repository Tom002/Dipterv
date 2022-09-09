using Dipterv.Shared.Dto.ShoppingCart;
using Stl.Fusion;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface IShoppingCartService
    {
        
        [ComputeMethod]
        Task<List<ShoppingCartItemDto>> GetCartItemsForCustomer(string shoppingCartKey, CancellationToken cancellationToken = default);

        [ComputeMethod]
        Task<ShoppingCartItemDto?> GetCartItemByShoppingCartKeyAndProductId(string shoppingCartKey, int productId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        Task<List<int>> GetCartItemIdsForCustomer(string shoppingCartKey, CancellationToken cancellationToken = default);

        [ComputeMethod]
        Task<ShoppingCartItemDto?> GetCartItem(int cartItemId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        Task<List<ShoppingCartItemDto>> GetManyCartItems(List<int> cartItemIdList, CancellationToken cancellationToken = default);

        [ComputeMethod]
        Task<int?> GetCartItemIdByShoppingCartKeyAndProductId(string shoppingCartKey, int productId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        Task<List<ShoppingCartItemDto>> GetCartItemsByProductId(int productId, CancellationToken cancellationToken = default);
    }
}
