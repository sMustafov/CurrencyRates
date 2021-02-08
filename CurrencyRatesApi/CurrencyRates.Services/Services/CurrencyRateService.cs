using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

using Microsoft.Extensions.Caching.Memory;

using CurrencyRates.Common;
using CurrencyRates.Entities.Models;
using CurrencyRates.Entities.XmlModel;
using CurrencyRates.Interfaces;
using CurrencyRates.Services.Utils.Exceptions;

namespace CurrencyRates.Services
{
    /// <summary>
    /// Implements the ICurrencyRateService
    /// </summary>
    public class CurrencyRateService : ICurrencyRateService
    {
        /// <summary> The <see cref="IMemoryCache"/>.</summary>
        private readonly IMemoryCache memoryCache;

        /// <summary> The <see cref="WebClient"/>.</summary>
        private readonly WebClient webClient;

        /// <summary> The <see cref="XmlSerializer"/>.</summary>
        private readonly XmlSerializer xmlSerializer;

        /// <summary> The xml data as string</summary>
        private string xmlData;

        /// <summary> The <see cref="EcbEnvelope"/>.</summary>
        private EcbEnvelope ecbEnvelope;

        /// <summary> The <see cref="BaseCurrency"/>.</summary>
        private BaseCurrency baseCurrency;

        /// <summary> The <see cref="QuoteCurrency"/>.</summary>
        private QuoteCurrency quoteCurrency;

        /// <summary> The <see cref="CurrencyPair"/>.</summary>
        private CurrencyPair currencyPair;

        /// <summary>
        /// Initialize new instance of this class
        /// </summary>
        /// <param name="memoryCache">The <see cref="IMemoryCache"/>.</param>
        public CurrencyRateService(IMemoryCache memoryCache)
        {
            this.xmlData = null;
            this.memoryCache = memoryCache;
            this.webClient = new WebClient();
            this.xmlSerializer = new XmlSerializer(typeof(EcbEnvelope));
        }

        /// <summary>
        /// Calculates the Currency pair rate
        /// </summary>
        /// <param name="currencyPairFromUrl">The currency pair which is give from the url.</param>
        /// <returns></returns>
        public CurrencyPair CalculateCurrencyPairRate(string currencyPairFromUrl)
        {
            if (currencyPairFromUrl == null)
            {
                throw new CustomInvalidOperationException(GlobalConstants.ERROR_CurrencyPairNotProvided);
            }

            if (currencyPairFromUrl.Length != 6)
            {
                throw new CustomInvalidOperationException(GlobalConstants.ERROR_CurrencyPairNotRightLength);
            }

            var givenBaseCurrency = currencyPairFromUrl.Substring(0, 3);
            var givenQuoteCurrency = currencyPairFromUrl.Substring(currencyPairFromUrl.Length - 3);

            // If base and quote currencies are the same, we do not need to download the xml file, we can just return rate = 1
            if (this.CheckIfBaseAndQuoteCurrenciesSame(givenBaseCurrency, givenQuoteCurrency))
            {
                this.currencyPair = new CurrencyPair
                {
                    Name = currencyPairFromUrl,
                    Rate = 1
                };

                return currencyPair;
            }

            // Add to in-memory cache or get it is already there
            this.AddOrGetInMemoryCache();

            // Extracting the info for given currencyPairFromUrl
            this.ExtractCurrencyAndRateFromXml(givenBaseCurrency, givenQuoteCurrency);

            if (this.baseCurrency.Name == null || this.quoteCurrency.Name == null)
            {
                throw new CustomArgumentException(GlobalConstants.ERROR_BaseAndQuoteCurrenciesNotHaveNameOrRate);
            }

            // Calculating currency pair rate
            // E.g. currencyPair = GBPUSD => EURUSD / EURGBP
            var currencyPairRate = Math.Round((this.quoteCurrency.Rate / this.baseCurrency.Rate), 4, MidpointRounding.AwayFromZero);

            // Creating currency pair
            this.currencyPair = new CurrencyPair
            {
                Name = baseCurrency.Name + quoteCurrency.Name,
                Rate = currencyPairRate
            };

            return this.currencyPair;
        }

        /// <summary>
        /// Extract currency and rate from xml
        /// </summary>
        /// <param name="givenBaseCurrency">The given base currency</param>
        /// <param name="givenQuoteCurrency">The give quote currency</param>
        private void ExtractCurrencyAndRateFromXml(string givenBaseCurrency, string givenQuoteCurrency)
        {
            this.DeserializeString();

            this.CreateBaseCurrency(givenBaseCurrency);
            this.CreateQuoteCurrency(givenQuoteCurrency);
        }

        /// <summary>
        /// Deserializes the xml string data
        /// </summary>
        private void DeserializeString()
        {
            using (var stringReader = new StringReader(this.xmlData))
            {
                this.ecbEnvelope = this.xmlSerializer.Deserialize(stringReader) as EcbEnvelope;
            }
        }

        /// <summary>
        /// Creates a Base Currency
        /// </summary>
        /// <param name="givenBaseCurrency">The given base currency</param>
        private void CreateBaseCurrency(string givenBaseCurrency)
        {
            var baseCurrencyInfo = this.ecbEnvelope.CubeRootEl[0].CubeItems.Find(c => c.Currency == givenBaseCurrency);

            this.baseCurrency = new BaseCurrency();
            this.baseCurrency.Name = this.CheckIfEuro(givenBaseCurrency) ? GlobalConstants.EURO_NAME : baseCurrencyInfo.Currency;
            this.baseCurrency.Rate = this.CheckIfEuro(givenBaseCurrency) ? GlobalConstants.EURO_RATE : baseCurrencyInfo.Rate;
        }
        
        /// <summary>
        /// Creates a Quote Currency
        /// </summary>
        /// <param name="givenQuoteCurrency">The given quote currency</param>
        private void CreateQuoteCurrency(string givenQuoteCurrency)
        {
            var quoteCurrencyInfo = this.ecbEnvelope.CubeRootEl[0].CubeItems.Find(c => c.Currency == givenQuoteCurrency);

            this.quoteCurrency = new QuoteCurrency();
            this.quoteCurrency.Name = this.CheckIfEuro(givenQuoteCurrency) ? GlobalConstants.EURO_NAME : quoteCurrencyInfo.Currency;
            this.quoteCurrency.Rate = this.CheckIfEuro(givenQuoteCurrency) ? GlobalConstants.EURO_RATE : quoteCurrencyInfo.Rate;
        }

        /// <summary>
        /// Check if the given currency name is EUR
        /// </summary>
        /// <param name="currencyName">Currency name</param>
        /// <returns></returns>
        private bool CheckIfEuro(string currencyName)
        {
            return currencyName == GlobalConstants.EURO_NAME;
        }

        /// <summary>
        /// Check if the currency names are the same
        /// </summary>
        /// <param name="baseCurrency">The base currency name</param>
        /// <param name="quoteCurrency">The quote currency name</param>
        /// <returns></returns>
        private bool CheckIfBaseAndQuoteCurrenciesSame(string baseCurrency, string quoteCurrency)
        {
            return baseCurrency == quoteCurrency;
        }

        /// <summary>
        /// Add to in-memory cache or get from there if it already exists
        /// </summary>
        private void AddOrGetInMemoryCache()
        {
            bool isExist = this.memoryCache.TryGetValue(GlobalConstants.XML_DOCUMENT_CACHE_KEY, out this.xmlData);
            if (!isExist)
            {
                this.xmlData = Encoding.Default.GetString(webClient.DownloadData(GlobalConstants.XML_DOCUMENT_URL));

                // TODO - Make it one day, for testing purposes - 20 seconds
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(20));

                this.memoryCache.Set(GlobalConstants.XML_DOCUMENT_CACHE_KEY, this.xmlData, cacheEntryOptions);
            }
        }
    }
}
