using System.Xml;

namespace CurrencyRatesApi.Interfaces
{
    public interface ICurrencyRateService
    {
        public XmlDocument LoadXml();
    }
}
