
using Stl.CommandR;
using Stl.Fusion.Authentication;
using System.Reactive;

namespace Dipterv.Shared.Dto
{
    public class DeleteProductReviewCommand : ICommand<Unit>
    {
        public Session Session { get; set; }
        public int ProductReviewId { get; set; }
        public int ProductId { get; set; }
    }
}
