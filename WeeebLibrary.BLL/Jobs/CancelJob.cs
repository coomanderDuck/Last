using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.Enums;
using WeeebLibrary.DAL.InterfacesDLL;

namespace WeeebLibrary.BLL.Jobs
{
    public class CancelJob : IJob
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<CancelJob> logger;

        public CancelJob(IServiceProvider serviceProvider, ILogger<CancelJob> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var orderRepository = scope.ServiceProvider.GetService<IRepository<Order>>();
                var bookRepository = scope.ServiceProvider.GetService<IRepository<Book>>();
                var orders = orderRepository.GetAll()
              .Include(b => b.Book)
              .ToList();
                if (orders != null)
                {
                    foreach (var order in orders.
                        Where(c => c.Book.Status == BookStatus.Booked).
                        Where(c => c.BookedTime.AddDays(7) <= DateTime.Now)) //Через неделю забронированный заказ отменяется
                    {
                        var book = bookRepository.Get(order.BookId);
                        book.Status = BookStatus.Available;
                        order.OrderStatus = OrderStatus.Сanceled;
                        bookRepository.Update(book);
                        orderRepository.Update(order);
                        logger.LogInformation("Заказ книги " + order.Book.Name + "отменён по истечению времени ожидания");
                    }
                }
                await Task.CompletedTask;
            }
        }
    }
}
