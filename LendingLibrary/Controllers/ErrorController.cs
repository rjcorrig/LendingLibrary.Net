using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LendingLibrary.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View("Error");
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View("NotFound");
        }

        public ActionResult Forbidden()
        {
            var status = TempData["StatusCode"] as int? ?? (int)HttpStatusCode.Forbidden;
            Response.StatusCode = status;

            var message = TempData["Message"] as String ?? "Forbidden";
            var controller = TempData["Controller"] as String ?? ControllerContext.RouteData.Values["controller"].ToString();
            var action = TempData["Action"] as String ?? ControllerContext.RouteData.Values["action"].ToString();

            var model = new HandleErrorInfo(new HttpException(status, message), controller, action);
            return View("Forbidden", model);
        }
    }
}
