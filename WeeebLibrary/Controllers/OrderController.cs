using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.Database;
using WeeebLibrary.Database.Entitys;
using WeeebLibrary.interfaces;
using WeeebLibrary.Models;

namespace WeeebLibrary.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepositiry orderRepositiry;
        private readonly Cart cart;
        private readonly UserManager<User> userManager;
        private readonly LDBContext lDBContext;

        public OrderController(IOrderRepositiry orderRepositiry, Cart cart, UserManager<User> userManager, LDBContext context)
        {
            this.orderRepositiry = orderRepositiry;
            this.cart = cart;
            this.userManager = userManager;
            this.lDBContext = context;

        }


        public IActionResult Checkout()
        {
            cart.ListItems = cart.GetItems();
            if (cart.ListItems.Count == 0)
            {
                ModelState.AddModelError("", "Вы не выбрали услугу!");
            }

            if (ModelState.IsValid)
            {
                var user = userManager.FindByNameAsync(User.Identity.Name).Result;
                orderRepositiry.CreateOrder(user);
                return RedirectToAction("Complete");
            }
            return View();


        }
        public IActionResult Complete()
        {
            ViewBag.Message = "Бронь прошла успешно. Ждём вас в нашей библиотеке!";
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
            return View(Orders);
        }
        public async Task<IActionResult> Give(int? id)
        {
            var order = await lDBContext.Order.FindAsync(id);
            var book = await lDBContext.Book.FindAsync(order.BookId);
            orderRepositiry.GiveBook(book);
            await lDBContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Take(int? id)
        {
            var order = await lDBContext.Order.FindAsync(id);
            var book = await lDBContext.Book.FindAsync(order.BookId);
            orderRepositiry.TakeBook(book);
            lDBContext.Order.Remove(order);
            await lDBContext.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}
