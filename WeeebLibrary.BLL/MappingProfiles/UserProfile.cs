using AutoMapper;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.DAL.Database.Entitys;

namespace WeeebLibrary.BLL.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
        }
    }
}
