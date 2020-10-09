using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WeeebLibrary.Enums
{
    public enum Status{ [Display(Name = "Доступна")] Available, [Display(Name = "Забронирована")] Booked, [Display(Name = "Отдана")] Taked }
}
