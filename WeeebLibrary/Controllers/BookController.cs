using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.Database;
using WeeebLibrary.Database.Entitys;
using WeeebLibrary.interfaces;
using WeeebLibrary.Models;
namespace WeeebLibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository bookRepositiry;
        private readonly LDBContext lDBContext;
        IWebHostEnvironment appEnvironment;

        public BookController(IBookRepository bookRepositiry, LDBContext lDBContext, IWebHostEnvironment appEnvironment)
        {
            this.lDBContext = lDBContext;
            this.appEnvironment = appEnvironment;
            this.bookRepositiry = bookRepositiry;
        }

        public async Task<IActionResult> Index(string searchString, string bookAutor, string bookGenre, string bookPublisher)
        {
            //Фильтрация
            IQueryable<string> autorQuery = from m in lDBContext.Book
                                            orderby m.Autor
                                            select m.Autor;
            IQueryable<string> genreQuery = from m in lDBContext.Book
                                            orderby m.Genre
                                            select m.Genre;
            IQueryable<string> publisherQuery = from m in lDBContext.Book
                                                orderby m.Publisher
                                                select m.Publisher;

            var books = from m in lDBContext.Book
                        select m;

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

            return View(bookGenreVM);
        }
        //Страница книги
        public IActionResult Details(int id)
        {
            var book = bookRepositiry.GetBook(id);
            return View(book);
        }
        //Создание книги
        [Authorize(Roles = "Библиотекарь")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Autor,Genre,Publisher,Desc,Img,ImgPath,Status")] Book book, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
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
                lDBContext.Add(book);
                await lDBContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // Изменение книги
        [Authorize(Roles = "Библиотекарь")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await lDBContext.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Autor,Genre,Publisher,Desc,Img,ImgPath,Status")] Book book, IFormFile uploadedFile)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
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
                    await bookRepositiry.UpdateBookAsync(book);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        //Удаление книги
        [Authorize(Roles = "Библиотекарь")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = bookRepositiry.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await lDBContext.Book.FindAsync(id);
            await bookRepositiry.DeleteBookAsync(book);
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return lDBContext.Book.Any(e => e.Id == id);
        }
    }
}
