using NUnit.Framework;
using System;
using LendingLibrary.Controllers;
using System.Web.Mvc;
using Moq;
using System.Web;
using System.Web.Routing;

namespace LendingLibrary.Tests.Controllers
{
    [TestFixture()]
    public class ErrorControllerTests
    {
        [Test()]
        public void Index_returns_Error_View()
        {
            var response = new Mock<HttpResponseBase>();
            response.SetupProperty(r => r.StatusCode);

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Response).Returns(response.Object);

            var reqCtx = new RequestContext(context.Object, new RouteData());

			var controller = new ErrorController();

            var ctlCtx = new ControllerContext(reqCtx, controller);
            controller.ControllerContext = ctlCtx;

            var result = controller.Index();

            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Error", ((ViewResult)result).ViewName);
            Assert.AreEqual(500, response.Object.StatusCode);
        }
    }
}
