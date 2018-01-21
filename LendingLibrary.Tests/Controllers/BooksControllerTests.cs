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
using Moq;
using System.Linq;

namespace LendingLibrary.Tests.Controllers
{
    [TestFixture()]
    public class BooksControllerTests
    {
        [Test()]
        public async Task Details_returns_ViewResult_if_mine()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var result = await controller.Details(21) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(Book), result.Model);

            var model = result.Model as Book;
            Assert.AreEqual(model.ID, 21);
        }

        [Test()]
        public async Task Details_returns_ViewResult_if_friends()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var result = await controller.Details(20) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(Book), result.Model);

            var model = result.Model as Book;
            Assert.AreEqual(model.ID, 20);
            Assert.AreNotEqual(model.Owner.Id, userId);
        }

        [Test()]
        public void Details_throws_Forbidden_if_not_friends()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Details(4));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test()]
        public void Details_throws_NotFound_if_not_found()
        {
            var mockContext = new MockContext();
            var controller = new BooksController(mockContext.Object);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Details(-1));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.NotFound));
        }    

        [Test()]
        public void Details_throws_BadRequest_if_no_Id_passed()
        {
            var mockContext = new MockContext();
            var controller = new BooksController(mockContext.Object);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Details(null));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.BadRequest));
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
            var friendId = "ffishpool-guid";
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

        [Test()]
        public void Create_returns_ViewResult_with_new_Book()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(Book), result.Model);
        }

        [Test()]
        public async Task Create_adds_Book_and_redirects_to_Index_if_valid()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var book = new Book()
            {
                ID = 59,
                ISBN = "1234567890",
                Title = "Harry Potter and the Half-Blood Prince",
                Author = "J. K. Rowling",
                Rating = 4
            };

            controller.ModelState.Clear();
            var result = await controller.Create(book) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");

            mockDbContext.MockBooks.Verify(m => m.Add(It.IsAny<Book>()), Times.Once());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.AtLeastOnce());
        }

        [Test()]
        public async Task Create_returns_Create_view_if_model_invalid()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var book = new Book()
            {
                ID = 159,
                ISBN = "1234567890",
                Author = "J. K. Rowling",
                Rating = 4
            };

            controller.ModelState.AddModelError("Title", "Title is required");
            var result = await controller.Create(book) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(Book), result.Model);
            Assert.AreEqual(book, result.Model);

            mockDbContext.MockBooks.Verify(m => m.Add(It.IsAny<Book>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Test()]
        public async Task Edit_returns_ViewResult_if_mine()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var result = await controller.Edit(21) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(Book), result.Model);

            var model = result.Model as Book;
            Assert.AreEqual(model.ID, 21);
        }

        [Test()]
        public void Edit_throws_Forbidden_if_not_mine()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Edit(4));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test()]
        public void Edit_throws_NotFound_if_not_found()
        {
            var mockContext = new MockContext();
            var controller = new BooksController(mockContext.Object);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Edit(-1));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.NotFound));
        }    

        [Test()]
        public void Edit_throws_BadRequest_if_no_Id_passed()
        {
            var mockContext = new MockContext();
            var controller = new BooksController(mockContext.Object);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Edit((int?)null));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test()]
        public async Task Edit_modifies_Book_and_redirects_to_Index_if_valid()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var book = mockDbContext.MockBooks.Object.FirstOrDefault(b => b.ID == 43);

            controller.ModelState.Clear();
            var result = await controller.Edit(book) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");

            mockDbContext.Verify(m => m.SetModified(It.IsAny<Book>()), Times.Once());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.AtLeastOnce());
        }

        [Test()]
        public async Task Edit_returns_Edit_view_if_model_invalid()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var book = mockDbContext.MockBooks.Object.FirstOrDefault(b => b.ID == 43);
            book.Title = null;

            controller.ModelState.AddModelError("Title", "Title is required");
            var result = await controller.Edit(book) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(Book), result.Model);
            Assert.AreEqual(book, result.Model);

            mockDbContext.Verify(m => m.SetModified(It.IsAny<Book>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Test()]
        public async Task Delete_returns_ViewResult_if_mine()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var result = await controller.Delete(21) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(Book), result.Model);

            var model = result.Model as Book;
            Assert.AreEqual(model.ID, 21);
        }

        [Test()]
        public void Delete_throws_Forbidden_if_not_mine()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Delete(4));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test()]
        public void Delete_throws_NotFound_if_not_found()
        {
            var mockContext = new MockContext();
            var controller = new BooksController(mockContext.Object);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Delete(-1));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test()]
        public void Delete_throws_BadRequest_if_no_Id_passed()
        {
            var mockContext = new MockContext();
            var controller = new BooksController(mockContext.Object);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Delete((int?)null));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.BadRequest));
        }
        [Test()]
        public async Task DeleteConfirmed_removes_Book_and_redirects_to_Index_if_mine()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var result = await controller.DeleteConfirmed(21) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");

            mockDbContext.MockBooks.Verify(m => m.Remove(It.IsAny<Book>()), Times.Once());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.AtLeastOnce());
        }

        [Test()]
        public void DeleteConfirmed_throws_NotFound_if_not_found()
        {
            var mockContext = new MockContext();
            var controller = new BooksController(mockContext.Object);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.DeleteConfirmed(-1));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test()]
        public void DeleteConfirmed_throws_BadRequest_if_no_Id_passed()
        {
            var mockContext = new MockContext();
            var controller = new BooksController(mockContext.Object);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.DeleteConfirmed((int?)null));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test()]
        public void DeleteConfirmed_throws_Forbidden_if_not_mine()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new BooksController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.DeleteConfirmed(4));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

    }
}
