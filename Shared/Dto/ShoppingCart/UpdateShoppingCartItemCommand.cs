using Stl.Fusion.Authentication;

namespace Dipterv.Shared.Dto.ShoppingCart
{
    public record UpdateShoppingCartItemCommand(Session Session, int CartItemId, int Quantity, int ProductId) : ISessionCommand<ShoppingCartItemDto>
    {
        public UpdateShoppingCartItemCommand() : this(Session.Null, 0, 0, 0) { }
    }
}
