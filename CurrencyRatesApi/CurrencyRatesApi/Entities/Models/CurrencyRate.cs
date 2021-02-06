
namespace CurrencyRatesApi.Entities.Models
{
    public class CurrencyRate
    {
        public string BaseCurrency { get; set; }
        public string BaseCurrencyRate { get; set; }

        public string QuoteCurrency { get; set; }
        public string QuoteCurrencyRate { get; set; }
    }
}
