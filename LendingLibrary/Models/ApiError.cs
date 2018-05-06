using System;
namespace LendingLibrary.Models
{
    /// <summary>
    /// Wraps an error message returned from an API call
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// The error message
        /// </summary>
        public string Message { get; set; }
    }
}
