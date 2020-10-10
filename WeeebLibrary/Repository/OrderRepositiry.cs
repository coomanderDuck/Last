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
        private readonly UserManager<User> userManager;


        public OrderRepositiry(LDBContext lDBContext, UserManager<User> userManager)
        {
            this.lDBContext = lDBContext;

            this.userManager = userManager;
        }
        public void CreateOrder(User user, Book book)
        {
            var order = new Order()
            {
                BookId = book.Id,
                OrderTime = DateTime.Now,
                UserId = user.Id
            };
            book.Status = Enums.Status.Booked;
            lDBContext.Order.Add(order);
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

