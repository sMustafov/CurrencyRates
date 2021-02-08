namespace CurrencyRatesApi.Common
{
    public static class GlobalConstants
    {
        /// <summary>
        /// Constants for logger.
        /// </summary>
        public const string ERROR_CurrencyPairNotProvided = "Not provided currencypair in url query!";
        public const string ERROR_CurrencyPairNotRightLength = "The currencypair must be 6 symbols!";
        public const string ERROR_BaseAndQuoteCurrenciesNotHaveNameOrRate = "The Base and Quote currencies should have names and rates!";
        public const string SUCCESS_CurrencyPairRateCalculated = "Successfully calculated currency pair rate!";
        public const string SUCCESS_StartedApplication = "Successfully started the application!";

        /// <summary>
        /// Constants for XML.
        /// </summary>
        public const string XML_DOCUMENT_CACHE_KEY = "XmlDocument_Cache_Key";
        public const string XML_DOCUMENT_URL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        /// <summary>
        /// Constants for Euro.
        /// </summary>
        public const string EURO_NAME = "EUR";
        public const decimal EURO_RATE = 1.00m;
    }
}
