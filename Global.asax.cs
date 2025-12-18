using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EventDeneme
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;
            int statusCode = httpException != null ? httpException.GetHttpCode() : 500;
            string errorMessage = httpException != null ? httpException.Message : "An error occurred while processing your request.";

            Response.StatusCode = statusCode;
            Response.TrySkipIisCustomErrors = true;

            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "Index";
            routeData.Values["statusCode"] = statusCode;
            routeData.Values["errorMessage"] = errorMessage;

            IController errorController = new Controllers.ErrorController();
            var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
            errorController.Execute(rc);
        }
    }
}


