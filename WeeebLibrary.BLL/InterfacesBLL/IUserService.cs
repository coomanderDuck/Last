using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.Models;

namespace WeeebLibrary.BLL.InterfacesBLL
{
    public interface IUserService
    {
        Task AddToRoleAsync(string id);

        List<UserDTO> ToListUsers();

        Task EditUserRolesAsync(string userId, List<string> roles);

        Task RegisterUsersAsync(UserDTO user);

        Task DeleteUserAsync(string id);

        Task SignOutUserAsync();

        Task<ChangeRoleViewModel> EditRolesAsync(UserDTO userDto);

        Task<UserDTO> GetUserAsync(string id);

        Task<IdentityResult> CreateUserAsync(UserDTO user, string password);

        Task<IdentityResult> EditUserAsync(EditUserViewModel model, string userId);

        Task<SignInResult> SignInUserAsync(Login model);

        Task<IdentityResult> ChangeUserPasswordAsync(ChangePasswordViewModel model);
    }
}
