﻿using System;

namespace Common.Lib.Models.DM
{
    public class User
    {
        public Guid? Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
    }
}
