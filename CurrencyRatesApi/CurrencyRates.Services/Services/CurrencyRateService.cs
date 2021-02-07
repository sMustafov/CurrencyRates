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
        private readonly IMemoryCache MemoryCache;
        private readonly WebClient WebClient;
        private readonly XmlSerializer XmlSerializer;

        private string XmlData;

        private EcbEnvelope EcbEnvelope;
        private BaseCurrency BaseCurrency;
        private QuoteCurrency QuoteCurrency;
        private CurrencyPair CurrencyPair;

        public CurrencyRateService(IMemoryCache memoryCache)
        {
            this.XmlData = null;
            this.MemoryCache = memoryCache;
            this.WebClient = new WebClient();
            this.XmlSerializer = new XmlSerializer(typeof(EcbEnvelope));
        }

        public CurrencyPair CalculateCurrencyPairRate(string currencyPair)
        {
            if (currencyPair == null)
            {
                throw new CustomInvalidOperationException(GlobalConstants.ERROR_CurrencyPairNotProvided);
            }

            if (currencyPair.Length != 6)
            {
                throw new CustomInvalidOperationException(GlobalConstants.ERROR_CurrencyPairNotRightLength);
            }

            // Add to in-memory cache or get it is already there
            this.AddOrGetInMemoryCache();

            // Extracting the info for given currencyPair
            this.ExtractCurrencyAndRateFromXml(currencyPair);

            if (BaseCurrency.Name == null || QuoteCurrency.Name == null)
            {
                throw new CustomArgumentException(GlobalConstants.ERROR_BaseAndQuoteCurrenciesNotHaveNameOrRate);
            }

            // Calculating currency pair rate
            // E.g. currencyPair = GBPUSD => EURUSD / EURGBP
            var currencyPairRate = Math.Round((QuoteCurrency.Rate / BaseCurrency.Rate), 2, MidpointRounding.AwayFromZero);

            // Creating currency pair
            CurrencyPair = new CurrencyPair
            {
                Name = BaseCurrency.Name + QuoteCurrency.Name,
                Rate = currencyPairRate
            };

            return CurrencyPair;
        }

        private void ExtractCurrencyAndRateFromXml(string currencyPair)
        {
            var givenBaseCurrency = currencyPair.Substring(0, 3);
            var givenQuoteCurrency = currencyPair.Substring(currencyPair.Length - 3);

            this.DeserializeString();

            this.CreateBaseCurrency(givenBaseCurrency);
            this.CreateQuoteCurrency(givenQuoteCurrency);
        }

        private void DeserializeString()
        {
            using (var stringReader = new StringReader(this.XmlData))
            {
                this.EcbEnvelope = this.XmlSerializer.Deserialize(stringReader) as EcbEnvelope;
            }
        }

        private void CreateBaseCurrency(string givenBaseCurrency)
        {
            var baseCurrencyInfo = this.EcbEnvelope.CubeRootEl[0].CubeItems.Find(c => c.Currency == givenBaseCurrency);
            this.BaseCurrency = new BaseCurrency
            {
                Name = baseCurrencyInfo.Currency,
                Rate = baseCurrencyInfo.Rate
            };
        }
        private void CreateQuoteCurrency(string givenQuoteCurrency)
        {
            var quoteCurrencyInfo = this.EcbEnvelope.CubeRootEl[0].CubeItems.Find(c => c.Currency == givenQuoteCurrency);
            this.QuoteCurrency = new QuoteCurrency
            {
                Name = quoteCurrencyInfo.Currency,
                Rate = quoteCurrencyInfo.Rate
            };
        }
        private void AddOrGetInMemoryCache()
        {
            bool isExist = this.MemoryCache.TryGetValue(GlobalConstants.XML_DOCUMENT_CACHE_KEY, out this.XmlData);
            if (!isExist)
            {
                this.XmlData = Encoding.Default.GetString(WebClient.DownloadData(GlobalConstants.XML_DOCUMENT_URL));

                // TODO - Make it one day, for testing purposes - 20 seconds
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(20));

                this.MemoryCache.Set(GlobalConstants.XML_DOCUMENT_CACHE_KEY, this.XmlData, cacheEntryOptions);
            }
        }
    }
}
