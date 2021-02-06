
using System.Collections.Generic;
using System.Xml;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using CurrencyRatesApi.Entities.Models;

namespace CurrencyRatesApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class CurrencyRateController : ControllerBase
    {
        private const string URLString = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        private readonly ILogger<CurrencyRateController> logger;
        private readonly IList<CurrencyRate> currencyRateList;

        private XmlDocument XmlDocument { get; set; }
        private CurrencyRate CurrencyRate { get; set; }

        public CurrencyRateController(ILogger<CurrencyRateController> logger)
        {
            this.logger = logger;
            this.currencyRateList = new List<CurrencyRate>();
            this.XmlDocument = new XmlDocument();
        }

        // http://localhost:port/rate?currencypair=GBPUSD
        [HttpGet("rate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<IEnumerable<CurrencyRate>> Get([FromQuery] string currencypair)
        {
            if (currencypair == null)
            {
                return this.BadRequest(400);
            }

            this.XmlDocument.Load(URLString);

            foreach (XmlNode nodes in this.XmlDocument.DocumentElement.ChildNodes)
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

            return this.Ok(this.currencyRateList);
        }
    }
}