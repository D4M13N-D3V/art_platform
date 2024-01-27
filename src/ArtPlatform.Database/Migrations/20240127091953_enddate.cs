using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    /// <inheritdoc />
    public partial class enddate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "SellerServices",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SellerServiceOrders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "SellerServiceOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "SellerServiceOrders",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "SellerServiceOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrders_SellerId",
                table: "SellerServiceOrders",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerServiceOrders_UserSellerProfiles_SellerId",
                table: "SellerServiceOrders",
                column: "SellerId",
                principalTable: "UserSellerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerServiceOrders_UserSellerProfiles_SellerId",
                table: "SellerServiceOrders");

            migrationBuilder.DropIndex(
                name: "IX_SellerServiceOrders_SellerId",
                table: "SellerServiceOrders");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SellerServiceOrders");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "SellerServiceOrders");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "SellerServiceOrders");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "SellerServiceOrders");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "SellerServices",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
