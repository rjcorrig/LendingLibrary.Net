using NUnit.Framework;
using System;
using LendingLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LendingLibrary.Tests.Models
{
    [TestFixture()]
    public class BookTests
    {
        [Test()]
        public void Model_IsValid()
        {
            var book = new Book()
            {
                ID = 321,
                ISBN = "9812715112",
                Title = "Harry Potter and the Chamber of Secrets",
                Author = "J. K. Rowling",
                Rating = 5
            };

            var context = new ValidationContext(book, null, null);
            var results = new List<ValidationResult>();
            var isModelValid = Validator.TryValidateObject(book, context, results, true);

            Assert.True(isModelValid);
        }

        [Test()]
        public void Model_not_IsValid()
        {
            var book = new Book()
            {
                ID = 321,
                ISBN = "9812715112",
                Rating = -3
            };

            var context = new ValidationContext(book, null, null);
            var results = new List<ValidationResult>();
            var isModelValid = Validator.TryValidateObject(book, context, results, true);

            Assert.False(isModelValid);
            Assert.AreEqual(3, results.Count);
        }
    }
}
