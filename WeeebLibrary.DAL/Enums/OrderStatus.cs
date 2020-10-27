using System.ComponentModel.DataAnnotations;

namespace WeeebLibrary.DAL.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Любой статус")]
        Any,

        [Display(Name = "Забронирован")]
        Booked,

        [Display(Name = "Отдан")]
        Taked,

        [Display(Name = "Завершён")]
        Completed,

        [Display(Name = "Отменён")]
        Сanceled
    }
}