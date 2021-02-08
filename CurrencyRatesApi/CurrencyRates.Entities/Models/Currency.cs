namespace CurrencyRates.Entities.Models
{
    /// <summary>
    /// Basic Currency class.
    /// </summary>
    public class Currency
    {
        /// <value>
        /// Set and Get the value of Name.
        /// </value>
        public string Name { get; set; }

        /// <value>
        /// Set and Get the value of Rate.
        /// </value>
        public decimal Rate { get; set; }
    }
}
