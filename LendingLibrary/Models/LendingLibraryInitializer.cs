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

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using LendingLibrary.Utils;
using CsvHelper;
using System.IO;
using System.Web.Hosting;

namespace LendingLibrary.Models
{
    public class LendingLibraryDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            SeedUsers(context);
            SeedBooks(context);
            SeedFriendships(context);

            base.Seed(context);
        }

        private void SeedUsers(ApplicationDbContext context)
        {
            using (var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context)))
            {
                using (var reader = File.OpenText(HostingEnvironment.MapPath("~/App_Data/ApplicationUsers_Seed.csv")))
                {
                    using (var csv = new CsvReader(reader))
                    {
                        csv.Configuration.HeaderValidated = null;
                        csv.Configuration.MissingFieldFound = null;
                        foreach (var user in csv.GetRecords<ApplicationUser>())
                        {
                            userManager.Create(user, "P@ssw0rd!");
                        }
                    }
                }
            }
        }

        private void SeedBooks(ApplicationDbContext context)
        {
            var rng = new Random();
			var numUsers = context.Users.Count();
            var users = context.Users.Include("Books").ToArray(); 

            using (var reader = File.OpenText(HostingEnvironment.MapPath("~/App_Data/Books_Seed.csv")))
            {
                using (var csv = new CsvReader(reader))
                {
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.MissingFieldFound = null;
                    foreach (var book in csv.GetRecords<Book>())
                    {
                        // Assign to a random user
                        var owner = users[rng.Next(numUsers)];
                        owner.Books.Add(new Book()
                        {
                            ID = book.ID,
                            Title = book.Title,
                            Author = book.Author,
                            Rating = book.Rating,
                            ISBN = book.ISBN
                        });
                    }
                }
            }
        }

        private void SeedFriendships(ApplicationDbContext context)
        {
            var robcory = context.Users.Include("Books").Include("Friendships").FirstOrDefault(u => u.UserName == "rob@cory.com");
            var foxyboots9 = context.Users.Include("Books").Include("Friendships").FirstOrDefault(u => u.UserName == "foxyboots9@gmail.com");
            var coryhome = context.Users.Include("Books").Include("Friendships").FirstOrDefault(u => u.UserName == "rcory@gmail.com");

            // An unconfirmed friendship request from robcory to foxyboots
            robcory.Friendships.Add(new Friendship { Friend = foxyboots9, RequestSent = DateTime.UtcNow });

            // A confirmed friendship between foxyboots and coryhome
            foxyboots9.Friendships.Add(new Friendship { Friend = coryhome, RequestSent = DateTime.UtcNow.AddDays(-5), RequestApproved = DateTime.UtcNow });
            coryhome.Friendships.Add(new Friendship { Friend = foxyboots9, RequestSent = DateTime.UtcNow.AddDays(-5), RequestApproved = DateTime.UtcNow });
        }
    }
}