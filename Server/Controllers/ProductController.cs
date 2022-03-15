using Dipterv.Shared.Dto;
using Dipterv.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Server;

namespace Dipterv.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController, JsonifyErrors]
    public class ProductController : ControllerBase, IProductService
    {
        private readonly IProductService _productService;

        public ProductController(
            IProductService productService)
        {
            _productService = productService;
        }

        [HttpPut]
        public Task Edit(
            [FromBody] UpdateProductCommand command,
            CancellationToken cancellationToken = default)
            => _productService.Edit(command, cancellationToken);

        [HttpPost]
        public Task Add(
            [FromBody] AddProductCommand command,
            CancellationToken cancellationToken = default)
            => _productService.Add(command, cancellationToken);

        [HttpDelete]
        public Task Delete(
            [FromBody] DeleteProductCommand command,
            CancellationToken cancellationToken = default)
            => _productService.Delete(command, cancellationToken);

        [HttpGet, Publish]
        public Task<ProductDto?> TryGet(int productId, CancellationToken cancellationToken = default)
            => _productService.TryGet(productId, cancellationToken);

        [HttpGet, Publish]
        public Task<ProductWithReviewsDto?> TryGetWithReviews(int productId, CancellationToken cancellationToken = default)
            => _productService.TryGetWithReviews(productId, cancellationToken);

        [HttpGet, Publish]
        public Task<List<ProductDto>> GetAll(string? searchTerm = default, CancellationToken cancellationToken = default)
            => _productService.GetAll(searchTerm, cancellationToken);
    }
}
