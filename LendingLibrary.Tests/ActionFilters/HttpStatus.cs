using NUnit.Framework;
using LendingLibrary.ActionFilters;
using System.Net;
using System.Web.Mvc;
using System.Web;
using Moq;

namespace LendingLibrary.Tests.ActionFilters
{
    [TestFixture()]
    public class HttpStatusTests
    {
        [Test()]
        public void Changes_Response_StatusCode_to_given_HttpStatusCode()
        {
            var context = new ActionExecutingContext();
            var filter = new HttpStatus(HttpStatusCode.NotAcceptable);

            var response = new Mock<HttpResponseBase>();
            response.SetupProperty(r => r.StatusCode);

            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.Response).Returns(response.Object);

            context.HttpContext = httpContext.Object;
            filter.OnActionExecuting(context);

            Assert.AreEqual(httpContext.Object.Response.StatusCode, (int)HttpStatusCode.NotAcceptable);
        }

        [Test()]
        public void Changes_Response_StatusCode_to_given_int()
        {
            var context = new ActionExecutingContext();
            var filter = new HttpStatus(406);

            var response = new Mock<HttpResponseBase>();
            response.SetupProperty(r => r.StatusCode);

            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.Response).Returns(response.Object);

            context.HttpContext = httpContext.Object;
            filter.OnActionExecuting(context);

            Assert.AreEqual(httpContext.Object.Response.StatusCode, 406);
        }

    }
}
