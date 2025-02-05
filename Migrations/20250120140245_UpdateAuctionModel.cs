using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionSimWebsite.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuctionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BidCount",
                table: "Auctions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "StartingPrice",
                table: "Auctions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BidCount",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "StartingPrice",
                table: "Auctions");
        }
    }
}
