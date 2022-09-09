using Dipterv.Shared.Dto;
using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Dto.ProductCategory;
using Dipterv.Shared.Dto.ProductPhoto;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Paging;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Authentication;
using Stl.Fusion.Server;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController, JsonifyErrors]
    public class ProductController : ControllerBase, IProductService
    {
        private readonly IProductService _productService;
        private readonly ISessionResolver _sessionResolver;

        public ProductController(
            IProductService productService,
            ISessionResolver sessionResolver)
        {
            _productService = productService;
            _sessionResolver = sessionResolver;
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
        public Task<ProductDto> TryGetProduct(int productId, CancellationToken cancellationToken = default)
            => _productService.TryGetProduct(productId, cancellationToken);

        [HttpGet, Publish]
        public Task<List<ProductDto>> GetAll([FromQuery] string? searchTerm = default, CancellationToken cancellationToken = default)
            => _productService.GetAll(searchTerm, cancellationToken);

        [HttpPost, Publish]
        public Task<List<ProductDto>> TryGetManyProducts([FromBody] List<int> productIdList, CancellationToken cancellationToken = default)
            => _productService.TryGetManyProducts(productIdList, cancellationToken);

        [HttpGet, Publish]
        public Task<ProductPhotoDto> TryGetProductPhoto(int productPhotoId, CancellationToken cancellationToken = default)
            => _productService.TryGetProductPhoto(productPhotoId, cancellationToken);

        [HttpGet, Publish]
        public Task<List<ProductPhotoDto>> TryGetManyProductPhotos(List<int> productPhotoIdList, CancellationToken cancellationToken = default)
            => _productService.TryGetManyProductPhotos(productPhotoIdList, cancellationToken);

        [HttpGet, Publish]
        public Task<List<ProductCategoryDto>> GetCategories(CancellationToken cancellationToken = default)
            => _productService.GetCategories(cancellationToken);
    }
}
