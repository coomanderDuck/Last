using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database.Entitys;

namespace WeeebLibrary.BLL.RoleInitializer
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "egorcos53@gmail.com";
            string password = "Pass123!";
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (await roleManager.FindByNameAsync("Библиотекарь") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Библиотекарь"));
            }
            if (await roleManager.FindByNameAsync("Клиент") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Клиент"));
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