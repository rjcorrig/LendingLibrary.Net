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

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using LendingLibrary.Models;
using Moq;

namespace LendingLibrary.Tests.Models
{
    public class MockContext : Mock<ApplicationDbContext>
    {
        public Mock<DbSet<Book>> MockBooks { get; private set; }
        public Mock<DbSet<ApplicationUser>> MockUsers { get; private set; }
        public Mock<DbSet<Friendship>> MockFriendships { get; private set; }
  
        public MockContext()
        {
            var userData = SetUpApplicationUsers();
            var bookData = SetUpBooks(userData);
            var friendshipData = SetUpFriendships(userData);

			MockUsers = CreateMockDbSet(userData);
            MockBooks = CreateMockDbSet(bookData);
            MockFriendships = CreateMockDbSet(friendshipData);

            Setup(m => m.Users).Returns(MockUsers.Object);
            Setup(m => m.Books).Returns(MockBooks.Object);
            Setup(m => m.Friendships).Returns(MockFriendships.Object);
        }

        protected IQueryable<ApplicationUser> SetUpApplicationUsers()
        {
            var userList = new List<ApplicationUser>();
            var importer = new SeedImporter();

            foreach (var user in importer.Get<ApplicationUser>())
            {
                userList.Add(new ApplicationUser
                {
                    Id = user.Id,
                    GivenName = user.GivenName,
                    FamilyName = user.FamilyName,
                    About = user.About,
                    Email = user.Email,
                    Address1 = user.Address1,
                    City = user.City,
                    State = user.State,
                    Postal = user.Postal,
                    Country = user.Country,
                    Latitude = user.Latitude,
                    Longitude = user.Longitude,
                    BirthDate = user.BirthDate,
                    Books = new List<Book>(),
                    Users = new List<Friendship>(),
                    Friendships = new List<Friendship>()
                });
            }

            return userList.AsQueryable();
        }

        protected IQueryable<Book> SetUpBooks(IQueryable<ApplicationUser> users)
        {
            var books = new List<Book>();
            var importer = new SeedImporter();
            var userArray = users.OrderBy(u => u.Id).ToArray();

            foreach (var book in importer.Get<Book>())
            {
                // Distribute books to each user until we run out
                var owner = userArray[(book.ID - 1) % userArray.Length];
                var newBook = new Book()
                {
                    ID = book.ID,
                    Owner = owner,
                    OwnerId = owner.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Rating = book.Rating,
                    ISBN = book.ISBN
                };

                books.Add(newBook);
                owner.Books.Add(newBook);
            }

            return books.AsQueryable();
        }

        protected IQueryable<Friendship> SetUpFriendships(IQueryable<ApplicationUser> users)
        {
            var friendships = new List<Friendship>();
            var userArray = users.OrderBy(u => u.Id).ToArray();

            for (var n = 0; n < userArray.Length / 3; n++)
            {
                var first = userArray[3 * n];
                var second = userArray[3 * n + 1];
                var third = userArray[3 * n + 2];
                var next = userArray[(3 * n + 4) % userArray.Length];

                var unconfirmed = new Friendship
                {
                    User = first,
                    UserId = first.Id,
                    Friend = second,
                    FriendId = second.Id,
                    RequestSent = DateTime.UtcNow
                };
                friendships.Add(unconfirmed);

                var confirmed = new Friendship
                {
                    User = second,
                    UserId = second.Id,
                    Friend = third,
                    FriendId = third.Id,
                    RequestSent = DateTime.UtcNow.AddDays(-n),
                    RequestApproved = DateTime.UtcNow
                };
                friendships.Add(confirmed);

                var reciprocal = new Friendship
                {
                    User = third,
                    UserId = third.Id,
                    Friend = second,
                    FriendId = second.Id,
                    RequestSent = DateTime.UtcNow.AddDays(-n),
                    RequestApproved = DateTime.UtcNow
                };
                friendships.Add(reciprocal);

                var confirmedNext = new Friendship
                {
                    User = third,
                    UserId = third.Id,
                    Friend = next,
                    FriendId = next.Id,
                    RequestSent = DateTime.UtcNow.AddDays(-n),
                    RequestApproved = DateTime.UtcNow
                };
                friendships.Add(confirmedNext);

                var reciprocalNext = new Friendship
                {
                    User = next,
                    UserId = next.Id,
                    Friend = third,
                    FriendId = third.Id,
                    RequestSent = DateTime.UtcNow.AddDays(-n),
                    RequestApproved = DateTime.UtcNow
                };
                friendships.Add(reciprocalNext);

                friendships.FindAll(f => f.User == first).ForEach(f => first.Friendships.Add(f));
                friendships.FindAll(f => f.User == second).ForEach(f => second.Friendships.Add(f));
                friendships.FindAll(f => f.User == third).ForEach(f => third.Friendships.Add(f));
                friendships.FindAll(f => f.User == next).ForEach(f => next.Friendships.Add(f));

                friendships.FindAll(f => f.Friend == first).ForEach(f => first.Users.Add(f));
                friendships.FindAll(f => f.Friend == second).ForEach(f => second.Users.Add(f));
                friendships.FindAll(f => f.Friend == third).ForEach(f => third.Users.Add(f));
                friendships.FindAll(f => f.Friend == next).ForEach(f => next.Users.Add(f));
            }

            return friendships.AsQueryable();
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

            // Must not cast as IDbSet<T> or Moq will fail to setup the extension method
            mockDbSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            return mockDbSet;
        }

    }
}
