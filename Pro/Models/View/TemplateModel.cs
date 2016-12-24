using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Pro.Models
{
    public class TemplateModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ProfessorUserId { get; set; }

        public ICollection<ContentTemplateModel> ContentTemplates { get; set; }

    }
}