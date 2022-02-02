using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectBot.DAL.Migrations
{
    public partial class mig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountUsers",
                table: "Works",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountUsers",
                table: "Works");
        }
    }
}
