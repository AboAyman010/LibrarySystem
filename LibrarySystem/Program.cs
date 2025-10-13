using LibrarySystem.DataAccess;
using LibrarySystem.Models;
using LibrarySystem.Repositories;
using LibrarySystem.Repositories.IRepositories;
using LibrarySystem.Utility.DbInitializer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Options;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;

namespace LibrarySystem.DataAccess
    

{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add MVC
            builder.Services.AddControllersWithViews();

            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity Configuration 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Option =>
            {
                Option.Password.RequiredLength = 8;
                Option.Password.RequireNonAlphanumeric = false;
                Option.User.RequireUniqueEmail = true;
            })
               .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            // Cookie paths
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Œ«’… »Ã“¡ ·Ê «·„” Œœ„ Õ«Ê· ÌœŒ· ’›Õ…  Õ «Ã  ”ÃÌ· œŒÊ· ÊÂÊ „‘ „”Ã· œŒÊ·° ÂÌ ÕÊ·  ·ﬁ«∆Ì« ·’›Õ… «··ÊÃ‰ œÌ

                options.LoginPath = "/Identity/Account/Login";
                //if user login but he is not admin this  will take him automatic to the NotFoundPage
                options.AccessDeniedPath = "/Customer/Home/NotFoundPage";
            });


            // Email & Repository
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IDBInitializer, DBInitializer>();

            builder.Services.AddScoped<IRepository<UserOTP>, Repository<UserOTP>>();

           

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            //Session will logout automatic after 50min
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(50);
            });
            var app = builder.Build();

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

            app.UseAuthentication(); //  „Â„ Ãœ« ·«“„ ﬁ»· UseAuthorization
            app.UseAuthorization();

            app.UseSession();


            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();


            //  «” œ⁄«¡ DBInitializer · Ê·Ìœ «·√œÊ«— Ê«·„” Œœ„ «·√Ê·

            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
                dbInitializer.Initialize();
            }

            app.Run();
        }
    }
}
