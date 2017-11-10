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
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LendingLibrary.Models;

namespace LendingLibrary.Controllers
{
    [Authorize]
    public class FriendshipsController : BaseController
    {
        public FriendshipsController()
        {
        }

        public FriendshipsController(IApplicationDbContext db, IApplicationUserManager manager) :
            base(db, manager)
        {
        }

        // GET: Friendships
        public async Task<ActionResult> Index(bool RequestSent = false)
        {
            ViewBag.RequestSent = RequestSent;

            var currentUser = await GetCurrentUserAsync();

            // First Where filter gets Friendships involving this user
            // Second removes duplicate confirmed friendships by limiting those to
            // rows where the current user is the owning user
            var friendships = db.Friendships.Include(f => f.Friend).Include(f => f.User)
                .Where(f => f.FriendId == currentUser.Id || f.UserId == currentUser.Id)
                .Where(f => !f.RequestApproved.HasValue || f.UserId == currentUser.Id);

            return View(await friendships.ToListAsync());
        }

        // GET: Friendships/Waiting
        // Return friendship requests waiting for this user's approval
        public async Task<ActionResult> Waiting(bool RequestConfirmed = false)
        {
            ViewBag.RequestConfirmed = RequestConfirmed;

            var currentUser = await GetCurrentUserAsync();

            var friendships = db.Friendships.Include(f => f.Friend).Include(f => f.User)
                .Where(f => f.FriendId == currentUser.Id)
                .Where(f => !f.RequestApproved.HasValue);

            return View(await friendships.ToListAsync());
        }

        // POST: Friendships/Confirm
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Confirm(string userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var currentUser = await GetCurrentUserAsync();
            var requestor = await manager.FindByIdAsync(userId);

            // Confirm the original friendship request
            var friendRequest = await db.Friendships.FirstOrDefaultAsync(f => f.UserId == requestor.Id && f.FriendId == currentUser.Id);
            if (friendRequest == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            friendRequest.RequestApproved = DateTime.UtcNow;

            // Find the matching reciprocal friendship record, if it exists
            var reciprocalRequest = await db.Friendships.FirstOrDefaultAsync(f => f.UserId == currentUser.Id && f.FriendId == requestor.Id);
            if (reciprocalRequest == null)
            {
                reciprocalRequest = new Friendship()
                {
                    User = currentUser,
                    Friend = requestor,
                    RequestSent = friendRequest.RequestSent,
                    RequestApproved = friendRequest.RequestApproved
                };
                db.Friendships.Add(reciprocalRequest);
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Waiting", new { RequestConfirmed = true });
        }

        // POST: Friendships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var currentUser = await GetCurrentUserAsync();
            var friend = await manager.FindByIdAsync(id);

            if (currentUser == null || friend == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var friendship = new Friendship()
            {
                User = currentUser,
                Friend = friend,
                RequestSent = DateTime.UtcNow
            };

            currentUser.Friendships.Add(friendship);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { RequestSent = true });
        }

        // GET: Friendships/Delete/5
        public async Task<ActionResult> Delete(string userId, string friendId)
        {
            if (userId == null || friendId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Friendship friendship = await db.Friendships.FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
            if (friendship == null)
            {
                return HttpNotFound();
            }
            return View(friendship);
        }

        // POST: Friendships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string userId, string friendId)
        {
            // Remove the targeted Friendship row
            var friendship = await db.Friendships.FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
            db.Friendships.Remove(friendship);

            // Remove the reciprocal row, if any
            var reciprocal = await db.Friendships.FirstOrDefaultAsync(f => f.UserId == friendId && f.FriendId == userId);
            if (reciprocal != null)
            {
                db.Friendships.Remove(reciprocal);
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> SearchForNew()
        {
            var currentUser = await GetCurrentUserAsync();

            // All users not me, 
            // with no friendship requests to me (otherUser.Friendships)
            // and no friendship requests from me (otherUser.Users)
            var nonFriends = await db.Users.Include("Friendships").Include("Users")
                .Where(u => u.Id != currentUser.Id)
                .Where(u => !u.Friendships.Any(f => f.FriendId == currentUser.Id))
                .Where(u => !u.Users.Any(f => f.UserId == currentUser.Id))
                .ToListAsync();

            // robcory should see coryhome (has a request to foxyboots)
            // foxyboots should see nobody (has a request from robcory and one to coryhome)
            // coryhome should see robcory (has a request from foxyboots)
            return View(nonFriends);
        }
    }
}
