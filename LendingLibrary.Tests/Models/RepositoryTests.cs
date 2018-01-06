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

using Moq;
using NUnit.Framework;
using LendingLibrary.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace LendingLibrary.Tests.Models
{
    [TestFixture()]
    public class RepositoryTests
    {
        #region User
        //        Task<ApplicationUser> GetUserByIdAsync(string userId);
        //        ApplicationUser GetUserById(string userId);
        //        Task<IEnumerable<ApplicationUser>> GetUsersUnknownToUserAsync(string userId);
        [Test()]
        public async Task GetUserByIdAsync_returns_correct_User()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var user = await repo.GetUserByIdAsync("coryhome-guid");

            Assert.AreEqual(user.Id, "coryhome-guid");
        }

        [Test()]
        public async Task GetUserByIdAsync_returns_null_on_no_match()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var user = await repo.GetUserByIdAsync("nosuchuser-guid");

            Assert.IsNull(user);
        }

        [Test()]
        public void GetUserById_returns_correct_User()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var user = repo.GetUserById("coryhome-guid");

            Assert.AreEqual(user.Id, "coryhome-guid");
        }

        [Test()]
        public void GetUserById_returns_null_on_no_match()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var user = repo.GetUserById("nosuchuser-guid");

            Assert.IsNull(user);
        }

        [Test()]
        public async Task GetUsersUnknownToUserAsync_returns_List_of_Users()
        {
            var userId = "coryhome-guid";
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);

            var currentUser = mockContext.MockUsers.Object.FirstOrDefault(u => u.Id == userId);
            Assert.IsNotNull(currentUser);

            var users = await repo.GetUsersUnknownToUserAsync(userId);

            Assert.IsInstanceOf(typeof(IEnumerable<ApplicationUser>), users);
            Assert.That(!users.Any(u => u.Id == userId));
            Assert.That(users.All(u => u.GetFriendshipStatusWith(currentUser) == FriendshipStatus.None));
        }
        #endregion

        #region Book

        [Test()]
        public void Add_Book_adds_Book_to_Books()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            repo.Add(new Book());

            mockContext.MockBooks.Verify(m => m.Add(It.IsAny<Book>()), Times.Once);
        }

        [Test()]
        public void Remove_Book_removes_Book_from_Books()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            repo.Remove(mockContext.Object.Books.First());

            mockContext.MockBooks.Verify(m => m.Remove(It.IsAny<Book>()), Times.Once);
        }

        [Test()]
        public async Task GetBookByIdAsync_returns_correct_Book()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var book = await repo.GetBookByIdAsync(43);

            Assert.AreEqual(book.ID, 43);
        }

        [Test()]
        public async Task GetBookByIdAsync_returns_null_on_no_match()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var book = await repo.GetBookByIdAsync(103);

            Assert.IsNull(book);
        }

        [Test()]
        public async Task GetBooksByOwnerIdAsync_returns_owned_Books()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var books = await repo.GetBooksByOwnerIdAsync("foxyboots9-guid");

            Assert.AreEqual(3, books.Count());
            foreach (var book in books)
            {
                Assert.AreEqual("foxyboots9-guid", book.Owner.Id);
            }
        }

        [Test()]
        public async Task GetBooksByOwnerIdAsync_returns_empty_Books_on_no_match()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var books = await repo.GetBooksByOwnerIdAsync("nosuchuser-guid");

            Assert.AreEqual(0, books.Count());
        }
        #endregion

        #region DbContext
        [Test()]
        public async Task SaveAsync_calls_Context_SaveChangesAsync()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);

            await repo.SaveAsync();
            mockContext.Verify(m => m.SaveChangesAsync(), Times.Once());
        }

        public void SetModified_calls_Context_SetModified()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);

            repo.SetModified(new Book());
            mockContext.Verify(m => m.SetModified(It.IsAny<object>()), Times.Once());
        }

        #endregion
    }
}
