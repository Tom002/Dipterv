using Dipterv.Shared.Dto;
using Dipterv.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stl.Fusion.Server;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductReviewController : ControllerBase, IProductReviewService
    {
        private readonly IProductReviewService _productReviewService;

        public ProductReviewController(IProductReviewService productReviewService)
        {
            _productReviewService = productReviewService;
        }

        [HttpPut]
        public Task Edit(
            [FromBody] UpdateProductReviewCommand command,
            CancellationToken cancellationToken = default)
            => _productReviewService.Edit(command, cancellationToken);

        [HttpPost]
        public Task Add(
            [FromBody] AddProductReviewCommand command,
            CancellationToken cancellationToken = default)
            => _productReviewService.Add(command, cancellationToken);

        [HttpDelete]
        public Task Delete(
            [FromBody] DeleteProductReviewCommand command,
            CancellationToken cancellationToken = default)
            => _productReviewService.Delete(command, cancellationToken);


        [HttpGet, Publish]
        public Task<ProductReviewDto?> TryGet(int productReviewId, CancellationToken cancellationToken = default)
            =>                                  _productReviewService.TryGet(productReviewId, cancellationToken);

        [HttpGet, Publish]
        public Task<List<int>> GetReviewIdsForProduct(int productId, CancellationToken cancellationToken = default)
            => _productReviewService.GetReviewIdsForProduct(productId, cancellationToken);

        [HttpPost, Publish]
        public Task<List<ProductReviewDto>> TryGetMany(List<int> productReviewIdList, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}

