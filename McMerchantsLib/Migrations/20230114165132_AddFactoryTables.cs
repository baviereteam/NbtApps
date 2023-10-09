using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McMerchants.Migrations
{
    /// <inheritdoc />
    public partial class AddFactoryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "factory_products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Item = table.Column<string>(type: "TEXT", nullable: true),
                    FactoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_factory_products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_factory_products_item_provider_regions_FactoryId",
                        column: x => x.FactoryId,
                        principalTable: "item_provider_regions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_factory_products_FactoryId",
                table: "factory_products",
                column: "FactoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "factory_products");
        }
    }
}
