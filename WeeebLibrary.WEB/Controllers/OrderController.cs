using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.BLL.Interfaces;
using WeeebLibrary.BLL.InterfacesBLL;

namespace WeeebLibrary.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBookService bookService;
        private readonly IOrderService orderService;
        private readonly IUserService userServices;

        public OrderController(IBookService bookService, IOrderService orderService, IUserService userServices)
        {
            this.bookService = bookService;
            this.orderService = orderService;
            this.userServices = userServices;
        }

        // Создание брони
        public async Task<IActionResult> CheckoutAsync(int id)
        {
            await orderService.MakeOrderAsync(id);
            return RedirectToAction("Complete");
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

        public async Task<IActionResult> Index()
        {
            var OrdersDto = orderService.GetOrders();
            if (User.IsInRole("Клиент"))
            {
                OrdersDto = await orderService.FindCustomerrOders(OrdersDto);
            }
            if (OrdersDto.Count() == 0)
            {
                return RedirectToAction("Empty");
            }
            return View(OrdersDto);
        }

        public IActionResult Give(int id)
        {
            orderService.GiveBook(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Take(int id)
        {
            orderService.TakeBook(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Cancel(int id)
        {
            orderService.DeleteOrder(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
