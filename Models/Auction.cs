using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AuctionSimWebsite.Models
{
    public class Auction
    {
        [Required(ErrorMessage = "Chọn một Sim Card.")]
        public int Id { get; set; }

        public int SimCardId { get; set; }
        public SimCard SimCard { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu không được bỏ trống.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc không được bỏ trống.")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Giá khởi điểm là bắt buộc.")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá khởi điểm phải lớn hơn 0.")]
        public decimal StartingPrice { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Giá hiện tại phải lớn hơn 0.")]
        public decimal HighestBid { get; set; } // Giá hiện tại, khởi tạo bằng StartingPrice

        [AllowNull]
        public int? WinnerId { get; set; }

        [AllowNull]
        [BindNever]
        public User Winner { get; set; }

        [Required]
        public string Status { get; set; } = "Active"; // Mặc định là "Active"

        // Đếm số lượng lượt đấu giá
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượt đấu giá phải không âm.")]
        public int BidCount { get; set; } = 0;

        // Thêm thuộc tính Bids để chứa danh sách các Bid
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();

        // Constructor khởi tạo giá đấu ban đầu
        public Auction()
        {
            HighestBid = StartingPrice; // Khi tạo phiên đấu giá, giá hiện tại là giá khởi điểm
        }
    }
}
