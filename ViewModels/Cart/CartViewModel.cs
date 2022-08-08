
namespace uhrenWelt.ViewModels.Cart
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public int OrderLineId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public decimal NetUnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public string ManufacturerName { get; set; }
        public string ProductName { get; set; }
        public string ImagePath { get; set; }
        public string PriceTotal { get; set; }
        public string LinePrice { get; set; }
    }
}