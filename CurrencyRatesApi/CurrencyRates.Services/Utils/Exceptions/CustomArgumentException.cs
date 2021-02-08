using System;

namespace CurrencyRates.Services.Utils.Exceptions
{
    /// <summary>
    /// Class for Custom Argument Exception
    /// </summary>
    public class CustomArgumentException : ArgumentException
    {
        /// <summary>
        /// Initializes new instance of this class
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        public CustomArgumentException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
