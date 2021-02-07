using System;

namespace CurrencyRates.Services.Utils.Exceptions
{
    public class CustomArgumentException : ArgumentException
    {
        public CustomArgumentException(string message)
            : base(message)
        {
        }
    }
}
