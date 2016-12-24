using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pro.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }

        public int UserRoleId { get; set; }
    }
}