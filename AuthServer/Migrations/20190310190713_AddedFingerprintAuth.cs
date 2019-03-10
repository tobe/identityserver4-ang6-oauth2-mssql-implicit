using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AuthServer.Migrations
{
    public partial class AddedFingerprintAuth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Nonce",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NonceExpiresAt",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PublicKey",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LoginChallenges",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: false),
                    Nonce = table.Column<byte[]>(nullable: true),
                    ParameterG = table.Column<string>(nullable: true),
                    ParameterP = table.Column<string>(nullable: true),
                    PrivateKey = table.Column<byte[]>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginChallenges", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginChallenges");

            migrationBuilder.DropColumn(
                name: "Nonce",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NonceExpiresAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PublicKey",
                table: "AspNetUsers");
        }
    }
}
