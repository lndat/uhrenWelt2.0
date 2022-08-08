using Microsoft.EntityFrameworkCore;
using uhrenWelt.Data;
using uhrenWelt.Interfaces;
using uhrenWelt.Models;

namespace uhrenWelt.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public VoucherService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<bool> CheckVoucher(string voucher)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var voucherCheck = await db.Vouchers
                   .Where(v => v.VoucherValue.ToLower() == voucher.ToLower() && v.Active == true)
                   .FirstOrDefaultAsync();

                if (voucherCheck != null) // TODO finish check 
                {
                    return true;
                }
            }

            return false;
        }

        public async Task CalculateVoucherAsync(string voucher, int orderId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                var getVoucher = await db.Vouchers
                   .Where(v => v.VoucherValue.ToLower() == voucher.ToLower() && v.Active == true)
                   .FirstOrDefaultAsync();

                var GetOrderToCalculate = await db.Orders
                    .Where(o => o.Id == orderId)
                    .FirstOrDefaultAsync();
 
                var totalPrice = GetOrderToCalculate.PriceTotal;
                var discount = (totalPrice /= 100) * getVoucher.VoucherDiscount;
                
                GetOrderToCalculate.VoucherId = getVoucher.Id;
                GetOrderToCalculate.PriceTotal -= discount;
                db.Entry(GetOrderToCalculate).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
    }
}