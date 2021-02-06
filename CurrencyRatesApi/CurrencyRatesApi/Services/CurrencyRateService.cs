using System;
using System.Xml;
using CurrencyRatesApi.Entities.Models;
using CurrencyRatesApi.Interfaces;

namespace CurrencyRatesApi.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {
        // TODO - extract to json
        private const string XML_DOCUMENT_URL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        private readonly XmlDocument XmlDocument;

        private BaseCurrency BaseCurrency;
        private QuoteCurrency QuoteCurrency;
        private CurrencyPair CurrencyPair;
        public CurrencyRateService()
        {
            // Creating new XML document object
            this.XmlDocument = new XmlDocument();
            
            // Loading XML document from ECB URL 
            this.XmlDocument.Load(XML_DOCUMENT_URL);
        }

        public CurrencyPair CalculateCurrencyPairRate(string currencyPair)
        {
            // Extracting the info for given currencyPair
            this.ExtractCurrencyAndRateFromXml(currencyPair);

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
    }
}
