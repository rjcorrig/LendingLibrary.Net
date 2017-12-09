﻿/*
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

namespace LendingLibrary.Tests.Models
{
    [TestFixture()]
    public class Repository
    {
        protected Mock<DbSet<Book>> mockBooks;

        [SetUp()]
        public void SetUp()
        {
            var data = new List<Book>
            {
                new Book { ID = 15, Author = "Jane Austen", Title = "Pride and Prejudice", ISBN = "78192775621", Rating = 5 },
                new Book { ID = 43, Author = "Diana Gabaldon", Title = "Outlander", ISBN = "615572515112", Rating = 5 },
                new Book { ID = 62, Author = "Emily Bronte", Title = "Wuthering Heights", ISBN = "78192775621", Rating = 5 }
            }.AsQueryable();

            mockBooks = new Mock<DbSet<Book>>();

            mockBooks.As<IDbAsyncEnumerable<Book>>() 
                .Setup(m => m.GetAsyncEnumerator()) 
                .Returns(new TestDbAsyncEnumerator<Book>(data.GetEnumerator())); 
 
            mockBooks.As<IQueryable<Book>>() 
                .Setup(m => m.Provider) 
                .Returns(new TestDbAsyncQueryProvider<Book>(data.Provider)); 
 
            mockBooks.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(data.Expression); 
            mockBooks.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(data.ElementType); 
            mockBooks.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }

        [Test()]
        public void Add_Book_adds_Book_to_Books()
        {
			var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockBooks.Object);

            var repo = new LendingLibrary.Models.Repository(mockContext.Object);

            repo.Add(new Book());

            mockBooks.Verify(m => m.Add(It.IsAny<Book>()), Times.Once);
        }

        [Test()]
        public async Task GetBookByIdAsync_returns_correct_Book()
        {
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockBooks.Object);

            var repo = new LendingLibrary.Models.Repository(mockContext.Object);
            var book = await repo.GetBookByIdAsync(43);

            Assert.AreEqual(book.ID, 43);
        }

        [Test()]
        public async Task GetBookByIdAsync_returns_null_on_no_match()
        {
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockBooks.Object);

            var repo = new LendingLibrary.Models.Repository(mockContext.Object);
            var book = await repo.GetBookByIdAsync(103);

            Assert.IsNull(book);
        }
    }
}
