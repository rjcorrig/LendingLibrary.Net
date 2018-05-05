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

using System.ComponentModel.DataAnnotations;

namespace LendingLibrary.Models
{
    public class Book
    {
        public virtual int ID { get; set; }
        public virtual string ISBN { get; set; }
        [Required]
        public virtual string Title { get; set; }
        [Required]
        public virtual string Author { get; set; }
        public virtual string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        [Range(0, 5)]
        public virtual double Rating { get; set; }
        public virtual string Genre { get; set; }
    }

    /// <summary>
    /// Book Data Transfer Object.
    /// </summary>
    public class BookDTO
    {
        /// <summary>
        /// The Book's id.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The Book's ISBN.
        /// </summary>
        public string ISBN { get; set; }

        /// <summary>
        /// The Book's title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The Book's author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The account id of the Book's owner
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// The Book's rating
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// The Book's genre
        /// </summary>
        public string Genre { get; set; }

        public BookDTO()
        {
        }

        public BookDTO(Book book) 
        {
            Author = book.Author;
            Genre = book.Genre;
            ISBN = book.ISBN;
            ID = book.ID;
            OwnerId = book.OwnerId;
            Rating = book.Rating;
            Title = book.Title;
        }
    }

}
