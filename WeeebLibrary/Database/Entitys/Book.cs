using WeeebLibrary.Enums;

namespace WeeebLibrary.Database.Entitys
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Autor { get; set; }
        public string Genre { set; get; }
        public string Publisher { get; set; }
        public string Desc { set; get; }
        public string img { set; get; }
        public Status Status { set; get; }


    }
}
