using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Pro.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Имя")]
        public string Name { get; set; }

        public string UserRoleName { get; set; }
    }
}