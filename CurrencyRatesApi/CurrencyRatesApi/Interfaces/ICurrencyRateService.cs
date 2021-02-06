using CurrencyRatesApi.Entities.Models;

namespace CurrencyRatesApi.Interfaces
{
    public interface ICurrencyRateService
    {
        public CurrencyPair CalculateCurrencyPairRate(string currencyPair);
    }
}
