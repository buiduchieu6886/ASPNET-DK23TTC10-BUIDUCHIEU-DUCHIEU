namespace AuctionSimWebsite.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int? AuctionId { get; set; }
        public Auction Auction { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionTime { get; set; }
        public string TransactionType { get; set; }
    }
}
