namespace AuctionSimWebsite.Models
{
    public class SimCard
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Carrier { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
