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

using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace LendingLibrary.Models
{
    public interface IApplicationDbContext : IDisposable
    {
        IDbSet<Book> Books { get; set; }
        IDbSet<Friendship> Friendships { get; set; }
        IDbSet<ApplicationUser> Users { get; set; }

        Task<int> SaveChangesAsync();
        DbEntityEntry Entry(object entity);

        Database Database { get; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        #region VSGenerated
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            var context = new ApplicationDbContext();
            context.Database.Log = Console.Write;
            return context;
        }
        #endregion

        #region Custom
        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new LendingLibraryDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Friendship>().HasKey(u => new { u.UserId, u.FriendId });
            modelBuilder.Entity<Friendship>().Property(u => u.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Friendship>().Property(u => u.FriendId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.Friendships).WithRequired(f => f.User).HasForeignKey(f => f.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.Users).WithRequired(f => f.Friend).HasForeignKey(f => f.FriendId).WillCascadeOnDelete(false);
        }

        public virtual IDbSet<Book> Books { get; set; }
        public virtual IDbSet<Friendship> Friendships { get; set; }
        #endregion
    }
}

