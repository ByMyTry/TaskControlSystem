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
    public class HomeController : Controller
    {        
        // GET: Home        
        public ActionResult Index(int? id)
        {
            if (id == null) id = Auth.CurrentUser().Id;
            if (Auth.CurrentUser().Id != id) return HttpNotFound();

            IEnumerable<Task> _tasks = SimpleConnector.Connection
                    .Query<Task>(
                    @"SELECT *
                        FROM  Tasks
                        WHERE Tasks.StudentUserId = @CurrentUserId
                        OR Tasks.Id = ANY 
                        (SELECT Tasks.Id FROM Users, Templates, Tasks
                        WHERE Templates.Id = Tasks.TemplateId
                        AND Users.Id = Templates.ProfessorUserId
                        AND Users.Id = @CurrentUserId)",
                        new { CurrentUserId = id }
                    );

            List<TaskModel> tasks = new List<TaskModel>();

            foreach (Task task in _tasks)
            {
                
                dynamic StudentName = SimpleConnector.Connection 
                    .QueryFirstOrDefault<dynamic>(
                    @"SELECT Users.Name, Users.Surname
                        FROM Users, Tasks 
                        WHERE Tasks.StudentUserId = Users.Id
                        AND @TaskId = Tasks.Id",
                        new { TaskId = task.Id }
                    );
                dynamic ProfessorName = SimpleConnector.Connection
                    .QueryFirstOrDefault<dynamic>(
                    @"SELECT Users.Name, Users.Surname
                        FROM Users, Tasks, Templates
                        WHERE Templates.ProfessorUserId = Users.Id
                        AND Tasks.TemplateId = Templates.Id
                        AND @TaskId = Tasks.Id",
                        new { TaskId = task.Id }
                    );
                string TemplateName = SimpleConnector.Connection
                    .QueryFirstOrDefault<String>(
                    @"SELECT Templates.Name
                        FROM Templates, Tasks
                        WHERE Tasks.TemplateId = Templates.Id
                        AND @TaskId = Tasks.Id",
                        new { TaskId = task.Id }
                    );
                tasks.Add(new TaskModel()
                {
                    Id = task.Id,
                    Name = task.Name,
                    isAccepted = task.isAccepted,
                    Student = StudentName.Name + " " + StudentName.Surname,
                    Professor = ProfessorName.Name + " " + ProfessorName.Surname,
                    Template = TemplateName,
                    CountChanges = task.CountChanges
                });
            }

            //List<TaskModel> tasks = new List<TaskModel>();

            // заполнение общих полей
            //tasks = TestData.TasksModel.Where(t => t.Id == id).ToList(); // тестовые, тут должны быть таски, которые относятся к идентифицированному юзеру            

            switch (Auth.CurrentUserRole().Id)
            {
                case (int)Auth.Roles.Admin:
                    ViewBag.Tamplate = TestData.Templates;
                    return View("IndexAdmin", tasks);
                case (int)Auth.Roles.Professor:
                    //ViewBag.Tamplate = TestData.Templates.Where(t => t.ProfessorUserId == id);
                    ViewBag.Tamplate = SimpleConnector.Connection
                        .Query<Template>(
                        @"SELECT *
                            FROM  Templates
                            WHERE Templates.ProfessorUserId = @CurrentUserId",
                            new { CurrentUserId = id }
                        );
                    return View("IndexProfessor", tasks);
                case (int)Auth.Roles.Student:
                    return View("IndexStudent", tasks);
            }
            return RedirectToAction("Login", "Account");
        }

        public ActionResult CreateTemplate()
        {
            TemplateModel template = new TemplateModel();
            template.ProfessorUserId = Auth.CurrentUser().Id;

            return View(template);
        }

        [HttpPost]
        public ActionResult CreateTemplate(TemplateModel template, string action)
        {
            try
            {
                if (action == "Отмена") return RedirectToAction("Index");

                if (ModelState.IsValid)
                {

                    //template.Id = TestData.Templates.Count + 1; // задай Id
                    // добавление шаблона
                    //TestData.Templates.Add(new Template { Id = template.Id, Name = template.Name, ProfessorUserId = template.ProfessorUserId });
                    SimpleConnector.Connection
                       .Execute(
                       "INSERT INTO Templates (Name, ProfessorUserId) VALUES (@Name, @ProfessorUserId)",
                           new { Name = template.Name, ProfessorUserId = template.ProfessorUserId }
                       );

                    int id = SimpleConnector.Connection
                        .QueryFirstOrDefault<int>(
                        @"SELECT Id
                            FROM  Templates
                            WHERE Templates.ProfessorUserId = @UserId
                            AND Templates.Name = @Name",
                            new { UserId = template.ProfessorUserId, Name = template.Name }
                        );

                    return RedirectToAction("EditTemplate", new { id = id });
                }
                else
                {
                    return View(template);
                }
            }
            catch
            {
                return View(template);
            }
        }

        public ActionResult EditTemplate(int id)
        {
            //int professorUserId = TestData.Templates.Find(t => t.Id == id).ProfessorUserId; // ищем владельца шаблона
            //Template bdTemplte = TestData.Templates.Find(t => t.Id == id); // достаём шаблон
            Template template = SimpleConnector.Connection
                        .QueryFirstOrDefault<Template>(
                        @"SELECT *
                            FROM  Templates
                            WHERE Templates.Id = @EditTemplateId",
                            new { EditTemplateId = id }
                        );
            if (template.ProfessorUserId != Auth.CurrentUser().Id) return HttpNotFound();         
        
           
            TemplateModel templateModel = new TemplateModel();
            templateModel.Id = template.Id;
            templateModel.Name = template.Name;
            //templateModel.ContentTemplates = TestData.ContentTemplatesModel.Where(ct => ct.TemplateId == id).ToList();
            templateModel.ContentTemplates = SimpleConnector.Connection
                        .Query<ContentTemplateModel>(
                        @"SELECT *
                            FROM  ContentTemplates
                            WHERE ContentTemplates.TemplateId = @EditTemplateId",
                            new { EditTemplateId = id }
                            ).ToList();

            return View(templateModel);
        }        

        [HttpPost]
        public ActionResult EditTemplate(TemplateModel template, string action)
        {
            try
            {
                if (action == "Назад") return RedirectToAction("Index");

                // изменение Name шаблона      
                //TestData.Templates.Find(t => t.Id == template.Id).Name = template.Name;
                SimpleConnector.Connection
                       .Execute(
                       "UPDATE Templates SET Templates.Name = @Name WHERE Templates.Id = @UpdateTemplateId",
                           new { UpdateTemplateId = template.Id, Name = template.Name }
                       );

                return RedirectToAction("Index");
            }
            catch
            {
                return View(template);
            }
        }

        [HttpPost]
        public ActionResult EditContentTemplate(ContentTemplateModel contentTemplate)
        {            
            /*ContentTemplateModel bdContentTemplate = TestData.ContentTemplatesModel.Find(ct => ct.Id == contentTemplate.Id); // достаём запись из бд
            // заполняем её            
            bdContentTemplate.Name = contentTemplate.Name;
            bdContentTemplate.Requirements = contentTemplate.Requirements;*/
            SimpleConnector.Connection
                .Execute(
                "UPDATE ContentTemplates SET Name = @Name, Requirements = @Requirements WHERE ContentTemplates.Id = @Id",
                    new { Id = contentTemplate.Id, Name = contentTemplate.Name, Requirements = contentTemplate.Requirements }
                );

            return RedirectToAction("EditTemplate", new { id = contentTemplate.TemplateId } );
        }

        public ActionResult AddContentTemplate(ContentTemplateModel contentTemplate)
        {
            /*contentTemplate.Id = TestData.ContentTemplatesModel.Count + 1; // задай Id
            // добавляем в базу           
            TestData.ContentTemplatesModel.Add(new ContentTemplateModel {
                Id = contentTemplate.Id,
                Name = contentTemplate.Name,
                Requirements = contentTemplate.Requirements,
                TemplateId = contentTemplate.TemplateId
            } );*/
            SimpleConnector.Connection
                .Execute(
                "INSERT INTO ContentTemplates (Name, Requirements, TemplateId) VALUES (@Name, @Requirements, @TemplateId)",
                    new { Name = contentTemplate.Name, Requirements = contentTemplate.Requirements, TemplateId = contentTemplate.TemplateId }
                );

            return RedirectToAction("EditTemplate", new { id = contentTemplate.TemplateId });
        }        

        public ActionResult DeleteTask(int id)
        {
            try
            {
                //if (Auth.CurrentUser().Id != id препода или владельца студента или админа) return HttpNotFound();
                if(Auth.CurrentUserRole().Id == (int)Auth.Roles.Professor)
                {
                    int professorId = SimpleConnector.Connection
                        .QueryFirstOrDefault<int>(
                        @"SELECT Users.Id FROM Users, Templates, Tasks
                            WHERE Templates.Id = Tasks.TemplateId
                            AND Users.Id = Templates.ProfessorUserId
                            AND Tasks.Id = @DeleteTaskId",
                            new { DeleteTaskId = id }
                            );
                    if(Auth.CurrentUserId() != professorId) return HttpNotFound();
                }
                else if(Auth.CurrentUserRole().Id == (int)Auth.Roles.Student)
                {
                    int studentId = SimpleConnector.Connection
                        .QueryFirstOrDefault<int>(
                        @"SELECT Users.Id FROM Users, Tasks
                            AND Users.Id = Tasks.StudentUserId
                            AND Tasks.Id = @DeleteTaskId",
                            new { DeleteTaskId = id }
                            );
                    if (Auth.CurrentUserId() != studentId) return HttpNotFound();
                }

                //TestData.TasksModel.Remove(TestData.TasksModel.Find(tm => tm.Id == id)); // удаляем таску
                SimpleConnector.Connection
                    .Execute(@"DELETE FROM contents WHERE TaskId = @DeleteTaskId", 
                    new { DeleteTaskId = id }
                    );
                SimpleConnector.Connection
                    .Execute(@"DELETE FROM Tasks WHERE Id = @DeleteTaskId",
                    new { DeleteTaskId = id }
                    );

                return RedirectToAction("Index");
            }
            catch
            {
                return HttpNotFound();
            }            
        }

        public ActionResult DeleteTemplate(int id)
        {
            try
            {
                //if (Auth.CurrentUser().Id != id владельца препода или админа) return HttpNotFound();
                if (Auth.CurrentUserRole().Id == (int)Auth.Roles.Professor)
                {
                    int professorId = SimpleConnector.Connection
                        .QueryFirstOrDefault<int>(
                        @"SELECT Users.Id FROM Users, Templates
                            WHERE Users.Id = Templates.ProfessorUserId
                            AND Templates.Id = @DeleteTemplateId",
                            new { DeleteTemplateId = id }
                            );
                    if (Auth.CurrentUserId() != professorId) return HttpNotFound();
                }
                else if (Auth.CurrentUserRole().Id == (int)Auth.Roles.Student)
                {
                    int studentId = SimpleConnector.Connection
                        .QueryFirstOrDefault<int>(
                        @"SELECT Users.Id FROM Users, Tasks
                            AND Users.Id = Tasks.StudentUserId
                            AND Templates.Id = @DeleteTemplateId",
                            new { DeleteTemplateId = id }
                            );
                    if (Auth.CurrentUserId() != studentId) return HttpNotFound();
                }

                //TestData.Templates.Remove(TestData.Templates.Find(t => t.Id == id)); // удаляем шаблон
                SimpleConnector.Connection
                    .Execute(@"DELETE FROM contents 
                                WHERE TaskId = ANY
                                    (SELECT Tasks.Id FROM Tasks, Templates 
                                    WHERE Tasks.TemplateId = Templates.Id 
                                    AND Templates.Id = @DeleteTemplateId)
                                OR ContentTemplateId = ANY
                                    (SELECT ContentTemplates.Id FROM ContentTEmplates, Templates 
                                    WHERE ContentTemplates.TemplateId = Templates.Id 
                                    AND Templates.Id = @DeleteTemplateId)",
                    new { DeleteTemplateId = id }
                    );
                /*SimpleConnector.Connection
                    .Execute(@"DELETE FROM Tasks
                                WHERE Tasks.Id IN
                                    (SELECT Tasks.Id FROM Tasks, Templates 
                                    WHERE Tasks.TemplateId = Templates.Id 
                                    AND Templates.Id = @DeleteTemplateId)",
                    new { DeleteTemplateId = id }
                    );*/
                SimpleConnector.Connection
                    .Execute(@"DELETE FROM Tasks
                                WHERE Tasks.TemplateId = @DeleteTemplateId",
                new { DeleteTemplateId = id }
                );
                SimpleConnector.Connection
                    .Execute(@"DELETE FROM ContentTemplates
                                WHERE ContentTemplates.TemplateId = @DeleteTemplateId",
                    new { DeleteTemplateId = id }
                    );
                SimpleConnector.Connection
                    .Execute(@"DELETE FROM Templates
                                WHERE Templates.Id = @DeleteTemplateId",
                    new { DeleteTemplateId = id }
                    );

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                return HttpNotFound();
            }            
        }

        public ActionResult CreateTask()
        {
            CE_TaskModel taskModel = new CE_TaskModel();

            // создаём список преподов для вью модели
            /*List<UserModel> professors = new List<UserModel>();

            foreach (User user in TestData.Users.Where(u => u.UserRoleId == (int)Auth.Roles.Professor))
            {
                professors.Add(new UserModel { Id = user.Id, Login = user.Login, Name = user.Name + " " + user.Surname });
            }*/
            List<UserModel> professors = SimpleConnector.Connection
                        .Query<UserModel>(
                        @"SELECT Users.Id, Users.Login, CONCAT(Users.Name, ' ', Users.Surname) as Name, UserRoles.Name as UserRoleName
                            FROM Users, UserRoles
                            WHERE Users.UserRoleId = @UserRoleId
                            AND Users.UserRoleId = UserRoles.Id",
                            new { UserRoleId = Auth.Roles.Professor }
                            ).ToList();

            taskModel.Professors = new SelectList(professors, "Id", "Name");
            
            return View(taskModel);
        }

        [HttpPost]
        public ActionResult CreateTask(CE_TaskModel task, string action)
        {
            try
            {
                if (action == "Отмена") return RedirectToAction("Index");

                if (ModelState.IsValid)
                {
                    /*task.Id = Auth.CurrentUser().Id; // задай Id
                                                     // добавление таски
                    TestData.TasksModel.Add(new TaskModel
                    {
                        Id = task.Id,
                        Name = task.Name,
                        Professor = task.ProfessorId.ToString(),    // тут id выбранного препода
                        Template = task.TemplateId.ToString(),      // тут id выбранного шаблона
                        Student = Auth.CurrentUser().Id.ToString()  // активный студент
                                                                    // текст задания в task.Text
                    });*/
                    SimpleConnector.Connection
                        .Execute(
                        @"INSERT INTO Tasks (Name, Text, IsAccepted, StudentUserId, TemplateId, CountChanges) 
                        VALUES (@Name, @Text, @IsAccepted, @StudentUserId, @TemplateId, @CountChanges)",
                            new { Name = task.Name, Text = task.Text, IsAccepted = 0, StudentUserId = Auth.CurrentUserId(), TemplateId = task.TemplateId, CountChanges = 0 }
                        );

                    return RedirectToAction("Index");
                }
                else
                {
                    /*List<UserModel> professors = new List<UserModel>();

                    foreach (User user in TestData.Users.Where(u => u.UserRoleId == (int)Auth.Roles.Professor))
                    {
                        professors.Add(new UserModel { Id = user.Id, Login = user.Login, Name = user.Name + " " + user.Surname });
                    }*/

                    List<UserModel> professors = SimpleConnector.Connection
                        .Query<UserModel>(
                        @"SELECT Users.Id, Users.Login, CONCAT(Users.Name, ' ', Users.Surname) as Name, UserRoles.Name as UserRoleName
                            FROM Users, UserRoles
                            WHERE Users.UserRoleId = @UserRoleId
                            AND Users.UserRoleId = UserRoles.Id",
                            new { UserRoleId = Auth.Roles.Professor }
                            ).ToList();

                    task.Professors = new SelectList(professors, "Id", "Name");

                    return View(task);
                }
            }
            catch
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult PossibleTemplates(int? ProfessorId)
        {
            CE_TaskModel taskModel = new CE_TaskModel();

            // создаём список шаблонов для вью модели
            /*List<Template> templates = new List<Template>();

            foreach (Template template in TestData.Templates.Where(u => u.ProfessorUserId == ProfessorId))
            {
                templates.Add(new Template { Id = template.Id, Name = template.Name });
            }*/

            List<Template> templates = SimpleConnector.Connection
                        .Query<Template>(
                        @"SELECT *
                            FROM Templates
                            WHERE Templates.ProfessorUserId = @ProfessorId",
                            new { ProfessorId = ProfessorId }
                            ).ToList();

            taskModel.Templates = new SelectList(templates, "Id", "Name");            

            return PartialView("PossibleTemplatesPartial", taskModel);
        }

        public ActionResult EditTask(int id)
        {
            // достаём таску из бд
            Task dbTask = SimpleConnector.Connection
                        .QueryFirstOrDefault<Task>(
                        @"SELECT *
                            FROM Tasks
                            WHERE Tasks.Id = @TaskId",
                            new { TaskId = id }
                            );
            int professorId = SimpleConnector.Connection
                        .QueryFirstOrDefault<int>(
                        @"SELECT Users.Id
                            FROM Tasks, Templates, Users
                            WHERE Tasks.Id = @TaskId
                            AND Tasks.TemplateId = Templates.Id
                            AND Users.Id = Templates.ProfessorUserId",
                            new { TaskId = id }
                            );
            //TaskModel task = TestData.TasksModel.Find(tm => tm.Id == id);

            // заполняем вью модель
            CE_TaskModel taskModel = new CE_TaskModel();

            taskModel.Name = dbTask.Name;
            taskModel.Text = dbTask.Text;

            // создаём список преподов для вью модели; выбран препод, ведущий таску
            /*List<UserModel> professors = new List<UserModel>();
            foreach (User user in TestData.Users.Where(u => u.UserRoleId == (int)Auth.Roles.Professor))
            {
                professors.Add(new UserModel { Id = user.Id, Login = user.Login, Name = user.Name + " " + user.Surname });
            }*/
            List<UserModel> professors = SimpleConnector.Connection
                        .Query<UserModel>(
                        @"SELECT Users.Id, Users.Login, CONCAT(Users.Name, ' ', Users.Surname) as Name, UserRoles.Name as UserRoleName
                            FROM Users, UserRoles
                            WHERE Users.UserRoleId = @UserRoleId
                            AND Users.UserRoleId = UserRoles.Id",
                            new { UserRoleId = Auth.Roles.Professor }
                            ).ToList();

            taskModel.Professors = new SelectList(professors, "Id", "Name",  professorId);

            // создаём список шаблонов для вью модели; шаблоны выбранного препада; выбран шаблон таски
            /*List<Template> templates = new List<Template>();
            foreach (Template template in TestData.Templates.Where(u => u.ProfessorUserId == professorId))
            {
                templates.Add(new Template { Id = template.Id, Name = template.Name });
            }*/
            List<Template> templates = SimpleConnector.Connection
                        .Query<Template>(
                        @"SELECT *
                            FROM Templates
                            WHERE Templates.ProfessorUserId = @ProfessorId",
                            new { ProfessorId = professorId }
                            ).ToList();
            taskModel.Templates = new SelectList(templates, "Id", "Name", dbTask.TemplateId);

            return View(taskModel);
        }

        [HttpPost]
        public ActionResult EditTask(CE_TaskModel task, string action)
        {
            try
            {
                if (action == "Отмена") return RedirectToAction("Index");

                // обновляем таску
                SimpleConnector.Connection
                    .Execute(
                    "UPDATE Tasks SET Name = @Name, TemplateId = @TemplateId WHERE Id = @Id",
                        new {Id = task.Id, Name = task.Name, TemplateId = task.TemplateId }
                    );
                SimpleConnector.Connection
                    .Execute(
                    "UPDATE Templates SET ProfessorUserId = @ProfessorUserId WHERE Id = @Id",
                        new { Id = task.TemplateId, ProfessorUserId = task.ProfessorId}
                    );
                /*TaskModel bdTask = TestData.TasksModel.Find(tm => tm.Id == task.Id);
                bdTask.Name = task.Name;
                bdTask.Professor = task.ProfessorId.ToString();     // тут id выбранного препода
                bdTask.Template = task.TemplateId.ToString();       // тут id выбранного шаблона*/

                return RedirectToAction("Index");

            }
            catch
            {
                return HttpNotFound();
            }
        }

        public ActionResult Task(int id)
        {
            LastTaskModel task = new LastTaskModel();
            task.LinkContentTemplates = new List<LinkContentTemplate>();
            //Task dbTask = TestData.Tasks.Find(t => t.Id == id);
            Task dbTask = SimpleConnector.Connection
                        .QueryFirstOrDefault<Task>(
                        @"SELECT *
                            FROM Tasks
                            WHERE Tasks.Id = @TaskId",
                            new { TaskId = id }
                            );

            task.Id = id;
            task.Name = dbTask.Name;
            task.Text = dbTask.Text;

            // заносим те ContentTemplate, которые имеют TemplateId равный TemplateId у таски
            IEnumerable<ContentTemplate> contentTemplates = SimpleConnector.Connection
                        .Query<ContentTemplate>(
                        @"SELECT *
                            FROM ContentTemplates
                            WHERE ContentTemplates.TemplateId = @TemplateId",
                            new { TemplateId = dbTask.TemplateId }
                            );
            foreach (var contentTemplate in contentTemplates)
            {
                // при этом нам нужно пройтись по этим ContentTemplate, чтобы посмотреть контент каждой из них
                //Content dbContent = TestData.Contents.Find(c => c.ContentTemplateId == contentTemplate.Id && c.TaskId == id);
                Content dbContent = SimpleConnector.Connection
                        .QueryFirstOrDefault<Content>(
                        @"SELECT *
                            FROM Contents, ContentTemplates
                            WHERE Contents.ContentTemplateId = ContentTemplates.Id
                            AND Contents.TaskId = @TaskId",
                            new { TaskId = id }
                            );

                // и чекнуть состояние контента
                bool isChangeContent = false;
                if (dbContent != null)
                {
                    if (Auth.CurrentUserRole().Id == (int)Auth.Roles.Student
                        && dbContent.ContentStatusId == (int)Auth.ContentStatus.ChangedProfessor) // помечаем тру, если сейчас зашёл студент, а изменён преподом
                        isChangeContent = true;
                    if (Auth.CurrentUserRole().Id == (int)Auth.Roles.Professor
                        && dbContent.ContentStatusId == (int)Auth.ContentStatus.ChangedStudent) // помечаем тру, если сейчас зашёл препод, а изменён студентом
                        isChangeContent = true;
                }
                // заносим всё во вью модель
                task.LinkContentTemplates.Add(new LinkContentTemplate { ContentTemplate = contentTemplate, isChange = isChangeContent });
            }

            return View(task);
        }

        public ActionResult LoadContent(int idContentTemplate, int idTask)
        {
            Content viewContent = new Content();
            // ContentTemplate это раздел шаблона
            // ищем контенты, относящиеся к выбранному ContentTemplate текущей таски
            //List<Content> dbContents = TestData.Contents.Where(c => c.ContentTemplateId == idContentTemplate && c.TaskId == idTask).ToList();
            List<Content> dbContents = SimpleConnector.Connection
                        .Query<Content>(
                        @"SELECT Contents.Id, Contents.Text, Contents.Comment, Contents.Date, Contents.TaskId, Contents.ContentTemplateId, Contents.ContentStatusId 
                            FROM Contents, ContentTemplates
                            WHERE Contents.ContentTemplateId = @ContentTemplateId
                            AND Contents.TaskId = @TaskId
                            AND Contents.ContentTemplateId = ContentTemplates.Id",
                            new { TaskId = idTask, ContentTemplateId = idContentTemplate }
                            ).ToList();
            if (dbContents.Count != 0) // если они есть
            {
                viewContent = dbContents.Find(c => c.ContentStatusId != (int)Auth.ContentStatus.Version);
            }
            else // если ни одного контекста, значит первый раз выбираем этот ContentTemplate текущей таски, тогда надо создать его
            {
                //int contentId = TestData.Contents.Count + 1; // это тебе не надо

                Auth.ContentStatus cntentStatus; // эта строка тебе нужна
                if (Auth.CurrentUserRole().Id == (int)Auth.Roles.Student) cntentStatus = Auth.ContentStatus.ChangedStudent; // и этот if тоже
                else cntentStatus = Auth.ContentStatus.ChangedProfessor;

                Content newContent = new Content
                { // нужно заполнять именно эти поля
                    //Id = contentId,
                    TaskId = idTask,
                    ContentTemplateId = idContentTemplate,
                    Date = DateTime.Now,
                    ContentStatusId = (int)cntentStatus
                };
                //TestData.Contents.Add(newContent);
                SimpleConnector.Connection
                .Execute(
                @"INSERT INTO Contents (Text, Comment, Date, TaskId, ContentTemplateId, ContentStatusId) 
                    VALUES (@Text, @Comment, @Date, @TaskId, @ContentTemplateId, @ContentStatusId)",
                    new {Text = newContent.Text, Comment = newContent.Comment , Date = newContent.Date, TaskId = newContent.TaskId, ContentTemplateId = newContent.ContentTemplateId, ContentStatusId = newContent.ContentStatusId }
                );

                viewContent = newContent;
            }
            // достаём из базы требования к ContentTemplate
            //ViewBag.Requirements = TestData.ContentTemplates.Find(ct => ct.Id == idContentTemplate).Requirements;
            ViewBag.Requirements = SimpleConnector.Connection
                        .QueryFirstOrDefault<string>(
                        @"SELECT Requirements
                            FROM ContentTemplates
                            WHERE ContentTemplates.Id = @Id",
                            new { Id = idContentTemplate }
                            );

            if (Auth.CurrentUserRole().Id == (int)Auth.Roles.Student)
                return PartialView("LoadContentStudentPartial", viewContent);
            else return PartialView("LoadContentProfessorPartial", viewContent);
        }

        [HttpPost]
        public ActionResult SaveContent(Content content, string action)
        {
            try
            {
                if (action == "Отмена") return RedirectToAction("Task", new { id = content.TaskId });

                // сохраняем в базу
                //Content bdContent = TestData.Contents.Find(c => c.Id == content.Id);
                Content bdContent = SimpleConnector.Connection
                        .QueryFirstOrDefault<Content>(
                        @"SELECT *
                            FROM Contents
                            WHERE Contents.Id = @Id",
                            new { Id = content.Id }
                            );
                bdContent.Date = DateTime.Now;
               

                int CountChanges = 0;
                Auth.ContentStatus contentStatus = (Auth.ContentStatus)bdContent.ContentStatusId;
                if (Auth.CurrentUserRole().Id == (int)Auth.Roles.Student) // если меняет студент
                {
                    bdContent.Text = content.Text;
                    if (contentStatus == Auth.ContentStatus.ChangedProfessor)
                    {
                        contentStatus = Auth.ContentStatus.ChangedStudent; // помечаем!
                        CountChanges = 1;
                    }
                }
                else // если меняет препод
                {
                    bdContent.Comment = content.Comment;
                    if (contentStatus == Auth.ContentStatus.ChangedStudent) CountChanges = -1;
                    if (action == "Принять")
                    {
                        contentStatus = Auth.ContentStatus.Accepted;
                        var notAccepted = SimpleConnector.Connection
                        .Query<Content>(
                        @"SELECT *
                            FROM Contents, Tasks, ContentStatuss
                            WHERE Contents.TaskId = Tasks.Id
                            AND Contents.ContentStatusId = ContentStatuss.Id
                            AND ContentStatuss.Status != 'Accepted'
                             AND Tasks.Id = @TaskId",
                            new { TaskId = content.TaskId }
                            );
                        if(notAccepted.Count() == 0)
                            SimpleConnector.Connection
                                .Execute(
                                "UPDATE Tasks SET IsAccepted = @IsAccepted WHERE Id = @Id",
                                    new { Id = content.TaskId, IsAccepted = true }
                                );

                        // тут пишем if количество Accepted контентов у таски = количеству разделов таски, то Task.isAccepted = true
                    }
                    else
                    {
                        contentStatus = Auth.ContentStatus.ChangedProfessor; // помечаем!                            
                    }
                }
                bdContent.ContentStatusId = (int)contentStatus;
                // и пишем в базу Tasks.CountChanges += CountChanges
                SimpleConnector.Connection
                        .Execute(
                        @"UPDATE Contents SET Text = @Text, Comment = @Comment, Date = @Date, TaskId = @TaskId, ContentTemplateId = @ContentTemplateId, ContentStatusId = @ContentStatusId
                        WHERE Id = @Id",
                            new { Id = bdContent.Id, Text = bdContent.Text, Comment = bdContent.Comment, Date = bdContent.Date, TaskId = bdContent.TaskId, ContentTemplateId = bdContent.ContentTemplateId, ContentStatusId = bdContent.ContentStatusId }
                        );

                return RedirectToAction("Task", new { id = content.TaskId });
            }
            catch(Exception e)
            {
                return HttpNotFound();
            }
        }
    }
}
