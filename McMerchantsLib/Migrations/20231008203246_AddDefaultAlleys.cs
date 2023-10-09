using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McMerchants.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAlleys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "default_alley",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Item = table.Column<string>(type: "TEXT", nullable: true),
                    AlleyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_default_alley", x => x.Id);
                    table.ForeignKey(
                        name: "FK_default_alley_alleys_AlleyId",
                        column: x => x.AlleyId,
                        principalTable: "alleys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_default_alley_AlleyId",
                table: "default_alley",
                column: "AlleyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "default_alley");
        }
    }
}
