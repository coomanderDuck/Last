using System.ComponentModel.DataAnnotations;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.Enums;

namespace WeeebLibrary.BLL.DTO
{
    public class BookDTO 
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
        public string Img { set; get; }

        public string ImgPath { get; set; }

        [Display(Name = "Статус")]
        public Status Status { set; get; }


    }
}
