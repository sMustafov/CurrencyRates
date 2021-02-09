namespace CurrencyRates.Common
{
    /// <summary>
    /// Global constants class
    /// </summary>
    public static class GlobalConstants
    {
        /// <summary>
        /// Constants for exceptions.
        /// </summary>
        public const string ERROR_CurrencyPairNotProvided = "Not provided currencypair in url query!";
        public const string ERROR_CurrencyPairNotRightLength = "The currencypair must be 6 symbols!";

        public const string ERROR_BaseCurrencyDoesNotExists = "The Base Currency: {0} does not exists!";
        public const string ERROR_QuoteCurrencyDoesNotExists = "The Quote Currency: {0} does not exists!";

        public const string SUCCESS_CurrencyPairRateCalculated = "Successfully calculated currency pair rate!";
        public const string SUCCESS_StartedApplication = "Successfully started the application!";

        public const string ERROR_NoXmlData = "There is no XML data for deserializing!";
        /// <summary>
        /// Constants for XML.
        /// </summary>
        public const string XML_DOCUMENT_CACHE_KEY = "XmlDocument_Cache_Key";        

        /// <summary>
        /// Constants for Euro.
        /// </summary>
        public const string EURO_NAME = "EUR";
        public const decimal EURO_RATE = 1.0000m;
    }
}
