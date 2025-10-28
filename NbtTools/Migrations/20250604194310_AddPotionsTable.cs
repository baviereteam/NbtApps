using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NbtTools.Migrations
{
    /// <inheritdoc />
    public partial class AddPotionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "potions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    StackSize = table.Column<byte>(type: "INTEGER", nullable: false),
                    PotionContents = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_potions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_potions_Name",
                table: "potions",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "potions");
        }
    }
}
