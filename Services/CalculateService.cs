namespace uhrenWelt.Services
{
    public static class CalculateService
    {
        public static decimal GetGrossPrice(decimal netUnitPrice, int taxRate) 
        {
            var totalPrice = netUnitPrice + (netUnitPrice / 100 * taxRate);
            return totalPrice;
        }
    }
}