using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using LendingLibrary.Models;
using Microsoft.AspNet.Identity;
using Unity.Attributes;

namespace LendingLibrary.Controllers
{
    public class BooksApiController : ApiController
    {
        protected IApplicationDbContext db;
        protected IRepository repo;
        protected Func<string> GetUserId;

        [InjectionConstructor]
        public BooksApiController(IApplicationDbContext db)
        {
            repo = new Repository(db);
            this.db = repo.Db;
        }

        public BooksApiController(IApplicationDbContext db, Func<string> getUserId)
        {
            repo = new Repository(db);
            this.db = repo.Db;

            GetUserId = getUserId;
        }

        protected async Task<ApplicationUser> GetCurrentUserAsync()
        {
            if (GetUserId != null)
            {
                return await repo.GetUserByIdAsync(GetUserId());
            }
            else
            {
                return await repo.GetUserByIdAsync(User.Identity.GetUserId());
            }
        }

        protected ApplicationUser GetCurrentUser()
        {
            if (GetUserId != null)
            {
                return repo.GetUserById(GetUserId());
            }
            else
            {
                return repo.GetUserById(User.Identity.GetUserId());
            }
        }

        protected string GetCurrentUserId()
        {
            if (GetUserId != null)
            {
                return GetUserId();
            }
            else
            {
                return User.Identity.GetUserId();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task<IHttpActionResult> GetBooks()
        {
            var currentUserId = GetCurrentUserId();
            var books = await repo.GetBooksByOwnerIdAsync(currentUserId);

            return Ok(books.Select(book => new Book 
            { 
                Author = book.Author,
                Genre = book.Genre,
                ISBN = book.ISBN,
                ID = book.ID,
                OwnerId = book.OwnerId,
                Rating = book.Rating,
                Title = book.Title
            }));
        }

        public async Task<IHttpActionResult> GetBook(int id)
        {
            var book = await repo.GetBookByIdAsync(id);
            return Ok(new Book
            {
                Author = book.Author,
                Genre = book.Genre,
                ISBN = book.ISBN,
                ID = book.ID,
                OwnerId = book.OwnerId,
                Rating = book.Rating,
                Title = book.Title
            });
        }
    }
}
