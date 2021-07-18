using System;

namespace TestBotSite.Models
{
    public class ErrorViewModel
    {
        public Exception ExceptionValue { get; set; }

        public ErrorViewModel(Exception exception)
        {
            ExceptionValue = exception;
        }
    }
}
