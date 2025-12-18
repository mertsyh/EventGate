using System;
using System.Web.Mvc;

namespace EventDeneme.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(int? statusCode, string errorMessage)
        {
            ViewBag.StatusCode = statusCode ?? 500;
            ViewBag.ErrorMessage = errorMessage ?? "An error occurred while processing your request.";
            Response.StatusCode = ViewBag.StatusCode;
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}

