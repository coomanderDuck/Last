using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeeebLibrary.DAL.Database.Entitys;


namespace WeeebLibrary.DAL.Database
{

    public class LDBContext : IdentityDbContext<User>
    {
        public DbSet<Book> Book { get; set; }

        public DbSet<Order> Order { get; set; }

        public LDBContext(DbContextOptions<LDBContext> options)
                : base(options)
        {
            Database.Migrate();
        }
    }

}
