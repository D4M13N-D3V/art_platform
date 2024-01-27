using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    /// <inheritdoc />
    public partial class removedumbshit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SellerProfilePortfolioPieces");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SellerProfilePortfolioPieces");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SellerProfilePortfolioPieces",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SellerProfilePortfolioPieces",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
