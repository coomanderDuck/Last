using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database.Entitys;

namespace WeeebLibrary.BLL.RoleInitializer
{
    public class CallRoleInitializer
    {
        public static async Task RoleInitializeAsync(IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            var scope = scopeFactory.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var rolesManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await RoleInitializer.InitializeAsync(userManager, rolesManager);
        }

    }
}
