﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.Database.Entitys;

namespace WeeebLibrary.Models
{
    public class BookGenreViewModel
    {
        public List<Book> Books { get; set; }
        public SelectList Autor { get; set; }
        public SelectList Genres { get; set; }
        public SelectList Publisher { get; set; }
        public string BookAutor { get; set; }
        public string BookGenre { get; set; }
        public string BookPublisher { get; set; }
        public string SearchString { get; set; }
    }
}
