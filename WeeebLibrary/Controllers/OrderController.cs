using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.Database;
using WeeebLibrary.Database.Entitys;
using WeeebLibrary.interfaces;

namespace WeeebLibrary.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookRepository bookRepositiry;
        private readonly IOrderRepositiry orderRepositiry;
        private readonly UserManager<User> userManager;
        private readonly LDBContext lDBContext;
        public OrderController(IOrderRepositiry orderRepositiry, IBookRepository bookRepositiry, UserManager<User> userManager, LDBContext lDBContext)
        {
            this.bookRepositiry = bookRepositiry;
            this.orderRepositiry = orderRepositiry;
            this.userManager = userManager;
            this.lDBContext = lDBContext;

        }
        public IActionResult Checkout(int id)
        {
            var book = bookRepositiry.Books.FirstOrDefault(i => i.Id == id);

            if ((book != null) & (book.Status == Enums.Status.Available))
            {
                var user = userManager.FindByNameAsync(User.Identity.Name).Result;
                orderRepositiry.CreateOrder(user, book);
                return RedirectToAction("Complete");
            }
            return View();
        }
        public IActionResult Complete()
        {
            ViewBag.Message = "Бронь прошла успешно. Ждём вас в нашей библиотеке!";
            return View();
        }
        public IActionResult Empty()
        {
            ViewBag.Message = "У вас пока нет забронированных книг";
            return View();
        }
        public IActionResult Index()
        {
            IEnumerable<Order> Orders = null;
            Orders = orderRepositiry.Orders;
            if (User.IsInRole("Клиент"))
            {
                var user = userManager.FindByNameAsync(User.Identity.Name).Result;
                Orders = Orders.Where(s => s.UserId.Contains(user.Id));
            }
            if (Orders.Count() == 0)
            {
                return RedirectToAction("Empty");
            }
            return View(Orders);
        }
        public async Task<IActionResult> Give(int? id)
        {
            var order = await lDBContext.Order.FindAsync(id);
            var book = await lDBContext.Book.FindAsync(order.BookId);
            orderRepositiry.GiveBook(book);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Take(int? id)
        {
            var order = await lDBContext.Order.FindAsync(id);
            var book = await lDBContext.Book.FindAsync(order.BookId);
            orderRepositiry.TakeBook(book, order);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Cancel(int? id)
        {
            var order = await lDBContext.Order.FindAsync(id);
            orderRepositiry.DeleteOrder(order);
            return RedirectToAction(nameof(Index));
        }
    }
}
