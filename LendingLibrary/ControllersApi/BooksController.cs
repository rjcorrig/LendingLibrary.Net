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
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using LendingLibrary.Models;
using Unity.Attributes;

namespace LendingLibrary.ControllersApi
{
    [Authorize]
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

        public async Task<IHttpActionResult> GetBooks()
        {
            var currentUserId = GetCurrentUserId();
            var books = await repo.GetBooksByOwnerIdAsync(currentUserId);

            return Ok(books.Select(book => new BookDTO(book))); 
        }

        public async Task<IHttpActionResult> GetBook(int id)
        {
            var book = await repo.GetBookByIdAsync(id);
            if (book == null) 
            {
                return NotFound();
            }

            var currentUserId = GetCurrentUserId();
            if (book.OwnerId != currentUserId)
            {
                var friendship = await repo.GetFriendshipBetweenUserIdsAsync(currentUserId, book.OwnerId);
                if (friendship == null || !friendship.RequestApproved.HasValue)
                {
                    return StatusCode(HttpStatusCode.Forbidden);
                }
            }

            return Ok(new BookDTO(book));
        }
    }
}
