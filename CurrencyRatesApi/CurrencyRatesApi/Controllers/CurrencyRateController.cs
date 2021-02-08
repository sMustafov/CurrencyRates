using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using CurrencyRatesApi.Common;
using CurrencyRatesApi.Entities.Models;
using CurrencyRatesApi.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CurrencyRatesApi.Controllers
{
    /// <summary>
    /// Controller class for getting currency pair
    /// </summary>
    [ApiController]
    [Route("/")]
    public class CurrencyRateController : ControllerBase
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<CurrencyRateController> logger;

        /// <summary>
        /// The <see cref="ICurrencyRateService"/>.
        /// </summary>
        private readonly ICurrencyRateService currencyRateService;

        /// <summary>
        /// Initialize a new instance of the <see cref="CurrencyRateController"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="currencyRateService">CurrencyRateService</param>
        public CurrencyRateController(ILogger<CurrencyRateController> logger, ICurrencyRateService currencyRateService)
        {
            this.logger = logger;
            this.currencyRateService = currencyRateService;
        }

        /// <summary>
        /// Entry point ot the controller
        /// </summary>
        /// <returns>Message to say what to do to see currency rate</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Get()
        {
            logger.LogInformation(GlobalConstants.SUCCESS_StartedApplication);

            return this.Ok("Get rate of converted currencies by requesting the following URL: /rate?currencypair=GBPUSD. (GBPUSD is example currency pair)!");
        }

        /// <summary>
        /// Getting currency rate
        /// </summary>
        /// <param name="currencypair">The currency pair</param>
        /// <returns>Currency pair name and rate</returns>
        [HttpGet("rate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<CurrencyPair> Get([FromQuery] string currencypair)
        {
            var currencyPairCalculatedInfo = this.currencyRateService.CalculateCurrencyPairRate(currencypair);

            logger.LogInformation(GlobalConstants.SUCCESS_CurrencyPairRateCalculated);

            return this.Ok(currencyPairCalculatedInfo);
        }
    }
}