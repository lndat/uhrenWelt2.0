using uhrenWelt.Data;
using uhrenWelt.Interfaces;
using uhrenWelt.Models;

namespace uhrenWelt.Services
{
    public class OrderService : IOrderService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task CreateNewOrder(AppUser user)
        {


            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                // await db.Orders.AddAsync();
                await db.SaveChangesAsync();
            }
        }
    }
}