using System.Linq;
using WeeebLibrary.DAL.Database;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.InterfacesDLL;

namespace WeeebLibrary.DAL.Repository.DAL
{
    public class OrderRepositiry : IRepository<Order>
    {
        private readonly LDBContext lDBContext;

        public OrderRepositiry(LDBContext lDBContext)
        {
            this.lDBContext = lDBContext;
        }

        public IQueryable<Order> GetAll()
        {
            return lDBContext.Set<Order>();
        }
        public Order Get(int orderId) => lDBContext.Order.FirstOrDefault(p => p.Id == orderId);

        public void Create(Order order)
        {
            lDBContext.Order.Add(order);
            lDBContext.SaveChanges();
        }

        public void Update(Order order)
        {
            lDBContext.Update(order);
            lDBContext.SaveChanges();
        }

        public void Delete(Order order)
        {
            lDBContext.Order.Remove(order);
            lDBContext.SaveChanges();
        }
    }
}

