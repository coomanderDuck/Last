using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.InterfacesBLL;
using WeeebLibrary.BLL.Models;

namespace WeeebLibrary.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService userServices;

        public AccountController(IUserService userServices)
        {
            this.userServices = userServices;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                var userDto = new UserDTO { Email = model.Email, UserName = model.Email, Name = model.Name, SecondName = model.SecondName, Phone = model.Phone };
                var result = await userServices.CreateUserAsync(userDto, model.Password);

                if (result.Succeeded)
                {
                    await userServices.RegisterUsersAsync(userDto);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new Login { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await userServices.SignInUserAsync(model);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await userServices.SignOutUserAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}