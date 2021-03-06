﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.Models;

namespace WeeebLibrary.BLL.Interfaces
{
    public interface IBookService
    {
        Task<BookGenreViewModel> FilterBooksAsync(string searchString, string bookAutor, string bookGenre, string bookPublisher, string sortedString);

        BookDTO GetBook(int id);

        Task CreateBookAsync(BookDTO bookDto, IFormFile uploadedFile);

        Task EditBookAsync(BookDTO bookDto, IFormFile uploadedFile);

        Task DeleteBookAsync(int id);

        bool BookExists(int id);
    }
}
