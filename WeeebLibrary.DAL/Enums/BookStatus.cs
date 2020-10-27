using System.ComponentModel.DataAnnotations;

namespace WeeebLibrary.DAL.Enums
{
    public enum BookStatus
    {
        [Display(Name = "Доступна")]
        Available,

        [Display(Name = "Забронирована")]
        Booked,

        [Display(Name = "Отдана")]
        Taked
    }
}
