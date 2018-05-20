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

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using LendingLibrary.Models;
using LendingLibrary.Utils.Extensions;

namespace LendingLibrary.GlobalHandlers
{
    /// <summary>
    /// Last-chance interceptor of messages returned from the Web Api Framework.
    /// </summary>
    public class GlobalMessageHandler : DelegatingHandler
    {
        /// <summary>
        /// Inspect all responses for HttpError content and ensure they follow 
        /// the Microsoft API Guidance for error responses
        /// </summary>
        /// <returns>
        /// An HttpResponseMessage containing either data from an action
        /// or a WrappedApiError object.
        /// </returns>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            switch (response.Content)
            {
                case ObjectContent<HttpError> httpErrorContent:

                    // Make sure any HttpErrors follow the Microsoft API guidance by ensuring
                    // they are wrapped in an WrappedApiError object
                    var httpError = await httpErrorContent.ReadAsAsync<HttpError>();
                    if (!httpError.ContainsKey("error"))
                    {
                        var apiError = new ApiError(response.StatusCode, httpError.Message)
                        {
                            Details = new ApiError[]
                            {
                                new ApiError(response.StatusCode, httpError.MessageDetail)
                            }
                        };
                        return request.CreateErrorResponse(response.StatusCode, apiError);
                    }

                    break;
            }

            return response;
        }

        /// <summary>
        /// Passes through to the protected SendAsync for unit test purposes
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public virtual async Task<HttpResponseMessage> SendAsyncTestHook(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await SendAsync(request, cancellationToken);
        }
    }
}
