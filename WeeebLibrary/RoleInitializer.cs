using WeeebLibrary.Database.Entitys;  
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace WeeebLibrary
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "egorco53gmail.com";
            string password = "Pass123!";
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
           
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User Admin = new User { Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(Admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(Admin, "Admin");
                }
            }
        }
    }
}