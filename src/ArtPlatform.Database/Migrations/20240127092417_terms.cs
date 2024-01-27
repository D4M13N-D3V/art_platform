using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    /// <inheritdoc />
    public partial class terms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TermsAcceptedDate",
                table: "SellerServiceOrders",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermsAcceptedDate",
                table: "SellerServiceOrders");
        }
    }
}
