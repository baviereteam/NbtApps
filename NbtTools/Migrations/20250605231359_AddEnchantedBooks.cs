using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NbtTools.Migrations
{
    /// <inheritdoc />
    public partial class AddEnchantedBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "enchanted_books",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    StackSize = table.Column<byte>(type: "INTEGER", nullable: false),
                    Enchantment = table.Column<string>(type: "TEXT", nullable: true),
                    Level = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enchanted_books", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_enchanted_books_Name",
                table: "enchanted_books",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "enchanted_books");
        }
    }
}
