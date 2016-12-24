using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pro.Models
{
    public class ContentTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Requirements { get; set; }

        public int TemplateId { get; set; }
    }
}