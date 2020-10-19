using System.Linq;

namespace WeeebLibrary.DAL.InterfacesDLL
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);

        void Create(T item);

        void Update(T item);

        void Delete(T item);

        IQueryable<T> GetAll();
    }
}
