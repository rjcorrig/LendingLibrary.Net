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
using System.Data.Entity;

namespace LendingLibrary.Tests.Models
{
    [TestFixture()]
    public class Repository
    {
        [Test()]
        public void Add_Book_adds_Book_to_Books()
        {
            var mockSet = new Mock<DbSet<Book>>();

			var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockSet.Object);

            var repo = new LendingLibrary.Models.Repository(mockContext.Object);
            repo.Add(new Book());

            mockSet.Verify(m => m.Add(It.IsAny<Book>()), Times.Once);
        }
    }
}
