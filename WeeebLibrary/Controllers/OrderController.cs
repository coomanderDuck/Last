using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public OrderController(IOrderRepositiry orderRepositiry, Cart cart,UserManager<User> userManager)
        {
            this.orderRepositiry = orderRepositiry;
            this.cart = cart;
            this.userManager = userManager;
        }
      
        
        public  IActionResult Checkout()
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
    }
}
