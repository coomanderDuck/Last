using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.Models;

namespace WeeebLibrary.BLL.Interfaces
{
    public interface IBookService
    {
        Task<BookGenreViewModel> FilterBooksAsync(string searchString, string bookAutor, string bookGenre, string bookPublisher);

        BookDTO GetBook(int id);

        IEnumerable<BookDTO> GetBooks();

        Task CreateBookAsync(BookDTO bookDto, IFormFile uploadedFile);

        Task EditBookAsync(BookDTO bookDto, IFormFile uploadedFile);

        Task DeleteBookAsync(int id);

        bool BookExists(int id);
    }
}
