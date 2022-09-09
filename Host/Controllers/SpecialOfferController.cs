using Dipterv.Shared.Dto.SpecialOffer;
using Dipterv.Shared.Interfaces.ComputeServices;
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
    public class SpecialOfferController : ControllerBase, ISpecialOfferService
    {
        private readonly ISpecialOfferService _specialOfferService;
        private readonly ISessionResolver _sessionResolver;

        public SpecialOfferController(ISpecialOfferService specialOfferService, ISessionResolver sessionResolver)
        {
            _specialOfferService = specialOfferService;
            _sessionResolver = sessionResolver;
        }

        [HttpPost]
        public Task AddSpecialOffer([FromBody] AddSpecialOfferCommand command, CancellationToken cancellationToken = default)
            => _specialOfferService.AddSpecialOffer(command, cancellationToken);

        [HttpPost]
        public Task AddSpecialOfferProduct([FromBody] AddSpecialOfferProductCommand command, CancellationToken cancellationToken = default)
            => _specialOfferService.AddSpecialOfferProduct(command, cancellationToken);

        [HttpPut]
        public Task Edit([FromBody] UpdateSpecialOfferCommand command, CancellationToken cancellationToken)
            => _specialOfferService.Edit(command, cancellationToken);

        [HttpGet, Publish]
        public Task<List<SpecialOfferDto>> GetActiveSpecialOffersForProduct(int productId, CancellationToken cancellationToken = default)
            => _specialOfferService.GetActiveSpecialOffersForProduct(productId, cancellationToken);

        [HttpGet, Publish]
        public Task<List<int>> GetValidSpecialOffersIdsForProduct(int productId, CancellationToken cancellationToken = default)
            => _specialOfferService.GetValidSpecialOffersIdsForProduct(productId, cancellationToken);

        [HttpGet, Publish]
        public Task<SpecialOfferDto?> TryGet(int specialOfferId, CancellationToken cancellationToken = default)
            => _specialOfferService.TryGet(specialOfferId, cancellationToken);

        [HttpGet, Publish]
        public Task<List<SpecialOfferDto>> TryGetMany(List<int> specialOfferIdList, CancellationToken cancellationToken = default)
            => _specialOfferService.TryGetMany(specialOfferIdList, cancellationToken);
    }
}
