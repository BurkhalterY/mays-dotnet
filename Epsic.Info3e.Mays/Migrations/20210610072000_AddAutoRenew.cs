using Microsoft.EntityFrameworkCore.Migrations;

namespace Epsic.Info3e.Mays.Migrations
{
    public partial class AddAutoRenew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoRenew",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoRenew",
                table: "AspNetUsers");
        }
    }
}
