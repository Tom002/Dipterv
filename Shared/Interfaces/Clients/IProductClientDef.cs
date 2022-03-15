using Dipterv.Shared.Dto;
using RestEase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("product")]
    public interface IProductClientDef
    {
        [Get("tryGet")]
        public Task<ProductDto?> TryGet(int productId, CancellationToken cancellationToken = default);

        [Get("tryGetWithReviews")]
        public Task<ProductWithReviewsDto?> TryGetWithReviews(int productId, CancellationToken cancellationToken = default);

        [Get("getAll")]
        public Task<List<ProductDto>> GetAll(string searchTerm = "", CancellationToken cancellationToken = default);

        [Put("edit")]
        public Task Edit([Body] UpdateProductCommand command, CancellationToken cancellationToken = default);

        [Post("add")]
        public Task Add([Body] AddProductCommand command, CancellationToken cancellationToken = default);

        [Delete("delete")]
        public Task Delete([Body] DeleteProductCommand command, CancellationToken cancellationToken = default);
    }
}
