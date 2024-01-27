using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    /// <inheritdoc />
    public partial class updatess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SellerServiceId",
                table: "SellerServiceOrderReviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrderReviews_SellerServiceId",
                table: "SellerServiceOrderReviews",
                column: "SellerServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerServiceOrderReviews_SellerServices_SellerServiceId",
                table: "SellerServiceOrderReviews",
                column: "SellerServiceId",
                principalTable: "SellerServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerServiceOrderReviews_SellerServices_SellerServiceId",
                table: "SellerServiceOrderReviews");

            migrationBuilder.DropIndex(
                name: "IX_SellerServiceOrderReviews_SellerServiceId",
                table: "SellerServiceOrderReviews");

            migrationBuilder.DropColumn(
                name: "SellerServiceId",
                table: "SellerServiceOrderReviews");
        }
    }
}
