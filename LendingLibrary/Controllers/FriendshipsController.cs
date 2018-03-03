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
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using LendingLibrary.Models;
using System.Web;
using Unity.Attributes;
using System.Linq;
using System.Data.Entity;

namespace LendingLibrary.Controllers
{
    [Authorize]
    public class FriendshipsController : BaseController
    {
        [InjectionConstructor]
        public FriendshipsController(IApplicationDbContext db) :
            base(db)
        {
        }

        public FriendshipsController(IApplicationDbContext db, Func<string> getUserId) :
            base(db, getUserId)
        {
        }

        // GET: Friendships
        public async Task<ActionResult> Index(bool RequestSent = false)
        {
            ViewBag.RequestSent = RequestSent;

            var currentUserId = GetCurrentUserId();

            return View(await repo.GetFriendshipsByUserId(currentUserId)
                        .Select(f => new FriendshipWithNames
                        {
                            UserId = f.UserId,
                            FriendId = f.FriendId,
                            UserName = f.User.GivenName + " " + f.User.FamilyName,
                            FriendName = f.Friend.GivenName + " " + f.Friend.FamilyName,
                            RequestSent = f.RequestSent,
                            RequestApproved = f.RequestApproved
                        })
                        .ToListAsync());
        }

        // GET: Friendships/Waiting
        // Return friendship requests waiting for this user's approval
        public async Task<ActionResult> Waiting(bool RequestConfirmed = false)
        {
            ViewBag.RequestConfirmed = RequestConfirmed;

            var currentUserId = GetCurrentUserId();

            return View(await repo.GetFriendshipsAwaitingApprovalByUserId(currentUserId)
                        .Select(f => new FriendshipWithNames
                        {
                            UserId = f.UserId,
                            FriendId = f.FriendId,
                            UserName = f.User.GivenName + " " + f.User.FamilyName,
                            FriendName = f.Friend.GivenName + " " + f.Friend.FamilyName,
                            RequestSent = f.RequestSent,
                            RequestApproved = f.RequestApproved
                        })
                        .ToListAsync());
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
                throw new HttpException((int)HttpStatusCode.BadRequest, "No user was passed");
            }

            var currentUserId = GetCurrentUserId();

            // Confirm the original friendship request
            var friendRequest = await repo.GetFriendshipBetweenUserIdsAsync(userId, currentUserId);
            if (friendRequest == null || friendRequest.RequestApproved.HasValue)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "No friendship request exists from that user");
            }
            friendRequest.RequestApproved = DateTime.UtcNow;

            // Find the matching reciprocal friendship record, if it exists
            var reciprocalRequest = await repo.GetFriendshipBetweenUserIdsAsync(currentUserId, userId);
            if (reciprocalRequest == null)
            {
                reciprocalRequest = new Friendship()
                {
                    UserId = currentUserId,
                    FriendId = userId,
                    RequestSent = friendRequest.RequestSent,
                    RequestApproved = friendRequest.RequestApproved
                };
                repo.Add(reciprocalRequest);
            }

            await repo.SaveAsync();
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
                throw new HttpException((int)HttpStatusCode.BadRequest, "No user was passed");
            }

            var currentUser = await GetCurrentUserAsync();
            if (id == currentUser.Id)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "You cannot friend yourself!");
            }

            var friend = await repo.GetUserByIdAsync(id);
            if (currentUser == null || friend == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "User does not exist");
            }

            if (currentUser.GetFriendshipStatusWith(friend) != FriendshipStatus.None)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "You have already connected with that user");
            }

            var friendship = new Friendship()
            {
                User = currentUser,
                Friend = friend,
                RequestSent = DateTime.UtcNow
            };

            currentUser.Friendships.Add(friendship);
            await repo.SaveAsync();
            return RedirectToAction("Index", new { RequestSent = true });
        }

        // GET: Friendships/Delete/5
        public async Task<ActionResult> Delete(string userId, string friendId)
        {
            if (userId == null || friendId == null)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "No user was passed");
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId != userId && currentUserId != friendId)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "You aren't allowed to delete others' connections!");
            }

            var friendship = await repo.GetFriendshipWithNamesBetweenUserIdsAsync(userId, friendId);
            if (friendship == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "No friendship request exists from that user");
            }
            return View(friendship);
        }

        // POST: Friendships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string userId, string friendId)
        {
            if (userId == null || friendId == null)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "No user was passed");
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId != userId && currentUserId != friendId)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "You aren't allowed to delete others' connections!");
            }

            // Locate the targeted Friendship row
            var friendship = await repo.GetFriendshipBetweenUserIdsAsync(userId, friendId);

            // Locate the reciprocal row, if any
            var reciprocal = await repo.GetFriendshipBetweenUserIdsAsync(friendId, userId);

            if (friendship == null && reciprocal == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "No connection exists between those users");
            }

            if (friendship != null)
            {
                repo.Remove(friendship);
            }

            if (reciprocal != null)
            {
                repo.Remove(reciprocal);
            }

            await repo.SaveAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> SearchForNew(int pageNo = 1, int perPage = 5)
        {
            var currentUserId = GetCurrentUserId();

            var suggestionQuery = repo.GetUsersUnknownToUser(currentUserId, perPage * (pageNo - 1), perPage + 1);

            var suggestions = await suggestionQuery.Select(u => new ApplicationUserNameAndCity
            {
                Id = u.Id,
                UserName = u.GivenName + " " + u.FamilyName,
                City = u.City,
                State = u.State,
                Country = u.Country
            }).ToListAsync();

            return View(new SearchForNewViewModel
            {
                PageNumber = pageNo,
                UsersPerPage = perPage,
                HasMore = suggestions.Count() > perPage,
                FriendSuggestions = suggestions.Take(perPage)
            });
        }
    }
}
