using System;
using System.Collections.Generic;
using System.Linq;

namespace WeeebLibrary.Models
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}