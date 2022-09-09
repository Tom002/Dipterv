using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Paging;
using RestEase;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("productSearch")]
    public interface IProductSearchClientDef
    {
        [Post("search")]
        public Task<PageResponse<ListProductDto>> Search([Body] ProductSearchDto searchDto, CancellationToken cancellationToken = default);
    }
}
