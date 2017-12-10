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
  
        public MockContext()
        {
            var userData = SetUpApplicationUsers();
            var bookData = SetUpBooks(userData);

            MockBooks = CreateMockDbSet(bookData);
            MockUsers = CreateMockDbSet(userData);

            Setup(m => m.Users).Returns(MockUsers.Object);
            Setup(m => m.Books).Returns(MockBooks.Object);

        }

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

    }
}
