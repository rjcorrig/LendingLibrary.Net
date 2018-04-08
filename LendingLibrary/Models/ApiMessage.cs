using System;
namespace LendingLibrary.Models
{
    public class ApiMessage
    {
        public string Message { private set; get; }

        public ApiMessage(string message)
        {
            Message = message;
        }
    }
}
