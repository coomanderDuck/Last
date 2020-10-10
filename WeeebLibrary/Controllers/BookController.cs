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
using WeeebLibrary.Models;
namespace WeeebLibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly LDBContext lDBContext;
        IWebHostEnvironment appEnvironment;

        public BookController(LDBContext lDBContext, IWebHostEnvironment appEnvironment)
        {
            this.lDBContext = lDBContext;
            this.appEnvironment = appEnvironment;
        }

        // GET: Book


        public async Task<IActionResult> Index(string searchString, string bookAutor, string bookGenre, string bookPublisher)
        {
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

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await lDBContext.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        [Authorize(Roles = "Библиотекарь")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Book/Edit/5
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

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    lDBContext.Update(book);
                    await lDBContext.SaveChangesAsync();
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

        // GET: Book/Delete/5
        [Authorize(Roles = "Библиотекарь")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await lDBContext.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await lDBContext.Book.FindAsync(id);
            lDBContext.Book.Remove(book);
            await lDBContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return lDBContext.Book.Any(e => e.Id == id);
        }



    }
}
