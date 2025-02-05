using Microsoft.AspNetCore.Mvc;
using AuctionSimWebsite.Models;
using Microsoft.AspNetCore.Identity;

namespace AuctionSimWebsite.Controllers.Admin
{
    [Route("Admin/[controller]")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UserManagementController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // Danh sách người dùng
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList(); // Lấy danh sách tất cả người dùng
            return View(users);
        }

        // Chi tiết người dùng
        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // Xóa người dùng
        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Message"] = "Người dùng đã được xóa thành công.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Có lỗi xảy ra khi xóa người dùng.";
            return RedirectToAction("Index");
        }
        // Chỉnh sửa người dùng (GET)
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // Chỉnh sửa người dùng (POST)
        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, User model)
        {
            if (string.IsNullOrEmpty(id) || id != model.Id)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Email = model.Email;
            user.Balance = model.Balance;
            user.Role = model.Role;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Message"] = "Cập nhật thông tin người dùng thành công.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Có lỗi xảy ra khi cập nhật thông tin.";
            return View(model);
        }

    }
}
