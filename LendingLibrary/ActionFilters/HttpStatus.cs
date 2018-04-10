using System;
using System.Net;
using System.Web.Mvc;

namespace LendingLibrary.ActionFilters
{
    public class HttpStatus : ActionFilterAttribute
    {
        protected int status;

        public HttpStatus(HttpStatusCode statusCode)
        {
            status = (int)statusCode;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.StatusCode = status;
        }
    }
}
