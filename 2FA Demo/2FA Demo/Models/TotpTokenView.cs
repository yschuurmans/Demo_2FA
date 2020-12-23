using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2FA_Demo.Models
{
    public class TotpTokenView
    {
        public string TokenUri { get; set; }
        public string Token { get; set; }
        public string Error { get; set; }
    }
}
