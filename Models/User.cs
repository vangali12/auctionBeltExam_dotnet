using System;
using System.Collections.Generic;

namespace beltexam.Models
{
    public class User: BaseEntity
    {
        public int userid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Wallet { get; set; }

        public User()
        {}
    }

}