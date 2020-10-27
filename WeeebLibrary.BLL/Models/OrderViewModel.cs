using System;
using System.Collections.Generic;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.Enums;

namespace WeeebLibrary.BLL.Models
{
    public class OrderViewModel
    {
        public List<Order> Orders { get; set; }

        public DateTime DateMin { get; set; }

        public DateTime DateMax { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public bool Save { get; set; }
    }
}
