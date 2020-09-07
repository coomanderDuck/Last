using WeeebLibrary.Database.Entitys;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WeeebLibrary.Database
{
   
        public class LDbContext : IdentityDbContext<User>
        {
        public DbSet<Book> Books { get; set; }
        public DbSet<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>> IdentityUserClaims { get; set; }

            public LDbContext(DbContextOptions<LDbContext> options)
                : base(options)
            {
                Database.EnsureCreated();
            }
        }
    
}
