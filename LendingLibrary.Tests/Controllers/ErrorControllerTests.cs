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
        protected ErrorController controller;
        protected Mock<HttpResponseBase> response;

        [SetUp()]
        public void SetupController()
        {
            response = new Mock<HttpResponseBase>();
            response.SetupProperty(r => r.StatusCode);

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Response).Returns(response.Object);

            var routeData = new RouteData();
            routeData.Values["controller"] = "AController";
            routeData.Values["action"] = "AnAction";

            var reqCtx = new RequestContext(context.Object, routeData);

            controller = new ErrorController();

            var ctlCtx = new ControllerContext(reqCtx, controller);
            controller.ControllerContext = ctlCtx;
        }

        [Test()]
        public void Index_returns_Error_View_with_500()
        {
            controller.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var result = controller.Index();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("Error", viewResult.ViewName);

            Assert.AreEqual((int)HttpStatusCode.InternalServerError, response.Object.StatusCode);
        }

        [Test()]
        public void Index_returns_Error_View_with_500_and_message()
        {
            var message = "A message";
            controller.TempData["Message"] = message;

            controller.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var result = controller.Index();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("Error", viewResult.ViewName);

            Assert.AreEqual((int)HttpStatusCode.InternalServerError, response.Object.StatusCode);

            Assert.IsInstanceOf(typeof(HandleErrorInfo), viewResult.Model);
            Assert.AreEqual(message, (viewResult.Model as HandleErrorInfo)?.Exception?.Message);
        }

        [Test()]
        public void Index_returns_Error_View_with_custom_httpStatus()
        {
            var status = (int)HttpStatusCode.NotImplemented;
            controller.TempData["StatusCode"] = status;
            var result = controller.Index();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("Error", viewResult.ViewName);

            Assert.AreEqual(status, response.Object.StatusCode);
        }

        [Test()]
        public void Forbidden_returns_Forbidden_View_with_403()
        {
            controller.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var result = controller.Forbidden();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("Forbidden", viewResult.ViewName);

            Assert.AreEqual((int)HttpStatusCode.Forbidden, response.Object.StatusCode);

            Assert.IsInstanceOf(typeof(HandleErrorInfo), viewResult.Model);
            Assert.AreEqual("Forbidden", (viewResult.Model as HandleErrorInfo)?.Exception?.Message);
        }

        [Test()]
        public void Forbidden_returns_Forbidden_View_with_403_and_message()
        {
            var message = "A message";
            controller.TempData["Message"] = message;

            controller.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var result = controller.Forbidden();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("Forbidden", viewResult.ViewName);

            Assert.AreEqual((int)HttpStatusCode.Forbidden, response.Object.StatusCode);

            Assert.IsInstanceOf(typeof(HandleErrorInfo), viewResult.Model);
            Assert.AreEqual(message, (viewResult.Model as HandleErrorInfo)?.Exception?.Message);
        }

        [Test()]
        public void Forbidden_returns_Forbidden_View_with_custom_httpStatus()
        {
            var status = (int)HttpStatusCode.NotImplemented;
            controller.TempData["StatusCode"] = status;
            var result = controller.Forbidden();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("Forbidden", viewResult.ViewName);

            Assert.AreEqual(status, response.Object.StatusCode);
        }

        [Test()]
        public void NotFound_returns_NotFound_View_with_404()
        {
            var status = (int)HttpStatusCode.NotFound;
            controller.HttpContext.Response.StatusCode = status;

            var result = controller.NotFound();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("NotFound", viewResult.ViewName);

            Assert.AreEqual(status, response.Object.StatusCode);

            Assert.IsInstanceOf(typeof(HandleErrorInfo), viewResult.Model);
            Assert.AreEqual("Not Found", (viewResult.Model as HandleErrorInfo)?.Exception?.Message);
        }

        [Test()]
        public void NotFound_returns_NotFound_View_with_404_and_message()
        {
            var message = "A message";
            controller.TempData["Message"] = message;

            var status = (int)HttpStatusCode.NotFound;
            controller.HttpContext.Response.StatusCode = status;

            var result = controller.NotFound();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("NotFound", viewResult.ViewName);

            Assert.AreEqual(status, response.Object.StatusCode);

            Assert.IsInstanceOf(typeof(HandleErrorInfo), viewResult.Model);
            Assert.AreEqual(message, (viewResult.Model as HandleErrorInfo)?.Exception?.Message);
        }

        [Test()]
        public void NotFound_returns_NotFound_View_with_custom_httpStatus()
        {
            var status = (int)HttpStatusCode.NotImplemented;
            controller.TempData["StatusCode"] = status;
            var result = controller.NotFound();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("NotFound", viewResult.ViewName);

            Assert.AreEqual(status, response.Object.StatusCode);
        }

        [Test()]
        public void BadRequest_returns_BadRequest_View_with_400()
        {
            var status = (int)HttpStatusCode.BadRequest;
            controller.HttpContext.Response.StatusCode = status;

            var result = controller.BadRequest();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("BadRequest", viewResult.ViewName);

            Assert.AreEqual(status, response.Object.StatusCode);

            Assert.IsInstanceOf(typeof(HandleErrorInfo), viewResult.Model);
            Assert.AreEqual("Bad Request", (viewResult.Model as HandleErrorInfo)?.Exception?.Message);
        }

        [Test()]
        public void BadRequest_returns_BadRequest_View_with_400_and_message()
        {
            var message = "A message";
            controller.TempData["Message"] = message;

            var status = (int)HttpStatusCode.BadRequest;
            controller.HttpContext.Response.StatusCode = status;

            var result = controller.BadRequest();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("BadRequest", viewResult.ViewName);

            Assert.AreEqual(status, response.Object.StatusCode);

            Assert.IsInstanceOf(typeof(HandleErrorInfo), viewResult.Model);
            Assert.AreEqual(message, (viewResult.Model as HandleErrorInfo)?.Exception?.Message);
        }

        [Test()]
        public void BadRequest_returns_BadRequest_View_with_custom_httpStatus()
        {
            var status = (int)HttpStatusCode.NotImplemented;
            controller.TempData["StatusCode"] = status;
            var result = controller.BadRequest();

            Assert.IsInstanceOf(typeof(ViewResult), result);

            var viewResult = result as ViewResult;
            Assert.AreEqual("BadRequest", viewResult.ViewName);

            Assert.AreEqual(status, response.Object.StatusCode);
        }

    }
}
