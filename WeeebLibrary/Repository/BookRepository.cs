using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.Database;
using WeeebLibrary.Database.Entitys;
using WeeebLibrary.Enums;
using WeeebLibrary.interfaces;

namespace WeeebLibrary.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly LDBContext lDBContext;

        public BookRepository(LDBContext lDBContext)
        {
            this.lDBContext = lDBContext;
        }
        public IEnumerable<Book> Books => lDBContext.Book;

        public Book GetBook(int? bookId) => lDBContext.Book.FirstOrDefault(p => p.Id == bookId);

        public async Task UpdateBookAsync(Book book)
        {
            lDBContext.Update(book);
            await lDBContext.SaveChangesAsync();
        }
        public async Task DeleteBookAsync(Book book)
        {
            lDBContext.Book.Remove(book);
            await lDBContext.SaveChangesAsync();
        }

    }
}

