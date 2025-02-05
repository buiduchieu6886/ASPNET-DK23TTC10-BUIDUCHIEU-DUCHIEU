using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuctionSimWebsite.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace AuctionSimWebsite.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<SimCard> SimCards { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mối quan hệ giữa Auction và Bid
            modelBuilder.Entity<Auction>()
                .HasMany(a => a.Bids)  // Auction có nhiều Bid
                .WithOne(b => b.Auction)  // Mỗi Bid thuộc về một Auction
                .HasForeignKey(b => b.AuctionId);  // Bid liên kết với Auction qua AuctionId

            // Mối quan hệ giữa User và Bid
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.UserId);
        }
    }
}
