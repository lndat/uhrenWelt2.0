
namespace uhrenWelt.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TaxRate { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}