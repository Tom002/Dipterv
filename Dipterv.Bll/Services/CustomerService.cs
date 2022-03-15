using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dipterv.Dal.DbContext;
using Dipterv.Shared.Dto.Customer;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.EntityFrameworkCore;
using Stl.Fusion.EntityFramework;

namespace Dipterv.Bll.Services
{
    public class CustomerService : DbServiceBase<FusionDbContext>, ICustomerService
    {
        private readonly IMapper _mapper;

        public CustomerService(
            IServiceProvider services,
            IMapper mapper) : base(services)
        {
            _mapper = mapper;
        }

        public virtual async Task<CustomerDto> GetCustomer(int customerId, CancellationToken cancellationToken = default)
        {
            await using var dbContext = CreateDbContext();

            var customer = await dbContext.Customers
                .AsQueryable()
                .Where(c => c.CustomerId == customerId)
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);

            if(customer != null)
            {
                return customer;
            }

            throw new System.Exception("Customer not found");
        }
    }
}
