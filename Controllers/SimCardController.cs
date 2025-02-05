using AuctionSimWebsite.Data;
using AuctionSimWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionSimWebsite.Controllers
{
    public class SimCardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SimCardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SimCard/Index
        public async Task<IActionResult> Index()
        {
            var simCards = await _context.SimCards.ToListAsync();
            return View(simCards);
        }

        // GET: SimCard/Create
        public IActionResult Create()
        {
            ViewBag.Carriers = new List<string>
            {
                "Viettel",
                "Vinaphone",
                "Mobiphone",
                "Vietnamobile",
                "Wintel",
                "iTelecom",
                "GMobile"
            };
            return View();
        }

        // POST: SimCard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhoneNumber,Carrier,Description,StartingPrice,Status")] SimCard simCard)
        {
            if (ModelState.IsValid)
            {
                simCard.CreatedAt = DateTime.Now; // Thêm ngày tạo cho sim card
                _context.Add(simCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(simCard);
        }

        // GET: SimCard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var simCard = await _context.SimCards.FindAsync(id);
            if (simCard == null)
            {
                return NotFound();
            }
            return View(simCard);
        }

        // POST: SimCard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PhoneNumber,Carrier,Description,StartingPrice,Status")] SimCard simCard)
        {
            if (id != simCard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(simCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SimCardExists(simCard.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(simCard);
        }

        // GET: SimCard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var simCard = await _context.SimCards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (simCard == null)
            {
                return NotFound();
            }

            return View(simCard);
        }

        // POST: SimCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var simCard = await _context.SimCards.FindAsync(id);
            _context.SimCards.Remove(simCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SimCardExists(int id)
        {
            return _context.SimCards.Any(e => e.Id == id);
        }
    }
}
