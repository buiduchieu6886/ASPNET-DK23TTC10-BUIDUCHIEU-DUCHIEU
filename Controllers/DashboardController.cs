using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionSimWebsite.Controllers
{
    public class DashboardController : Controller
    {
        // Action hiển thị Dashboard
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            return View();
        }

    }
}
