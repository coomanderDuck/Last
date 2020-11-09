using Microsoft.AspNetCore.Mvc;
using Parser;
using System.Threading.Tasks;

namespace WeeebLibrary.WEB.Controllers
{
    public class ParserController : Controller
    {
        private readonly IParserBooks parserBooks;

        public ParserController(IParserBooks parserBooks)
        {
            this.parserBooks = parserBooks;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Parser(int parsCount)
        {
            await parserBooks.Pars(parsCount);
            return RedirectToAction("ParserFinish");
        }

        public IActionResult ParserFinish()
        {
            ViewBag.Message = "Парсинг книг закончен";
            return View();
        }
    }
}