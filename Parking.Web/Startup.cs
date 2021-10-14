using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parking.Core.Factories;
using Parking.Core.Managers;
using Parking.Infrastructure.Data;
using Parking.SharedKernel.Models;
using Parking.Web.Controllers;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ParkingContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ParkingContext"), b => b.MigrationsAssembly("Parking.Infrastructure")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ParkingContext>();

            services.AddScoped<InitController>();
            services.AddScoped<UserTokenManager>();
            services.AddScoped<CheckInManager>();
            services.AddScoped<PlaceManager>();
            services.AddSingleton<PlaceMapFactory>();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();    // подключение аутентификации
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Accounts}/{action=Login}/{id?}");
            });
        }
    }
}
