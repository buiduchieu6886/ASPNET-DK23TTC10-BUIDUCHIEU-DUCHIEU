using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using AuctionSimWebsite.Models;
using AuctionSimWebsite.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AuctionSimWebsite.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Auth/Login
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Nếu người dùng đã đăng nhập, chuyển hướng đến trang chủ
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Nếu dữ liệu không hợp lệ, trả về form đăng nhập với lỗi.
            }

            // Tìm user bằng Username hoặc Email
            var user = await _userManager.FindByNameAsync(model.UsernameOrEmail) ??
                       await _userManager.FindByEmailAsync(model.UsernameOrEmail);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model); // Nếu không tìm thấy user hoặc mật khẩu không đúng, hiển thị lỗi.
            }
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Dashboard");
            }
            // Tạo các claims cho người dùng sau khi đăng nhập thành công
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName), // Tên người dùng
                new Claim(ClaimTypes.NameIdentifier, user.Id), // ID của người dùng
                new Claim(ClaimTypes.Role, "User") // Gán role mặc định cho người dùng, có thể thay đổi tùy vào yêu cầu
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Cấu hình các thuộc tính của Authentication (cookie)
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe, // Nếu "Nhớ tôi" được chọn, cookie sẽ tồn tại lâu hơn
                ExpiresUtc = DateTime.UtcNow.AddDays(7) // Đặt thời gian hết hạn của cookie
            };

            // Đăng nhập người dùng và lưu thông tin vào cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
            await _signInManager.SignInAsync(user, isPersistent: false);
            // Sau khi đăng nhập thành công, chuyển hướng người dùng đến trang chính
            return RedirectToAction("Index", "Home");
        }


        // POST: Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ CSRF
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ sau khi đăng xuất
        }

        // GET: Auth/Register
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra email đã tồn tại
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu và xác nhận mật khẩu không khớp.");
                return View(model);
            }

            // Tạo người dùng mới
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = "User"
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Đăng nhập ngay sau khi đăng ký thành công
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }

            // Nếu có lỗi khi tạo người dùng, thêm thông báo lỗi vào ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}
