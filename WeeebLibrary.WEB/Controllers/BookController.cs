using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.Interfaces;

namespace WeeebLibrary.Controllers
{
    public class BookController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IBookService bookService;
        const string librarianRole = "Библиотекарь";

        public BookController(IOrderService orderService, IBookService bookService)
        {
            this.orderService = orderService;
            this.bookService = bookService;
        }

        public async Task<IActionResult> Index(string searchString, string bookAutor, string bookGenre, string bookPublisher, string sortedString)
        {
            //Фильтрация
            var bookGenreVM = await bookService.FilterBooksAsync(searchString, bookAutor, bookGenre, bookPublisher, sortedString);
            return View(bookGenreVM);
        }

        //Страница книги
        public IActionResult Details(int id)
        {
            var bookDto = bookService.GetBook(id);
            return View(bookDto);
        }

        //Создание книги
        [Authorize(Roles = librarianRole)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Autor,Genre,Publisher,Desc,Img,ImgPath,Status")] BookDTO bookDto, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                await bookService.CreateBookAsync(bookDto, uploadedFile);
                return RedirectToAction(nameof(Index));
            }
            return View(bookDto);
        }

        // Изменение книги
        [Authorize(Roles = librarianRole)]
        public IActionResult Edit(int id)
        {
            var book = bookService.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Autor,Genre,Publisher,Desc,Img,ImgPath,Status")] BookDTO bookDto, IFormFile uploadedFile)
        {
            if (id != bookDto.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //Изменение книги в сервисе
                    await bookService.EditBookAsync(bookDto, uploadedFile);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!bookService.BookExists(bookDto.Id))
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
            return View(bookDto);
        }

        //Удаление книги
        [Authorize(Roles = librarianRole)]
        public IActionResult Delete(int id)
        {
            var book = bookService.GetBook(id);
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
            await bookService.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
