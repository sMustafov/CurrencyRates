using System.Xml;
using CurrencyRatesApi.Interfaces;

namespace CurrencyRatesApi.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {
        // TODO - extract to json
        private const string URLString = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        private readonly XmlDocument XmlDocument;
        public CurrencyRateService()
        {
            this.XmlDocument = new XmlDocument();
        }

        public XmlDocument LoadXml()
        {
            this.XmlDocument.Load(URLString);

            return XmlDocument;
        }
    }
}
