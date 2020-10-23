using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using WeeebLibrary.BLL;
using WeeebLibrary.BLL.Jobs;
using WeeebLibrary.BLL.MappingProfiles;
using WeeebLibrary.BLL.RoleInitializer;
using WeeebLibrary.Parser;

namespace WeeebLibrary
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //подключение коллекции сервисов из класса MyConfigServiceCollection
            services.AddMyConfig(Configuration);

            // Добавляем Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Добавляем our job
            services.AddSingleton<CancelJob>();
            services.AddSingleton<BookParser>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(CancelJob),
                cronExpression: "0/5 * * * * ?")); // Запускать каждые 5 секунд
            services.AddSingleton(new JobSchedule(
                jobType: typeof(BookParser),
                cronExpression: "0 8 * * * ?")); //Каждый день в 8:00
            services.AddHostedService<QuartzHostedService>();

            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BookProfile());
                mc.AddProfile(new UserProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllersWithViews();
            services.AddDistributedMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            await CallRoleInitializer.RoleInitializeAsync(app);
            //await CallParser.BooksParsingAsync(app);
        }
    }
}
