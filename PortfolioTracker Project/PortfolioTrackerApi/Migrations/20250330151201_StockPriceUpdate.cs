using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class StockPriceUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "StocksPrice",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "StocksPrice");
        }
    }
}
