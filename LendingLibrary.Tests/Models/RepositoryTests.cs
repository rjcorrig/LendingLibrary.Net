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
        public void GetUsersUnknownToUser_returns_Queryable_of_Users()
        {
            var userId = "coryhome-guid";
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);

            var currentUser = mockContext.MockUsers.Object.FirstOrDefault(u => u.Id == userId);
            Assert.IsNotNull(currentUser);

            var users = repo.GetUsersUnknownToUser(userId);

            Assert.IsInstanceOf(typeof(IQueryable<ApplicationUser>), users);
            Assert.That(!users.Any(u => u.Id == userId));
            Assert.That(users.All(u => currentUser.GetFriendshipStatusWith(u.Id) == FriendshipStatus.None));
        }

        [Test()]
        public void GetUsersUnknownToUser_returns_paged_List_of_Users()
        {
            var userId = "coryhome-guid";
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);

            var currentUser = mockContext.MockUsers.Object.FirstOrDefault(u => u.Id == userId);
            Assert.IsNotNull(currentUser);

            var take = 5;
            var users = repo.GetUsersUnknownToUser(userId, take: take);

            Assert.IsInstanceOf(typeof(IQueryable<ApplicationUser>), users);
            Assert.That(!users.Any(u => u.Id == userId));
            Assert.That(users.All(u => currentUser.GetFriendshipStatusWith(u.Id) == FriendshipStatus.None));
            Assert.AreEqual(take, users.Count());
        }

        [Test()]
        public void GetUsersUnknownToUser_returns_next_paged_List_of_Users()
        {
            var userId = "coryhome-guid";
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);

            var currentUser = mockContext.MockUsers.Object.FirstOrDefault(u => u.Id == userId);
            Assert.IsNotNull(currentUser);

            var perPage = 5;
            var page1 = repo.GetUsersUnknownToUser(userId, 0, perPage).ToArray();
            var page2 = repo.GetUsersUnknownToUser(userId, perPage, perPage).ToArray();

            Assert.That(!page2.Any(u => u.Id == userId));
            Assert.That(page2.All(u => currentUser.GetFriendshipStatusWith(u.Id) == FriendshipStatus.None));
            Assert.AreEqual(perPage, page2.Count());
            Assert.AreNotEqual(page1[0], page2[0]);
            Assert.AreNotEqual(page1[perPage-1], page2[perPage-1]);
            Assert.AreNotEqual(page1[perPage - 1], page2[0]);
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
            var book = await repo.GetBookByIdAsync(-1);

            Assert.IsNull(book);
        }

        [Test()]
        public async Task GetBooksByOwnerIdAsync_returns_owned_Books()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var books = await repo.GetBooksByOwnerIdAsync("foxyboots9-guid");

            Assert.AreEqual(2, books.Count());
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

        #region Friendship

        [Test()]
        public void GetFriendshipsByUserIdAsync_returns_Friendships_with_User()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var userId = "foxyboots9-guid";
            var friendships = repo.GetFriendshipsByUserId(userId);

            Assert.IsInstanceOf(typeof(IQueryable<Friendship>), friendships);
            Assert.Greater(friendships.Count(), 0);
            Assert.That(friendships.All(f => f.UserId == userId ^ f.FriendId == userId));
        }

        [Test()]
        public void GetFriendshipsAwaitingApprovalByUserIdAsync_returns_Friendships_waiting_for_User()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var userId = "rsoame-guid";
            var friendships = repo.GetFriendshipsAwaitingApprovalByUserId(userId);

            Assert.IsInstanceOf(typeof(IQueryable<Friendship>), friendships);
            Assert.Greater(friendships.Count(), 0);
            Assert.AreEqual("robcory-guid", friendships.First().UserId);
            Assert.That(friendships.All(f => f.FriendId == userId && !f.RequestApproved.HasValue));
        }

        [Test()]
        // robcory and rmckune don't know each other
        [TestCase("robcory-guid", "rmckune-guid", false)]
        [TestCase("rmckune-guid", "robcory-guid", false)]
        // rsoame and rmckune are friends
        [TestCase("rsoame-guid", "rmckune-guid", true)]
        [TestCase("rmckune-guid", "rsoame-guid", true)]
        // Requested by robcory but not approved by rsoame
		[TestCase("robcory-guid", "rsoame-guid", true)]
        [TestCase("rsoame-guid", "robcory-guid", false)] 
        public async Task GetFriendshipBetweenUserIdsAsync_returns_Friendship_or_null(string userId, string friendId, bool expected)
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);

            var friendship = await repo.GetFriendshipBetweenUserIdsAsync(userId, friendId);
            Assert.AreEqual(expected, friendship != null);
            if (friendship != null)
            {
                Assert.IsInstanceOf(typeof(Friendship), friendship);
            }
        }

        [Test()]
        // robcory and rmckune don't know each other
        [TestCase("robcory-guid", "rmckune-guid", false)]
        [TestCase("rmckune-guid", "robcory-guid", false)]
        // rsoame and rmckune are friends
        [TestCase("rsoame-guid", "rmckune-guid", true)]
        [TestCase("rmckune-guid", "rsoame-guid", true)]
        // Requested by robcory but not approved by rsoame
        [TestCase("robcory-guid", "rsoame-guid", true)]
        [TestCase("rsoame-guid", "robcory-guid", false)]
        public async Task GetFriendshipWithNamesBetweenUserIdsAsync_returns_FriendshipWithNames_or_null(string userId, string friendId, bool expected)
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);

            var friendship = await repo.GetFriendshipWithNamesBetweenUserIdsAsync(userId, friendId);
            Assert.AreEqual(expected, friendship != null);
            if (friendship != null)
            {
                Assert.IsInstanceOf(typeof(FriendshipWithNames), friendship);
            }
        }


        [Test()]
        public void Add_Friendship_adds_Friendship_to_Friendships()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            repo.Add(new Friendship());

            mockContext.MockFriendships.Verify(m => m.Add(It.IsAny<Friendship>()), Times.Once);
        }

        [Test()]
        public void Remove_Friendship_removes_Friendship_from_Friendships()
        {
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            repo.Remove(mockContext.Object.Friendships.First());

            mockContext.MockFriendships.Verify(m => m.Remove(It.IsAny<Friendship>()), Times.Once);
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
