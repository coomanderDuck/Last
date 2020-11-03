using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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
        private readonly IWebHostEnvironment appEnvironment;

        public OrderController(IBookService bookService, IOrderService orderService, IUserService userServices, IWebHostEnvironment appEnvironment)
        {
            this.bookService = bookService;
            this.orderService = orderService;
            this.userServices = userServices;
            this.appEnvironment = appEnvironment;
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
                var file_path = orderService.SaveReport(orderVM.Orders);
                file_path = Path.Combine(appEnvironment.ContentRootPath, file_path);
                // Тип файла - content-type
                string file_type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                // Имя файла - необязательно
                string file_name = "Отчёт.xlsx";
                return PhysicalFile(file_path, file_type, file_name);
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
