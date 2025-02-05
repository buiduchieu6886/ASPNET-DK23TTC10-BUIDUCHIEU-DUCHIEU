using AuctionSimWebsite.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AuctionSimWebsite.Models;
using AuctionSimWebsite.Controllers;

namespace AuctionSimWebsite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình DbContext với SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Cấu hình Identity cho người dùng và mật khẩu
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Thêm dịch vụ MVC (bao gồm controllers và views)
            builder.Services.AddControllersWithViews();

            // Cấu hình dịch vụ Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian session hết hạn
                options.Cookie.HttpOnly = true; // Cookie chỉ truy cập qua HTTP
                options.Cookie.IsEssential = true; // Bắt buộc cookie (dành cho GDPR)
            });

            // Cấu hình dịch vụ Cookie Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login"; // Định nghĩa trang đăng nhập
                    options.LogoutPath = "/Auth/Logout"; // Định nghĩa trang đăng xuất
                    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Thời gian hết hạn cookie
                    options.SlidingExpiration = true; // Cho phép gia hạn khi người dùng hoạt động
                });

            var app = builder.Build();

            // Sử dụng các file tĩnh như hình ảnh, CSS, JS
            app.UseStaticFiles();

            // Sử dụng Session
            app.UseSession();

            // Cấu hình Middleware cho Authentication và Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Cấu hình routing cho các controller
            app.UseRouting();

            // Định nghĩa route mặc định
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Chạy ứng dụng
            app.Run();
        }
    }
}
