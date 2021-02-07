using CurrencyRates.Services.Utils.Exceptions;
using CurrencyRatesApi.Common;
using CurrencyRatesApi.Entities.Models;
using CurrencyRatesApi.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Xml;

namespace CurrencyRatesApi.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly IMemoryCache memoryCache;

        private XmlDocument XmlDocument;

        private BaseCurrency BaseCurrency;
        private QuoteCurrency QuoteCurrency;
        private CurrencyPair CurrencyPair;

        public CurrencyRateService(IMemoryCache memoryCache)
        {
            this.XmlDocument = new XmlDocument();
            this.memoryCache = memoryCache;
        }

        public CurrencyPair CalculateCurrencyPairRate(string currencyPair)
        {
            if (currencyPair == null)
            {
                throw new CustomArgumentException(GlobalConstants.ERROR_CurrencyPairNotProvided);
            }

            if (currencyPair.Length != 6)
            {
                throw new CustomArgumentException(GlobalConstants.ERROR_CurrencyPairNotRightLength);
            }

            // Add to in-memory cache or get it is already there
            this.AddOrGetFromInMemoryCache();

            // Extracting the info for given currencyPair
            this.ExtractCurrencyAndRateFromXml(currencyPair);

            // Calculating currency pair rate
            // E.g. currencyPair = GBPUSD => EURUSD / EURGBP
            var currencyPairRate = Math.Round((QuoteCurrency.Rate / BaseCurrency.Rate), 2, MidpointRounding.AwayFromZero);

            if (BaseCurrency.Name == null || QuoteCurrency.Name == null)
            {
                throw new CustomArgumentException("Невалидно!");

                //return null;
            }

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
            var baseCurrency = currencyPair.Substring(0, 3);
            var quoteCurrency = currencyPair.Substring(currencyPair.Length - 3);

            foreach (XmlNode nodes in this.XmlDocument.DocumentElement.ChildNodes)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node.Name == "Cube")
                    {
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            if (childNode.Attributes[0].Value == baseCurrency)
                            {
                                BaseCurrency = new BaseCurrency
                                {
                                    Name = childNode.Attributes[0].Value,
                                    Rate = decimal.Parse(childNode.Attributes[1].Value)
                                };
                            }

                            if (childNode.Attributes[0].Value == quoteCurrency)
                            {
                                QuoteCurrency = new QuoteCurrency
                                {
                                    Name = childNode.Attributes[0].Value,
                                    Rate = decimal.Parse(childNode.Attributes[1].Value)
                                };

                            }
                        }
                    }
                }
            }
        }

        private void AddOrGetFromInMemoryCache()
        {
            bool isExist = this.memoryCache.TryGetValue(GlobalConstants.XML_DOCUMENT_CACHE_KEY, out this.XmlDocument);
            if (!isExist)
            {
                this.XmlDocument = new XmlDocument();

                this.XmlDocument.Load(GlobalConstants.XML_DOCUMENT_URL);

                // TODO - Make it one day, for testing purposes - 20 seconds
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(20));

                this.memoryCache.Set(GlobalConstants.XML_DOCUMENT_CACHE_KEY, this.XmlDocument, cacheEntryOptions);
            }
        }
    }
}
