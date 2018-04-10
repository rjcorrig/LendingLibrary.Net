using System;
using System.Web.Mvc;

namespace LendingLibrary.Utils.Extensions
{
    public static class ControllerExtensions
    {
        public static string RouteName(this ControllerBase controller)
        {
            return controller.ControllerContext.RouteData.Values["controller"].ToString();
        }

        public static string RouteAction(this ControllerBase controller)
        {
            return controller.ControllerContext.RouteData.Values["action"].ToString();
        }
    }
}
