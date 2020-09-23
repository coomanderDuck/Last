using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeeebLibrary.Database.Entitys
{
    public class CartItem
    {
        public int Id { get; set; }
        public Book book { get; set; }
       

        public string CartId { get; set; }
    }
}
