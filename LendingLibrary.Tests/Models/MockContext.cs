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
                Books = new List<Book>(),
                Users = new List<Friendship>(),
                Friendships = new List<Friendship>()
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
                Books = new List<Book>(),
                Users = new List<Friendship>(),
                Friendships = new List<Friendship>()
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
                Books = new List<Book>(),
                Users = new List<Friendship>(),
                Friendships = new List<Friendship>()
            };

            return new List<ApplicationUser>
            {
                robcory, foxyboots9, coryhome
            }.AsQueryable();
        }

        protected IQueryable<Book> SetUpBooks(IQueryable<ApplicationUser> users)
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
