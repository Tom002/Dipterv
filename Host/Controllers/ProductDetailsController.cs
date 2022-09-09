using Dipterv.Shared.Dto;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Authentication;
using Stl.Fusion.Server;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductDetailsController : ControllerBase, IProductDetailsService
    {
        private readonly IProductDetailsService _productDetailsService;
        private readonly ISessionResolver _sessionResolver;

        public ProductDetailsController(
            IProductDetailsService productDetailsService,
            ISessionResolver sessionResolver)
        {
            _productDetailsService = productDetailsService;
            _sessionResolver = sessionResolver;
        }

        [HttpGet, Publish]
        public Task<ProductDetailsDto> TryGetWithDetails(Session session, int productId, CancellationToken cancellationToken)
            => _productDetailsService.TryGetWithDetails(_sessionResolver.Session, productId, cancellationToken);
    }
}
