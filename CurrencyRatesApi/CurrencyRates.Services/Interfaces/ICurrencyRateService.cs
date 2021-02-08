using CurrencyRates.Entities.Models;

namespace CurrencyRates.Interfaces
{
    /// <summary>
    /// Interface for CurrencyRateService
    /// </summary>
    public interface ICurrencyRateService
    {
        /// <summary>
        /// Calculates currency pair rate
        /// </summary>
        /// <param name="currencyPair">The given currency pair</param>
        /// <returns></returns>
        public CurrencyPair CalculateCurrencyPairRate(string currencyPair);
    }
}
