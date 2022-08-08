using uhrenWelt.Models;

namespace uhrenWelt.Interfaces
{
    public interface IOrderService
    {
        Task CreateNewOrder(AppUser user);
    }
}