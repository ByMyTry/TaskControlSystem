using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Pro.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Название")]        
        public string Name { get; set; }

        [Display(Name = "Студент")]
        public string Student { get; set; }

        [Display(Name = "Преподаватель")]
        public string Professor { get; set; }

        [Display(Name = "Тип работы")]
        public string Template { get; set; }

        [Display(Name = "Принято")]
        public bool isAccepted { get; set; }

        [Display(Name = "Количество изменений")]
        public int CountChanges { get; set; }

    }
}