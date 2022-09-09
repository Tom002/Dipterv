using Dipterv.Dal.DbContext;
using Dipterv.Shared.Dto.ShoppingCart;
using Dipterv.Dal.Model;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.ComputeServices;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.EntityFramework;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Dipterv.Bll.Services
{
    public class ShoppingCartService : DbServiceBase<FusionDbContext>, IShoppingCartService
    {
        private readonly IDbEntityResolver<int, ShoppingCartItem> _shoppingCartItemResolver;
        private readonly IDbEntityResolver<string, ShoppingCartItem> _shoppingCartItemResolverByCartKey;
        private readonly IAuth _authService;
        private readonly IMapper _mapper;

        public ShoppingCartService(
            IMapper mapper,
            IServiceProvider services,
            IAuth authService,
            IDbEntityResolver<int, ShoppingCartItem> shoppingCartItemResolver,
            IDbEntityResolver<string, ShoppingCartItem> shoppingCartItemResolverByCartKey)
            : base(services)
        {
            _mapper = mapper;
            _shoppingCartItemResolver = shoppingCartItemResolver;
            _shoppingCartItemResolverByCartKey = shoppingCartItemResolverByCartKey;
            _authService = authService;
        }

        [ComputeMethod]
        public async virtual Task<List<ShoppingCartItemDto>> GetCartItemsForCustomer(string shoppingCartKey, CancellationToken cancellationToken = default)
        {
            var cartItemIdList = await GetCartItemIdsForCustomer(shoppingCartKey);

            var cartItems = await Task.WhenAll(cartItemIdList.Select(async cartItemId =>
            {
                return await GetCartItem(cartItemId, cancellationToken);
            }));

            return cartItems.Where(c => c != null).ToList();
        }

        [ComputeMethod]
        public async virtual Task<List<int>> GetCartItemIdsForCustomer(string shoppingCartKey, CancellationToken cancellationToken = default)
        {
            await using var dbContext = CreateDbContext(readWrite: false);

            return await dbContext.ShoppingCartItems
                .Where(s => s.ShoppingCartId == shoppingCartKey)
                .Select(s => s.ShoppingCartItemId)
                .ToListAsync();
        }

        [ComputeMethod]
        public virtual async Task<ShoppingCartItemDto?> GetCartItem (int cartItemId, CancellationToken cancellationToken = default)
        {
            var cartItem = await _shoppingCartItemResolver.Get(cartItemId, cancellationToken);
            if (cartItem != null)
                return _mapper.Map<ShoppingCartItemDto>(cartItem);

            return default;
        }

        [ComputeMethod]
        public virtual async Task<List<ShoppingCartItemDto>> GetManyCartItems(List<int> cartItemIdList, CancellationToken cancellationToken = default)
        {
            var cartItems = await Task.WhenAll(cartItemIdList.Select(async cartItemId =>
            {
                return await GetCartItem(cartItemId, cancellationToken);
            }));
            return cartItems.ToList();
        }


        [ComputeMethod]
        public virtual async Task<int?> GetCartItemIdByShoppingCartKeyAndProductId(string shoppingCartKey, int productId, CancellationToken cancellationToken = default)
        {
            var dbContext = CreateDbContext(readWrite: false);

            return await dbContext.ShoppingCartItems
                .Where(s => s.ShoppingCartId == shoppingCartKey && s.ProductId == productId)
                .Select(s => s.ShoppingCartItemId)
                .SingleOrDefaultAsync();
        }

        [ComputeMethod]
        public virtual async Task<ShoppingCartItemDto?> GetCartItemByShoppingCartKeyAndProductId(string shoppingCartKey, int productId, CancellationToken cancellationToken = default)
        {
            var cartItemId = await GetCartItemIdByShoppingCartKeyAndProductId(shoppingCartKey, productId, cancellationToken);
            if(cartItemId.HasValue)
                return await GetCartItem(cartItemId.Value, cancellationToken);

            return default;
        }

        [ComputeMethod]
        public virtual async Task<List<ShoppingCartItemDto>> GetCartItemsByProductId(int productId, CancellationToken cancellationToken = default)
        {
            var dbContext = CreateDbContext(readWrite: false);
            var cartItemIdList = await dbContext.ShoppingCartItems
                .Where(sci => sci.ProductId == productId)
                .Select(sci => sci.ShoppingCartItemId)
                .ToListAsync();

            return await GetManyCartItems(cartItemIdList, cancellationToken);   
        }
    }
}
