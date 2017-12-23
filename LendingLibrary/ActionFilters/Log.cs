using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LendingLibrary.ActionFilters
{
    public class Log : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            LogEvent("OnActionExecuting", filterContext.RouteData, filterContext.HttpContext.Response);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            LogEvent("OnActionExecuted", filterContext.RouteData, filterContext.HttpContext.Response);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            LogEvent("OnResultExecuting", filterContext.RouteData, filterContext.HttpContext.Response);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            LogEvent("OnResultExecuted", filterContext.RouteData, filterContext.HttpContext.Response);
        }

        private void LogEvent(string methodName, RouteData routeData, HttpResponseBase response)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            var message = $@"{methodName} controller:{controllerName} action:{actionName}
            status:{response.Status} code:{response.StatusCode} sub:{response.SubStatusCode} desc:{response.StatusDescription}";
            Debug.WriteLine(message, "Action Filter Log");
        }
    }
}

