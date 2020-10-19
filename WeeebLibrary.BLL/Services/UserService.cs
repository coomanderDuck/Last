﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.InterfacesBLL;
using WeeebLibrary.BLL.Models;
using WeeebLibrary.DAL.Database.Entitys;

namespace WeeebLibrary.BLL.Services
{
    class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        RoleManager<IdentityRole> roleManager;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
        }
        public List<UserDTO> ToListUsers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            return mapper.Map<List<User>, List<UserDTO>>(userManager.Users.ToList());
        }

        public async Task RegisterUsersAsync(UserDTO userDto)
        {
            var user = NewUser(userDto);
            await userManager.AddToRoleAsync(user, "Клиент");
            await signInManager.SignInAsync(user, false);
        }

        public async Task<IdentityResult> CreateUserAsync(UserDTO userDto, string password)
        {
            var user = NewUser(userDto);
            var result = await userManager.CreateAsync(user, password);
            return result;
        }

        public User NewUser(UserDTO userDto)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();

            return mapper.Map<UserDTO, User>(userDto);
        }

        public async Task<SignInResult> SignInUserAsync(Login model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            return result;
        }

        public async Task SignOutUserAsync()
        {
            await signInManager.SignOutAsync();
        }

        public void AddToRoleAsync(UserDTO userDto)
        {
            var user = NewUser(userDto);
            userManager.AddToRoleAsync(user, "Клиент");
        }

        public async Task<UserDTO> GetUserAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();

            return mapper.Map<User, UserDTO>(user);
        }

        public async Task<IdentityResult> EditUserAsync(EditUserViewModel model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            user.Email = model.Email;
            user.UserName = model.Email;
            user.SecondName = model.SecondName;
            var result = await userManager.UpdateAsync(user);
            return result;
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
            }
        }

        public async Task<IdentityResult> ChangeUserPasswordAsync(ChangePasswordViewModel model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var _passwordValidator =
                       httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
            var _passwordHasher =
                        httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;
            var result =
                await _passwordValidator.ValidateAsync(userManager, user, model.NewPassword);

            if (result.Succeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                await userManager.UpdateAsync(user);
            }
            return result;
        }

        public async Task<ChangeRoleViewModel> EditRolesAsync(UserDTO userDto)
        {
            var user = NewUser(userDto);
            // получем список ролей пользователя
            var userRoles = await userManager.GetRolesAsync(user);
            var allRoles = roleManager.Roles.ToList();
            var model = new ChangeRoleViewModel
            {
                UserId = userDto.Id,
                UserEmail = userDto.Email,
                UserRoles = userRoles,
                AllRoles = allRoles
            };
            return model;
        }

        public async Task<IList<string>> GetUsersRolesAsync(UserDTO userDto)
        {
            var user = NewUser(userDto);
            var userRoles = await userManager.GetRolesAsync(user);
            return userRoles;
        }
        public async Task EditUserRolesAsync(string userId, List<string> roles)
        {
            var user = await userManager.FindByIdAsync(userId);

            // получем список ролей пользователя
            var userRoles = await userManager.GetRolesAsync(user);

            // получаем все роли
            var allRoles = roleManager.Roles.ToList();

            // получаем список ролей, которые были добавлены
            var addedRoles = roles.Except(userRoles);

            // получаем роли, которые были удалены
            var removedRoles = userRoles.Except(roles);

            await userManager.AddToRolesAsync(user, addedRoles);
            await userManager.RemoveFromRolesAsync(user, removedRoles);
        }

        public async Task<UserDTO> GetTheUser()
        {
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
            var userDto = new UserDTO { Email = user.Email, UserName = user.Email, Name = user.Name, SecondName = user.SecondName, Phone = user.Phone };
            return userDto;
        }
    }
}
