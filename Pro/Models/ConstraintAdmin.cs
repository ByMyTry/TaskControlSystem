using System.Web;
using System.Web.Routing;

using Pro.Logic;

namespace Pro.Models
{
    public class ConstraintAdmin : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName,
                RouteValueDictionary values, RouteDirection routeDirection)
        {
            return !(Auth.CurrentUserRole().Id == (int)Auth.Roles.Admin);
        }
    }
}