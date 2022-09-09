using Dipterv.Shared.Dto.SpecialOffer;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface ISpecialOfferService
    {
        [ComputeMethod]
        public Task<List<SpecialOfferDto>> GetActiveSpecialOffersForProduct(int productId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<List<int>> GetValidSpecialOffersIdsForProduct(int productId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<SpecialOfferDto?> TryGet(int specialOfferId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<List<SpecialOfferDto>> TryGetMany(List<int> specialOfferIdList, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task Edit(UpdateSpecialOfferCommand command, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task AddSpecialOffer(AddSpecialOfferCommand command, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task AddSpecialOfferProduct(AddSpecialOfferProductCommand command, CancellationToken cancellationToken = default);
    }
}
