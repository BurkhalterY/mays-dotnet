using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Epsic.Info3e.Mays.Migrations
{
    public partial class AddExpirationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "AspNetUsers");
        }
    }
}
