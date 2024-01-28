using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    /// <inheritdoc />
    public partial class lmao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SuspendAdminId",
                table: "UserSellerProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Suspended",
                table: "UserSellerProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedDate",
                table: "UserSellerProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspendedReason",
                table: "UserSellerProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnsuspendDate",
                table: "UserSellerProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BanAdminId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Banned",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "BannedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannedReason",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspendAdminId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Suspended",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspendedReason",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnbanDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnsuspendDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuspendAdminId",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "Suspended",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "SuspendedDate",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "SuspendedReason",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "UnsuspendDate",
                table: "UserSellerProfiles");

            migrationBuilder.DropColumn(
                name: "BanAdminId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Banned",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannedReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuspendAdminId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Suspended",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuspendedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuspendedReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UnbanDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UnsuspendDate",
                table: "Users");
        }
    }
}
