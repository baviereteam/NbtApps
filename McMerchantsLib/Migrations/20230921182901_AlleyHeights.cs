using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McMerchants.Migrations
{
    /// <inheritdoc />
    public partial class AlleyHeights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndY",
                table: "alleys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartY",
                table: "alleys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndY",
                table: "alleys");

            migrationBuilder.DropColumn(
                name: "StartY",
                table: "alleys");
        }
    }
}
