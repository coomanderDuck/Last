using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeeebLibrary.DAL.Database.Entitys;

namespace WeeebLibrary.BLL.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string UserId { get; set; }

        public DateTime OrderTime { get; set; }

        public virtual Book Book { get; set; }

        public virtual User User { get; set; }
    }
}
