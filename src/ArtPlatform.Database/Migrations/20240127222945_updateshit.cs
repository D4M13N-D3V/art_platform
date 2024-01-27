using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    /// <inheritdoc />
    public partial class updateshit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeAccountId",
                table: "UserSellerProfiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeAccountId",
                table: "UserSellerProfiles");
        }
    }
}
