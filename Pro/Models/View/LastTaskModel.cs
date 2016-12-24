using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Pro.Models
{
    public class LastTaskModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Задание")]
        public string Text { get; set; }        

        [Display(Name = "Разделы")]
        public ICollection<LinkContentTemplate> LinkContentTemplates { get; set; }
    }

    public class LinkContentTemplate
    {
        public ContentTemplate ContentTemplate;
        public bool isChange;
    }
}