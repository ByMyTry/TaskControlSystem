using System;

namespace Pro.Models
{
    public class LogModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public User User { get; set; }
        public LogAction LogAction { get; set; }
    }
}