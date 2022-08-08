using uhrenWelt.Models;

namespace uhrenWelt.Interfaces
{
    public interface ICartService
    {
        Task CreateCartAsync(AppUser user);
        Task AddProductToCart(string appUserId, int productId, int quantity);
        Task ChangeCartQuantity(string appUserId, int changeQuantity, int orderLineId);
        Task RemoveProductFromCart(string appUserId, int orderLineId);
        Task<Order> GetCustomerCart(string appUserId);
    }
}