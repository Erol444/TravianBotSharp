using System;

namespace MainCore.Exceptions
{
    public class StopNowException : Exception
    {
        public StopNowException(string message) : base(message)
        {
        }
    }
}