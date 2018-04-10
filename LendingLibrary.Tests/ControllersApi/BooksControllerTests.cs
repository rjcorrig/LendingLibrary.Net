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

using LendingLibrary.ControllersApi;
using LendingLibrary.Models;
using LendingLibrary.Tests.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace LendingLibrary.Tests.ControllersApi
{
    [TestFixture()]
    public class BooksControllerTests
    {
        [Test()]
        public async Task GetBooks_returns_logged_in_Users_books()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var result = await controller.GetBooks() as OkNegotiatedContentResult<IEnumerable<BookDTO>>;
            Assert.IsNotNull(result);

            foreach (var book in result.Content)
            {
                Assert.AreEqual(userId, book.OwnerId);
            }
        }    

        [Test()]
        public async Task GetBook_returns_OkNegotiatedContentResult_if_mine()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);
            var bookId = 21;

            var result = await controller.GetBook(bookId) as OkNegotiatedContentResult<BookDTO>;
            Assert.IsNotNull(result);

            var book = result.Content;
            Assert.AreEqual(userId, book.OwnerId);
            Assert.AreEqual(bookId, book.ID);
        }

        [Test()]
        public async Task GetBook_returns_OkNegotiatedContentResult_if_friends()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);
            var bookId = 20;

            var result = await controller.GetBook(bookId) as OkNegotiatedContentResult<BookDTO>;
            Assert.IsNotNull(result);

            var book = result.Content;
            Assert.AreNotEqual(userId, book.OwnerId);
            Assert.AreEqual(bookId, book.ID);
        }

        [Test()]
        public async Task GetBook_returns_Forbidden_if_not_friends()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);
            var bookId = 4;

            var req = new HttpRequestMessage();
            controller.ControllerContext.Request = req;

            var result = await controller.GetBook(bookId) as ResponseMessageResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.Response.StatusCode);
        }

        [Test()]
        public async Task GetBook_returns_NotFound_if_not_found()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);
            var bookId = -1;

            var result = await controller.GetBook(bookId) as NotFoundResult;
            Assert.IsNotNull(result);
        }

    }
}
