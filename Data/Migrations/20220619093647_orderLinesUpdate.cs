using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uhrenWelt.Data.Migrations
{
    public partial class orderLinesUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NetUnitPrice",
                table: "OrderLines",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetUnitPrice",
                table: "OrderLines");
        }
    }
}
