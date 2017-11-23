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
                bool isRunningOnMono = CrossPlatform.IsRunningOnMono;

                var robcory = new ApplicationUser
                {
                    GivenName = "Rob",
                    FamilyName = "Cory",
                    About = "Robin @ Work",
                    UserName = "rob@cory.com",
                    Email = "rob@cory.com",
                    Address1 = "123 Main St",
                    City = "Anytown",
                    State = "MI",
                    Postal = "45123",
                    Country = "USA",
                    BirthDate = new DateTime(1975, 12, 6),
                };
                userManager.Create(robcory, "P@ssw0rd!");

                var foxyboots9 = new ApplicationUser
                {
                    GivenName = "Jen",
                    FamilyName = "Cory",
                    About = "Red Hot Fox",
                    UserName = "foxyboots9@gmail.com",
                    Email = "foxyboots9@gmail.com",
                    Address1 = "987 Elm St",
                    City = "Someplace",
                    State = "WV",
                    Postal = "25123",
                    Country = "USA",
                    BirthDate = new DateTime(1975, 09, 19)
                };
                userManager.Create(foxyboots9, "P@ssw0rd!");

                var coryhome = new ApplicationUser
                {
                    GivenName = "Rob",
                    FamilyName = "Cory",
                    About = "Rob @ Home",
                    UserName = "rcory@gmail.com",
                    Email = "rcory@gmail.com",
                    Address1 = "555 State Rd",
                    City = "Nowhere",
                    State = "OH",
                    Postal = "35123",
                    Country = "USA",
                    BirthDate = new DateTime(1975, 12, 6),
                };
                userManager.Create(coryhome, "P@ssw0rd!");
            }
        }

        private void SeedBooks(ApplicationDbContext context)
        {
            var robcory = context.Users.Include("Books").FirstOrDefault(u => u.UserName == "rob@cory.com");
            var foxyboots9 = context.Users.Include("Books").FirstOrDefault(u => u.UserName == "foxyboots9@gmail.com");
            var coryhome = context.Users.Include("Books").FirstOrDefault(u => u.UserName == "rcory@gmail.com");

            robcory.Books.Add(new Book { Author = "Charles Dickens", Title = "A Tale of Two Cities", ISBN = "99177615628", Rating = 3 });
            robcory.Books.Add(new Book { Author = "James Joyce", Title = "A Portrait of the Artist as a Young Man", ISBN = "98155659891", Rating = 4 });
            robcory.Books.Add(new Book { Author = "Fyodor Dostoyevsky", Title = "Crime and Punishment", ISBN = "97826678161" , Rating = 2 });

            foxyboots9.Books.Add(new Book { Author = "Jane Austen", Title = "Pride and Prejudice", ISBN = "78192775621", Rating = 5 });
            foxyboots9.Books.Add(new Book { Author = "Diana Gabaldon", Title = "Outlander", ISBN = "615572515112", Rating = 5 });
            foxyboots9.Books.Add(new Book { Author = "Emily Bronte", Title = "Wuthering Heights", ISBN = "78192775621", Rating = 5 });

            coryhome.Books.Add(new Book { Author = "Mary Shelley", Title = "Frankenstein", ISBN = "78712661612", Rating = 4 });
            coryhome.Books.Add(new Book { Author = "Larry Niven", Title = "Ringworld", ISBN = "782627657134", Rating = 5 });
            coryhome.Books.Add(new Book { Author = "Isaac Asimov", Title = "Foundation", ISBN = "867856985515", Rating = 3 });
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