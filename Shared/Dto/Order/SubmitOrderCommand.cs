using Stl.CommandR;
using Stl.Fusion.Authentication;
using System.Reactive;

namespace Dipterv.Shared.Dto.Order
{
    public class SubmitOrderCommand : ICommand<Unit>
    {
        public Session Session { get; set; }
    }
}
