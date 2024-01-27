using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    /// <inheritdoc />
    public partial class lmaomore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositAmount",
                table: "SellerServices");

            migrationBuilder.DropColumn(
                name: "DepositRequired",
                table: "SellerServices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DepositAmount",
                table: "SellerServices",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DepositRequired",
                table: "SellerServices",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
