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

using NUnit.Framework;
using System.Threading.Tasks;
using System.Web.Mvc;
using LendingLibrary.Controllers;
using LendingLibrary.Tests.Models;
using LendingLibrary.Models;
using System.Net;
using System.Web;

namespace LendingLibrary.Tests.Controllers
{
    [TestFixture()]
    public class BooksControllerTests
    {
        [Test()]
        public async Task Details_returns_ViewResult_if_found()
        {
            var mockContext = new MockContext();
            var controller = new BooksController(mockContext.Object);
            var result = await controller.Details(43) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(Book), result.Model);

            var model = result.Model as Book;
            Assert.AreEqual(model.ID, 43);
        }

        [Test()]
        public void Details_throws_NotFound_if_not_found()
        {
            var mockContext = new MockContext();
            var controller = new BooksController(mockContext.Object);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Details(41));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.NotFound));
        }    

        [Test()]
        public async Task Index_returns_logged_in_Users_books()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var result = await controller.Index(null) as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as BookIndexViewModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(model.User.Id, userId);
            foreach (var book in model.Books)
            {
                Assert.AreEqual(book.Owner.Id, userId);
            }
        }    

        [Test()]
        public async Task Index_returns_a_Friends_books()
        {
            var userId = "foxyboots9-guid";
            var friendId = "coryhome-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var result = await controller.Index(friendId) as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as BookIndexViewModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(model.User.Id, friendId);
            foreach (var book in model.Books)
            {
                Assert.AreEqual(book.Owner.Id, friendId);
            }
        }    

        [Test()]
        public void Index_throws_Forbidden_on_strangers_books()
        {
            var userId = "foxyboots9-guid";
            var strangerId = "robcory-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Index(strangerId));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.Forbidden));
        }    
    }
}
