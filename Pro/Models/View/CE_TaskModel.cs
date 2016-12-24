using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Pro.Models
{
    public class CE_TaskModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Задание")]
        public string Text { get; set; }

        [Required]
        public int? ProfessorId { get; set; }

        [Display(Name = "Преподаватель")]
        public SelectList Professors { get; set; }

        [Required]
        public int? TemplateId { get; set; }

        [Display(Name = "Тип работы")]
        public SelectList Templates { get; set; }
        
    }
}