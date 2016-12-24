using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pro.Models
{
    public class Template
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int ProfessorUserId { get; set; }
    }
}