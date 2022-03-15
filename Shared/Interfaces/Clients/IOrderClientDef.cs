using Dipterv.Shared.Dto.Order;
using RestEase;
using Stl.Fusion.Authentication;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("order")]
    public interface IOrderClientDef
    {
        [Get("getMyOrders")]
        public Task<List<OrderHeaderDto>> GetMyOrders([Query] Session session, CancellationToken cancellationToken = default);

        [Post("submitOrder")]
        public Task SubmitOrder([Body] SubmitOrderCommand submitOrder, CancellationToken cancellationToken = default);
    }
}
