using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Paging;
using Stl.Fusion;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface IProductSearchService
    {

        [ComputeMethod]
        public Task<PageResponse<ListProductDto>> Search(ProductSearchDto searchDto, CancellationToken cancellationToken);
    }
}
