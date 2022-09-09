using AutoMapper;
using Dipterv.Bll.Helper;
using Dipterv.Bll.Interfaces;
using Dipterv.Dal.DbContext;
using Dipterv.Dal.Model;
using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Dto.ShoppingCart;
using Dipterv.Shared.Exceptions;
using Dipterv.Shared.Helper;
using Dipterv.Shared.Interfaces;
using Dipterv.Shared.Interfaces.ComputeServices;
using Microsoft.EntityFrameworkCore;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.EntityFramework;

namespace Dipterv.Bll.Services
{
    public class ShoppingCartDetailsService : DbServiceBase<FusionDbContext>, IShoppingCartDetailsService
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISpecialOfferService _specialOfferService;
        private readonly IProductInventoryService _productInventoryService;
        private readonly IAuth _authService;
        private readonly IMapper _mapper;
        private readonly ICommandInvalidationHelperService _commandInvalidationHelperService;

        public ShoppingCartDetailsService(
            IMapper mapper,
            IServiceProvider services,
            IAuth authService,
            IShoppingCartService shoppingCartService,
            ISpecialOfferService specialOfferService,
            IProductInventoryService productInventoryService,
            IProductService productService,
            ICommandInvalidationHelperService commandInvalidationHelperService)
            : base(services)
        {
            _mapper = mapper;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _specialOfferService = specialOfferService;
            _productInventoryService = productInventoryService;
            _authService = authService;
            _commandInvalidationHelperService = commandInvalidationHelperService;
        }

        [ComputeMethod]
        public async virtual Task<ShoppingCartDto> GetShoppingCartForCustomer(Session session, CancellationToken cancellationToken = default)
        {
            string shoppingCartKey = null;
            var user = await _authService.GetUser(session, cancellationToken);
            if (user.IsAuthenticated && user.Claims.ContainsKey("customer_id"))
            {
                var customerId = int.Parse(user.Claims["customer_id"]);
                shoppingCartKey = ShoppingCartHelper.GetCustomerShoppingCartKey(customerId);
            }
            else
            {
                shoppingCartKey = ShoppingCartHelper.GetGuestShoppingCartKey(session.Id);
            }

            var cartItems = await _shoppingCartService.GetCartItemsForCustomer(shoppingCartKey, cancellationToken);
            if (cartItems.Any())
            {
                var productIds = cartItems.Select(c => c.ProductId).ToList();
                var products = await _productService.TryGetManyProducts(productIds, cancellationToken);

                var specialOfferIds = products.SelectMany(p => p.SpecialOfferIds).ToList();
                var specialOffers = await _specialOfferService.TryGetMany(specialOfferIds, cancellationToken);
                var inventories = await _productInventoryService.GetInventoriesForProductIdList(products.Select(p => p.ProductId).ToList(), cancellationToken);
                var shoppingCartItems = await _shoppingCartService.GetManyCartItems(products.SelectMany(p => p.ShoppingCartItemIds).ToList(), cancellationToken);
                var primaryImageIds = products.Where(p => p.PrimaryProductPhotoId.HasValue).ToDictionary(p => p.ProductId, p => p.PrimaryProductPhotoId.Value);
                var primaryProductImages = await _productService.TryGetManyProductPhotos(primaryImageIds.Values.ToList(), cancellationToken);

                var result = new ShoppingCartDto();
                foreach (var item in cartItems)
                {
                    var itemDetails = _mapper.Map<ShoppingCartItemDetailsDto>(item);

                    var correspondingProduct = _mapper.Map<ListProductDto>(products.Single(p => p.ProductId == item.ProductId)); 

                    var correspondingSpecialOffers = specialOffers.Where(s => s.ValidForProductIds.Contains(item.ProductId)).ToList();
                    ProductHelper.FillSpecialOfferData(correspondingProduct, correspondingSpecialOffers);

                    var correspondingInventories = inventories.Where(i => i.ProductId == item.ProductId).ToList();
                    ProductHelper.FillInventoryData(correspondingProduct, correspondingInventories);

                    var correspondingShoppingCartItems = shoppingCartItems.Where(r => r.ProductId == correspondingProduct.ProductId).ToList();
                    ProductHelper.FillShoppingCartData(correspondingProduct, correspondingShoppingCartItems, correspondingInventories.Sum(pi => pi.Quantity), shoppingCartKey);

                    if (primaryImageIds.TryGetValue(correspondingProduct.ProductId, out int primaryImageId))
                    {
                        var correspondingProductImage = primaryProductImages.First(p => p.ProductPhotoId == primaryImageId);
                        correspondingProduct.ThumbnailImage = correspondingProductImage.ThumbNailPhoto;
                    }

                    itemDetails.AvailableSpecialOffers = correspondingSpecialOffers;
                    itemDetails.Product = correspondingProduct;

                    result.ShoppingCartItems.Add(itemDetails);
                }

                var allProductsCanBeBought = products.All(p => p.CanBuyProduct);
                var allProductsInStock = result.ShoppingCartItems.All(s => s.Quantity <= s.Product.CurrentStock);
                var allProductsReserved = result.ShoppingCartItems.All(s => s.Quantity == s.ReservedQuantity);

                result.CanSendOrder = allProductsCanBeBought && allProductsInStock && allProductsReserved;

                return result;
            }

            return new ShoppingCartDto();
        }

        [CommandHandler]
        public async virtual Task<ShoppingCartItemDto> AddProductToCart(AddShoppingCartItemCommand command, CancellationToken cancellationToken = default)
        {
            string shoppingCartKey = null;
            var user = await _authService.GetUser(command.Session, cancellationToken);
            if (user != null && user.IsAuthenticated && user.Claims.ContainsKey("customer_id"))
            {
                var customerId = int.Parse(user.Claims["customer_id"]);
                shoppingCartKey = ShoppingCartHelper.GetCustomerShoppingCartKey(customerId);
            }
            else
            {
                shoppingCartKey = ShoppingCartHelper.GetGuestShoppingCartKey(command.Session.Id);
            }

            if (Computed.IsInvalidating())
            {
                _ = _shoppingCartService.GetCartItemsByProductId(command.ProductId, cancellationToken);
                _ = _productService.TryGetProduct(command.ProductId, cancellationToken);
                _ = _shoppingCartService.GetCartItemIdsForCustomer(shoppingCartKey, cancellationToken);
                _ = _shoppingCartService.GetCartItemIdByShoppingCartKeyAndProductId(shoppingCartKey, command.ProductId, cancellationToken);

                // Remove these
                _ = _shoppingCartService.GetCartItemIdsForCustomer(ShoppingCartHelper.GetCustomerShoppingCartKey(1), cancellationToken);
                _ = _shoppingCartService.GetCartItemIdsForCustomer(ShoppingCartHelper.GetCustomerShoppingCartKey(2), cancellationToken);
                _ = _shoppingCartService.GetCartItemIdsForCustomer(ShoppingCartHelper.GetCustomerShoppingCartKey(1002), cancellationToken);
                _ = _shoppingCartService.GetCartItemIdByShoppingCartKeyAndProductId(ShoppingCartHelper.GetCustomerShoppingCartKey(1), command.ProductId, cancellationToken);
                _ = _shoppingCartService.GetCartItemIdByShoppingCartKeyAndProductId(ShoppingCartHelper.GetCustomerShoppingCartKey(2), command.ProductId, cancellationToken);
                _ = _shoppingCartService.GetCartItemIdByShoppingCartKeyAndProductId(ShoppingCartHelper.GetCustomerShoppingCartKey(1002), command.ProductId, cancellationToken);
                return default;
            }

            var dbContext = await CreateCommandDbContext(cancellationToken);

            var product = await _productService.TryGetProduct(command.ProductId, cancellationToken);
            if (product == null || !product.CanBuyProduct)
                throw new BusinessException($"Product with id: {product.ProductId} was not found");

            var inventories = await _productInventoryService.GetInventoriesForProduct(product.ProductId, cancellationToken);
            var currentStock = inventories.Sum(i => i.Quantity);

            if (currentStock < command.Quantity)
                throw new BusinessException($"Selected quantity is no longer in stock");

            var existingCartItems = await _shoppingCartService.GetCartItemsByProductId(command.ProductId, cancellationToken);
            if (existingCartItems.Any(c => c.ShoppingCartId == shoppingCartKey))
                throw new BusinessException($"This item is already in the cart");

            var reservedQuantity = (existingCartItems.Sum(c => (int?) c.ReservedQuantity)) ?? 0;
            var freeQuantity = (currentStock - reservedQuantity);

            var newShoppingCartItem = new ShoppingCartItem
            {
                DateCreated = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ProductId = command.ProductId,
                Quantity = command.Quantity,
                ShoppingCartId = shoppingCartKey,
                ReservedQuantity = Math.Min(freeQuantity, command.Quantity)
            };
            dbContext.ShoppingCartItems.Add(newShoppingCartItem);
            await dbContext.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ShoppingCartItemDto>(newShoppingCartItem);
        }

        [CommandHandler]
        public async virtual Task RemoveProductFromCart(RemoveShoppingCartItemCommand command, CancellationToken cancellationToken = default)
        {
            string shoppingCartKey = null;
            var user = await _authService.GetUser(command.Session, cancellationToken);
            if (user != null && user.IsAuthenticated && user.Claims.ContainsKey("customer_id"))
            {
                var customerId = int.Parse(user.Claims["customer_id"]);
                shoppingCartKey = ShoppingCartHelper.GetCustomerShoppingCartKey(customerId);
            }
            else
            {
                shoppingCartKey = ShoppingCartHelper.GetGuestShoppingCartKey(command.Session.Id);
            }

            if (Computed.IsInvalidating())
            {
                _ = _shoppingCartService.GetCartItemsByProductId(command.ProductId);
                _ = _productService.TryGetProduct(command.ProductId, cancellationToken);
                _ = _shoppingCartService.GetCartItem(command.CartItemId, cancellationToken);
                _ = _shoppingCartService.GetCartItemIdsForCustomer(shoppingCartKey, cancellationToken);

                var cartItemsToInvalidate = _commandInvalidationHelperService.GetShoppingCartItemsToInvalidateAfterRemove(command);
                if (cartItemsToInvalidate != null && cartItemsToInvalidate.Any())
                    foreach (var itereservedItemId in cartItemsToInvalidate)
                    {
                        _ = _shoppingCartService.GetCartItem(itereservedItemId, cancellationToken);
                    }
                return;
            }

            await using var dbContext = await CreateCommandDbContext(cancellationToken);

            var shoppingCartItems = await dbContext.ShoppingCartItems
                .Where(s => s.ProductId == command.ProductId)
                .ToListAsync();

            var productStock = await _productInventoryService.ProductGetTotalStock(command.ProductId, cancellationToken);
            var reservedQuantity = (shoppingCartItems.Sum(sci => (int?)sci.ReservedQuantity)) ?? 0;
            var newFreeQuantity = (productStock - reservedQuantity) + command.Quantity;

            var unreservedShoppingCartItems = shoppingCartItems
                .Where(sci => sci.ShoppingCartItemId != command.CartItemId &&
                              sci.ReservedQuantity < sci.Quantity)
                .OrderByDescending(sci => sci.DateCreated).ToList();
            var reservedItemIdList = new List<int>();
            if(unreservedShoppingCartItems.Any())
            {
                var itemToReserve = unreservedShoppingCartItems.First();
                while (itemToReserve != null && (itemToReserve.Quantity - itemToReserve.ReservedQuantity) <= newFreeQuantity)
                {
                    newFreeQuantity -= (itemToReserve.Quantity - itemToReserve.ReservedQuantity);
                    itemToReserve.ReservedQuantity = itemToReserve.Quantity;
                    reservedItemIdList.Add(itemToReserve.ShoppingCartItemId);
                    unreservedShoppingCartItems.Remove(itemToReserve);
                    itemToReserve = unreservedShoppingCartItems.FirstOrDefault();
                }
            }
            _commandInvalidationHelperService.AddShoppingCartItemsToInvalidateAfterRemove(command, reservedItemIdList);

            var shoppingCartItemToRemove = await dbContext.ShoppingCartItems
               .SingleOrDefaultAsync(s => s.ShoppingCartItemId == command.CartItemId &&
                                          s.ShoppingCartId == shoppingCartKey &&
                                          s.ProductId == command.ProductId &&
                                          s.Quantity == command.Quantity);

            if (shoppingCartItemToRemove == null)
                throw new BusinessException($"The shopping cart item doesn't exist");

            dbContext.ShoppingCartItems.Remove(shoppingCartItemToRemove);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        [CommandHandler]
        public async virtual Task<ShoppingCartItemDto> UpdateProductQuantityInCart(UpdateShoppingCartItemCommand command, CancellationToken cancellationToken = default)
        {
            if (Computed.IsInvalidating())
            {
                _ = _shoppingCartService.GetCartItem(command.CartItemId, cancellationToken);
                var cartItemsToInvalidate = _commandInvalidationHelperService.GetShoppingCartItemsToInvalidateAfterUpdate(command);
                if (cartItemsToInvalidate != null && cartItemsToInvalidate.Any())
                {
                    foreach (var cartItemToInvalidate in cartItemsToInvalidate)
                    {
                        _ = _shoppingCartService.GetCartItem(cartItemToInvalidate, cancellationToken);
                    }
                }

                return default;
            }

            string shoppingCartKey = null;
            var user = await _authService.GetUser(command.Session, cancellationToken);
            if (user != null && user.IsAuthenticated && user.Claims.ContainsKey("customer_id"))
            {
                var customerId = int.Parse(user.Claims["customer_id"]);
                shoppingCartKey = ShoppingCartHelper.GetCustomerShoppingCartKey(customerId);
            }
            else
            {
                shoppingCartKey = ShoppingCartHelper.GetGuestShoppingCartKey(command.Session.Id);
            }

            await using var dbContext = await CreateCommandDbContext(cancellationToken);
            var shoppingCartItem = await dbContext.ShoppingCartItems.SingleOrDefaultAsync(s => s.ShoppingCartItemId == command.CartItemId && s.ShoppingCartId == shoppingCartKey);

            if (shoppingCartItem == null)
                throw new BusinessException($"The shopping cart item doesn't exist");

            var productStock = await _productInventoryService.ProductGetTotalStock(command.ProductId, cancellationToken);
            var shoppingCartItems = await dbContext.ShoppingCartItems
                    .Where(s => s.ProductId == command.ProductId)
                    .ToListAsync();
            var reservedQuantity = (shoppingCartItems.Sum(sci => (int?)sci.ReservedQuantity)) ?? 0;
            var freeQuantity = productStock - reservedQuantity;

            if (command.Quantity < shoppingCartItem.Quantity && shoppingCartItem.Quantity != shoppingCartItem.ReservedQuantity)
            {
                var reserveQuantityToRedistribute = Math.Max(shoppingCartItem.ReservedQuantity - command.Quantity, 0);
                if(reserveQuantityToRedistribute > 0)
                {
                    shoppingCartItem.ReservedQuantity = command.Quantity;
                    var unreservedShoppingCartItems = shoppingCartItems
                        .Where(sci => sci.ShoppingCartItemId != command.CartItemId &&
                                      sci.ReservedQuantity < sci.Quantity)
                        .OrderByDescending(sci => sci.DateCreated).ToList();
                    var reservedItemIdList = new List<int>();
                    if (unreservedShoppingCartItems.Any())
                    {
                        var itemToReserve = unreservedShoppingCartItems.First();
                        while (itemToReserve != null && reserveQuantityToRedistribute > 0)
                        {
                            var itemFreeReserveCapacity = itemToReserve.Quantity - itemToReserve.ReservedQuantity;
                            var redistributeQuantity = Math.Min(itemFreeReserveCapacity, reserveQuantityToRedistribute);

                            itemToReserve.ReservedQuantity += redistributeQuantity;
                            reserveQuantityToRedistribute -= redistributeQuantity;

                            reservedItemIdList.Add(itemToReserve.ShoppingCartItemId);
                            unreservedShoppingCartItems.Remove(itemToReserve);
                            itemToReserve = unreservedShoppingCartItems.FirstOrDefault();
                        }
                        _commandInvalidationHelperService.AddShoppingCartItemsToInvalidateAfterUpdate(command, reservedItemIdList);
                    }
                }
            }
            else if(command.Quantity > shoppingCartItem.Quantity && shoppingCartItem.Quantity != shoppingCartItem.ReservedQuantity)
            {
                // Semmi teendő
            }
            else if(command.Quantity < shoppingCartItem.Quantity && shoppingCartItem.Quantity == shoppingCartItem.ReservedQuantity)
            {
                var reserveQuantityToRedistribute = shoppingCartItem.Quantity - command.Quantity;
                shoppingCartItem.ReservedQuantity = command.Quantity;

                var unreservedShoppingCartItems = shoppingCartItems
                    .Where(sci => sci.ShoppingCartItemId != command.CartItemId &&
                                  sci.ReservedQuantity < sci.Quantity)
                    .OrderByDescending(sci => sci.DateCreated).ToList();
                var reservedItemIdList = new List<int>();
                if (unreservedShoppingCartItems.Any())
                {
                    var itemToReserve = unreservedShoppingCartItems.First();
                    while (itemToReserve != null && reserveQuantityToRedistribute > 0)
                    {
                        var itemFreeReserveCapacity = itemToReserve.Quantity - itemToReserve.ReservedQuantity;
                        var redistributeQuantity = Math.Min(itemFreeReserveCapacity, reserveQuantityToRedistribute);

                        itemToReserve.ReservedQuantity += redistributeQuantity;
                        reserveQuantityToRedistribute -= redistributeQuantity;

                        reservedItemIdList.Add(itemToReserve.ShoppingCartItemId);
                        unreservedShoppingCartItems.Remove(itemToReserve);
                        itemToReserve = unreservedShoppingCartItems.FirstOrDefault();
                    }
                    _commandInvalidationHelperService.AddShoppingCartItemsToInvalidateAfterUpdate(command, reservedItemIdList);
                }
            }
            else if (command.Quantity > shoppingCartItem.Quantity && shoppingCartItem.Quantity == shoppingCartItem.ReservedQuantity)
            {
                var cartItemQuantityIncrease = command.Quantity - shoppingCartItem.Quantity;
                var reserveIncreaseCapacity = Math.Min(freeQuantity, cartItemQuantityIncrease);

                shoppingCartItem.ReservedQuantity += reserveIncreaseCapacity;
            }

            shoppingCartItem.Quantity = command.Quantity;
            shoppingCartItem.ModifiedDate = DateTime.Now;
            await dbContext.SaveChangesAsync();

            return _mapper.Map<ShoppingCartItemDto>(shoppingCartItem);
        }
    }
}
