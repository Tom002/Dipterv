using Dipterv.Bll.Interfaces;
using Dipterv.Shared.Dto.ShoppingCart;

namespace Dipterv.Bll.Services
{
    public class CommandInvalidationHelperService : ICommandInvalidationHelperService
    {
        private Dictionary<RemoveShoppingCartItemCommand, List<int>> _shoppingCartItemsToInvalidateAfterRemoveDictionary = new();
        private Dictionary<UpdateShoppingCartItemCommand, List<int>> _shoppingCartItemsToInvalidateAfterUpdateDictionary = new();

        public List<int>? GetShoppingCartItemsToInvalidateAfterRemove(RemoveShoppingCartItemCommand command)
        {
            if(_shoppingCartItemsToInvalidateAfterRemoveDictionary.TryGetValue(command, out var result))
                return result;
            return default;
        }

        public void AddShoppingCartItemsToInvalidateAfterRemove(RemoveShoppingCartItemCommand command, List<int> cartItemIdsToInvalidate)
        {
            _shoppingCartItemsToInvalidateAfterRemoveDictionary.Add(command, cartItemIdsToInvalidate);
        }

        public List<int>? GetShoppingCartItemsToInvalidateAfterUpdate(UpdateShoppingCartItemCommand command)
        {
            if (_shoppingCartItemsToInvalidateAfterUpdateDictionary.TryGetValue(command, out var result))
                return result;
            return default;
        }

        public void AddShoppingCartItemsToInvalidateAfterUpdate(UpdateShoppingCartItemCommand command, List<int> cartItemIdsToInvalidate)
        {
            _shoppingCartItemsToInvalidateAfterUpdateDictionary.Add(command, cartItemIdsToInvalidate);
        }
    }
}
