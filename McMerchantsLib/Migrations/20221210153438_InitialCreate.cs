using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McMerchants.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "item_provider_regions",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: true),
                    logo = table.Column<string>(type: "TEXT", nullable: true),
                    dimension = table.Column<string>(type: "TEXT", nullable: true),
                    startX = table.Column<int>(type: "INTEGER", nullable: false),
                    startY = table.Column<int>(type: "INTEGER", nullable: false),
                    startZ = table.Column<int>(type: "INTEGER", nullable: false),
                    endX = table.Column<int>(type: "INTEGER", nullable: false),
                    endY = table.Column<int>(type: "INTEGER", nullable: false),
                    endZ = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_provider_regions", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_provider_regions");
        }
    }
}
