using System.Linq;
using System.Threading.Tasks;

namespace WeeebLibrary.DAL.InterfacesDLL
{
    public interface IRepository<T>
    {
        T Get(int id);

        Task CreateAsync(T item);

        void Update(T item);

        void Delete(T item);

        IQueryable<T> GetAll();
    }
}
