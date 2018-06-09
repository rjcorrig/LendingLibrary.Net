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
using System.ComponentModel.DataAnnotations;

namespace LendingLibrary.Models
{
    public enum FriendshipStatus
    {
        None,       /* No relationship established */
        Sent,       /* Request sent, from POV of sender */
        Received,   /* Request received, from POV of receiver */
        Approved    /* Request approved */
    }

    public class Friendship
    {
        public virtual string UserId { get; set; }
        public virtual string FriendId { get; set; }
        [Required, Display(Name = "Request Sent")]
        public virtual DateTime RequestSent { get; set; }
        [Display(Name = "Request Approved")]
        public virtual DateTime? RequestApproved { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Friend { get; set; }
    }
}