using System.ComponentModel.DataAnnotations;
using WeeebLibrary.Enums;

namespace WeeebLibrary.Database.Entitys
{
    public class Book
    {
        public int Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Автор")]
        public string Autor { get; set; }

        [Display(Name = "Жанр")]
        public string Genre { set; get; }

        [Display(Name = "Издатель")]
        public string Publisher { get; set; }

        [Display(Name = "Описание")]
        public string Desc { set; get; }

        [Display(Name = "Обложка")]
        public string img { set; get; }

        [Display(Name = "Статус")]
        public Status Status { set; get; }


    }
}
