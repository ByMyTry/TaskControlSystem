using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pro.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool isAccepted { get; set; }

        public int StudentUserId { get; set; }        
        public int TemplateId { get; set; }
        public int CountChanges { get; set; }
    }
}