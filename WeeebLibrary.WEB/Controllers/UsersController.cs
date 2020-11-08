using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.InterfacesBLL;
using WeeebLibrary.BLL.Models;

namespace WeeebLibrary.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUserService userServices;

        public UsersController(IUserService userServices, IMapper mapper)
        {
            this.userServices = userServices;
            this.mapper = mapper;
        }

        public IActionResult Index() => View(userServices.ToListUsers());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userDto = mapper.Map<CreateUserViewModel, UserDTO>(model);
                var result = await userServices.CreateUserAsync(userDto, model.Password);

                if (result.Succeeded)
                {
                    await userServices.AddToRoleAsync(userDto);
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
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await userServices.GetUserAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel { Id = user.Id, Email = user.Email, SecondName = user.SecondName };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userDto = await userServices.GetUserAsync(model.Id);
                if (userDto != null)
                {
                    var result = await userServices.EditUserAsync(model, userDto.Id);
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
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            await userServices.DeleteUserAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangePassword(string id)
        {
            var userDTO = await userServices.GetUserAsync(id);

            if (userDTO == null)
            {
                return NotFound();
            }
            var model = new ChangePasswordViewModel { Id = userDTO.Id, Email = userDTO.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userDto = await userServices.GetUserAsync(model.Id);
                if (userDto != null)
                {
                    var result = await userServices.ChangeUserPasswordAsync(model);
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
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }
    }
}