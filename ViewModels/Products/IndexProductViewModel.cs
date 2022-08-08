namespace uhrenWelt.ViewModels.Products
{
    public class IndexProductViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Price { get; set; }
        public string ManufacturerName { get; set; }
        public int ManufacturerId { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}