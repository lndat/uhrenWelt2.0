
namespace uhrenWelt.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal PriceTotal { get; set; }
        public DateTime? DateOrdered { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public IEnumerable<OrderLine> OrderLines { get; set; }
        public int? VoucherId { get; set; }
        public Voucher Voucher { get; set; }
    }
}