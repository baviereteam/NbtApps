using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McMerchants.Migrations
{
    /// <inheritdoc />
    public partial class CreateBomTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bom",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "bom_item",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    RequiredQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    BomId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bom_item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bom_item_bom_BomId",
                        column: x => x.BomId,
                        principalTable: "bom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bom_item_BomId",
                table: "bom_item",
                column: "BomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bom_item");

            migrationBuilder.DropTable(
                name: "bom");
        }
    }
}
