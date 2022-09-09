using Stl.CommandR;
using Stl.Fusion.Authentication;
using System.Reactive;

namespace Dipterv.Shared.Dto.ShoppingCart
{
    public record AddShoppingCartItemCommand(Session Session, int ProductId, short Quantity) : ISessionCommand<ShoppingCartItemDto>
    {
        public AddShoppingCartItemCommand() : this(Session.Null, 0, 0) { }
    }
}
