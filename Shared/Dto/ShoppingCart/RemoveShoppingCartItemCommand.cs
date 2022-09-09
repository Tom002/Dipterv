using Stl.Fusion.Authentication;
using System.Reactive;

namespace Dipterv.Shared.Dto.ShoppingCart
{
    public record RemoveShoppingCartItemCommand(Session Session, int CartItemId, int ProductId, int Quantity) : ISessionCommand<Unit>
    {
        public RemoveShoppingCartItemCommand() : this(Session.Null, 0, 0, 0) { }
    }
}
