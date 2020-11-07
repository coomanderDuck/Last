using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.BLL.InterfacesBLL;

namespace WeeebLibrary.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUserService userServices;

        public RolesController(RoleManager<IdentityRole> roleManager, IUserService userServices)
        {
            this.roleManager = roleManager;
            this.userServices = userServices;
        }

        public IActionResult Index() => View(roleManager.Roles.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }

        public IActionResult UserList() => View(userServices.ToListUsers());

        public async Task<IActionResult> Edit(string userId)
        {
            // получаем пользователя
            var userDto = await userServices.GetUserAsync(userId);
            if (userDto != null)
            {
                // получем список ролей пользователя
                var model = await userServices.EditRolesAsync(userDto);
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            // получаем пользователя
            var userDto = await userServices.GetUserAsync(userId);
            if (userDto != null)
            {
                await userServices.EditUserRolesAsync(userId, roles);
                return RedirectToAction("UserList");
            }
            return NotFound();
        }
    }
}