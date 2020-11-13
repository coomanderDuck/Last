using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.Interfaces;
using WeeebLibrary.BLL.Models;
using WeeebLibrary.DAL;
using WeeebLibrary.DAL.Database;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.InterfacesDLL;

namespace WeeebLibrary.BLL.Services
{
    class BookService : IBookService
    {
        private readonly IMapper mapper;
        private readonly LDBContext lDBContext;
        private readonly IRepository<Book> bookRepository;

        public BookService(LDBContext lDBContext, IRepository<Book> bookRepository, IMapper mapper)
        {
            this.lDBContext = lDBContext;
            this.bookRepository = bookRepository;
            this.mapper = mapper;
        }

        //Фильтрация книг
        public async Task<BookGenreViewModel> FilterBooksAsync(string searchString, string bookAutor, string bookGenre, string bookPublisher)
        {
            IQueryable<string> autorQuery = bookRepository.GetAll()
                                            .OrderByExp("Autor")
                                            .Select(b => b.Autor);
            IQueryable<string> genreQuery = bookRepository.GetAll()
                                            .OrderByExp("Genre")
                                            .Select(b => b.Genre);
            IQueryable<string> publisherQuery = bookRepository.GetAll()
                                            .OrderByExp("Publisher")
                                            .Select(b => b.Publisher);

            var books = bookRepository.GetAll();

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.WhereExp("Name", "Contains", searchString);
            }

            if (!string.IsNullOrEmpty(bookAutor))
            {
                books = books.WhereExp("Autor", "Contains", bookAutor);
            }

            if (!string.IsNullOrEmpty(bookGenre))
            {
                books = books.WhereExp("Genre", "Contains", bookGenre);
            }

            if (!string.IsNullOrEmpty(bookPublisher))
            {
                books = books.WhereExp("Publisher", "Contains", bookPublisher);
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
            var book = bookRepository.Get(id);
            return mapper.Map<Book, BookDTO>(book);
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

            await bookRepository.CreateAsync(book);
        }

        public async Task EditBookAsync(BookDTO bookDto, IFormFile uploadedFile)
        {
            var book = bookRepository.Get(bookDto.Id);

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
            bookRepository.Update(book);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await lDBContext.Book.FindAsync(id);
            bookRepository.Delete(book);
        }

        public bool BookExists(int id)
        {
            return lDBContext.Book.Any(e => e.Id == id);
        }
    }
}
