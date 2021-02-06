namespace CurrencyRatesApi.Utils
{
    public static class GlobalConstants
    {
        // Logs
        public const string ERROR_CurrencyPairNotProvided = "Not provided currencypair in url query!";
        public const string ERROR_CurrencyPairNotRightLength = "The currencypair must be 6 symbols!";
        public const string ERROR_BaseAndQuoteCurrenciesNotHaveNameOrRate = "The Base and Quote currencies should have names and rates!";
        public const string SUCCESS_CurrencyPairRateCalculated = "Successfully calculated currency pair rate!";

        // XML
        public const string XML_DOCUMENT_CACHE_KEY = "XmlDocument_Cache_Key";
        public const string XML_DOCUMENT_URL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

    }
}
