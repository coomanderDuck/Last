using System.Linq;
using WeeebLibrary.DAL.Database;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.InterfacesDLL;

namespace WeeebLibrary.DAL.Repository
{
    public class LastIdRepository : ILastIdRepository
    {
        private readonly LDBContext lDBContext;

        public LastIdRepository(LDBContext lDBContext)
        {
            this.lDBContext = lDBContext;
        }

        public void Create(LastParserId item)
        {
            lDBContext.LastParserId.Add(item);          
            lDBContext.SaveChanges();
        }

        public LastParserId Get() => lDBContext.LastParserId.FirstOrDefault();

        public void Update(LastParserId item)
        {
            lDBContext.Update(item);
            lDBContext.SaveChanges();
        }
    }
}
