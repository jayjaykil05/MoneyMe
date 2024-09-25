using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyMe.Models;
using MoneyMe.Services;
using System.Text;
using System.Text.Json;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        ////DEPENDENCY INJECTION
        builder.Services.AddDbContext<MoneymeDbContext>(item => item.UseNpgsql(builder.Configuration.GetConnectionString("PostgreDbConnection")));
        builder.Services.AddScoped<UsersService>();


        builder.Services.AddControllers();
        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        builder.Services.Configure<CookieTempDataProviderOptions>(options =>
        {
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddSession(options =>
        {
            options.Cookie.IsEssential = true;
        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseAuthentication();
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
