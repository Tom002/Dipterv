using Dipterv.Shared.Dto.ShoppingCart;

namespace Dipterv.Bll.Interfaces
{
    public interface ICommandInvalidationHelperService
    {
        public List<int>? GetShoppingCartItemsToInvalidateAfterRemove(RemoveShoppingCartItemCommand command);
        public void AddShoppingCartItemsToInvalidateAfterRemove(RemoveShoppingCartItemCommand command, List<int> cartItemIdsToInvalidate);
        public List<int>? GetShoppingCartItemsToInvalidateAfterUpdate(UpdateShoppingCartItemCommand command);
        public void AddShoppingCartItemsToInvalidateAfterUpdate(UpdateShoppingCartItemCommand command, List<int> cartItemIdsToInvalidate);
    }
}
