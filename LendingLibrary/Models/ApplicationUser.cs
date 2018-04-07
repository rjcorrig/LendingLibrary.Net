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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LendingLibrary.Utils;

namespace LendingLibrary.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        #region VSGenerated
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            return await GenerateUserIdentityAsync(manager, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        #endregion

        #region Custom

        [Required]
        public virtual string GivenName { get; set; }
        [Required]
        public virtual string FamilyName { get; set; }
        [Required]
        public virtual DateTime BirthDate { get; set; }
        public virtual string About { get; set; }
        [Required]
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        [Required]
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Postal { get; set; }
        public virtual string Country { get; set; }
        [Range(-90.0, 90.0)]
        public virtual double? Latitude { get; set; }
        [Range(-180.0, 180.0)]
        public virtual double? Longitude { get; set; }
        public virtual ICollection<Book> Books { get; set; }

        // Friendship relations
        // Needed to set up this way to make EF modeler happy
        // As a business rule they should be set up in pairs once a request is approved

        // Users is the set of Friendships where f.UserId = other user, f.FriendId is me
        // i.e. friendship requests originated from someone else to me
        public virtual ICollection<Friendship> Users { get; set; }
        // Friendships is the set of Friendships where f.UserId = me, f.FriendId is other user
        // i.e. friendship requests originated by me to someone else
        public virtual ICollection<Friendship> Friendships { get; set; }

        public FriendshipStatus GetFriendshipStatusWith(ApplicationUser other)
        {
            return GetFriendshipStatusWith(other.Id);
        }

        public FriendshipStatus GetFriendshipStatusWith(string otherId)
        {
            // Do we have a friendship request (approved or not) sent to the other user?
            var friend = Friendships.FirstOrDefault(f => f.FriendId == otherId);
            if (friend != null)
            {
                // Yes, we do
                if (friend.RequestApproved.HasValue)
                {
                    // and it's approved
                    return FriendshipStatus.Approved;
                }
                else
                {
                    // but it's not approved
                    return FriendshipStatus.Sent;
                }
            }

            // Ok, do we have a friendship request sent to us by the other user?
            friend = Users.FirstOrDefault(u => u.UserId == otherId);
            if (friend == null)
            {
                // No, never heard of them
                return FriendshipStatus.None;
            }
            else if (friend.RequestApproved.HasValue)
            {
                // Yes, and we approved it
                return FriendshipStatus.Approved;
            } 
            else
            {
                // Yes, but we haven't approved it yet
                return FriendshipStatus.Received;
            }

        }

        public async Task<GeoPoint> UpdateLatLong(IGeocoder geo)
        {
            var geoPoint = await geo.GeocodeAsync(
                        $"{Address1}, {Address2}, {City} {State} {Postal} {Country}"
                    );
            Latitude = geoPoint?.Latitude;
            Longitude = geoPoint?.Longitude;

            return geoPoint;
        }
        #endregion
    }
}