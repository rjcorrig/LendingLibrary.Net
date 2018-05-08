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
        protected HttpError _httpError;

        /// <summary>
        /// The error code
        /// </summary>
        [Required]
        public string Code {
            get => _httpError["Code"].ToString();
            set {
                _httpError["Code"] = value;
            } 
        }

        /// <summary>
        /// The error message
        /// </summary>
        [Required]
        public string Message {
            get => _httpError["Message"].ToString();
            set {
                _httpError["Message"] = value;
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
}
