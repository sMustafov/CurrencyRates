using System.Xml;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using CurrencyRatesApi.Entities.Models;
using CurrencyRatesApi.Interfaces;

namespace CurrencyRatesApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class CurrencyRateController : ControllerBase
    {
        private readonly ILogger<CurrencyRateController> logger;
        private readonly ICurrencyRateService currencyRateService;
        
        private XmlDocument xmlDocument;

        private CurrencyRate CurrencyRate { get; set; }

        public CurrencyRateController(ILogger<CurrencyRateController> logger, ICurrencyRateService currencyRateService)
        {
            this.logger = logger;
            this.currencyRateService = currencyRateService;
        }

        // http://localhost:port/rate?currencypair=GBPUSD
        [HttpGet("rate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CurrencyRate> Get([FromQuery] string currencypair)
        {
            if (currencypair == null)
            {
                return this.BadRequest(400);
            }

            if (currencypair.Length != 6)
            {
                return this.BadRequest(400);
            }

            this.xmlDocument = this.currencyRateService.LoadXml();

            var baseCurrency = currencypair.Substring(0, 3);
            var quoteCurrency = currencypair.Substring(currencypair.Length - 3);

            CurrencyRate = new CurrencyRate
            {
                BaseCurrency = "",
                BaseCurrencyRate = "",
                QuoteCurrency = "",
                QuoteCurrencyRate = ""
            };

            foreach (XmlNode nodes in this.xmlDocument.DocumentElement.ChildNodes)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node.Name == "Cube")
                    {
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            if (childNode.Attributes[0].Value == baseCurrency)
                            {
                                CurrencyRate.BaseCurrency = childNode.Attributes[0].Value;
                                CurrencyRate.BaseCurrencyRate = childNode.Attributes[1].Value;
                            }

                            if (childNode.Attributes[0].Value == quoteCurrency)
                            {
                                CurrencyRate.QuoteCurrency = childNode.Attributes[0].Value;
                                CurrencyRate.QuoteCurrencyRate = childNode.Attributes[1].Value;

                            }                            
                        }
                    }
                }
            }

            return this.Ok(CurrencyRate);
        }
    }
}