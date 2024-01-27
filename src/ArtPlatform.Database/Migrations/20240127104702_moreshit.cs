using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    /// <inheritdoc />
    public partial class moreshit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SellerServiceId",
                table: "SellerProfilePortfolioPieces",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SellerProfilePortfolioPieces_SellerServiceId",
                table: "SellerProfilePortfolioPieces",
                column: "SellerServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerProfilePortfolioPieces_SellerServices_SellerServiceId",
                table: "SellerProfilePortfolioPieces",
                column: "SellerServiceId",
                principalTable: "SellerServices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerProfilePortfolioPieces_SellerServices_SellerServiceId",
                table: "SellerProfilePortfolioPieces");

            migrationBuilder.DropIndex(
                name: "IX_SellerProfilePortfolioPieces_SellerServiceId",
                table: "SellerProfilePortfolioPieces");

            migrationBuilder.DropColumn(
                name: "SellerServiceId",
                table: "SellerProfilePortfolioPieces");
        }
    }
}
