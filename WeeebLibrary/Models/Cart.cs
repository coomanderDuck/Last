using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.Database;
using WeeebLibrary.Database.Entitys;

namespace WeeebLibrary.Models
{
    public class Cart
    {
        private readonly LDBContext lDbContext;
        public Cart(LDBContext lDbContext)
        {
            this.lDbContext = lDbContext;
        }
        public string CartId { get; set; }

        public List<CartItem> ListItems { get; set; }

        public static Cart GetCart(IServiceProvider books)
        {
            ISession session = books.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = books.GetService<LDBContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", cartId);

            return new Cart(context) { CartId = cartId };
        }

        public void AddToCart(Book book)
        {
            lDbContext.CartItem.Add(new CartItem
            {
                CartId = CartId,
                book = book,
                
            }
                );
            lDbContext.SaveChanges();
        }

        public List<CartItem> GetItems()
        {
            return lDbContext.CartItem.Where(c => c.CartId == CartId).Include(s => s.book).ToList();
        }
    }
}

