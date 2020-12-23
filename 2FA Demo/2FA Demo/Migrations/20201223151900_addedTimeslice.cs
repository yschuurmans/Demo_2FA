using Microsoft.EntityFrameworkCore.Migrations;

namespace _2FA_Demo.Migrations
{
    public partial class addedTimeslice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Timeslice",
                table: "Accounts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timeslice",
                table: "Accounts");
        }
    }
}
