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

using LendingLibrary.Api.GlobalHandlers;
using LendingLibrary.Api.Models;
using LendingLibrary.Api.Utils.Extensions;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace LendingLibrary.Tests.GlobalHandlers
{
    [TestFixture()]
    public class GlobalMessageHandlerTests
    {
        #region Tests
        [Test()]
        public async Task GlobalMessageHandler_should_wrap_HttpError_in_WrappedApiError()
        {
            var handler = new GlobalMessageHandler();

            var message = "foo";
            var messageDetail = "bar";
            handler.InnerHandler = new HttpErrorResponder(message, messageDetail);

            var request = new HttpRequestMessage();

            var response = await handler.SendAsyncTestHook(request, new CancellationToken());

            var result = await response.Content.ReadAsAsync<WrappedApiError<ApiError>>();

            Assert.IsNotNull(result.Error);

            // Outer error should have correct code string and message
            Assert.AreEqual(response.StatusCode.ToString(), result.Error.Code);
            Assert.AreEqual(message, result.Error.Message);

            // Details should have correct code string and message detail
            Assert.AreEqual(1, result.Error.Details?.Length);
            Assert.AreEqual(response.StatusCode.ToString(), result.Error.Details?[0]?.Code);
            Assert.AreEqual(messageDetail, result.Error.Details?[0]?.Message);
        }

        [Test()]
        public async Task GlobalMessageHandler_should_pass_a_WrappedApiError_unmodified()
        {
            var handler = new GlobalMessageHandler();
            handler.InnerHandler = new WrappedApiErrorResponder();

            var request = new HttpRequestMessage();

            var response = await handler.SendAsyncTestHook(request, new CancellationToken());

            var result = await response.Content.ReadAsAsync<WrappedApiError<ForbiddenApiError>>();
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(HttpStatusCode.Forbidden.ToString(), result.Error.Code);
        }

        [Test()]
        public async Task GlobalMessageHandler_should_pass_Ok_Content_unmodified()
        {
            var handler = new GlobalMessageHandler();
            handler.InnerHandler = new OkResponder();

            var config = new HttpConfiguration();
            var request = new HttpRequestMessage();
            request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            var response = await handler.SendAsyncTestHook(request, new CancellationToken());

            var result = await response.Content.ReadAsAsync<BookDTO>();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Title);
        }
        #endregion

        #region Helper Classes
        private class HttpErrorResponder : HttpMessageHandler
        {
            public string Message { get; private set; }
            public string MessageDetail { get; private set; }

            public HttpErrorResponder(string message, string messageDetail)
            {
                Message = message;
                MessageDetail = messageDetail;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(request.CreateErrorResponse(HttpStatusCode.NotFound, new HttpError
                {
                    Message = Message,
                    MessageDetail = MessageDetail
                }));
            }
        }

        private class WrappedApiErrorResponder : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var forbiddenError = new ForbiddenApiError("You must be friends with the owner to view this book");

                var forbidden = request.CreateErrorResponse(
                    HttpStatusCode.Forbidden,
                    forbiddenError);

                return Task.FromResult(forbidden);
            }
        }

        private class OkResponder : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var book = new BookDTO
                {
                    ID = 1,
                    Author = "Terry Jones",
                    Title = "Who Killed Chaucer?",
                    Rating = 4,
                    Genre = "History",
                    ISBN = "0312335873",
                    OwnerId = "someuser-guid"
                };

                var result = request.CreateResponse(book);

                return Task.FromResult(result);
            }
        }
        #endregion
    }
}
