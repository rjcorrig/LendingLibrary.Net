/*
 * LendingLibrary - An online private bookshelf catalog and sharing application
 * Copyright (C) 2017 Robert Corrigan
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.ComponentModel.DataAnnotations;
using System.Net;
using Newtonsoft.Json;

namespace LendingLibrary.Models
{
	/// <summary>
    /// Wraps an error message returned from an API call
    /// </summary>
    public class WrappedApiError<T> where T : ApiError, new()
    {
        /// <summary>
        /// The error object
        /// </summary>
        [Required]
		[JsonProperty(PropertyName = "error")]
        public T Error { get; set; }

		public WrappedApiError()
		{
		}

        public WrappedApiError(T apiError)
        {
            Error = apiError;
        }

        public WrappedApiError(string message)
        {
            Error = new T()
            {
                Message = message
            };
        }
    }

	/// <summary>
    /// An API Error object conforming to Microsoft API guidelines
    /// </summary>
    public class ApiError
    {
        /// <summary>
		/// One of a server-defined set of error codes
        /// </summary>
        [Required]
        [JsonProperty(PropertyName = "code")]
		public string Code { get; set; }

        /// <summary>
		/// A human-readable representation of the error
        /// </summary>
        [Required]
		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }

        /// <summary>
        /// The target of the error
        /// </summary>
		[JsonProperty(PropertyName = "target")]
		public string Target { get; set; }

        /// <summary>
		/// An array of details about specific errors that led to this reported error
        /// </summary>
		[JsonProperty(PropertyName = "details")]
		public ApiError[] Details { get; set; }

        /// <summary>
		/// An object containing more specific information than the current object about the error
        /// </summary>
		[JsonProperty(PropertyName = "innererror")]
		public InnerApiError InnerError { get; set; }

		public ApiError()
		{
		}

		public ApiError(string code, string message)
		{
			Code = code;
			Message = message;
		}

		public ApiError(HttpStatusCode code, string message)
			: this(code.ToString(), message)
		{
		}

		public ApiError(int code, string message)
			: this(code.ToString(), message)
		{
		}
    }
    
    /// <summary>
	/// An object containing more specific information than the containing object about the error
    /// </summary>
    public class InnerApiError
	{
		/// <summary>
		/// A more specific error code than was provided by the containing error
        /// </summary>
        /// <value>The code.</value>
		[JsonProperty(PropertyName = "code")]
		public string Code { get; set; }

        /// <summary>
		/// An object containing more specific information than the current object about the error
        /// </summary>
		[JsonProperty(PropertyName = "innererror")]
		public InnerApiError InnerError { get; set; }
	}

    /// <summary>
    /// Wraps a NotFound error from an API call
    /// </summary>
    public class NotFoundApiError : ApiError
    {
        public NotFoundApiError(string message)
			: base(HttpStatusCode.NotFound, message)
        {
        }

        public NotFoundApiError()
            : this("Not Found")
        {
        }
    }

    /// <summary>
    /// Wraps a Forbidden error from an API call
    /// </summary>
    public class ForbiddenApiError : ApiError
    {
        public ForbiddenApiError(string message)
			: base(HttpStatusCode.Forbidden, message)
        {
        }

        public ForbiddenApiError()
            : this("Forbidden")
        {
        }
    }

    /// <summary>
    /// Wraps an Unauthorized error from an API call
    /// </summary>
    public class UnauthorizedApiError : ApiError
    {
        public UnauthorizedApiError(string message)
            : base(HttpStatusCode.Unauthorized, message)
        {
        }

        public UnauthorizedApiError()
            : this("Unauthorized")
        {
        }
    }

	/// <summary>
    /// Wraps a general exception or error from an API call
    /// </summary>
    public class InternalServerApiError : ApiError
    {
		public InternalServerApiError(string message)
			: base(HttpStatusCode.InternalServerError, message)
        {
        }

		public InternalServerApiError()
            : this("Internal Server Error")
        {
        }
    }
}
