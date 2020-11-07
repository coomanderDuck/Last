using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database.Entitys;

namespace WeeebLibrary.BLL.RoleInitializer
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string adminRole = "Admin";
            const string librarianRole = "Библиотекарь";
            const string clientRole = "Клиент";
            const string adminEmail = "admin@gmail.com";
            const string librarianEmail = "librarian@gmail.com";
            const string password = "Pass123!";

            if (await roleManager.FindByNameAsync(adminRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            if (await roleManager.FindByNameAsync(librarianRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(librarianRole));
            }

            if (await roleManager.FindByNameAsync(clientRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(clientRole));
            }

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new User { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(admin, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, adminRole);
                }
            }
            if (await userManager.FindByNameAsync(librarianEmail) == null)
            {
                var librarian = new User { Email = librarianEmail, UserName = librarianEmail };
                var result = await userManager.CreateAsync(librarian, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(librarian, librarianRole);
                }
            }
        }
    }
}