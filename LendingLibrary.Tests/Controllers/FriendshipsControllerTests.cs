using NUnit.Framework;
using System;
using LendingLibrary.Tests.Models;
using LendingLibrary.Controllers;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using LendingLibrary.Models;

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

    }
}
