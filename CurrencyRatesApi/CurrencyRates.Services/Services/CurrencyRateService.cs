using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

using Microsoft.Extensions.Caching.Memory;

using CurrencyRatesApi.Common;
using CurrencyRatesApi.Entities.Models;
using CurrencyRates.Entities.XmlModel;
using CurrencyRatesApi.Interfaces;
using CurrencyRates.Services.Utils.Exceptions;

namespace CurrencyRatesApi.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly IMemoryCache memoryCache;
        private readonly WebClient webClient;
        private readonly XmlSerializer xmlSerializer;

        private string xmlData;

        private EcbEnvelope ecbEnvelope;
        private BaseCurrency baseCurrency;
        private QuoteCurrency quoteCurrency;
        private CurrencyPair currencyPair;

        public CurrencyRateService(IMemoryCache memoryCache)
        {
            this.xmlData = null;
            this.memoryCache = memoryCache;
            this.webClient = new WebClient();
            this.xmlSerializer = new XmlSerializer(typeof(EcbEnvelope));
        }

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

        private void ExtractCurrencyAndRateFromXml(string givenBaseCurrency, string givenQuoteCurrency)
        {
            this.DeserializeString();

            this.CreateBaseCurrency(givenBaseCurrency);
            this.CreateQuoteCurrency(givenQuoteCurrency);
        }

        private void DeserializeString()
        {
            using (var stringReader = new StringReader(this.xmlData))
            {
                this.ecbEnvelope = this.xmlSerializer.Deserialize(stringReader) as EcbEnvelope;
            }
        }

        private void CreateBaseCurrency(string givenBaseCurrency)
        {
            var baseCurrencyInfo = this.ecbEnvelope.CubeRootEl[0].CubeItems.Find(c => c.Currency == givenBaseCurrency);

            this.baseCurrency = new BaseCurrency();
            this.baseCurrency.Name = this.CheckIfEuro(givenBaseCurrency) ? GlobalConstants.EURO_NAME : baseCurrencyInfo.Currency;
            this.baseCurrency.Rate = this.CheckIfEuro(givenBaseCurrency) ? GlobalConstants.EURO_RATE : baseCurrencyInfo.Rate;
        }
        
        private void CreateQuoteCurrency(string givenQuoteCurrency)
        {
            var quoteCurrencyInfo = this.ecbEnvelope.CubeRootEl[0].CubeItems.Find(c => c.Currency == givenQuoteCurrency);

            this.quoteCurrency = new QuoteCurrency();
            this.quoteCurrency.Name = this.CheckIfEuro(givenQuoteCurrency) ? GlobalConstants.EURO_NAME : quoteCurrencyInfo.Currency;
            this.quoteCurrency.Rate = this.CheckIfEuro(givenQuoteCurrency) ? GlobalConstants.EURO_RATE : quoteCurrencyInfo.Rate;
        }

        private bool CheckIfEuro(string currency)
        {
            return currency == GlobalConstants.EURO_NAME;
        }

        private bool CheckIfBaseAndQuoteCurrenciesSame(string baseCurrency, string quoteCurrency)
        {
            return baseCurrency == quoteCurrency;
        }

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
