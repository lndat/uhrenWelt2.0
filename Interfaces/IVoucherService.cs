using uhrenWelt.Models;

namespace uhrenWelt.Interfaces
{
    public interface IVoucherService
    {
        Task<bool> CheckVoucher(string voucher);
        Task CalculateVoucherAsync(string voucher, int orderId);

    }
}