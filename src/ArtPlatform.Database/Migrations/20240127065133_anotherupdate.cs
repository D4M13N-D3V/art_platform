using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    /// <inheritdoc />
    public partial class anotherupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellerConfiguredSettings");

            migrationBuilder.DropTable(
                name: "SellerSettings");

            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "UserSellerProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "SocialMediaLinks",
                table: "UserSellerProfiles",
                type: "text[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Biography",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "SocialMediaLinks",
                table: "UserSellerProfiles");

            migrationBuilder.CreateTable(
                name: "SellerSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DefaultValue = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SellerConfiguredSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerSettingId = table.Column<int>(type: "integer", nullable: false),
                    UserSellerProfileId = table.Column<int>(type: "integer", nullable: false),
                    UserSellerSettingId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerConfiguredSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerConfiguredSettings_SellerSettings_SellerSettingId",
                        column: x => x.SellerSettingId,
                        principalTable: "SellerSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellerConfiguredSettings_UserSellerProfiles_UserSellerProfi~",
                        column: x => x.UserSellerProfileId,
                        principalTable: "UserSellerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellerConfiguredSettings_SellerSettingId",
                table: "SellerConfiguredSettings",
                column: "SellerSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerConfiguredSettings_UserSellerProfileId",
                table: "SellerConfiguredSettings",
                column: "UserSellerProfileId");
        }
    }
}
