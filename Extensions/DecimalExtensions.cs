namespace uhrenWelt.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToPriceString(this decimal value, string currencyText)
        {
            return value.ToString("N2") + " " + currencyText;
        }
    }
}