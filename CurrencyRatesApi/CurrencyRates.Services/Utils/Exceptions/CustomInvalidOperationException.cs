using System;

namespace CurrencyRates.Services.Utils.Exceptions
{
    public class CustomInvalidOperationException : InvalidOperationException
    {
        public CustomInvalidOperationException(string message)
            : base(message)
        {
        }
    }
}
