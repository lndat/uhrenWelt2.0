using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uhrenWelt.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string VoucherName { get; set; }
        public string VoucherValue { get; set; }
        public int VoucherDiscount { get; set; }
        public bool Active { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}