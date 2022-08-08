namespace uhrenWelt.ViewModels.Products
{
    public class Top5ViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal NetUnitPrice { get; set; }
        public string UnitPrice { get; set; }
        public string ManufacturerName { get; set; }
        public string ImagePath { get; set; }
        public int Quantity { get; set; }
        public int TaxRate { get; set; }
    }
}