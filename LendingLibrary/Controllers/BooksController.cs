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

using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LendingLibrary.Models;
using System.Linq;
using LendingLibrary.Utils;
using System;

namespace LendingLibrary.Controllers
{
    [Authorize]
    public class BooksController : BaseController
    {
        public BooksController()
        {
        }

        public BooksController(IApplicationDbContext db) : 
            base(db)
        {
        }

        public BooksController(IApplicationDbContext db, Func<string> getUserId) :
            base(db, getUserId)
        {
        }

        // GET: Books
        public async Task<ActionResult> Index(string userId)
        {
            var currentUser = await GetCurrentUserAsync();
            
            // If no user supplied, look at my bookshelf
            if (userId == null) { 
                userId = currentUser.Id;
            }

            // If not my bookshelf, check to see I'm permitted to view it--am I friends with the target user?
            if (userId != currentUser.Id)
            {
                var friendship = currentUser.Friendships.FirstOrDefault(f => f.FriendId == userId && f.RequestApproved.HasValue);
                if (friendship == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            var model = new BookIndexViewModel();
            model.Books = await repo.GetBooksByOwnerIdAsync(userId);
            model.User = await repo.GetUserByIdAsync(userId);

            if (model.User == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // GET: Books/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Book book = await repo.GetBookByIdAsync(id);
 
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            Book book = new Book();
            return View(book);
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,ISBN,Title,Author,Rating")] Book book)
        {
            var currentUser = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                book.Owner = currentUser;
                repo.Add(book);
                await repo.SaveAsync();
                return RedirectToAction("Index");
            }

            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Book book = await repo.GetBookByIdAsync(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            else 
            {
                // Check if it's my book to modify
                var currentUser = await GetCurrentUserAsync();
                if (book.Owner.Id != currentUser.Id)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,ISBN,Title,Author,Rating")] Book book)
        {
            if (ModelState.IsValid)
            {
                repo.SetModified(book);
                await repo.SaveAsync();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = await repo.GetBookByIdAsync(id);

            if (book == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Check if it's my book to modify
                var currentUser = await GetCurrentUserAsync();
                if (book.Owner.Id != currentUser.Id)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Book book = await repo.GetBookByIdAsync(id);
            repo.Remove(book);
            await repo.SaveAsync();
            return RedirectToAction("Index");
        }
    }
}
