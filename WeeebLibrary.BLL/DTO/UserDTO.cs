using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeeebLibrary.BLL.DTO
{
    public class UserDTO : IdentityUser
    {
        public string Name { get; set; }

        public string SecondName { get; set; }

        public string Phone { get; set; }
    }
}
