using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parser;
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
            //����������� ��������� �������� �� ������ MyConfigServiceCollection
            services.AddMyConfig(Configuration);

            services.AddTransient<IParserBooks, ParserBooks>();

            // ��������� Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // ��������� our job
            services.AddSingleton<CancelJob>();
            services.AddSingleton<BookParserJob>();
            services.AddSingleton(new JobSchedule(
               jobType: typeof(CancelJob),
               cronExpression: "0 0/1 * 1/1 * ? *")); // ��������� ������ ������
            services.AddSingleton(new JobSchedule(
                jobType: typeof(BookParserJob),
                cronExpression: "0 36 3 1/1 * ? *"));  //��������� ������ ���� 
          services.AddHostedService<QuartzHostedService>(); 
          services.AddHostedService<InitializerHostedService>();

            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BookProfile());
                mc.AddProfile(new UserProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddHttpClient();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllersWithViews();
            services.AddDistributedMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}
