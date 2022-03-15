using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto.Order;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.EntityFrameworkCore;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.EntityFramework;

namespace Dipterv.Bll.Services
{
    public class OrderService : DbServiceBase<FusionDbContext>, IOrderService
    {
        private readonly IAuth _authService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public OrderService(
            IServiceProvider services,
            IAuth authService,
            ICustomerService customerService,
            IProductService productService,
            IMapper mapper) : base(services)
        {
            _authService = authService;
            _customerService = customerService;
            _productService = productService;
            _mapper = mapper;
        }

        public virtual async Task<List<OrderHeaderDto>> GetMyOrders(Session session, CancellationToken cancellationToken = default)
        {
            await using var dbContext = CreateDbContext();

            var sessionInfo = await _authService.GetSessionInfo(session, cancellationToken);
            if(sessionInfo.IsAuthenticated)
            {
                var user = await _authService.GetUser(session, CancellationToken.None);

                if(user.Claims.ContainsKey("read_orders"))
                {
                    var userId = long.Parse(user.Id.Value);
                    var applicationUser = await dbContext.Users.AsQueryable().SingleAsync(u => u.Id == userId);

                    if (applicationUser.CustomerId.HasValue)
                    {
                        return await dbContext.SalesOrderHeaders
                            .AsQueryable()
                            .Where(s => s.CustomerId == applicationUser.CustomerId.Value)
                            .ProjectTo<OrderHeaderDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);
                    }
                }
            }

            return new List<OrderHeaderDto>();
        }

        public virtual async Task SubmitOrder(SubmitOrderCommand submitOrder, CancellationToken cancellationToken = default)
        {
            if (Computed.IsInvalidating())
            {
                _ = GetMyOrders(submitOrder.Session, cancellationToken);
                return;
            }

            await using var dbContext = CreateDbContext(readWrite: true);

            var user = await _authService.GetUser(submitOrder.Session, CancellationToken.None);
            var hasClaim = user.Claims.TryGetValue("customer_id", out string customerId);

            if (hasClaim)
            {
                var customerIdInt = int.Parse(customerId);
                var customer = await _customerService.GetCustomer(customerIdInt);

                var product = await _productService.TryGet(submitOrder.ProductId, cancellationToken);

                var specialOfferProduct = await dbContext.SpecialOfferProducts.AsQueryable()
                    .SingleOrDefaultAsync(s => s.SpecialOfferId == 1 && s.ProductId == product.ProductId);

                var order = new SalesOrderHeader
                {
                    RevisionNumber = 8,
                    OrderDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(5),
                    Status = 1,
                    CustomerId = customerIdInt,

                    BillToAddressId = 1,
                    ShipToAddressId = 1,

                    ShipMethodId = 1,
                    SubTotal = product.ListPrice * submitOrder.Quantity,
                    TaxAmt = 10,
                    Freight = 10,

                    SalesOrderDetails = new List<SalesOrderDetail>
                    {
                        new SalesOrderDetail
                        {
                            OrderQty = submitOrder.Quantity,
                            ProductId = submitOrder.Quantity,
                            SpecialOfferId = specialOfferProduct.SpecialOfferId,
                            SpecialOfferProduct = specialOfferProduct,
                            UnitPrice = product.ListPrice,
                            UnitPriceDiscount = 0
                        }
                    }
                };

                try
                {
                    dbContext.SalesOrderHeaders.Add(order);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    var a = 5;
                }
            }
            else
            {
                throw new Exception("Only customers can have orders");
            }
        }
    }
}
