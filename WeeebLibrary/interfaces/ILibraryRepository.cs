﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.Database.Entitys;

namespace WeeebLibrary.interfaces
{
    public interface ILibraryRepository
    {
        IEnumerable<Book> Books { get; }


        Book getObjectBook(int bookId);
    }
}
