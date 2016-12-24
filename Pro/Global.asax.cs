using System.Web.Mvc;
using System.Web.Routing;

using Pro.Logic;
using Pro.Models;

namespace Pro
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            SimpleConnector.Connect();
            Initializer.InitDB();
        }

        protected void Application_End()
        {
            SimpleConnector.Close();
        }
    }
}
