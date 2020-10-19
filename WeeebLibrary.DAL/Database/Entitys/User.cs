using Microsoft.AspNetCore.Identity;

namespace WeeebLibrary.DAL.Database.Entitys
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public string SecondName { get; set; }

        public string Phone { get; set; }
    }
}
