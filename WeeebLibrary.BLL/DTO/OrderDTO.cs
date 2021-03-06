﻿using System;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.Enums;

namespace WeeebLibrary.BLL.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string UserId { get; set; }

        public  OrderStatus OrderStatus { get; set; }

        public DateTime BookedTime { get; set; }

        public DateTime TakedTime { get; set; }

        public DateTime CompletedTime { get; set; }

        public DateTime СanceledTime { get; set; }

        public virtual Book Book { get; set; }

        public virtual User User { get; set; }
    }
}
