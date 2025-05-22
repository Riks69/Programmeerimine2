using System;
using KooliProjekt.Controllers;
using KooliProjekt.Data;
using KooliProjekt.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KooliProjekt.IntegrationTests.Helpers
{
    public class FakeStartup //: Startup
    {
        public FakeStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            // Kasutame mälupõhist andmebaasi iga testi jaoks
            var dbGuid = Guid.NewGuid().ToString();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbGuid);  // Eraldi mälupõhine andmebaas iga testi jaoks
            });

            // Lisame oma teenused
            services.AddScoped<IBookingService, BookingService>();  // Booking teenus
            services.AddScoped<ICarService, CarService>();          // Car teenus
            services.AddScoped<ICustomerService, CustomerService>(); // Customer teenus
            services.AddScoped<IInvoiceService, InvoiceService>();  // Invoice teenus

            // Identity teenuste registreerimine
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews()
                    .AddApplicationPart(typeof(HomeController).Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Seadistame testide jaoks vajalikud keskmised teenused
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}/{pathStr?}");
            });

            // Veendume, et andmebaas oleks loodud
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                if (dbContext == null)
                {
                    throw new NullReferenceException("Ei suuda leida dbContext'i");
                }

                dbContext.Database.EnsureCreated();  // Loome mälupõhise andmebaasi iga testi jaoks
            }
        }
    }
}
