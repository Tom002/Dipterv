using Dipterv.Shared.Dto;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces
{
    public interface IProductReviewService
    {
        [ComputeMethod]
        public Task<ProductReviewDto?> TryGet(int productReviewId, CancellationToken cancellationToken);

        [ComputeMethod]
        public Task<List<ProductReviewDto>> TryGetMany(List<int> productReviewIdList, CancellationToken cancellationToken);

        [ComputeMethod]
        public Task<List<int>> GetReviewIdsForProduct(int productId, CancellationToken cancellationToken);

        [CommandHandler]
        public Task Edit(UpdateProductReviewCommand command, CancellationToken cancellationToken);

        [CommandHandler]
        public Task Add(AddProductReviewCommand command, CancellationToken cancellationToken);

        [CommandHandler]
        public Task Delete(DeleteProductReviewCommand command, CancellationToken cancellationToken);
    }
}
