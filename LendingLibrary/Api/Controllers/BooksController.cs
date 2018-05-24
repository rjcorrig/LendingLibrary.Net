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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using LendingLibrary.Api.Models;
using LendingLibrary.Models;
using Swashbuckle.Swagger.Annotations;
using Unity.Attributes;
using LendingLibrary.Api.Utils.Extensions;
using LendingLibrary.Api.Filters;

namespace LendingLibrary.Api.Controllers
{
    [ApiAuthorize]
    [RoutePrefix("api/books")]
    public class BooksController : BaseController
    {
        [InjectionConstructor]
        public BooksController(IApplicationDbContext db) :
            base(db)
        {
        }

        public BooksController(IApplicationDbContext db, Func<string> getUserId) :
            base(db, getUserId)
        {
        }

        /// <summary>
        /// Gets all of the books in the user's bookshelf
        /// </summary>
        /// <remarks>Returns an array of all Book objects in the user's bookshelf</remarks>
        [Route("")]
        [ResponseType(typeof(IEnumerable<BookDTO>))]
        [SwaggerResponse(401, "The client is not logged in", typeof(WrappedApiError<UnauthorizedApiError>))]
        public async Task<IHttpActionResult> GetBooks()
        {
            var currentUserId = GetCurrentUserId();
            var books = await repo.GetBooksByOwnerIdAsync(currentUserId);

            return Ok(books.Select(book => new BookDTO(book)));
        }

        /// <summary>
        /// Gets a single Book by its id.
        /// </summary>
        /// <remarks>Returns a single book object, or a NotFound or Forbidden response</remarks>
        /// <param name="id">The id of the Book</param>
        [Route("{id}")]
        [ResponseType(typeof(BookDTO))]
        [SwaggerResponse(401, "The client is not logged in", typeof(WrappedApiError<UnauthorizedApiError>))]
        [SwaggerResponse(403, "The book does not belong to the logged in account", typeof(WrappedApiError<ForbiddenApiError>))]
        [SwaggerResponse(404, "No book with that id exists", typeof(WrappedApiError<NotFoundApiError>))]
        public async Task<IHttpActionResult> GetBook([FromUri]int id)
        {
            var book = await repo.GetBookByIdAsync(id);
            if (book == null)
            {
                var notFoundError = new NotFoundApiError($"No book with id {id} exists");

                var notFound = ControllerContext.Request.CreateErrorResponse(
                    HttpStatusCode.NotFound, notFoundError);

                return ResponseMessage(notFound);
            }

            var currentUserId = GetCurrentUserId();
            if (book.OwnerId != currentUserId)
            {
                var friendship = await repo.GetFriendshipBetweenUserIdsAsync(currentUserId, book.OwnerId);
                if (friendship == null || !friendship.RequestApproved.HasValue)
                {
                    var forbiddenError = new ForbiddenApiError("You must be friends with the owner to view this book");

                    var forbidden = ControllerContext.Request.CreateErrorResponse(
                        HttpStatusCode.Forbidden,
                        forbiddenError);

                    return ResponseMessage(forbidden);
                }
            }

            return Ok(new BookDTO(book));
        }
    }
}
