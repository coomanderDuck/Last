using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeeebLibrary.Database.Entitys
{
    public class Order
    {
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderTime { get; set; }
        public virtual Book book { get; set; }
        public virtual User user { get; set; }

    }
}
