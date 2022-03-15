using Dipterv.Shared.Dto.Order;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface IOrderService
    {
        [ComputeMethod]
        public Task<List<OrderHeaderDto>> GetMyOrders(Session session, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task SubmitOrder(SubmitOrderCommand submitOrder, CancellationToken cancellationToken = default);
    }
}
