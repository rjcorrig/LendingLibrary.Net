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
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System;

namespace LendingLibrary.Tests.Models
{
    [TestFixture()]
    public class RepositoryTests
    {
//        protected Mock<DbSet<Book>> mockBooks;
//        protected Mock<DbSet<ApplicationUser>> mockUsers;

        protected virtual IQueryable<ApplicationUser> SetUpApplicationUsers()
        {
            var robcory = new ApplicationUser
            {
                Id = "robcory-guid",
                GivenName = "Rob",
                FamilyName = "Cory",
                About = "Robin @ Work",
                UserName = "rob@cory.com",
                Email = "rob@cory.com",
                Address1 = "123 Main St",
                City = "Anytown",
                State = "MI",
                Postal = "45123",
                Country = "USA",
                BirthDate = new DateTime(1975, 12, 6),
                Books = new List<Book>()
            };

            var foxyboots9 = new ApplicationUser
            {
                Id = "foxyboots9-guid",
                GivenName = "Jen",
                FamilyName = "Cory",
                About = "Red Hot Fox",
                UserName = "foxyboots9@gmail.com",
                Email = "foxyboots9@gmail.com",
                Address1 = "987 Elm St",
                City = "Someplace",
                State = "WV",
                Postal = "25123",
                Country = "USA",
                BirthDate = new DateTime(1975, 09, 19),
                Books = new List<Book>()
            };

            var coryhome = new ApplicationUser
            {
                Id = "coryhome-guid",
                GivenName = "Rob",
                FamilyName = "Cory",
                About = "Rob @ Home",
                UserName = "rcory@gmail.com",
                Email = "rcory@gmail.com",
                Address1 = "555 State Rd",
                City = "Nowhere",
                State = "OH",
                Postal = "35123",
                Country = "USA",
                BirthDate = new DateTime(1975, 12, 6),
                Books = new List<Book>()
            };

            return new List<ApplicationUser>
            {
                robcory, foxyboots9, coryhome
            }.AsQueryable();
        }

        protected virtual IQueryable<Book> SetUpBooks(IQueryable<ApplicationUser> users)
        {
            var robcory = users.Where(u => u.Id == "robcory-guid").FirstOrDefault();
            var foxyboots9 = users.Where(u => u.Id == "foxyboots9-guid").FirstOrDefault();
            var coryhome = users.Where(u => u.Id == "coryhome-guid").FirstOrDefault();

            var books = new List<Book>();

            books.Add(new Book { ID = 1, Owner = robcory, Author = "Charles Dickens", Title = "A Tale of Two Cities", ISBN = "99177615628", Rating = 3 });
            books.Add(new Book { ID = 2, Owner = robcory, Author = "James Joyce", Title = "A Portrait of the Artist as a Young Man", ISBN = "98155659891", Rating = 4 });
            books.Add(new Book { ID = 4, Owner = robcory, Author = "Fyodor Dostoyevsky", Title = "Crime and Punishment", ISBN = "97826678161", Rating = 2 });

            books.Add(new Book { ID = 43, Owner = foxyboots9, Author = "Jane Austen", Title = "Pride and Prejudice", ISBN = "78192775621", Rating = 5 });
            books.Add(new Book { ID = 57, Owner = foxyboots9, Author = "Diana Gabaldon", Title = "Outlander", ISBN = "615572515112", Rating = 5 });
            books.Add(new Book { ID = 123, Owner = foxyboots9, Author = "Emily Bronte", Title = "Wuthering Heights", ISBN = "78192775621", Rating = 5 });

            books.Add(new Book { ID = 122, Owner = coryhome, Author = "Mary Shelley", Title = "Frankenstein", ISBN = "78712661612", Rating = 4 });
            books.Add(new Book { ID = 3, Owner = coryhome, Author = "Larry Niven", Title = "Ringworld", ISBN = "782627657134", Rating = 5 });
            books.Add(new Book { ID = 321, Owner = coryhome, Author = "Isaac Asimov", Title = "Foundation", ISBN = "867856985515", Rating = 3 });

            books.FindAll(b => b.Owner == robcory).ForEach(b => robcory.Books.Add(b));
            books.FindAll(b => b.Owner == foxyboots9).ForEach(b => foxyboots9.Books.Add(b));
            books.FindAll(b => b.Owner == coryhome).ForEach(b => coryhome.Books.Add(b));

            return books.AsQueryable();
        }

        protected Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockDbSet = new Mock<DbSet<T>>();

            mockDbSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));

            mockDbSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));

            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockDbSet;
        }

        [SetUp()]
        public void SetUp()
        {
            var userData = SetUpApplicationUsers();
            var bookData = SetUpBooks(userData);

//            mockBooks = CreateMockDbSet(bookData);
//            mockUsers = CreateMockDbSet(userData);
        }

        #region User
        //        Task<ApplicationUser> GetUserByIdAsync(string userId);
        //        ApplicationUser GetUserById(string userId);
        //        TODO: Task<IEnumerable<ApplicationUser>> GetUsersUnknownToUserAsync(string userId);
        [Test()]
        public async Task GetUserByIdAsync_returns_correct_User()
        {
            //            var mockContext = new Mock<ApplicationDbContext>();
            //            mockContext.Setup(m => m.Users).Returns(mockUsers.Object);

            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var user = await repo.GetUserByIdAsync("coryhome-guid");

            Assert.AreEqual(user.Id, "coryhome-guid");
        }

        [Test()]
        public async Task GetUserByIdAsync_returns_null_on_no_match()
        {
            //var mockContext = new Mock<ApplicationDbContext>();
            //mockContext.Setup(m => m.Users).Returns(mockUsers.Object);

            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var user = await repo.GetUserByIdAsync("nosuchuser-guid");

            Assert.IsNull(user);
        }

        [Test()]
        public void GetUserById_returns_correct_User()
        {
//            var mockContext = new Mock<ApplicationDbContext>();
//            mockContext.Setup(m => m.Users).Returns(mockUsers.Object);

            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var user = repo.GetUserById("coryhome-guid");

            Assert.AreEqual(user.Id, "coryhome-guid");
        }

        [Test()]
        public void GetUserById_returns_null_on_no_match()
        {
//            var mockContext = new Mock<ApplicationDbContext>();
//            mockContext.Setup(m => m.Users).Returns(mockUsers.Object);

            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var user = repo.GetUserById("nosuchuser-guid");

            Assert.IsNull(user);
        }
        #endregion

        #region Book

        [Test()]
        public void Add_Book_adds_Book_to_Books()
        {
//            var mockContext = new Mock<ApplicationDbContext>();
//            mockContext.Setup(m => m.Books).Returns(mockBooks.Object);

            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);

            repo.Add(new Book());

            mockContext.MockBooks.Verify(m => m.Add(It.IsAny<Book>()), Times.Once);
        }

        [Test()]
        public void Remove_Book_removes_Book_from_Books()
        {
//            var mockContext = new Mock<ApplicationDbContext>();
//            mockContext.Setup(m => m.Books).Returns(mockBooks.Object);
            var mockContext = new MockContext();

            var repo = new Repository(mockContext.Object);

            repo.Remove(mockContext.MockBooks.Object.First());

            mockContext.MockBooks.Verify(m => m.Remove(It.IsAny<Book>()), Times.Once);
        }

        [Test()]
        public async Task GetBookByIdAsync_returns_correct_Book()
        {
            //var mockContext = new Mock<ApplicationDbContext>();
            //mockContext.Setup(m => m.Books).Returns(mockBooks.Object);
            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var book = await repo.GetBookByIdAsync(43);

            Assert.AreEqual(book.ID, 43);
        }

        [Test()]
        public async Task GetBookByIdAsync_returns_null_on_no_match()
        {
//            var mockContext = new Mock<ApplicationDbContext>();
//            mockContext.Setup(m => m.Books).Returns(mockBooks.Object);

            var mockContext = new MockContext();
            var repo = new Repository(mockContext.Object);
            var book = await repo.GetBookByIdAsync(103);

            Assert.IsNull(book);
        }

        [Test()]
        public async Task GetBooksByOwnerIdAsync_returns_owned_Books()
        {
//            var mockContext = new Mock<ApplicationDbContext>();
//            mockContext.Setup(m => m.Books).Returns(mockBooks.Object);

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
//            var mockContext = new Mock<ApplicationDbContext>();
//            mockContext.Setup(m => m.Books).Returns(mockBooks.Object);

            var mockContext = new MockContext();

            var repo = new Repository(mockContext.Object);
            var books = await repo.GetBooksByOwnerIdAsync("nosuchuser-guid");

            Assert.AreEqual(0, books.Count());
        }
		#endregion
    }
}
