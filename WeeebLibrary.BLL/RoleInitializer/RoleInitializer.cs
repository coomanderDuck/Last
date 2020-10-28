using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database.Entitys;

namespace WeeebLibrary.BLL.RoleInitializer
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = "admin@gmail.com";
            string librarianEmail = "librarian@gmail.com";
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
                var Admin = new User { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(Admin, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(Admin, "Admin");
                }
            }
            if (await userManager.FindByNameAsync(librarianEmail) == null)
            {
                var librarian = new User { Email = librarianEmail, UserName = librarianEmail };
                var result = await userManager.CreateAsync(librarian, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(librarian, "Admin");
                }
            }
        }
    }
}