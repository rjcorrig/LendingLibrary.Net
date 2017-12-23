﻿using NUnit.Framework;
using System;
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
        public void Changes_Response_StatusCode_to_given_value()
        {
            var context = new ActionExecutedContext();
            var filter = new HttpStatus(HttpStatusCode.NotAcceptable);

            var response = new Mock<HttpResponseBase>();
            response.SetupProperty(r => r.StatusCode);

            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.Response).Returns(response.Object);

            context.HttpContext = httpContext.Object;
            filter.OnActionExecuted(context);

            Assert.AreEqual(httpContext.Object.Response.StatusCode, (int)HttpStatusCode.NotAcceptable);
        }
    }
}