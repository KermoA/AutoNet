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

			builder.Services.AddScoped<ICarsServices, CarsServices>();

            builder.Services.AddDbContext<AutoNetContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddTransient<ConfirmationEmail>();
            builder.Services.AddScoped<IFileServices, FileServices>();

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
