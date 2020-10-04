using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.Database;
using WeeebLibrary.Database.Entitys;
using WeeebLibrary.interfaces;

namespace WeeebLibrary.Repository
{
    public class BookRepository : ILibraryRepository
    {
        private readonly LDBContext lDBContext;

        public BookRepository(LDBContext lDBContext)
        {
            this.lDBContext = lDBContext;
        }

        public IEnumerable<Book> Books => lDBContext.Book;

        public IEnumerable<Book> GetBooks => lDBContext.Book.Where(p => p.Available.Contains("Available"));

        public Book getObjectBook(int bookId) => lDBContext.Book.FirstOrDefault(p => p.Id == bookId);
    }
}

