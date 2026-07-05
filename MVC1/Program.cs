using AutoMapper;
using GYMsystem.DAL.Context;
using GYMsystem.DAL.DataSeeding;
using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.classes;
using GYMsystem.DAL.Repositories.interfaces;
using GYMSystem.BLL;
using GYMSystem.BLL.Services.AttachmentService;
using GYMSystem.BLL.Services.classes;
using GYMSystem.BLL.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MVC1
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            
            builder.Services.AddDbContext<GYMDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

           // builder.Services.AddScoped<IplanRepository,PlanRepository>();
           builder.Services.AddScoped<ISessionRepository, SessionRepository>();
           builder.Services.AddScoped<IBookingRepository, BookingRepository>();
           builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();

            builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            builder.Services.AddScoped<IMemberServices , MemberServices>();
            builder.Services.AddScoped<IPlanServices, PlanServices>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();          
            builder.Services.AddScoped<ISessionServices, SessionServices>();             
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IMembershipService, MembershipService>();

            builder.Services.AddAutoMapper(m=>m.AddProfile(new MappingProfile()));
            builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<GYMDBContext>();
            var app = builder.Build();

            await app.MigrateAndSeedDatabaseAsync();




            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
