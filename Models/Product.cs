
namespace uhrenWelt.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal NetUnitPrice { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public IEnumerable<OrderLine> OderLines { get; set; }
    }
}