using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;
using WeeebLibrary.BLL.Models;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.Enums;

namespace WeeebLibrary.BLL.Interfaces
{
    public interface IOrderService
    {
        OrderDTO GetOrder(int id);

        IEnumerable<OrderDTO> GetOrders();

        Task MakeOrderAsync(int id);

        Task<IEnumerable<OrderDTO>> FindCustomerrOders(IEnumerable<OrderDTO> OrdersDto);

        Task<OrderViewModel> FilrterOdersAsync(OrderStatus orderStatus, DateTime min, DateTime max);

        void GiveBook(int orderId);

        void TakeBook(int orderId);

        void DeleteOrder(int id);

        MemoryStream SaveReport(List<Order> orders);
    }
}
