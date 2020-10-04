using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeeebLibrary.Database.Entitys;
using WeeebLibrary.interfaces;
using WeeebLibrary.Models;

namespace WeeebLibrary.Controllers
{
    public class CartController : Controller
    {
        private readonly ILibraryRepository _libRep;
        private readonly Cart _cart;

        public CartController(ILibraryRepository libRep, Cart cart)
        {
            _libRep = libRep;
            _cart = cart;
        }

        public ViewResult Index()
        {
            var items = _cart.GetItems();
            _cart.ListItems = items;

            var obj = new CartViewModel
            {
                cart = _cart
            };

            return View(obj);
        }

        public RedirectToActionResult addToCart(int id)
        {
            var item = _libRep.Books.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                _cart.AddToCart(item);
            }
            return RedirectToAction("Index");
        }
    }
}