using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pro.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int UserId { get; set; }        
        public int LogActionId { get; set; }
    }
}