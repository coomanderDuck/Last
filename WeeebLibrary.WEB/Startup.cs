using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeeebLibrary.BLL;
using WeeebLibrary.BLL.RoleInitializer;
using WeeebLibrary.BLL.Jobs;
using Quartz;
using Quartz.Spi;
using Quartz.Impl;

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

            // ��������� Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // ��������� our job
            services.AddSingleton<AutoCancelJob>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(AutoCancelJob),
                cronExpression: "0/5 * * * * ?")); // ��������� ������ 5 ������
            services.AddHostedService<QuartzHostedService>();

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

        }
    }
}