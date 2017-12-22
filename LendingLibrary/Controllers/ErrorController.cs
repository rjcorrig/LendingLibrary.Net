﻿using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LendingLibrary.Controllers
{
    public class ErrorController : BaseController
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
            var controller = TempData["Controller"] as String ?? Name;
            var action = TempData["Action"] as String ?? Action;
            var message = TempData["Message"] as String ?? "Forbidden";
			var status = TempData["StatusCode"] as int? ?? (int)HttpStatusCode.Forbidden;

			Response.StatusCode = status;
            var model = new HandleErrorInfo(new HttpException(status, message), controller, action);
            return View("Forbidden", model);
        }
    }
}
