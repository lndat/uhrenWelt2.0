
namespace uhrenWelt.ViewModels.Order
{
    public class OrderViewModel
    {        
        public int OrderId { get; set; }
        public string PriceTotal { get; set; }
        public DateTime? DateOrdered { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? VoucherId { get; set; }
        public IList<OrderLineViewModel> OrderLines { get; set; }
        public OrderViewModel()
        {
            OrderLines = new List<OrderLineViewModel>();
        }
    }
}