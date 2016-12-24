using System.Web;
using Dapper;

using Pro.Models;

namespace Pro.Logic
{
    public class Auth
    {
        public const string cookieName = "name";

        public enum Roles { Admin = 1,  Student = 2, Professor = 3};

        public enum ContentStatus { Accepted = 1, ChangedStudent = 2, ChangedProfessor = 3, Version = 4 };

        public static bool IsAuth()
        {
            var cookie = HttpContext.Current.Request.Cookies.Get(cookieName);
            return cookie != null;
        }

        public static int? CurrentUserId()
        {
            int? currentUserId = null;
            var cookie = HttpContext.Current.Request.Cookies.Get(cookieName);
            if (cookie != null)
            {
                string password = cookie["pass"];
                string login = cookie["login"];
                currentUserId = SimpleConnector.Connection
                    .QueryFirstOrDefault<int>(
                    "SELECT Id FROM Users WHERE password = @pass AND login = @login",
                    new { pass = password, login = login }
                    );
            }
            return currentUserId;
        }

        public static User CurrentUser()
        {
            User currentUser = null;
            var cookie = HttpContext.Current.Request.Cookies.Get(cookieName);
            if (cookie != null)
            {
                string password = cookie["pass"];
                string login = cookie["login"];
                currentUser = SimpleConnector.Connection
                    .QueryFirstOrDefault<User>(
                    "SELECT * FROM Users WHERE password = @pass AND login = @login",
                    new { pass = password, login = login }
                    );
            }
            return currentUser;
        }


        /*public static int? CurrentUserRoleId()
        {
            int? currentUserRoleId = null;
            int? currentUserId = Auth.CurrentUserId();
            if (currentUserId != null)
            {
                currentUserRoleId = SimpleConnector.Connection
                    .QueryFirstOrDefault<int>(
                    "SELECT Id FROM UserRoles WHERE UserId = @CurrUserId",
                    new { CurrUserId = currentUserId }
                    );
            }
            return currentUserRoleId;
        }*/

        public static UserRole CurrentUserRole()
        {
            UserRole currentUserRole = null;
            User currentUser = Auth.CurrentUser();
            if (currentUser != null)
            {
                currentUserRole = SimpleConnector.Connection
                    .QueryFirstOrDefault<UserRole>(
                    "SELECT * FROM UserRoles WHERE Id = @CurrUserRoleId",
                    new { CurrUserRoleId = currentUser.UserRoleId }
                    );
            }
            return currentUserRole;
        }
    }
}