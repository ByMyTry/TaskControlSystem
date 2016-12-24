using System.Web;
using System.Web.Routing;

using Pro.Logic;

namespace Pro.Models
{
    public class ConstraintAuthenticated : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName,
                RouteValueDictionary values, RouteDirection routeDirection)
        {
            return !Auth.IsAuth();
        }
    }
}