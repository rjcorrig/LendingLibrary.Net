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

using NUnit.Framework;
using LendingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendingLibrary.Tests.Models
{
    [TestFixture]
    public class ApplicationUserTests
    {
        [Test]
        public void GetFriendshipStatusWithTest()
        {
            var noFriends = new ApplicationUser {
                Users = new List<Friendship>(),
                Friendships = new List<Friendship>(),
                Id = Guid.NewGuid().ToString(),
                GivenName = "No Friends"
            };

            var sender = new ApplicationUser
            {
                Users = new List<Friendship>(),
                Friendships = new List<Friendship>(),
                Id = Guid.NewGuid().ToString(),
                GivenName = "Sender"
            };

            var receiver = new ApplicationUser
            {
                Users = new List<Friendship>(),
                Friendships = new List<Friendship>(),
                Id = Guid.NewGuid().ToString(),
                GivenName = "Receiver"
            };

            var popular = new ApplicationUser
            {
                Users = new List<Friendship>(),
                Friendships = new List<Friendship>(),
                Id = Guid.NewGuid().ToString(),
                GivenName = "Popular"
            };

            var request = new Friendship
            {
                User = sender,
                UserId = sender.Id,
                Friend = receiver,
                FriendId = receiver.Id,
                RequestSent = DateTime.UtcNow
            };
            sender.Friendships.Add(request);
            receiver.Users.Add(request);

            var popularIsSender = new Friendship
            {
                User = popular,
                UserId = popular.Id,
                Friend = receiver,
                FriendId = receiver.Id,
                RequestSent = DateTime.UtcNow,
                RequestApproved = DateTime.UtcNow
            };
            var popularIsSenderReciprocal = new Friendship
            {
                User = popularIsSender.Friend,
                UserId = popularIsSender.FriendId,
                Friend = popularIsSender.User,
                FriendId = popularIsSender.UserId,
                RequestSent = popularIsSender.RequestSent,
                RequestApproved = popularIsSender.RequestApproved
            };
            popular.Friendships.Add(popularIsSender);
            popular.Friendships.Add(popularIsSenderReciprocal);
            receiver.Users.Add(popularIsSender);
            receiver.Users.Add(popularIsSenderReciprocal);

            var popularIsReceiver = new Friendship
            {
                User = sender,
                UserId = sender.Id,
                Friend = popular,
                FriendId = popular.Id,
                RequestSent = DateTime.UtcNow,
                RequestApproved = DateTime.UtcNow
            };
            var popularIsReceiverReciprocal = new Friendship
            {
                User = popularIsReceiver.Friend,
                UserId = popularIsReceiver.FriendId,
                Friend = popularIsReceiver.User,
                FriendId = popularIsReceiver.UserId,
                RequestSent = popularIsReceiver.RequestSent,
                RequestApproved = popularIsReceiver.RequestApproved
            };

            sender.Friendships.Add(popularIsReceiver);
            sender.Friendships.Add(popularIsReceiverReciprocal);
            popular.Users.Add(popularIsReceiver);
            popular.Users.Add(popularIsReceiverReciprocal);

            Assert.AreEqual(FriendshipStatus.Sent, sender.GetFriendshipStatusWith(receiver));
            Assert.AreEqual(FriendshipStatus.Received, receiver.GetFriendshipStatusWith(sender));

            Assert.AreEqual(FriendshipStatus.None, noFriends.GetFriendshipStatusWith(receiver));

            Assert.AreEqual(FriendshipStatus.Approved, sender.GetFriendshipStatusWith(popular));
            Assert.AreEqual(FriendshipStatus.Approved, popular.GetFriendshipStatusWith(sender));
            Assert.AreEqual(FriendshipStatus.Approved, receiver.GetFriendshipStatusWith(popular));
            Assert.AreEqual(FriendshipStatus.Approved, popular.GetFriendshipStatusWith(receiver));

        }
    }
}