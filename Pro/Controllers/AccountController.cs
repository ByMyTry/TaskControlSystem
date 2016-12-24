using System;
using System.Web;
using System.Web.Mvc;
using Dapper;

using Pro.Logic;
using Pro.Models;

namespace Pro.Controllers
{
    public class AccountController : Controller
    {
        public const string cookieName = "name";

        // GET: Account
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // пытаемся найти юзера
                User currentUser = SimpleConnector.Connection
                    .QueryFirstOrDefault<User>(
                    "SELECT * FROM Users WHERE password = @pass AND login = @login",
                    new { pass = model.Password, login = model.Login }
                    );

                if (currentUser != null)
                {
                    var cookie = this.HttpContext.Request.Cookies.Get(cookieName);
                    if (cookie == null)
                    {
                        var cook = new HttpCookie(cookieName);
                        cook["pass"] = model.Password;
                        cook["login"] = model.Login;
                        this.HttpContext.Response.Cookies.Set(cook);
                    }

                    ////log user login
                    Logger.Log("login");
                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
            }
            ViewBag.returnUrl = returnUrl;
            //return View(model);
            return RedirectToRoute(returnUrl);
        }

        public ActionResult Register()
        {
            //RegisterModel register = new RegisterModel();
            //register.Roles = new SelectList(TestData.UserRoles, "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // попытка регистрации
                ////add user to db
                SimpleConnector.Connection
                    .Execute(
                    "INSERT INTO Users (Name, Surname, UserRoleId, Login, Password) VAlUES (@Name, @Surname, @UserRoleId, @Login, @Password)",
                    new { Name = model.Name, Surname = model.Surname, UserRoleId = model.RoleId, Login = model.Login, Password = model.Password }
                    );

                int currentUserId = SimpleConnector.Connection
                    .QueryFirstOrDefault<int>(
                    "SELECT Id FROM Users WHERE password = @pass AND login = @login",
                    new { pass = model.Password, login = model.Login }
                    );

                ////log user register
                Logger.Log("register", currentUserId);

                this.Login(new LoginModel { Password = model.Password, Login = model.Login }, "");

                if (true)
                {
                    return RedirectToAction("Login");
                }
            }

            return View();
        }

        public ActionResult Logout()
        {
            var cookie = this.HttpContext.Request.Cookies.Get(cookieName);
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddHours(-1);
                this.HttpContext.Response.Cookies.Set(cookie);
            }

            ////log user logout
            Logger.Log("logout");

            return RedirectToAction("Login");
        }
    }
}