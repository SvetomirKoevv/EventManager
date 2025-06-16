using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class UserResultSet : IdentityResultSet<User>
    {
        public UserResultSet(IdentityResult identityResult, User entity) : base(identityResult, entity) { }
    }
}
