using Dipterv.Shared.Dto;
using RestEase;
using Stl.Fusion.Authentication;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("productDetails")]
    public interface IProductDetailsClientDef
    {
        [Get("tryGetWithDetails")]
        public Task<ProductDetailsDto?> TryGetWithDetails(Session session, int productId, CancellationToken cancellationToken = default);
    }
}
