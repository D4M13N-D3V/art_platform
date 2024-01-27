using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SellerSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DefaultValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Biography = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    UserSellerProfileId = table.Column<int>(type: "integer", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    AddressCountry = table.Column<string>(type: "text", nullable: false),
                    AddressCity = table.Column<string>(type: "text", nullable: false),
                    AddressStreet = table.Column<string>(type: "text", nullable: false),
                    AddressHouseNumber = table.Column<string>(type: "text", nullable: false),
                    AddressPostalCode = table.Column<string>(type: "text", nullable: false),
                    AddressRegion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSellerProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    AgeRestricted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSellerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSellerProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellerConfiguredSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserSellerProfileId = table.Column<int>(type: "integer", nullable: false),
                    UserSellerSettingId = table.Column<int>(type: "integer", nullable: false),
                    SellerSettingId = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "SellerProfilePortfolioPieces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerProfileId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FileReference = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerProfilePortfolioPieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerProfilePortfolioPieces_UserSellerProfiles_SellerProfi~",
                        column: x => x.SellerProfileId,
                        principalTable: "UserSellerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellerServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerProfileId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    DepositRequired = table.Column<bool>(type: "boolean", nullable: false),
                    DepositAmount = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerServices_UserSellerProfiles_SellerProfileId",
                        column: x => x.SellerProfileId,
                        principalTable: "UserSellerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellerServiceOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BuyerId = table.Column<string>(type: "text", nullable: false),
                    SellerServiceId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerServiceOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerServiceOrders_SellerServices_SellerServiceId",
                        column: x => x.SellerServiceId,
                        principalTable: "SellerServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellerServiceOrders_Users_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellerServiceOrderMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerServiceOrderId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerServiceOrderMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerServiceOrderMessages_SellerServiceOrders_SellerServic~",
                        column: x => x.SellerServiceOrderId,
                        principalTable: "SellerServiceOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellerServiceOrderMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellerServiceOrderReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewerId = table.Column<string>(type: "text", nullable: false),
                    SellerServiceOrderId = table.Column<int>(type: "integer", nullable: false),
                    Review = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerServiceOrderReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerServiceOrderReviews_SellerServiceOrders_SellerService~",
                        column: x => x.SellerServiceOrderId,
                        principalTable: "SellerServiceOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellerServiceOrderReviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellerServiceOrderMessageAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerServiceOrderMessageId = table.Column<int>(type: "integer", nullable: false),
                    FileReference = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerServiceOrderMessageAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerServiceOrderMessageAttachments_SellerServiceOrderMess~",
                        column: x => x.SellerServiceOrderMessageId,
                        principalTable: "SellerServiceOrderMessages",
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

            migrationBuilder.CreateIndex(
                name: "IX_SellerProfilePortfolioPieces_SellerProfileId",
                table: "SellerProfilePortfolioPieces",
                column: "SellerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrderMessageAttachments_SellerServiceOrderMess~",
                table: "SellerServiceOrderMessageAttachments",
                column: "SellerServiceOrderMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrderMessages_SellerServiceOrderId",
                table: "SellerServiceOrderMessages",
                column: "SellerServiceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrderMessages_SenderId",
                table: "SellerServiceOrderMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrderReviews_ReviewerId",
                table: "SellerServiceOrderReviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrderReviews_SellerServiceOrderId",
                table: "SellerServiceOrderReviews",
                column: "SellerServiceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrders_BuyerId",
                table: "SellerServiceOrders",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrders_SellerServiceId",
                table: "SellerServiceOrders",
                column: "SellerServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServices_SellerProfileId",
                table: "SellerServices",
                column: "SellerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSellerProfiles_UserId",
                table: "UserSellerProfiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellerConfiguredSettings");

            migrationBuilder.DropTable(
                name: "SellerProfilePortfolioPieces");

            migrationBuilder.DropTable(
                name: "SellerServiceOrderMessageAttachments");

            migrationBuilder.DropTable(
                name: "SellerServiceOrderReviews");

            migrationBuilder.DropTable(
                name: "SellerSettings");

            migrationBuilder.DropTable(
                name: "SellerServiceOrderMessages");

            migrationBuilder.DropTable(
                name: "SellerServiceOrders");

            migrationBuilder.DropTable(
                name: "SellerServices");

            migrationBuilder.DropTable(
                name: "UserSellerProfiles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
