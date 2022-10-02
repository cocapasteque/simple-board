using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleBoard.Migrations
{
    public partial class addedTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Time",
                table: "Entries",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Entries");
        }
    }
}
