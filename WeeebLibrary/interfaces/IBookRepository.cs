using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.Database.Entitys;

namespace WeeebLibrary.interfaces
{
    public interface IBookRepository
    {
        IEnumerable<Book> Books { get; }

        Book GetBook(int? bookId);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(Book book);
    }
}
