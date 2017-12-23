using NUnit.Framework;
using System;
using LendingLibrary.Controllers;
using System.Web.Mvc;
using Moq;
using System.Web;
using System.Web.Routing;
using System.Net;

namespace LendingLibrary.Tests.Controllers
{
    [TestFixture()]
    public class ErrorControllerTests
    {
        [Test()]
        public void Index_returns_Error_View_with_500()
        {
            var response = new Mock<HttpResponseBase>();
            response.SetupProperty(r => r.StatusCode);

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Response).Returns(response.Object);

            var routeData = new RouteData();
            routeData.Values["controller"] = "AController";
            routeData.Values["action"] = "AnAction";

            var reqCtx = new RequestContext(context.Object, routeData);

			var controller = new ErrorController();

            var ctlCtx = new ControllerContext(reqCtx, controller);
            controller.ControllerContext = ctlCtx;

            var result = controller.Index();

            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Error", ((ViewResult)result).ViewName);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, response.Object.StatusCode);
        }

        [Test()]
        public void Forbidden_returns_Forbidden_View_with_403()
        {
            var response = new Mock<HttpResponseBase>();
            response.SetupProperty(r => r.StatusCode);

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Response).Returns(response.Object);

            var routeData = new RouteData();
            routeData.Values["controller"] = "AController";
            routeData.Values["action"] = "AnAction";

            var reqCtx = new RequestContext(context.Object, routeData);

            var controller = new ErrorController();

            var ctlCtx = new ControllerContext(reqCtx, controller);
            controller.ControllerContext = ctlCtx;

            var result = controller.Forbidden();

            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Forbidden", ((ViewResult)result).ViewName);
            Assert.AreEqual((int)HttpStatusCode.Forbidden, response.Object.StatusCode);
        }
    }
}
