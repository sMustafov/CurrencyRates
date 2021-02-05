using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace CurrencyRatesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyRateController : ControllerBase
    {
        const string URLString = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        private readonly ILogger<CurrencyRateController> logger;
        private readonly List<CurrencyRate> currencyRateList;


        public CurrencyRateController(ILogger<CurrencyRateController> logger)
        {
            this.logger = logger;
            this.currencyRateList = new List<CurrencyRate>();
        }

        public CurrencyRate CurrencyRate { get; private set; }

        [HttpGet]
        public IEnumerable<CurrencyRate> Get()
        {

            XmlDocument myXmlDocument = new XmlDocument();
            myXmlDocument.Load(URLString);

            foreach (XmlNode nodes in myXmlDocument.DocumentElement.ChildNodes)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node.Name == "Cube")
                    {
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            CurrencyRate = new CurrencyRate
                            {
                                Currency = childNode.Attributes[0].Value,
                                Rate = childNode.Attributes[1].Value
                            };

                            this.currencyRateList.Add(CurrencyRate);
                        }
                    }
                }
            }

            return this.currencyRateList;
        }
    }
}
