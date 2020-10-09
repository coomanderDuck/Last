using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeeebLibrary.Database;
using WeeebLibrary.Database.Entitys;
using WeeebLibrary.interfaces;
using WeeebLibrary.Models;


namespace WeeebLibrary.Repository
{
    public class OrderRepositiry : IOrderRepositiry
    {
        private readonly LDBContext lDBContext;
        private readonly Cart cart;
        private readonly UserManager<User> userManager;


        public OrderRepositiry(LDBContext lDBContext, Cart cart, UserManager<User> userManager)
        {
            this.lDBContext = lDBContext;
            this.cart = cart;
            this.userManager = userManager;
        }
        public void CreateOrder(User user)
        {

            
            
            cart.ListItems = cart.GetItems();
            var items = cart.ListItems;
            foreach (var el in items)
            {
                var order = new Order()
                {
                    BookId = el.book.Id,
                    OrderTime = DateTime.Now,
                    UserId = user.Id
                };
                el.book.Status = Enums.Status.Booked;
                lDBContext.Order.Add(order);
            }
            //cart.ListItems.Clear();
            lDBContext.SaveChanges();
        }
        public void GiveBook(Book book)
        {
            book.Status = Enums.Status.Taked;
            lDBContext.Update(book);
            
        }
        public void TakeBook(Book book)
        {
            book.Status = Enums.Status.Available;
            lDBContext.Update(book);

        }


        public IEnumerable<Order> Orders => lDBContext.Order.Include(c => c.Book).Include(c => c.User);
    }
}

