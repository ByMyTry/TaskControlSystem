using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Pro.Models
{
    public class ContentTemplateModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Требования")]
        public string Requirements { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int TemplateId { get; set; }
    }
}