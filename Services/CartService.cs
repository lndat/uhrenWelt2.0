using Microsoft.EntityFrameworkCore;
using uhrenWelt.Data;
using uhrenWelt.Interfaces;
using uhrenWelt.Models;

namespace uhrenWelt.Services
{
    public class CartService : ICartService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IProductService _productService;

        public CartService(IServiceScopeFactory scopeFactory, IProductService productService)
        {
            _productService = productService;
            _scopeFactory = scopeFactory;
        }

        public async Task CreateCartAsync(AppUser user)
        {
            var cart = new Order
            {
                AppUserId = user.Id,
                DateOrdered = null,
                PriceTotal = 0,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Street = user.Street,
                City = user.City,
                Zip = user.Zip
            };

            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                await db.Orders.AddAsync(cart);
                await db.SaveChangesAsync();
            }
        }

        public async Task AddProductToCart(string appUserId, int productId, int quantity)
        {
            var cart = await GetCustomerCart(appUserId);
            var orderLine = cart.OrderLines.FirstOrDefault(ol => ol.ProductId == productId);

            if (orderLine != null)
            {
                orderLine.Quantity += quantity;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                    db.Entry(orderLine).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
            else
            {
                var product = await _productService.GetProductById(productId);

                var newOrderLine = new OrderLine
                {
                    Quantity = quantity,
                    NetUnitPrice = product.NetUnitPrice,
                    OrderId = cart.Id,
                    ProductId = productId,
                    TaxRate = product.Category.TaxRate
                };

                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                    db.Entry(newOrderLine).State = EntityState.Added;
                    await db.SaveChangesAsync();
                }
            }
             
            await UpdateCartTotal(appUserId);
        }

        public async Task ChangeCartQuantity(string appUserId, int changeQuantity, int orderLineId)
        {
            var cart = await GetCustomerCart(appUserId);
            var orderLine = cart.OrderLines.FirstOrDefault(ol => ol.Id == orderLineId);

            if (orderLine.Quantity <= 0)
            {
                await RemoveProductFromCart(appUserId, orderLineId);
            }
            else
            {
                orderLine.Quantity += changeQuantity;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                    db.Entry(orderLine).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }    

            await UpdateCartTotal(appUserId);
        }

        public async Task RemoveProductFromCart(string appUserId, int orderLineId)
        {
            var cart = await GetCustomerCart(appUserId);
            
            using (var scope = _scopeFactory.CreateScope())
            {
                var orderLineToDelete = new OrderLine
                {
                    Id = orderLineId
                };

                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Entry(orderLineToDelete).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }

            await UpdateCartTotal(appUserId);
        }

        private async Task UpdateCartTotal(string appUserId)
        {
            var cart = await GetCustomerCart(appUserId);
            cart.PriceTotal = cart.OrderLines.Sum(ol => ol.Quantity * Services.CalculateService.GetGrossPrice(ol.NetUnitPrice, ol.TaxRate));

            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Entry(cart).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public async Task<Order> GetCustomerCart(string appUserId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                return await db.Orders
                    .Include(o => o.OrderLines)
                    .Where(o => o.AppUserId == appUserId && o.DateOrdered == null)
                    .FirstOrDefaultAsync();
            }
        }
    }
}