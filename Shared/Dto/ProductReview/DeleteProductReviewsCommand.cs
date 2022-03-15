using Stl.CommandR;
using System.Collections.Generic;
using System.Reactive;

namespace Dipterv.Shared.Dto
{
    public class DeleteProductReviewsCommand : ICommand<Unit>
    {
        public List<int> ProductReviewsIds { get; set; }
    }
}
