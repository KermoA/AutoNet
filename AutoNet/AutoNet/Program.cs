using AutoNet.ApplicationServices.Services;
using AutoNet.Core.Domain;
using AutoNet.Core.ServiceInterface;
using AutoNet.Data;
using AutoNet.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoNet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure Identity with custom settings
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 5;

                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
                .AddEntityFrameworkStores<AutoNetContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("CustomEmailConfirmation")
                .AddDefaultUI();

            // Configure External Authentication providers
            builder.Services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.ClientId = builder.Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.ClientSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddGitHub(githubOptions =>
                {
                    githubOptions.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
                    githubOptions.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
                    githubOptions.Scope.Add("read:user");
                    githubOptions.Scope.Add("user:email");
                });

            // Register application services
            builder.Services.AddScoped<ICarsServices, CarsServices>();
            builder.Services.AddScoped<IEmailServices, EmailServices>();

            // Add DbContext for SQL Server
            builder.Services.AddDbContext<AutoNetContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add transient services
            builder.Services.AddTransient<ConfirmationEmail>();
            builder.Services.AddScoped<IFileServices, FileServices>();

            // Add session services for storing complex objects
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set session timeout
                options.Cookie.HttpOnly = true;  // For security, ensure the cookie is only accessible by the server
                options.Cookie.IsEssential = true;  // Make session cookie essential for the application to work
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Enable session middleware
            app.UseSession();

            app.UseAuthorization();

            // Define routing rules
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
