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

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace LendingLibrary.Models
{
    public class ApplicationUserConfiguration : EntityTypeConfiguration<ApplicationUser>
    {
        public ApplicationUserConfiguration()
        {
            // Many to many between Friendship and ApplicationUser
            HasMany(u => u.Friendships).WithRequired(f => f.User).HasForeignKey(f => f.UserId).WillCascadeOnDelete(false);
            HasMany(u => u.Users).WithRequired(f => f.Friend).HasForeignKey(f => f.FriendId).WillCascadeOnDelete(false);

            Property(u => u.GivenName).IsRequired().HasMaxLength(60);
            Property(u => u.FamilyName).HasMaxLength(60);
            Property(u => u.BirthDate).IsRequired();
            Property(u => u.About).HasMaxLength(255);
            Property(u => u.Address1).IsRequired().HasMaxLength(100);
            Property(u => u.Address2).HasMaxLength(100);
            Property(u => u.City).IsRequired().HasMaxLength(50);
            Property(u => u.State).HasMaxLength(30);
            Property(u => u.Postal).HasMaxLength(10);
            Property(u => u.Country).HasMaxLength(30);
            Property(u => u.Latitude).IsOptional();
            Property(u => u.Longitude).IsOptional();
        }
    }

    public class BookConfiguration : EntityTypeConfiguration<Book>
    {
        public BookConfiguration()
        {
            // Book object storage constraints
            Property(b => b.ISBN).HasMaxLength(13);
            Property(b => b.Title).IsRequired().HasMaxLength(256);
            Property(b => b.Author).IsRequired().HasMaxLength(256);
            Property(b => b.Genre).HasMaxLength(25);
        }
    }

    public class FriendshipConfiguration : EntityTypeConfiguration<Friendship>
    {
        public FriendshipConfiguration()
        {
            HasKey(u => new { u.UserId, u.FriendId });
            Property(u => u.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(u => u.FriendId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(u => u.RequestSent).IsRequired();
            Property(u => u.RequestApproved).IsOptional();
        }
    }
}
