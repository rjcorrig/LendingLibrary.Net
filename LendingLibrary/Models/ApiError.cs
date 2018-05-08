using System;
using System.Net;
using System.Web.Http;

namespace LendingLibrary.Models
{
    /// <summary>
    /// Encapsulates an HttpError object with properties usable by ApiExplorer
    /// </summary>
    public class ApiError
    {
        protected HttpError _httpError;

        /// <summary>
        /// The error code
        /// </summary>
        public string Code {
            get => _httpError["Code"].ToString();
            set {
                _httpError["Code"] = value.ToString();
            } 
        }

        /// <summary>
        /// The error message
        /// </summary>
        public string Message {
            get => _httpError["Message"].ToString();
            set {
                _httpError["Message"] = value.ToString();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LendingLibrary.Models.ApiError"/> class.
        /// </summary>
        public ApiError()
        {
            _httpError = new HttpError();
        }

        public ApiError(HttpError httpError)
        {
            _httpError = httpError;
        }
    }

    /// <summary>
    /// Wraps an error message returned from an API call
    /// </summary>
    public class WrappedApiError
    {
        /// <summary>
        /// The error object
        /// </summary>
        public ApiError Error { get; private set; }

        public WrappedApiError(ApiError apiError)
        {
            Error = apiError;
        }

        public WrappedApiError(HttpStatusCode code, string message)
        {
            Error = new ApiError
            {
                Code = code.ToString(),
                Message = message
            };
        }
    }
}
