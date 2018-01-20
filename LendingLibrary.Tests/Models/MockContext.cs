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
            var robcory = users.Where(u => u.Id == "robcory-guid").FirstOrDefault();
            var foxyboots9 = users.Where(u => u.Id == "foxyboots9-guid").FirstOrDefault();
            var coryhome = users.Where(u => u.Id == "coryhome-guid").FirstOrDefault();

            var friendships = new List<Friendship>();

            // An unconfirmed friendship request from robcory to foxyboots
            friendships.Add(new Friendship { 
                User = robcory, 
                UserId = robcory.Id,
                Friend = foxyboots9,
                FriendId = foxyboots9.Id,
                RequestSent = DateTime.UtcNow 
            });

            // A confirmed friendship between foxyboots and coryhome
            friendships.Add(new Friendship { 
                User = foxyboots9, 
                UserId = foxyboots9.Id,
                Friend = coryhome, 
                FriendId = coryhome.Id,
                RequestSent = DateTime.UtcNow.AddDays(-5), 
                RequestApproved = DateTime.UtcNow 
            });
            friendships.Add(new Friendship { 
                User = coryhome, 
                UserId = coryhome.Id,
                Friend = foxyboots9, 
                FriendId = foxyboots9.Id,
                RequestSent = DateTime.UtcNow.AddDays(-5), 
                RequestApproved = DateTime.UtcNow 
            });

            friendships.FindAll(f => f.User == robcory).ForEach(f => robcory.Friendships.Add(f));
            friendships.FindAll(f => f.User == foxyboots9).ForEach(f => foxyboots9.Friendships.Add(f));
            friendships.FindAll(f => f.User == coryhome).ForEach(f => coryhome.Friendships.Add(f));

            friendships.FindAll(f => f.Friend == robcory).ForEach(f => robcory.Users.Add(f));
            friendships.FindAll(f => f.Friend == foxyboots9).ForEach(f => foxyboots9.Users.Add(f));
            friendships.FindAll(f => f.Friend == coryhome).ForEach(f => coryhome.Users.Add(f));

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
