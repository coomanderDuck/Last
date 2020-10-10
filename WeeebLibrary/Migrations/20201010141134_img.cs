using Microsoft.EntityFrameworkCore.Migrations;

namespace WeeebLibrary.Migrations
{
    public partial class img : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "img",
                table: "Book",
                newName: "Img");

            migrationBuilder.AddColumn<string>(
                name: "ImgPath",
                table: "Book",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgPath",
                table: "Book");

            migrationBuilder.RenameColumn(
                name: "Img",
                table: "Book",
                newName: "img");
        }
    }
}
