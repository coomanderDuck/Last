using Microsoft.AspNetCore.Identity;

namespace WeeebLibrary.BLL.DTO
{
    public class UserDTO : IdentityUser
    {
        public string Name { get; set; }

        public string SecondName { get; set; }

        public string Phone { get; set; }
    }
}
