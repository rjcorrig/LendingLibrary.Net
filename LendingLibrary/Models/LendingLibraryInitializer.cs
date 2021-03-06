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

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;

namespace LendingLibrary.Models
{
    public class LendingLibraryDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        private readonly SeedImporter importer = new SeedImporter();

        protected override void Seed(ApplicationDbContext context)
        {
            try
            {
                // Turn off change detection for speed
                context.Configuration.AutoDetectChangesEnabled = false;

                SeedUsers(context);
                SeedBooks(context);
                SeedFriendships(context);
            }
            finally
            {
                // Resume change detection
                context.Configuration.AutoDetectChangesEnabled = true;
            }

            base.Seed(context);
        }

        private void SeedUsers(ApplicationDbContext context)
        {
            using (var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context)))
            {
                foreach (var user in importer.Get<ApplicationUser>())
                {
                    userManager.Create(user, "P@ssw0rd!");
                }
            }
        }

        private void SeedBooks(ApplicationDbContext context)
        {
            var users = context.Users.Include("Books").OrderBy(u => u.Id).ToArray();

            foreach (var book in importer.Get<Book>())
            {
                // Distribute books to each user until we run out
                var owner = users[(book.ID - 1) % users.Length];
                owner.Books.Add(new Book()
                {
                    ID = book.ID,
                    Title = book.Title,
                    Author = book.Author,
                    Rating = book.Rating,
                    ISBN = book.ISBN,
                    Genre = book.Genre
                });
            }
        }

        private void SeedFriendships(ApplicationDbContext context)
        {
            /*
             * Assign friendships among triplets of users
             * Unconfirmed request to next user, followed by confirmed friendship between next two users,
             * followed by confirmed friendship to middle of next triplet
             * [0 -> 1], [1 <-> 2], [2 <-> 4]
             * [3 -> 4], [4 <-> 5], [5 <-> 7]
             * [6 -> 7], [7 <-> 8], [8 <-> 10]
             * ...
             * [51 -> 52], [52 <-> 53], [53 <-> 1]
             * [3n -> 3n+1], [3n+1 <-> 3n+2], [3n+2 <-> 3n+4 % N]
             */

            var users = context.Users.Include("Books").Include("Friendships").ToArray();

            for (var n = 0; n < users.Length / 3; n++)
            {
                users[3 * n].Friendships.Add(new Friendship 
                { 
                    Friend = users[3 * n + 1], 
                    RequestSent = DateTime.UtcNow 
                });

                users[3 * n + 1].Friendships.Add(new Friendship
                {
                    Friend = users[3 * n + 2],
                    RequestSent = DateTime.UtcNow.AddDays(-n),
                    RequestApproved = DateTime.UtcNow
                });            

                users[3 * n + 2].Friendships.Add(new Friendship
                {
                    Friend = users[3 * n + 1],
                    RequestSent = DateTime.UtcNow.AddDays(-n),
                    RequestApproved = DateTime.UtcNow
                });            

                users[3 * n + 2].Friendships.Add(new Friendship
                {
                    Friend = users[(3 * n + 4) % users.Length],
                    RequestSent = DateTime.UtcNow.AddDays(-n),
                    RequestApproved = DateTime.UtcNow
                });

                users[(3 * n + 4) % users.Length].Friendships.Add(new Friendship
                {
                    Friend = users[3 * n + 2],
                    RequestSent = DateTime.UtcNow.AddDays(-n),
                    RequestApproved = DateTime.UtcNow
                });            
            }
        }
    }
}