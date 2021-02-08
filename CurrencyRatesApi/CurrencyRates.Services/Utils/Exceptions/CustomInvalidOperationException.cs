using System;

namespace CurrencyRates.Services.Utils.Exceptions
{
    /// <summary>
    /// Class for Custom Invalid Operation Exception
    /// </summary>
    public class CustomInvalidOperationException : InvalidOperationException
    {
        /// <summary>
        /// Initializes new instance of this class
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        public CustomInvalidOperationException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
