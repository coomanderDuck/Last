using AutoMapper;
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
        private readonly RoleManager<IdentityRole> roleManager; 
        private readonly IMapper mapper;
        const string clientRole = "Клиент";

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }
        public List<UserDTO> ToListUsers()
        {
            return mapper.Map<List<User>, List<UserDTO>>(userManager.Users.ToList());
        }

        public async Task RegisterUsersAsync(UserDTO userDto)
        {
            var user = await userManager.FindByIdAsync(userDto.Id);
            await userManager.AddToRoleAsync(user, clientRole);
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

        public async Task AddToRoleAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            await userManager.AddToRoleAsync(user, clientRole);
        }

        public async Task<UserDTO> GetUserAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            return mapper.Map<User, UserDTO>(user);
        }

        public async Task<IdentityResult> EditUserAsync(EditUserViewModel model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            mapper.Map(model, user);

            //user = mapper.Map<EditUserViewModel, User>(model); //меняется айди

            var result = await userManager.UpdateAsync(user);

            return result;
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                await userManager.DeleteAsync(user);
            }
        }

        public async Task<IdentityResult> ChangeUserPasswordAsync(ChangePasswordViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            return result;
        }

        public async Task<ChangeRoleViewModel> EditRolesAsync(UserDTO userDto)
        {
            var user = NewUser(userDto);
            // получем список ролей пользователя
            var userRoles = await userManager.GetRolesAsync(user);
            var allRoles = roleManager.Roles.ToList();
            var model = mapper.Map<UserDTO, ChangeRoleViewModel>(userDto);
            model.UserRoles = userRoles;
            model.AllRoles = allRoles;

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
            return mapper.Map<User, UserDTO>(user);
        }
    }
}

