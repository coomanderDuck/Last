using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.Interfaces;
using WeeebLibrary.BLL.Models;
using WeeebLibrary.DAL.Database;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.InterfacesDLL;

namespace WeeebLibrary.BLL.Services
{
    class BookService : IBookService
    {
        private readonly IMapper mapper;
        private readonly LDBContext lDBContext;
        private readonly IRepository<Book> bookRepositiry;

        public BookService(LDBContext lDBContext, IRepository<Book> bookRepositiry, IMapper mapper)
        {
            this.lDBContext = lDBContext;
            this.bookRepositiry = bookRepositiry;
            this.mapper = mapper;
        }

        //Фильтрация книг
        public async Task<BookGenreViewModel> FilterBooksAsync(string searchString, string bookAutor, string bookGenre, string bookPublisher)
        {
            IQueryable<string> autorQuery = bookRepositiry.GetAll()
                                            .OrderBy(b => b.Autor)
                                            .Select(b => b.Autor);
            IQueryable<string> genreQuery = bookRepositiry.GetAll()
                                            .OrderBy(b => b.Genre)
                                            .Select(b => b.Genre);
            IQueryable<string> publisherQuery = bookRepositiry.GetAll()
                                            .OrderBy(b => b.Publisher)
                                            .Select(b => b.Publisher);

            var books = bookRepositiry.GetAll();

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(bookAutor))
            {
                books = books.Where(x => x.Autor == bookAutor);
            }

            if (!string.IsNullOrEmpty(bookGenre))
            {
                books = books.Where(x => x.Genre == bookGenre);
            }

            if (!string.IsNullOrEmpty(bookPublisher))
            {
                books = books.Where(x => x.Publisher == bookPublisher);
            }

            var bookGenreVM = new BookGenreViewModel
            {
                Autor = new SelectList(await autorQuery.Distinct().ToListAsync()),
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Publisher = new SelectList(await publisherQuery.Distinct().ToListAsync()),
                Books = await books.ToListAsync()
            };
            return bookGenreVM;
        }

        public BookDTO GetBook(int id)
        {
            var book = bookRepositiry.Get(id);

            return mapper.Map<Book, BookDTO>(book);
        }

        public IEnumerable<BookDTO> GetBooks()
        {
            // применяем автомаппер для проекции одной коллекции на другую

            return mapper.Map<IEnumerable<Book>, List<BookDTO>>(bookRepositiry.GetAll());
        }

        public async Task CreateBookAsync(BookDTO bookDto, IFormFile uploadedFile)
        {
            // путь к папке Files
            string path = "/Files/" + uploadedFile.FileName;
            // сохраняем файл в папку Files в каталоге wwwroot
            using (var fileStream = new FileStream("wwwroot" + path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }
            var book = new Book
            {
                Id = bookDto.Id,
                Name = bookDto.Name,
                Autor = bookDto.Autor,
                Genre = bookDto.Genre,
                Publisher = bookDto.Publisher,
                Desc = bookDto.Desc,
                Img = uploadedFile.FileName,
                ImgPath = path,
                Status = bookDto.Status
            };

            bookRepositiry.Create(book);
        }

        public async Task EditBookAsync(BookDTO bookDto, IFormFile uploadedFile)
        {

            var book = bookRepositiry.Get(bookDto.Id);

            book.Name = bookDto.Name;
            book.Autor = bookDto.Autor;
            book.Genre = bookDto.Genre;
            book.Publisher = bookDto.Publisher;
            book.Desc = bookDto.Desc;
            book.Status = bookDto.Status;
            if (uploadedFile != null)
            {
                // путь к папке Files
                string path = "/Files/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream("wwwroot" + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                book.Img = uploadedFile.FileName;
                book.ImgPath = path;
            }
            bookRepositiry.Update(book);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await lDBContext.Book.FindAsync(id);
            bookRepositiry.Delete(book);
        }

        public bool BookExists(int id)
        {
            return lDBContext.Book.Any(e => e.Id == id);
        }
    }
}
