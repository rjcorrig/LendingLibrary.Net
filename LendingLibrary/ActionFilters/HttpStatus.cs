using System;
using System.Net;
using System.Web.Mvc;

namespace LendingLibrary.ActionFilters
{
    public class HttpStatus : ActionFilterAttribute
    {
        public HttpStatusCode Status { get; set; }

        public HttpStatus(HttpStatusCode statusCode)
        {
            Status = statusCode;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.StatusCode = (int)Status;
        }
    }
}
