using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeeebLibrary.BLL.Interfaces;
using WeeebLibrary.BLL.InterfacesBLL;
using WeeebLibrary.BLL.Services;
using WeeebLibrary.DAL.Database;
using WeeebLibrary.DAL.Database.Entitys;
using WeeebLibrary.DAL.InterfacesDLL;
using WeeebLibrary.DAL.Repository;
using WeeebLibrary.DAL.Repository.DAL;
using WeeebLibrary.Repository;


namespace WeeebLibrary.BLL
{
    public static class MyConfigServiceCollectionExtensions
    {

        public static IServiceCollection AddMyConfig(
                this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContextPool<LDBContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

             services.AddIdentity<User, IdentityRole>(opts =>
             {
             })
                 .AddEntityFrameworkStores<LDBContext>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IBookService, BookService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRepository<Book>, BookRepository>();
            services.AddTransient<IRepository<Order>, OrderRepositiry>();
            return services;
        }
    }
}
