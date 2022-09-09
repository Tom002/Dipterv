using AutoMapper;
using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto.SpecialOffer;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.EntityFrameworkCore;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.EntityFramework;

namespace Dipterv.Bll.Services
{
    public class SpecialOfferService : DbServiceBase<FusionDbContext>, ISpecialOfferService
    {
        private readonly IDateService _dateService;
        private readonly IMapper _mapper;
        private readonly IDbEntityResolver<int, SpecialOffer> _specialOfferResolver;

        public SpecialOfferService(
            IServiceProvider services,
            IDateService dateService,
            IDbEntityResolver<int, SpecialOffer> specialOfferResolver,
            IMapper mapper)
                : base(services)
        {
            _dateService = dateService;
            _specialOfferResolver = specialOfferResolver;
            _mapper = mapper;
        }


        [ComputeMethod]
        public virtual async Task<List<SpecialOfferDto>> GetActiveSpecialOffersForProduct(int productId, CancellationToken cancellationToken = default)
        {
            var validSpecialOfferIds = await GetValidSpecialOffersIdsForProduct(productId, cancellationToken);

            var specialOffers = await _specialOfferResolver.GetMany(validSpecialOfferIds, cancellationToken);

            return _mapper.Map<List<SpecialOfferDto>>(specialOffers.Values.ToList());
        }

        [ComputeMethod]
        public virtual async Task<List<int>> GetValidSpecialOffersIdsForProduct(int productId, CancellationToken cancellationToken = default)
        {
            var currentDate = await _dateService.GetCurrentDate();

            await using var dbContext = CreateDbContext();

            return await dbContext.SpecialOfferProducts.AsQueryable()
                .Where(s => s.ProductId == productId)
                .Where(s => s.SpecialOffer.StartDate.Date <= currentDate.Date && s.SpecialOffer.EndDate.Date >= currentDate.Date)
                .Select(s => s.SpecialOfferId)
                .ToListAsync();
        }

        [ComputeMethod]
        public virtual async Task<SpecialOfferDto?> TryGet(int specialOfferId, CancellationToken cancellationToken = default)
        {
            var specialOffer = await _specialOfferResolver.Get(specialOfferId, cancellationToken);
            if (specialOffer is SpecialOffer)
                return _mapper.Map<SpecialOfferDto>(specialOffer);
            return default;
        }

        [ComputeMethod]
        public virtual async Task<List<SpecialOfferDto>> TryGetMany(List<int> specialOfferIdList, CancellationToken cancellationToken = default)
        {
            var specialOffers = await Task.WhenAll(specialOfferIdList.Select(async specialOfferId =>
            {
                return await TryGet(specialOfferId, cancellationToken);
            }));
            return specialOffers.ToList();
        }

        [CommandHandler]
        public virtual async Task Edit(UpdateSpecialOfferCommand command, CancellationToken cancellationToken = default)
        {
            if (Computed.IsInvalidating())
            {
                _ = TryGet(command.SpecialOfferId, cancellationToken);
                return;
            }

            using var dbContext = await CreateCommandDbContext(cancellationToken);
            var specialOffer = await dbContext.SpecialOffers.AsQueryable().SingleOrDefaultAsync(s => s.SpecialOfferId == command.SpecialOfferId, cancellationToken);
            if (specialOffer is SpecialOffer)
            {
                _mapper.Map(command, specialOffer);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        [CommandHandler]
        public virtual async Task AddSpecialOffer(AddSpecialOfferCommand command, CancellationToken cancellationToken = default)
        {
            using var dbContext = await CreateCommandDbContext(cancellationToken);
            var specialOffer = _mapper.Map<SpecialOffer>(command);
            dbContext.SpecialOffers.Add(specialOffer);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        [CommandHandler]
        public virtual async Task AddSpecialOfferProduct(AddSpecialOfferProductCommand command, CancellationToken cancellationToken = default)
        {
            if (Computed.IsInvalidating())
            {
                _ = GetValidSpecialOffersIdsForProduct(command.ProductId, cancellationToken);
                return;
            }

            using var dbContext = await CreateCommandDbContext(cancellationToken);
            var specialOfferProduct = new SpecialOfferProduct { ProductId = command.ProductId, SpecialOfferId = command.SpecialOfferId };
            dbContext.SpecialOfferProducts.Add(specialOfferProduct);
            await dbContext.SaveChangesAsync();
        }
    }
}
