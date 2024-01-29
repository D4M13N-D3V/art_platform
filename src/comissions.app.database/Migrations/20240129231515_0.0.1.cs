using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace comissions.app.database.Migrations
{
    /// <inheritdoc />
    public partial class _001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Biography = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    UserSellerProfileId = table.Column<int>(type: "integer", nullable: true),
                    Banned = table.Column<bool>(type: "boolean", nullable: false),
                    BannedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnbanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BannedReason = table.Column<string>(type: "text", nullable: true),
                    BanAdminId = table.Column<string>(type: "text", nullable: true),
                    Suspended = table.Column<bool>(type: "boolean", nullable: false),
                    SuspendedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnsuspendDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SuspendedReason = table.Column<string>(type: "text", nullable: true),
                    SuspendAdminId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SellerProfileRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcceptedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Accepted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerProfileRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerProfileRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSellerProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Biography = table.Column<string>(type: "text", nullable: false),
                    SocialMediaLinks = table.Column<List<string>>(type: "text[]", nullable: false),
                    AgeRestricted = table.Column<bool>(type: "boolean", nullable: false),
                    StripeAccountId = table.Column<string>(type: "text", nullable: true),
                    PrepaymentRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Suspended = table.Column<bool>(type: "boolean", nullable: false),
                    SuspendedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnsuspendDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SuspendedReason = table.Column<string>(type: "text", nullable: true),
                    SuspendAdminId = table.Column<string>(type: "text", nullable: true)
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
                name: "SellerServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerProfileId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Archived = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "SellerProfilePortfolioPieces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SellerProfileId = table.Column<int>(type: "integer", nullable: false),
                    FileReference = table.Column<string>(type: "text", nullable: false),
                    SellerServiceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerProfilePortfolioPieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerProfilePortfolioPieces_SellerServices_SellerServiceId",
                        column: x => x.SellerServiceId,
                        principalTable: "SellerServices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SellerProfilePortfolioPieces_UserSellerProfiles_SellerProfi~",
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
                    SellerId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TermsAcceptedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentUrl = table.Column<string>(type: "text", nullable: true)
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
                        name: "FK_SellerServiceOrders_UserSellerProfiles_SellerId",
                        column: x => x.SellerId,
                        principalTable: "UserSellerProfiles",
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
                    SellerServiceId = table.Column<int>(type: "integer", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                        name: "FK_SellerServiceOrderReviews_SellerServices_SellerServiceId",
                        column: x => x.SellerServiceId,
                        principalTable: "SellerServices",
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
                name: "IX_SellerProfilePortfolioPieces_SellerProfileId",
                table: "SellerProfilePortfolioPieces",
                column: "SellerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerProfilePortfolioPieces_SellerServiceId",
                table: "SellerProfilePortfolioPieces",
                column: "SellerServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerProfileRequests_UserId",
                table: "SellerProfileRequests",
                column: "UserId");

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
                name: "IX_SellerServiceOrderReviews_SellerServiceId",
                table: "SellerServiceOrderReviews",
                column: "SellerServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrderReviews_SellerServiceOrderId",
                table: "SellerServiceOrderReviews",
                column: "SellerServiceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrders_BuyerId",
                table: "SellerServiceOrders",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerServiceOrders_SellerId",
                table: "SellerServiceOrders",
                column: "SellerId");

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
                name: "SellerProfilePortfolioPieces");

            migrationBuilder.DropTable(
                name: "SellerProfileRequests");

            migrationBuilder.DropTable(
                name: "SellerServiceOrderMessageAttachments");

            migrationBuilder.DropTable(
                name: "SellerServiceOrderReviews");

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
