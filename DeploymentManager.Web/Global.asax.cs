using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using DeploymentManager.Web.App_Start;

namespace DeploymentManager.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var config = GlobalConfiguration.Configuration;

            config.Routes.MapHttpRoute(
                "WebApi",
                "api/{controller}/{action}/{id}", 
                new {id = RouteParameter.Optional}
            );

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

      
    }
}