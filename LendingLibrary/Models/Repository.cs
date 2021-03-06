﻿/*
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
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LendingLibrary.Models
{
    public class Repository : IRepository
    {
        public IApplicationDbContext Db { get; protected set; }

        public Repository()
        {
            var context = new ApplicationDbContext();
            context.Database.Log = Console.Write;

            Db = context;
        }

        public Repository(IApplicationDbContext context)
        {
            context.Database.Log = Console.Write;

            Db = context;
        }

        #region User
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await Db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        }

        public ApplicationUser GetUserById(string userId)
        {
            return Db.Users.Where(u => u.Id == userId).FirstOrDefault();
        }

        public IQueryable<ApplicationUser> GetUsersUnknownToUser(string userId, int skip = 0, int take = int.MaxValue)
        {
            // All users not me, 
            // with no friendship requests to me (otherUser.Friendships)
            // and no friendship requests from me (otherUser.Users)
            return Db.Users.Include("Friendships").Include("Users")
                           .Where(u => u.Id != userId)
                           .Where(u => !u.Friendships.Any(f => f.FriendId == userId))
                           .Where(u => !u.Users.Any(f => f.UserId == userId))
                           .OrderBy(u => u.Id)
                           .Skip(() => skip)
                           .Take(() => take);
        }

        public async Task<ApplicationUserName> GetUserNameByIdAsync(string userId)
        {
            return await Db.Users.Where(u => u.Id == userId)
                           .Select(u => new ApplicationUserName
                           {
                                Id = u.Id,
                                UserName = u.GivenName + " " + u.FamilyName
                           }).FirstOrDefaultAsync();
        }
        #endregion

        #region Book
        public async Task<Book> GetBookByIdAsync(int? id)
        {
            return await Db.Books.FirstOrDefaultAsync(b => b.ID == id);
        }

        public async Task<IEnumerable<Book>> GetBooksByOwnerIdAsync(string userId)
        {
            return await Db.Books.Where(b => b.Owner.Id == userId).ToListAsync();
        }

        public Book Add(Book book)
        {
            return Db.Books.Add(book);
        }

        public Book Remove(Book book)
        {
            return Db.Books.Remove(book);
        }
        #endregion

        #region Friendship
        public IQueryable<Friendship> GetFriendshipsByUserId(string userId)
        {
            // First Where filter gets Friendships involving this user
            // Second removes duplicate confirmed friendships by limiting those to
            // rows where the current user is the owning user
            return Db.Friendships.Include(f => f.Friend).Include(f => f.User)
                     .Where(f => f.FriendId == userId || f.UserId == userId)
                     .Where(f => !f.RequestApproved.HasValue || f.UserId == userId);
        }

        public IQueryable<Friendship> GetFriendshipsAwaitingApprovalByUserId(string userId)
        {
            // Return friendship requests waiting for this user's approval (no approval date yet)
            return Db.Friendships.Include(f => f.Friend).Include(f => f.User)
                     .Where(f => f.FriendId == userId)
                     .Where(f => !f.RequestApproved.HasValue);
        }

        public async Task<Friendship> GetFriendshipBetweenUserIdsAsync(string userId, string friendId)
        {
            return await Db.Friendships.FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
        }

        public async Task<FriendshipWithNames> GetFriendshipWithNamesBetweenUserIdsAsync(string userId, string friendId)
        {
            return await Db.Friendships.Include(f => f.Friend).Include(f => f.User)
                           .Select(f => new FriendshipWithNames
                            {
                                UserId = f.UserId,
                                FriendId = f.FriendId,
                                UserName = f.User.GivenName + " " + f.User.FamilyName,
                                FriendName = f.Friend.GivenName + " " + f.Friend.FamilyName,
                                RequestSent = f.RequestSent,
                                RequestApproved = f.RequestApproved
                            })
                           .FirstOrDefaultAsync(f => f.UserId == userId && f.FriendId == friendId);
        }

        public Friendship Add(Friendship friendship)
        {
            return Db.Friendships.Add(friendship);            
        }

        public Friendship Remove(Friendship friendship)
        {
            return Db.Friendships.Remove(friendship);
        }

        #endregion

        #region DbContext
        public async Task<int> SaveAsync()
        {
            return await Db.SaveChangesAsync();
        }

        public void SetModified(object entity)
        {
            Db.SetModified(entity);
        }
        #endregion
    }
}
