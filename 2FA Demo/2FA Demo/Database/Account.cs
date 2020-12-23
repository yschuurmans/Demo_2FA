using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2FA_Demo.Database
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Secret { get; set; }
        public string Salt { get; internal set; }
        public long Timeslice { get; internal set; }
    }
}
