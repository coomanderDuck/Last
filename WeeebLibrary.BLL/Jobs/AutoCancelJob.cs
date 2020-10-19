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
    public class AutoCancelJob : IJob
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<AutoCancelJob> logger;

        public AutoCancelJob(IServiceProvider serviceProvider, ILogger<AutoCancelJob> logger)
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
                        Where(c => c.Book.Status == Status.Booked).
                        Where(c => c.OrderTime.AddMinutes(5) <= DateTime.Now))
                    {
                        orderRepository.Delete(order);
                        var book = bookRepository.Get(order.BookId);
                        book.Status = Status.Available;
                        logger.LogInformation("order deleted");
                    }
                }
                await Task.CompletedTask;
            }
        }
    }
}
