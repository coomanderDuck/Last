using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.BLL.Interfaces;
using WeeebLibrary.BLL.InterfacesBLL;
using WeeebLibrary.BLL.Models;
using WeeebLibrary.DAL.Enums;

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

        [Authorize(Roles = "Библиотекарь")]
        public async Task<IActionResult> Reports(OrderStatus orderStatus, DateTime minDate, DateTime maxDate, bool save =false)
        {
            if (minDate > maxDate) 
            {
                ViewBag.DateError = "Некорректный отрезок времени";
                return View();
            }

            var orderVM = await orderService.FilrterOdersAsync(orderStatus, minDate, maxDate);

            if (save == true)
            {
                orderService.SaveReport(orderVM.Orders);
            }
            return View(orderVM);
        }

        public IActionResult Save()
        {
            ViewBag.Message = "Отчёт скачен";
            return View();
        }

        [Authorize(Roles = "Библиотекарь")]
        public IActionResult Give(int id)
        {
            orderService.GiveBook(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Библиотекарь")]
        public IActionResult Take(int id)
        {
            orderService.TakeBook(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Клиент")]
        public IActionResult Cancel(int id)
        {
            orderService.DeleteOrder(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
