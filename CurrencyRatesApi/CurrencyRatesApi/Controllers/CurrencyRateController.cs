using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using CurrencyRatesApi.Entities.Models;
using CurrencyRatesApi.Interfaces;
using CurrencyRatesApi.Common;

namespace CurrencyRatesApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class CurrencyRateController : ControllerBase
    {
        private readonly ILogger<CurrencyRateController> logger;
        private readonly ICurrencyRateService currencyRateService;
        public CurrencyRateController(ILogger<CurrencyRateController> logger, ICurrencyRateService currencyRateService)
        {
            this.logger = logger;
            this.currencyRateService = currencyRateService;
        }

        // http://localhost:port/rate?currencypair=GBPUSD
        [HttpGet("rate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CurrencyPair> Get([FromQuery] string currencypair)
        {
            if (currencypair == null)
            {
                logger.LogError(GlobalConstants.ERROR_CurrencyPairNotProvided);
                return this.BadRequest(400);
            }

            if (currencypair.Length != 6)
            {
                logger.LogError(GlobalConstants.ERROR_CurrencyPairNotRightLength);
                return this.BadRequest(400);
            }

            var currencyPairCalculatedInfo = this.currencyRateService.CalculateCurrencyPairRate(currencypair);

            if (currencyPairCalculatedInfo == null)
            {
                logger.LogError(GlobalConstants.ERROR_BaseAndQuoteCurrenciesNotHaveNameOrRate);
                return this.BadRequest(400);
            }

            logger.LogInformation(GlobalConstants.SUCCESS_CurrencyPairRateCalculated);


            return this.Ok(currencyPairCalculatedInfo);
        }
    }
}