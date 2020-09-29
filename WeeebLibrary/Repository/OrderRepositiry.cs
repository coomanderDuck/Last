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
        public void createOrder(User user)
        {
            cart.listItems = cart.getItems();
            var items = cart.listItems;
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

