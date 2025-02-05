using Microsoft.AspNetCore.Mvc;
using AuctionSimWebsite.Models;
using Microsoft.EntityFrameworkCore;
using AuctionSimWebsite.Data;

namespace AuctionSimWebsite.Controllers
{
    public class BidController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BidController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách các bid
        public async Task<IActionResult> Index(int auctionId)
        {
            var auction = await _context.Auctions
                .Include(a => a.SimCard)
                .Include(a => a.Bids)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(a => a.Id == auctionId);

            if (auction == null)
            {
                return NotFound();
            }

            return View(auction.Bids);
        }

        // Chi tiết bid
        public async Task<IActionResult> Details(int id)
        {
            var bid = await _context.Bids
                .Include(b => b.Auction)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (bid == null)
            {
                return NotFound();
            }
            return View(bid);
        }

        // Tạo bid mới
        public IActionResult Create(int auctionId)
        {
            var auction = _context.Auctions.Find(auctionId);
            if (auction == null)
            {
                return NotFound();
            }
            ViewData["AuctionId"] = auctionId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bid bid, int auctionId)
        {
            if (ModelState.IsValid)
            {
                var auction = await _context.Auctions.FindAsync(auctionId);
                if (auction == null)
                {
                    return NotFound();
                }

                bid.AuctionId = auctionId;
                bid.BidTime = DateTime.Now;
                _context.Add(bid);
                await _context.SaveChangesAsync();

                // Cập nhật giá thầu cao nhất của phiên đấu giá
                if (bid.BidAmount > auction.HighestBid)
                {
                    auction.HighestBid = bid.BidAmount;
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index), new { auctionId = auctionId });
            }
            return View(bid);
        }

        // Xóa bid
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid != null)
            {
                _context.Bids.Remove(bid);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { auctionId = bid.AuctionId });
        }
    }
}
