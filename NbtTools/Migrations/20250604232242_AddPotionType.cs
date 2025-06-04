using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NbtTools.Migrations
{
    /// <inheritdoc />
    public partial class AddPotionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypeId",
                table: "potions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_potions_TypeId",
                table: "potions",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_potions_items_TypeId",
                table: "potions",
                column: "TypeId",
                principalTable: "items",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_potions_items_TypeId",
                table: "potions");

            migrationBuilder.DropIndex(
                name: "IX_potions_TypeId",
                table: "potions");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "potions");
        }
    }
}
