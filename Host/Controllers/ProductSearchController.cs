using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Interfaces.ComputeServices;
using Dipterv.Shared.Paging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Authentication;
using Stl.Fusion.Server;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductSearchController : ControllerBase, IProductSearchService
    {
        private readonly IProductSearchService _productSearchService;
        private readonly ISessionResolver _sessionResolver;

        public ProductSearchController(IProductSearchService productSearchService, ISessionResolver sessionResolver)
        {
            _sessionResolver = sessionResolver;
            _productSearchService = productSearchService;
        }

        [HttpPost, Publish]
        public Task<PageResponse<ListProductDto>> Search([FromBody] ProductSearchDto searchDto, CancellationToken cancellationToken = default)
            => _productSearchService.Search(new ProductSearchDto { Filter = searchDto.Filter, PageRequest = searchDto.PageRequest, Session = _sessionResolver.Session }, cancellationToken);
    }
}
