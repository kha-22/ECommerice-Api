using Microsoft.EntityFrameworkCore.Migrations;

namespace ECommerice.Infrastructure.Migrations
{
    public partial class Create_DB2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Contactus");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Contactus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Contactus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Contactus",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
