using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LendingLibrary.ActionFilters;
using LendingLibrary.Utils.Extensions;

namespace LendingLibrary.Controllers
{
    [Log()]
    public class ErrorController : Controller
    {
        [HttpStatus(HttpStatusCode.InternalServerError)]
        public ActionResult Index()
        {
            HandleErrorInfo model = CreateViewModel();
            return View("Error", model);
        }

        [HttpStatus(HttpStatusCode.NotFound)]
        public ActionResult NotFound()
        {
            HandleErrorInfo model = CreateViewModel();
            return View("NotFound", model);
        }

        [HttpStatus(HttpStatusCode.BadRequest)]
        public ActionResult BadRequest()
        {
            HandleErrorInfo model = CreateViewModel();
            return View("BadRequest", model);
        }

        [HttpStatus(HttpStatusCode.Forbidden)]
        public ActionResult Forbidden()
        {
            HandleErrorInfo model = CreateViewModel();
            return View("Forbidden", model);
        }

        protected HandleErrorInfo CreateViewModel()
        {
            var controller = TempData["Controller"] as String ?? this.RouteName();
            var action = TempData["Action"] as String ?? this.RouteAction();

            Response.StatusCode = TempData["StatusCode"] as int? ?? Response.StatusCode;
            var message = TempData["Message"] as String ?? HttpWorkerRequest.GetStatusDescription(Response.StatusCode);

            var model = new HandleErrorInfo(new HttpException(Response.StatusCode, message), controller, action);
            return model;
        }
    }
}
