using Company.Data.Context;
using Company.Repository.Interfaces;
using Company.Repository.Repositories;
using Company.Services.Interfaces;
using Company.Services.Mapping;
using Company.Services.Services;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Company.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Company.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<CompanyDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            //  builder.Services.AddScoped<IDepartmentRepository, DepartmentRepoository>();
            builder.Services.AddScoped<IDepartmentService, DepartmentServices>();
            builder.Services.AddScoped<IEmployeeService,EmployeeService>();
            builder.Services.AddAutoMapper(x=>x.AddProfile(new EmployeeProfile()));
            builder.Services.AddAutoMapper(x => x.AddProfile(new DepartmentProfile()));

            builder.Services.AddIdentity<ApplicationUser,IdentityRole>(config=>
            {
                config.Password.RequiredUniqueChars = 2;
                config.Password.RequireUppercase = true;
                config.Password.RequireLowercase = true;
                config.Password.RequireDigit= true;
                config.Password.RequireNonAlphanumeric = true;
                config.Password.RequiredLength = 6;
                config.User.RequireUniqueEmail = true;
                config.Lockout.AllowedForNewUsers = true;
                config.Lockout.MaxFailedAccessAttempts = 3;
                config.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromHours(1);
                
            }
            ).AddEntityFrameworkStores<CompanyDBContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.Cookie.HttpOnly = true;
                option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                option.SlidingExpiration = true;
                option.LoginPath = "/Account/Login";
                option.LogoutPath= "/Account/Logout";
                option.AccessDeniedPath= "/Account/AccessDenied";
                option.Cookie.Name = "Hamada Cookies";  
                option.Cookie.SecurePolicy= CookieSecurePolicy.Always;
                option.Cookie.SameSite=SameSiteMode.Strict;
            }
            );
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

            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
            
        }
    }
}