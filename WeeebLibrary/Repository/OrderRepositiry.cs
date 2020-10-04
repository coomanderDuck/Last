using System;
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


        public OrderRepositiry(LDBContext lDBContext, Cart cart)
        {
            this.lDBContext = lDBContext;
            this.cart = cart;

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
                lDBContext.Order.Add(order);
            }
            lDBContext.SaveChanges();
        }
    }
}

