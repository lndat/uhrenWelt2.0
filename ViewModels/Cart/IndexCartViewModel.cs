
namespace uhrenWelt.ViewModels.Cart
{
    public class IndexCartViewModel
    {
    public List<CartViewModel> Cart { get; set; }
    public string PriceTotal { get; set; }
    public int QuantityTotal { get; set; }
    public string Voucher { get; set; }
    }
}