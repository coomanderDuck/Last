using AutoMapper;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.DAL.Database.Entitys;

namespace WeeebLibrary.BLL.MappingProfiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDTO>();
            CreateMap<BookDTO, Book>();
        }
    }
}
