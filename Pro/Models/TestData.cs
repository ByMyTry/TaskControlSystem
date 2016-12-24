using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pro.Models
{
    public class TestData
    {
        static public List<Content> Contents = new List<Content>
        {
            new Content { Id = 1, TaskId = 2, ContentTemplateId =  3, Date = DateTime.Now, ContentStatusId = 1, Text = "Моё введение", Comment = "Введение ок"},
            new Content { Id = 2, TaskId = 2, ContentTemplateId =  4, Date = DateTime.Now, ContentStatusId = 3, Text = "Моё описание", Comment = "Описание ок"},
        };

        static public List<ContentStatus> ContentStatusS = new List<ContentStatus>
        {
            new ContentStatus { Id = 1, Status = "Accepted" },
            new ContentStatus { Id = 2, Status = "ChangedStudent" },
            new ContentStatus { Id = 3, Status = "ChangedProfessor" },
            new ContentStatus { Id = 4, Status = "Version" },
        };

        static public List<Task> Tasks = new List<Task>
        {
            new Task { Id = 1, Name = "11", StudentUserId = 1, TemplateId = 1, isAccepted = true, Text = "222" },
            new Task { Id = 2, Name = "Моя супер работа", StudentUserId = 1, TemplateId = 2, Text = "Сделать супер работу" },
            new Task { Id = 3, Name = "11", StudentUserId = 1, TemplateId = 1, Text = "222" },
            new Task { Id = 4, Name = "11", StudentUserId = 1, TemplateId = 1, Text = "222" },
        };

        static public List<Template> Templates = new List<Template>
        {
            new Template { Id = 1, Name = "11", ProfessorUserId = 4 },
            new Template { Id = 2, Name = "22", ProfessorUserId = 4 },
            new Template { Id = 3, Name = "33", ProfessorUserId = 5 },
            new Template { Id = 4, Name = "44", ProfessorUserId = 5 },
        };

        static public List<ContentTemplate> ContentTemplates = new List<ContentTemplate>
        {
            new ContentTemplate { Id = 1, Name = "11", Requirements = "111", TemplateId = 1 },
            new ContentTemplate { Id = 2, Name = "22", Requirements = "222", TemplateId = 1 },
            new ContentTemplate { Id = 3, Name = "Введение", Requirements = "Что-то вводное", TemplateId = 2 },
            new ContentTemplate { Id = 4, Name = "Описание", Requirements = "Что-то описательное", TemplateId = 2 },
            new ContentTemplate { Id = 5, Name = "Заключение", Requirements = "Что-то заключительное", TemplateId = 2 },
        };

        static public List<User> Users = new List<User>
        {
            new User { Id = 1, Login = "Некто", Name = "Некто", Surname = "Нектович", Password = "q123321", UserRoleId = 1 },
            new User { Id = 2, Login = "Иван", Name = "Иван", Surname = "Крученков", Password = "q123321", UserRoleId = 2 },
            new User { Id = 3, Login = "Антон", Name = "Антон", Surname = "Талецкий", Password = "q123321", UserRoleId = 2 },
            new User { Id = 4, Login = "Вячеслав", Name = "Вячеслав", Surname = "Проволоцкий", Password = "q123321", UserRoleId = 3 },
            new User { Id = 5, Login = "Наталья", Name = "Наталья", Surname = "Волорова", Password = "q123321", UserRoleId = 3 },
        };

        static public List<UserRole> UserRoles = new List<UserRole>
        {
            new UserRole { Id = 1, Name = "Админ" },
            new UserRole { Id = 3, Name = "Преподаватель" },
            new UserRole { Id = 2, Name = "Студент" },
        };

        // ----- вью-модели -----

        static public List<TaskModel> TasksModel = new List<TaskModel>
        {
            new TaskModel { Id = 1, Name = "11", Professor = "4", Student = "dd", Template = "1", isAccepted = true, CountChanges = 0 },
            new TaskModel { Id = 1, Name = "11", Professor = "4", Student = "dd", Template = "1", CountChanges = 0 },
            new TaskModel { Id = 2, Name = "22", Professor = "5", Student = "dd", Template = "3", isAccepted = true, CountChanges = 0 },
            new TaskModel { Id = 2, Name = "22", Professor = "5", Student = "dd", Template = "4", CountChanges = 0 },
            new TaskModel { Id = 4, Name = "33", Professor = "4", Student = "dd", Template = "2", isAccepted = true, CountChanges = 0 },
            new TaskModel { Id = 4, Name = "33", Professor = "4", Student = "dd", Template = "2", CountChanges = 0 },
        };

        static public List<TemplateModel> TemplatesModel = new List<TemplateModel>
        {
            new TemplateModel { Id = 1, Name = "11", ProfessorUserId = 4 },
            new TemplateModel { Id = 2, Name = "22", ProfessorUserId = 4 },
            new TemplateModel { Id = 3, Name = "33", ProfessorUserId = 5 },
            new TemplateModel { Id = 4, Name = "44", ProfessorUserId = 5 },
        };

        static public List<ContentTemplateModel> ContentTemplatesModel = new List<ContentTemplateModel>
        {
            new ContentTemplateModel { Id = 1, Name = "11", Requirements = "111", TemplateId = 1 },
            new ContentTemplateModel { Id = 2, Name = "22", Requirements = "222", TemplateId = 1 },
            new ContentTemplateModel { Id = 3, Name = "33", Requirements = "333", TemplateId = 2 },
            new ContentTemplateModel { Id = 4, Name = "44", Requirements = "444", TemplateId = 2 },
        };
    }
}