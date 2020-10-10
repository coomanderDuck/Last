using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.Database.Entitys;

namespace WeeebLibrary.interfaces
{
    public interface IOrderRepositiry
    {
        void CreateOrder(User user, Book book);
        void GiveBook(Book book);
        void TakeBook(Book book, Order order);
        void DeleteOrder(Order order);
        IEnumerable<Order> Orders { get; }
    }
}
