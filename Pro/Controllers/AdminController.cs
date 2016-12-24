using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;

using Pro.Logic;
using Pro.Models;

namespace Pro.Controllers
{
    public class AdminController : Controller
    {
        private Dictionary<String, String[]> ScriptsByRole = new Dictionary<string, string[]>()
        {
            {"admin", new String[]{ @"DELETE FROM Logs WHERE UserId = @DeleteUserId" } }, //удалить логи админа
            {"student", new String[]{
                @"DELETE FROM contents
                    WHERE TaskId = Any
                        (SELECT id FROM Tasks 
                            WHERE StudentUserId = @DeleteUserId)", //удалить контенты связанные с тасками студента
                @"DELETE FROM Tasks WHERE StudentUserId = @DeleteUserId", //удалить таски студента
                @"DELETE FROM Logs WHERE UserId = @DeleteUserId",//удалить логи студента
                "DELETE FROM Users WHERE Id = @DeleteUserId" //удалить студента
                } },
            {"professor", new String[]{ //OR od AND????????????????????????????????????????????????????????????????????????????????/
                @"DELETE FROM Contents 
                    WHERE ContentTemplateId = ANY
	                    (SELECT id FROM ContentTemplates 
	                        WHERE TemplateId = ANY
		                        (SELECT Id FROM Templates 
		                            WHERE ProfessorUserId = @DeleteUserID))
                    OR TaskId = ANY 
	                    (SELECT id FROM Tasks
	                        WHERE TemplateId = ANY
		                        (SELECT Id FROM Templates 
		                            WHERE ProfessorUserId = @DeleteUserID))", //удалить контенты ссылающиеся на темплейт контенты, которые ссылаются на темплейты препода
                @"DELETE FROM ContentTemplates
                    WHERE TemplateId = ANY
                        (SELECT Id FROM Templates
                            WHERE ProfessorUserId = @DeleteUserID)", //удалить контенттемплейты которые ссылаются на темплейты препода
                @"DELETE FROM Tasks
	                WHERE TemplateId = ANY
		                (SELECT Id FROM Templates 
		                    WHERE ProfessorUserId = @DeleteUserID)", //удалить таски которые ссылаются на темплейты препода
                @"DELETE FROM Templates WHERE ProfessorUserId = @DeleteUserID)", //удалить темплейты препода
                @"DELETE FROM Logs WHERE UserId = @DeleteUserId", //удалить логи препода
                "DELETE FROM Users WHERE Id = @DeleteUserId", //удалить препода
                }}
        };

        private void DeleteUserById(int DeleteUserId)
        {
            //проверка роли
            UserRole userRole = SimpleConnector.Connection
                .QueryFirstOrDefault<UserRole>(
                @"SELECT UserRoles.Id, UserRoles.Name
                        FROM Users, UserRoles
                        WHERE Users.UserRoleId = UserRoles.Id
                        AND Users.Id = @DeleteUserId",
                    new { DeleteUserId = DeleteUserId }
                );

            //выбор сценария
            foreach (string script in ScriptsByRole[userRole.Name])
                SimpleConnector.Connection
                    .Execute(script, new { DeleteUserId = DeleteUserId }
                    );
        }

        // GET: Admin
        [HttpGet]
        public ActionResult UserList(int? DeleteUserId)
        {
            //удалить юзера через Id если инт не null
            if (DeleteUserId != null)
                DeleteUserById((int)DeleteUserId);

            IEnumerable<UserModel> users = SimpleConnector.Connection
                    .Query<UserModel>(
                    @"SELECT Users.Id, Users.Login, Users.Name, UserRoles.Name as UserRoleName
                        FROM Users, UserRoles 
                        WHERE Users.UserRoleId = UserRoles.Id"
                    );

            return View(users);
        }

        public ActionResult UserLogs(int? UserId)
        {
            IEnumerable<Log> logs = SimpleConnector.Connection
                .Query<Log>(
                @"SELECT *
                    FROM Logs
                    WHERE Logs.UserId = @UserId",
                new { UserId = UserId }
                );
            List<LogModel> logModels = new List<LogModel>();
            foreach (var log in logs)
            {
                logModels.Add(new LogModel()
                {
                    Id = log.Id,
                    Date = log.Date,
                    User = SimpleConnector.Connection
                        .QueryFirstOrDefault<User>(
                        @"SELECT *
                            FROM Users
                            WHERE Users.Id = @UserId",
                        new { UserId = log.UserId }
                        ),
                    LogAction = SimpleConnector.Connection
                        .QueryFirstOrDefault<LogAction>(
                        @"SELECT *
                            FROM LogActions
                            WHERE LogActions.Id = @LogActionId",
                        new { LogActionId = log.LogActionId }
                        )
                });
            }
            return View(logModels);
        }
    }
}