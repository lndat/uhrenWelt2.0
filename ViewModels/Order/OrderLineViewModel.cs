using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uhrenWelt.ViewModels.Order
{
    public class OrderLineViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public int Quantity { get; set; }
        public string NetUnitPrice { get; set; }
        public string LinePrice { get; set; }
        public int TaxRate { get; set; }
    }
}