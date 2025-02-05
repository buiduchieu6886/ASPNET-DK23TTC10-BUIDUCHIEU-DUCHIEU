using Microsoft.AspNetCore.Identity;

namespace AuctionSimWebsite.Models
{
    public class User : IdentityUser
    {
        public decimal Balance { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thêm thuộc tính Bids để liên kết với Bid (một User có thể có nhiều Bid)
        public ICollection<Bid> Bids { get; set; }
    }
}
