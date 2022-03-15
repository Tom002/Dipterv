using Dipterv.Shared.Dto.SpecialOffer;
using RestEase;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("specialOffer")]
    public interface ISpecialOfferClientDef
    {
        [Get("getActiveSpecialOffersForProduct")]
        public Task<List<SpecialOfferDto>> GetActiveSpecialOffersForProduct(int productId, CancellationToken cancellationToken = default);

        [Get("getValidSpecialOffersIdsForProduct")]
        public Task<List<int>> GetValidSpecialOffersIdsForProduct(int productId, CancellationToken cancellationToken = default);

        [Get("tryGet")]
        public Task<SpecialOfferDto?> TryGet(int specialOfferId, CancellationToken cancellationToken = default);

        [Put("edit")]
        public Task Edit([Body] UpdateSpecialOfferCommand command, CancellationToken cancellationToken = default);

        [Post("addSpecialOffer")]
        public Task AddSpecialOffer([Body] AddSpecialOfferCommand command, CancellationToken cancellationToken = default);

        [Post("addSpecialOfferProduct")]
        public Task AddSpecialOfferProduct([Body] AddSpecialOfferProductCommand command, CancellationToken cancellationToken = default);
    }
}
