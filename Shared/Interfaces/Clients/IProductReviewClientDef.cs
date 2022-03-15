using Dipterv.Shared.Dto;
using RestEase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("productReview")]
    public interface IProductReviewClientDef
    {
        [Get("tryGet")]
        public Task<ProductReviewDto?> TryGet(int productReviewId, CancellationToken cancellationToken = default);

        [Get("getReviewIdsForProduct")]
        public Task<List<int>> GetReviewIdsForProduct(int productId, CancellationToken cancellationToken = default);

        [Put("edit")]
        public Task Edit([Body] UpdateProductReviewCommand command, CancellationToken cancellationToken = default);

        [Post("add")]
        public Task Add([Body] AddProductReviewCommand command, CancellationToken cancellationToken = default);

        [Delete("delete")]
        public Task Delete([Body] DeleteProductReviewCommand command, CancellationToken cancellationToken = default);
    }
}
