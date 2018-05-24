using LendingLibrary.Models;
using Newtonsoft.Json;

namespace LendingLibrary.Api.Models
{
    /// <summary>
    /// Book Data Transfer Object.
    /// </summary>
    public class BookDTO
    {
        /// <summary>
        /// The Book's id.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }

        /// <summary>
        /// The Book's ISBN.
        /// </summary>
        [JsonProperty(PropertyName = "isbn")]
        public string ISBN { get; set; }

        /// <summary>
        /// The Book's title
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// The Book's author
        /// </summary>
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        /// <summary>
        /// The account id of the Book's owner
        /// </summary>
        [JsonProperty(PropertyName = "ownerId")]
        public string OwnerId { get; set; }

        /// <summary>
        /// The Book's rating
        /// </summary>
        [JsonProperty(PropertyName = "rating")]
        public double Rating { get; set; }

        /// <summary>
        /// The Book's genre
        /// </summary>
        [JsonProperty(PropertyName = "genre")]
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
