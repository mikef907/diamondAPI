using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Lib.Models.DM
{
    public class AuthenticationModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
