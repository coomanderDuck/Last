using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.Interfaces;
using WeeebLibrary.BLL.Models;
using WeeebLibrary.DAL;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.Enums;
using WeeebLibrary.DAL.InterfacesDLL;


namespace WeeebLibrary.Repository
{
    public class OrderService : IOrderService
    {
        private readonly UserManager<User> userManager;
        private readonly IRepository<Book> bookRepository;
        private readonly IRepository<Order> orderRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public OrderService(IRepository<Book> bookRepository, IRepository<Order> orderRepository, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            this.bookRepository = bookRepository;
            this.orderRepository = orderRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public OrderDTO GetOrder(int id)
        {
            var order = orderRepository.Get(id);

            var orderDto = new OrderDTO
            {
                Id = order.Id,
                BookId = order.BookId,
                UserId = order.UserId,
                OrderStatus = order.OrderStatus,
                BookedTime = order.BookedTime,
                TakedTime = order.TakedTime,
                CompletedTime = order.CompletedTime,
                СanceledTime = order.СanceledTime,
                Book = order.Book,
                User = order.User
            };
            return orderDto;
        }

        public IEnumerable<OrderDTO> GetOrders()
        {
            var ordersDto = orderRepository.GetAll()
                .Include(b => b.Book)
                .Include(b => b.User)
                .OrderByExp("BookedTime")
                .Where(b => b.OrderStatus != OrderStatus.Completed)
                .Where(b => b.OrderStatus != OrderStatus.Сanceled)
                .Select(b => new OrderDTO
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    UserId = b.UserId,
                    OrderStatus = b.OrderStatus,
                    BookedTime = b.BookedTime,
                    TakedTime = b.TakedTime,
                    CompletedTime = b.CompletedTime,
                    СanceledTime = b.СanceledTime,
                    Book = b.Book,
                    User = b.User
                })
                .ToList();

            return ordersDto;
        }

        public async Task MakeOrderAsync(int id)
        {
            var book = bookRepository.Get(id);
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
            if ((book != null) & (book.Status == BookStatus.Available))
            {
                var order = new Order()
                {
                    BookId = book.Id,
                    BookedTime = DateTime.Now,
                    UserId = user.Id,
                    OrderStatus = OrderStatus.Booked
                };
                book.Status = BookStatus.Booked;
                bookRepository.Update(book);
                await orderRepository.CreateAsync(order);
            }
        }

        public void GiveBook(int orderId)
        {
            var order = orderRepository.Get(orderId);
            var book = bookRepository.Get(order.BookId);
            book.Status = BookStatus.Taked;
            order.OrderStatus = OrderStatus.Taked;
            order.TakedTime = DateTime.Now;
            bookRepository.Update(book);
            orderRepository.Update(order);
        }

        public void TakeBook(int orderId)
        {
            var order = orderRepository.Get(orderId);
            var book = bookRepository.Get(order.BookId);
            book.Status = BookStatus.Available;
            order.OrderStatus = OrderStatus.Completed;
            order.CompletedTime = DateTime.Now;
            bookRepository.Update(book);
            orderRepository.Update(order);
        }

        public void DeleteOrder(int id)
        {
            var order = orderRepository.Get(id);
            var book = bookRepository.Get(order.BookId);
            book.Status = BookStatus.Available;
            order.OrderStatus = OrderStatus.Сanceled;
            order.СanceledTime = DateTime.Now;
            bookRepository.Update(book);
            orderRepository.Update(order);
        }

        public async Task<IEnumerable<OrderDTO>> FindCustomerrOders(IEnumerable<OrderDTO> OrdersDto)
        {
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
            OrdersDto = OrdersDto.Where(s => s.UserId.Contains(user.Id));
            return OrdersDto;
        }

        public async Task<OrderViewModel> FilrterOdersAsync(OrderStatus orderStatus, DateTime min, DateTime max)
        {
            var dateNull = new DateTime();
            var orders = orderRepository.GetAll()
                                        .Include(b => b.Book)
                                        .Include(b => b.User)
                                        .OrderByExp("Id");

            if ((min != dateNull) & (max != dateNull))
            {
                orders = orders.Where(c => c.BookedTime >= min && c.BookedTime <= max).OrderByExp("Id");
            }
            if (orderStatus != OrderStatus.Any)
            {
                orders = orders.Where(s => s.OrderStatus == orderStatus).OrderByExp("Id");
            }

            var orderVM = new OrderViewModel
            {
                Orders = await orders.ToListAsync(),
                DateMin = min,
                DateMax = max
            };

            return orderVM;
        }

        public MemoryStream SaveReport(List<Order> orders)
        {
            var NullTime = new DateTime();
            //Рабочая книга Excel
            XSSFWorkbook wb;
            //Лист в книге Excel
            XSSFSheet sh;

            //Создаем рабочую книгу
            wb = new XSSFWorkbook();
            //Создаём лист в книге
            sh = (XSSFSheet)wb.CreateSheet("Лист 1");

            //Заполняемые строки
            var rows = new List<string> {
            "Номер брони","Книга","Пользователь","Статус заказа","Время бронирования",
            "Время выдачи", "Время заврешения заказа", "Время отмены заказа"
            };

            var i = 1;
            var j = 0;

            //Создаем строку c заголовками
            var currentRow = sh.CreateRow(0);
            foreach (var row in rows)
            {
                //в строке создаём ячеёку с указанием столбца
                var currentCell = currentRow.CreateCell(j);
                //в ячейку запишем заголовок
                currentCell.SetCellValue(row);
                //Выравним размер столбца по содержимому
                sh.AutoSizeColumn(j);
                j++;
            }

            //Запускаем цыкл по строке
            foreach (var order in orders)
            {
                //Создаем строку
                var currentRoww = sh.CreateRow(i);

                var currentCell = currentRoww.CreateCell(0);//в строке создаём ячеqку с указанием столбца
                currentCell.SetCellValue(order.Id);//в ячейку запишем информацию

                currentCell = currentRoww.CreateCell(1);
                currentCell.SetCellValue(order.Book.Name);

                currentCell = currentRoww.CreateCell(2);
                currentCell.SetCellValue(order.User.Email);

                currentCell = currentRoww.CreateCell(3);
                switch (order.OrderStatus)
                {
                    case OrderStatus.Booked:
                        currentCell.SetCellValue("Забронирован");
                        break;
                    case OrderStatus.Completed:
                        currentCell.SetCellValue("Завершён");
                        break;
                    case OrderStatus.Taked:
                        currentCell.SetCellValue("Отдан");
                        break;
                    case OrderStatus.Сanceled:
                        currentCell.SetCellValue("Отменён");
                        break;
                }

                currentCell = currentRoww.CreateCell(4);
                if (order.BookedTime != NullTime)
                {
                    currentCell.SetCellValue(order.BookedTime.ToString());
                }

                currentCell = currentRoww.CreateCell(5);
                if (order.TakedTime != NullTime)
                {
                    currentCell.SetCellValue(order.TakedTime.ToString());
                }

                currentCell = currentRoww.CreateCell(6);
                if (order.CompletedTime != NullTime)
                {
                    currentCell.SetCellValue(order.CompletedTime.ToString());
                }

                currentCell = currentRoww.CreateCell(7);
                if (order.СanceledTime != NullTime)
                {
                    currentCell.SetCellValue(order.СanceledTime.ToString());
                }

                //Выравним размер столбца по содержимому
                sh.AutoSizeColumn(j);
                i++;
            }

            ByteArrayOutputStream bos = new ByteArrayOutputStream();
            try
            {
                wb.Write(bos);
            }
            finally
            {
                bos.Close();
            }
            byte[] bytes = bos.ToByteArray();
            var memoryStream = new MemoryStream(bytes);

            return memoryStream;
        }
    }
}

