using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.InterfacesDLL;

namespace WeeebLibrary.DAL.Repository
{
    public class BookRepository : IRepository<Book>
    {
        private readonly LDBContext lDBContext;

        public BookRepository(LDBContext lDBContext)
        {
            this.lDBContext = lDBContext;
        }
        public IQueryable<Book> GetAll()
        {
            return lDBContext.Set<Book>();
        }

        public Book Get(int Id) => lDBContext.Book.FirstOrDefault(p => p.Id == Id);

        public void Create(Book book)
        {
            lDBContext.Book.Add(book);
            lDBContext.SaveChanges();
        }

        public void Update(Book book)
        {
            lDBContext.Update(book);
            lDBContext.SaveChanges();
        }
        public void Delete(Book book)
        {
            lDBContext.Book.Remove(book);
            lDBContext.SaveChanges();
        }

    }
}

