using Dipterv.Shared.Dto.Order;
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
    [ApiController]
    public class OrderController : ControllerBase, IOrderService
    {
        private readonly IOrderService _orderService;
        private readonly ISessionResolver _sessionResolver;

        public OrderController(IOrderService orderService, ISessionResolver sessionResolver)
        {
            _orderService = orderService;
            _sessionResolver = sessionResolver;
        }

        [HttpGet, Publish]
        public async Task<List<OrderHeaderDto>> GetMyOrders([FromQuery] Session session, CancellationToken cancellationToken = default)
        {
            return await _orderService.GetMyOrders(_sessionResolver.Session, cancellationToken);
        }

        [HttpPost]
        public async Task SubmitOrder([FromBody] SubmitOrderCommand submitOrder, CancellationToken cancellationToken = default)
        {
            await _orderService.SubmitOrder(submitOrder, cancellationToken);
        }
    }
}
