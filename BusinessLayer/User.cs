using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class User : IdentityUser
    {
        public List<Event> Events { get; set; }

        public User()
        {
            this.Events = new List<Event>();
        }

        public User(string username_, string email_)
        {
            this.UserName = username_;
            this.Email = email_;

            this.Events = new List<Event>();
        }
    }
}
