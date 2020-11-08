using AutoMapper;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.Models;
using WeeebLibrary.DAL.Database.Entitys;

namespace WeeebLibrary.BLL.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<UserDTO, ChangeRoleViewModel>()
                .ForMember("UserId", opt => opt.MapFrom(c => c.Id))
                .ForMember("UserEmail", opt => opt.MapFrom(c => c.Email));
            CreateMap<EditUserViewModel, User>()
                .ForMember("UserName", opt => opt.MapFrom(c => c.Email))
                .ForMember("Id", config => config.Ignore());
            CreateMap<CreateUserViewModel, UserDTO>();
        }
    }
}
