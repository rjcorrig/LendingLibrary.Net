using NUnit.Framework;
using System;
using LendingLibrary.Tests.Models;
using LendingLibrary.Controllers;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using LendingLibrary.Models;
using System.Web;
using System.Net;
using Moq;

namespace LendingLibrary.Tests.Controllers
{
    [TestFixture()]
    public class FriendshipsControllerTests
    {
        [Test()]
        public async Task Index_returns_logged_in_Users_Friendships_in_View()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var result = await controller.Index() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as IEnumerable<Friendship>;
            Assert.IsNotNull(model);

            foreach (var friendship in model)
            {
                Assert.That(friendship.FriendId == userId || friendship.UserId == userId);
            }
        }    

        [Test()]
        [TestCase(false)]
        [TestCase(true)]
        public async Task Index_passes_RequestSent_to_ViewBag(bool RequestSent)
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var result = await controller.Index(RequestSent) as ViewResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(RequestSent, result.ViewBag.RequestSent);
        }    

        [Test()]
        public async Task Waiting_returns_Friendships_waiting_for_approval()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var result = await controller.Waiting() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as IEnumerable<Friendship>;
            Assert.IsNotNull(model);

            foreach (var friendship in model)
            {
                Assert.That(friendship.FriendId == userId && !friendship.RequestApproved.HasValue);
            }
        }    

        [Test()]
        [TestCase(false)]
        [TestCase(true)]
        public async Task Waiting_passes_RequestConfirmed_to_ViewBag(bool RequestConfirmed)
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var result = await controller.Waiting(RequestConfirmed) as ViewResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(RequestConfirmed, result.ViewBag.RequestConfirmed);
        }    

        [Test()]
        public void Confirm_throws_BadRequest_if_no_userId_passed()
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Confirm(null));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.BadRequest));

            mockDbContext.MockFriendships.Verify(m => m.Add(It.IsAny<Friendship>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Test()]
        [TestCase("coryhome-guid")]
        [TestCase("nosuchuser-guid")]
        public void Confirm_throws_NotFound_if_no_request_from_userId(string requestorId)
        {
            var userId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Confirm(requestorId));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.NotFound));

            mockDbContext.MockFriendships.Verify(m => m.Add(It.IsAny<Friendship>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Test()]
        public async Task Confirm_redirects_to_Waiting_if_valid()
        {
            var userId = "foxyboots9-guid";
            var requestorId = "robcory-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var result = await controller.Confirm(requestorId) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Waiting", result.RouteValues["action"]);
            Assert.AreEqual(true, result.RouteValues["RequestConfirmed"]);

            mockDbContext.MockFriendships.Verify(m => m.Add(It.IsAny<Friendship>()), Times.AtMostOnce());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.AtLeastOnce());
        }

        [Test()]
        public void Create_throws_BadRequest_if_no_userId_passed()
        {
            var userId = "robcory-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Create(null));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.BadRequest));

            mockDbContext.MockFriendships.Verify(m => m.Add(It.IsAny<Friendship>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Test()]
        public void Create_throws_BadRequest_if_self_passed()
        {
            var userId = "robcory-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Create(userId));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.BadRequest));

            mockDbContext.MockFriendships.Verify(m => m.Add(It.IsAny<Friendship>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Test()]
        public void Create_throws_BadRequest_if_Users_have_a_Friendship()
        {
            var userId = "robcory-guid";
            var targetUserId = "foxyboots9-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Create(targetUserId));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.BadRequest));

            mockDbContext.MockFriendships.Verify(m => m.Add(It.IsAny<Friendship>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Test()]
        public void Create_throws_NotFound_if_target_User_not_found()
        {
            var userId = "robcory-guid";
            var targetUserId = "nosuchuser-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var httpException = Assert.ThrowsAsync<HttpException>(async () => await controller.Create(targetUserId));
            Assert.That(httpException.GetHttpCode(), Is.EqualTo((int)HttpStatusCode.NotFound));

            mockDbContext.MockFriendships.Verify(m => m.Add(It.IsAny<Friendship>()), Times.Never());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Never());
        }

        [Test()]
        public async Task Create_redirects_to_Index_if_valid()
        {
            var userId = "robcory-guid";
            var targetUserId = "coryhome-guid";
            var mockDbContext = new MockContext();
            var controller = new FriendshipsController(mockDbContext.Object, () => userId);

            var result = await controller.Create(targetUserId) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual(true, result.RouteValues["RequestSent"]);

            mockDbContext.MockFriendships.Verify(m => m.Add(It.IsAny<Friendship>()), Times.AtMostOnce());
            mockDbContext.Verify(m => m.SaveChangesAsync(), Times.AtLeastOnce());
        }


    }
}
