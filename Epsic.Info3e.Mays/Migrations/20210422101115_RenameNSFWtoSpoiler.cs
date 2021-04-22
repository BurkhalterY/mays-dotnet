using Microsoft.EntityFrameworkCore.Migrations;

namespace Epsic.Info3e.Mays.Migrations
{
    public partial class RenameNSFWtoSpoiler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsNSFW",
                table: "Posts",
                newName: "IsSpoiler");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSpoiler",
                table: "Posts",
                newName: "IsNSFW");
        }
    }
}
