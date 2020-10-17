using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.Interfaces;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.Enums;
using WeeebLibrary.DAL.InterfacesDLL;

namespace WeeebLibrary.Repository
{
    public class OrderService : IOrderService
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<Book> bookRepositiry;
        private readonly IRepository<Order> orderRepositiry;
        private readonly IHttpContextAccessor httpContextAccessor;
        public OrderService(IRepository<Book> bookRepositiry, IRepository<Order> orderRepositiry, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            this.bookRepositiry = bookRepositiry;
            this.orderRepositiry = orderRepositiry;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public OrderDTO GetOrder(int id)
        {
            var order = orderRepositiry.Get(id);

            var orderDto = new OrderDTO
            {
                Id = order.Id,
                BookId = order.BookId,
                UserId = order.UserId,
                OrderTime = order.OrderTime,
                Book = order.Book,
                User = order.User
            };

            return orderDto;
        }

        public IEnumerable<OrderDTO> GetOrders()
        {
            var orders = orderRepositiry.GetAll()
                .Include(b => b.Book)
                .Include(b => b.User)
                .OrderBy(b => b.Book.Name)
                .Select(b => new OrderDTO
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    UserId = b.UserId,
                    OrderTime = b.OrderTime,
                    Book = b.Book,
                    User = b.User
                })
                .ToList();

            return orders;
        }

        public async Task MakeOrderAsync(int id)
        {
            var book = bookRepositiry.Get(id);
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
            if ((book != null) & (book.Status == Status.Available))
            {
                var order = new Order()
                {
                    BookId = book.Id,
                    OrderTime = DateTime.Now,
                    UserId = user.Id
                };
                book.Status = Status.Booked;
                bookRepositiry.Update(book);
                orderRepositiry.Create(order);
            }
        }

        public void GiveBook(int orderId)
        {
            var order = orderRepositiry.Get(orderId);
            var book = bookRepositiry.Get(order.BookId);
            book.Status = Status.Taked;
            bookRepositiry.Update(book);
        }

        public void TakeBook(int orderId)
        {
            var order = orderRepositiry.Get(orderId);
            var book = bookRepositiry.Get(order.BookId);
            book.Status = Status.Available;
            bookRepositiry.Update(book);
            orderRepositiry.Delete(order);
        }
        public void DeleteOrder(int id)
        {
            var order = orderRepositiry.Get(id);
            var book = bookRepositiry.Get(order.BookId);
            book.Status = Status.Available;
            orderRepositiry.Delete(order);
        }

        public async Task<IEnumerable<OrderDTO>> FindCustomerrOders(IEnumerable<OrderDTO> OrdersDto)
        {
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
            OrdersDto = OrdersDto.Where(s => s.UserId.Contains(user.Id));
            return OrdersDto;
        }
    }
}

