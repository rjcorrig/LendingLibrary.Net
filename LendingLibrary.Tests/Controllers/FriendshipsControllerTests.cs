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
        public async Task Index_returns_logged_in_Users_Friendships()
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
    }
}
