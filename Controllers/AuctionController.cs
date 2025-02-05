using Microsoft.AspNetCore.Mvc;
using AuctionSimWebsite.Models;
using Microsoft.EntityFrameworkCore;
using AuctionSimWebsite.Data;
using System.Security.Claims;

namespace AuctionSimWebsite.Controllers
{
    public class AuctionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuctionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách tất cả các phiên đấu giá
        public async Task<IActionResult> Index()
        {
            var auctions = await _context.Auctions.Include(a => a.SimCard).Include(a => a.Winner).ToListAsync();
            return View(auctions);
        }

        // Chi tiết phiên đấu giá
        public async Task<IActionResult> Details(int id)
        {
            var auction = await _context.Auctions
                .Include(a => a.SimCard)
                .Include(a => a.Winner)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            return View(auction);
        }

        // GET: Auction/Create
        public IActionResult Create()
        {
            // Lấy danh sách sim cards
            ViewData["SimCards"] = _context.SimCards.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Auction auction)
        {
            try {
                // Đảm bảo không gán giá trị cho Bids hoặc Winner
                auction.Bids = null; // Hoặc không cần set vì đã có mặc định
                auction.Winner = null;
                auction.WinnerId = null;
                auction.StartingPrice = auction.HighestBid;
                auction.BidCount = 0;
                _context.Add(auction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            } catch (Exception ex)
            {
                // Nếu ModelState không hợp lệ, quay lại form và load danh sách SimCard
                ViewData["SimCards"] = _context.SimCards.ToList();
                return View(auction);
            }
        }

        // Cập nhật trạng thái đấu giá
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            auction.Status = status;
            _context.Update(auction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Xóa phiên đấu giá
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction != null)
            {
                _context.Auctions.Remove(auction);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CreateBid(int auctionId, decimal bidAmount)
        {
            var auction = await _context.Auctions.FindAsync(auctionId);
            if (auction == null)
            {
                return NotFound();
            }

            var bid = new Bid
            {
                AuctionId = auctionId,
                BidAmount = bidAmount,
                BidTime = DateTime.Now,
                UserId = User.Identity.Name // Lấy ID người dùng hiện tại
            };

            _context.Add(bid);
            await _context.SaveChangesAsync();

            // Cập nhật giá thầu cao nhất của phiên đấu giá
            if (bid.BidAmount > auction.HighestBid)
            {
                auction.HighestBid = bid.BidAmount;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Bid", new { auctionId = auctionId });
        }
        public async Task<IActionResult> ActiveAuctions(string CarrierFilter)
        {
            // Truy vấn cơ bản
            var auctionsQuery = _context.Auctions
                .Include(a => a.SimCard) // Lấy thông tin Sim liên quan
                .Where(a => a.Status == "Active" && a.EndTime > DateTime.Now)
                .AsQueryable();

            // Áp dụng bộ lọc nếu có tham số CarrierFilter
            if (!string.IsNullOrEmpty(CarrierFilter))
            {
                auctionsQuery = auctionsQuery.Where(a => a.SimCard.Carrier == CarrierFilter);
            }

            // Thực thi truy vấn
            var activeAuctions = await auctionsQuery.ToListAsync();
            ViewBag.CarrierFilter = CarrierFilter;
            return View(activeAuctions);
        }
        [HttpGet]
        public async Task<IActionResult> AuctionDetail(int id)
        {
            var auction = await _context.Auctions
                .Include(a => a.SimCard)
                .FirstOrDefaultAsync(a => a.Id == id && a.Status == "Active");

            if (auction == null)
            {
                return NotFound("Không tìm thấy phiên đấu giá hoặc phiên đã kết thúc.");
            }

            return View(auction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBid(int auctionId, decimal bidAmount)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, redirectToLogin = true, message = "Bạn cần đăng nhập để thực hiện đấu giá." });
            }

            var auction = await _context.Auctions
                .Include(a => a.SimCard)
                .FirstOrDefaultAsync(a => a.Id == auctionId && a.Status == "Active");

            if (auction == null)
            {
                return Json(new { success = false, message = "Phiên đấu giá không tồn tại hoặc đã kết thúc." });
            }

            if (bidAmount <= auction.HighestBid)
            {
                return Json(new { success = false, message = "Giá đấu phải cao hơn giá hiện tại." });
            }

            // Lưu lịch sử đấu giá
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Lấy UserId từ thông tin đăng nhập
            var bid = new Bid
            {
                AuctionId = auctionId,
                UserId = userId,
                BidAmount = bidAmount,
                BidTime = DateTime.Now
            };

            _context.Bids.Add(bid);

            // Cập nhật giá đấu cao nhất
            auction.HighestBid = bidAmount;
            auction.BidCount += 1;

            // Cập nhật cơ sở dữ liệu
            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đấu giá thành công!" });
        }
    }
}
