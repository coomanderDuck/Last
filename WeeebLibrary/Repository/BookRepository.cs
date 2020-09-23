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

        public IEnumerable<Book> Books => lDBContext.Books;



        public Book getObjectBook(int bookId) => lDBContext.Books.FirstOrDefault(p => p.Id == bookId);
    }
}

