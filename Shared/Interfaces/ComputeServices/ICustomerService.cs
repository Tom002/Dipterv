using Dipterv.Shared.Dto.Customer;
using Stl.Fusion;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface ICustomerService
    {
        [ComputeMethod]
        public Task<CustomerDto> GetCustomer(int customerId, CancellationToken cancellationToken = default);
    }
}
