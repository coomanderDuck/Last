using System.Collections.Generic;
using System.Threading.Tasks;
using WeeebLibrary.BLL.DTO;

namespace WeeebLibrary.BLL.Interfaces
{
    public interface IOrderService
    {
        OrderDTO GetOrder(int id);

        IEnumerable<OrderDTO> GetOrders();

        Task MakeOrderAsync(int id);

        Task<IEnumerable<OrderDTO>> FindCustomerrOders(IEnumerable<OrderDTO> OrdersDto);

        void GiveBook(int orderId);

        void TakeBook(int orderId);

        void DeleteOrder(int id);
    }
}
