using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto.Order;
using Dipterv.Shared.Exceptions;
using Dipterv.Shared.Helper;
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
        private readonly IProductInventoryService _productInventoryService;
        private readonly IShoppingCartDetailsService _shoppingCartDetailsService;
        private readonly IMapper _mapper;

        public OrderService(
            IServiceProvider services,
            IAuth authService,
            ICustomerService customerService,
            IProductService productService,
            IProductInventoryService productInventoryService,
            IShoppingCartDetailsService shoppingCartDetailsService,
            IMapper mapper) : base(services)
        {
            _authService = authService;
            _customerService = customerService;
            _productService = productService;
            _productInventoryService = productInventoryService;
            _shoppingCartDetailsService = shoppingCartDetailsService;
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

        public virtual async Task SubmitOrder(SubmitOrderCommand command, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();

            if (Computed.IsInvalidating())
            {
                _ = GetMyOrders(command.Session, cancellationToken);
                _ = _productInventoryService.GetInventoriesForProduct(3, cancellationToken);
                _ = _productInventoryService.ProductGetTotalStock(3, cancellationToken);
                return;
            }

            await using var dbContext = await CreateCommandDbContext(cancellationToken);

            var shoppingCart = await _shoppingCartDetailsService.GetShoppingCartForCustomer(command.Session);

            if (!shoppingCart.CanSendOrder)
                throw new BusinessException("Can't send this order currently");

            if (!shoppingCart.ShoppingCartItems.Any())
                throw new BusinessException("Can't send order without items");

            int? customerId = null;
            var user = await _authService.GetUser(command.Session, cancellationToken);
            if (user.IsAuthenticated && user.Claims.ContainsKey("customer_id"))
            {
                customerId = int.Parse(user.Claims["customer_id"]);
            }

            // Product inventory
            var productId = shoppingCart.ShoppingCartItems.First().ProductId;
            var productInventory = await dbContext.ProductInventories.Where(pi => pi.ProductId == productId).FirstAsync();

            productInventory.Quantity -= Convert.ToInt16(shoppingCart.ShoppingCartItems.First().Quantity);
            await dbContext.SaveChangesAsync();

            //var order = new SalesOrderHeader
            //{
            //    RevisionNumber = 8,
            //    OrderDate = DateTime.Now,
            //    DueDate = DateTime.Now.AddDays(5),
            //    Status = 1,
            //    // TODO: Guest user order
            //    CustomerId = customerId.HasValue ? customerId.Value : 1,

            //    BillToAddressId = 1,
            //    ShipToAddressId = 1,
            //    ShipMethodId = 1,
            //    SubTotal = product.ListPrice * submitOrder.Quantity,
            //    TaxAmt = 10,
            //    Freight = 10,

            //    SalesOrderDetails = new List<SalesOrderDetail>
            //        {
            //            new SalesOrderDetail
            //            {
            //                OrderQty = submitOrder.Quantity,
            //                ProductId = submitOrder.Quantity,
            //                SpecialOfferId = specialOfferProduct.SpecialOfferId,
            //                SpecialOfferProduct = specialOfferProduct,
            //                UnitPrice = product.ListPrice,
            //                UnitPriceDiscount = 0
            //            }
            //        }
            //};

            //try
            //{
            //    dbContext.SalesOrderHeaders.Add(order);
            //    await dbContext.SaveChangesAsync();
            //}
            //catch (Exception e)
            //{
            //    var a = 5;
            //}
        }
    }
}
