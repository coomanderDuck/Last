using System;

namespace WeeebLibrary.DAL.Database.Entitys
{
    public class Order
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string UserId { get; set; }

        public DateTime OrderTime { get; set; }

        public virtual Book Book { get; set; }

        public virtual User User { get; set; }
    }
}
