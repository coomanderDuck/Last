using WeeebLibrary.DAL.Database.Entitys;

namespace WeeebLibrary.DAL.InterfacesDLL
{
    public interface ILastIdRepository
    {
        public void Create(LastParserId item);

        public LastParserId Get();
        public void Update(LastParserId item);
    }
}
