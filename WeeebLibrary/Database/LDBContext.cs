using WeeebLibrary.Database.Entitys;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WeeebLibrary.Database
{
   
        public class LDBContext : IdentityDbContext<User>
        {
        public DbSet<Book> Books { get; set; }
        
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Order> Order { get; set; }


        public LDBContext(DbContextOptions<LDBContext> options)
                : base(options)
            {
             Database.Migrate();
            }
        }
    
}
