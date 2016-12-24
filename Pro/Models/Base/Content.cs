using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pro.Models
{
    public class Content
    {
        public int Id { get; set; }        
        public string Text { get; set; }
        public string Comment { get; set; }  
        public DateTime Date { get; set; }
        
        public int TaskId { get; set; }
        public int ContentTemplateId { get; set; }
        public int ContentStatusId { get; set; }
    }
}