using Dipterv.Shared.Dto;
using Stl.Fusion;
using Stl.Fusion.Authentication;

namespace Dipterv.Client.Interfaces
{
    public interface IProductDetailsService
    {
        [ComputeMethod]
        public Task<ProductDetailsDto> GetProductDetails(Session session, int productId, int orderQuantity, CancellationToken cancellationToken = default);
    }
}
