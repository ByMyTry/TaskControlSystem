using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Pro.Models;

namespace Pro
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Account",
                url: "Account/{action}",
                defaults: new { controller = "Account", action = "Index" }
            );

            routes.MapRoute(
                name: "IsAuthenticated",
                url: "{*catchall}",
                defaults: new { controller = "Account", action = "Login" },
                constraints: new { httpMethod = new ConstraintAuthenticated() }
            );

            routes.MapRoute(
                name: "Admin",
                url: "Admin/{*catchall}",
                defaults: new { controller = "Account", action = "Login" },
                constraints: new { httpMethod = new ConstraintAdmin() }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
