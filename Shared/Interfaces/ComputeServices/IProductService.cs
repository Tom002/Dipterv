using Dipterv.Shared.Dto;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces
{
    public interface IProductService
    {
        [ComputeMethod]
        public Task<ProductDto?> TryGet(int productId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<ProductWithReviewsDto?> TryGetWithReviews(int productId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<List<ProductDto>> GetAll(string searchTerm = default, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task Edit(UpdateProductCommand command, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task Add(AddProductCommand command, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task Delete(DeleteProductCommand command, CancellationToken cancellationToken = default);


    }
}
