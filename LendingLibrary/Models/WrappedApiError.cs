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
using System.Web.Http;

namespace LendingLibrary.Models
{
    /// <summary>
    /// Encapsulates an HttpError object with properties usable by ApiExplorer
    /// </summary>
    public class ApiError
    {
        internal HttpError HttpError { get; private set; }

        /// <summary>
        /// The error code
        /// </summary>
        [Required]
        public string Code {
            get => HttpError["Code"].ToString();
            set {
                HttpError["Code"] = value;
            } 
        }

        /// <summary>
        /// The error message
        /// </summary>
        [Required]
        public string Message {
            get => HttpError["Message"].ToString();
            set {
                HttpError["Message"] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LendingLibrary.Models.ApiError"/> class.
        /// </summary>
        public ApiError()
        {
            HttpError = new HttpError();
        }

        public ApiError(HttpError httpError)
        {
            HttpError = httpError;
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
        [Required]
        public ApiError Error { get; private set; }

        public WrappedApiError(ApiError apiError)
        {
            Error = apiError;
        }

        public WrappedApiError(HttpError httpError)
        {
            Error = new ApiError(httpError);
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

    /// <summary>
    /// Wraps a NotFound error from an API call
    /// </summary>
    public class WrappedNotFoundApiError : WrappedApiError
    {
        public WrappedNotFoundApiError(string message)
            : base(HttpStatusCode.NotFound, message)
        {
        }

        public WrappedNotFoundApiError()
            : this("Not Found")
        {
        }
    }

    /// <summary>
    /// Wraps a Forbidden error from an API call
    /// </summary>
    public class WrappedForbiddenApiError : WrappedApiError
    {
        public WrappedForbiddenApiError(string message)
            : base(HttpStatusCode.Forbidden, message)
        {
        }

        public WrappedForbiddenApiError()
            : this("Forbidden")
        {
        }
    }

    /// <summary>
    /// Wraps an Unauthorized error from an API call
    /// </summary>
    public class WrappedUnauthorizedApiError : WrappedApiError
    {
        public WrappedUnauthorizedApiError(string message)
            : base(HttpStatusCode.Unauthorized, message)
        {
        }

        public WrappedUnauthorizedApiError()
            : this("Unauthorized")
        {
        }
    }
}
