using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;

namespace LendingLibrary.ActionFilters
{
    public class Log : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LogEvent("OnActionExecuting", filterContext.RouteData, filterContext.HttpContext.Response.StatusCode);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            LogEvent("OnActionExecuted", filterContext.RouteData, filterContext.HttpContext.Response.StatusCode);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            LogEvent("OnResultExecuting", filterContext.RouteData, filterContext.HttpContext.Response.StatusCode);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            LogEvent("OnResultExecuted", filterContext.RouteData, filterContext.HttpContext.Response.StatusCode);
        }

        private void LogEvent(string methodName, RouteData routeData, int status)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            var message = String.Format("{0} controller:{1} action:{2} status:{3}", methodName, controllerName, actionName, status);
            Debug.WriteLine(message, "Action Filter Log");
        }
    }
}

