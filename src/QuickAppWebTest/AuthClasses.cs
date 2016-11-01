using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickAppWebTest
{
    public class AuthUser
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class UserData
    {
        public static AuthUser[] Users { get; } = {
                new AuthUser {Name = "user1", Password = "pwd1"},
                new AuthUser {Name = "user2", Password = "pwd2"},
                new AuthUser {Name = "user3", Password = "pwd3"}
            };
    }

}
