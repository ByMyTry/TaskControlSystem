using Dapper;

using Pro.Logic;

namespace Pro.Models
{
    public class Initializer
    {
        public static void InitDB()
        {
            /*SimpleConnector.Connection
                .Execute(
                "INSERT INTO UserRoles (Name) VAlUES (@Name)",
                new[] { new { Name = "admin" }, new { Name = "student" }, new { Name = "professor" } }
                );

            SimpleConnector.Connection
                .Execute(
                "INSERT INTO Users (Name, Surname, Login, Password, UserRoleId) VAlUES (@Name, @Surname, @Login, @Password, @UserRoleId)",
                new[] {
                    new { Name = "admin1", Surname = "admin1", Login = "admin1", Password = "admin1", UserRoleId = 1 },
                    new { Name = "student1", Surname = "student1", Login = "student1", Password = "student1", UserRoleId = 2},
                    new { Name = "professor1", Surname = "proffessor1", Login = "professor1", Password = "professor1", UserRoleId = 3}
                }
                );

            SimpleConnector.Connection
                .Execute(
                "INSERT INTO LogActions (Action) VAlUES (@Action)",
                new[] {
                    new { Action = "login"},
                    new { Action = "logout"},
                    new { Action = "register"},
                    new { Action = "create task" },
                    new { Action = "change task"},
                    new { Action = "handed task"},
                    new { Action = "create task tamplate" },
                    new { Action = "check task"},
                    new { Action = "accept task"}
                }
                );

            //добавить состояния
            SimpleConnector.Connection
                .Execute(
                "INSERT INTO ContentStatuss (Status) VALUES (@Status)",
                new[] {
                    new { Status = "Accepted"},
                    new { Status = "ChangedStudent"},
                    new { Status = "ChangedProfessor"},
                    new { Status = "Version" }
                }
                );

            //sc.Add<Template>(new Template(){Name = "Kursavoi", ProfessorUserId = 3});
            SimpleConnector.Connection
                .Execute(
                "INSERT INTO Templates (Name, ProfessorUserId) VALUES (@Name, @ProfessorUserId)",
                    new { Name = "Laba" , ProfessorUserId = 3}
                );

            //sc.Add<ContentTemplate>(new ContentTemplate(){Name = "Vvedenie", Requirements = "something"});// оПасно
            SimpleConnector.Connection
                .Execute(
                "INSERT INTO ContentTemplates (Name, Requirements, TemplateId) VALUES (@Name, @Requirements, @TemplateId)",
                    new { Name = "Vvedenie", Requirements = "something", TemplateId = 1}
                );

            //Добавить 1 таску и 2 контента
            SimpleConnector.Connection
                .Execute(
                @"INSERT INTO Tasks (Name, Text, IsAccepted, StudentUserId, TemplateId, CountChanges) 
                VALUES (@Name, @Text, @IsAccepted, @StudentUserId, @TemplateId, @CountChanges)",
                    new { Name = "Tasochka", Text = "napisat labu", IsAccepted = 0, StudentUserId = 2, TemplateId = 1, CountChanges = 0 }
                );

            SimpleConnector.Connection
                .Execute(
                @"INSERT INTO Contents (Text, Comment, Date, TaskId, ContentTemplateId, ContentStatusId) 
                VALUES (@Text, @Comment, @Date, @TaskId, @ContentTemplateId, @ContentStatusId)",
                new[] {
                    new { Text = "int i = 3", Comment = "wtf", Date = System.DateTime.Now, TaskId = 1, ContentTemplateId = 1, ContentStatusId = Auth.ContentStatus.Accepted },
                    //new { Text = "int i = 3", Comment = "wtf", Date = System.DateTime.Now, TaskId = 1, ContentTemplateId = 1, ContentStatusId = 4 },
                }
                );*/
        }
    }
}