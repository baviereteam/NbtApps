using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace McMerchants.Migrations
{
    /// <inheritdoc />
    public partial class AddItemProviderUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_default_alley_alleys_AlleyId",
                table: "default_alley");

            migrationBuilder.DropForeignKey(
                name: "FK_factory_products_item_provider_regions_FactoryId",
                table: "factory_products");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "item_provider_regions",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "dimension",
                table: "item_provider_regions",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "URL",
                table: "item_provider_regions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Item",
                table: "factory_products",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FactoryId",
                table: "factory_products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Item",
                table: "default_alley",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AlleyId",
                table: "default_alley",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "alleys",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_default_alley_alleys_AlleyId",
                table: "default_alley",
                column: "AlleyId",
                principalTable: "alleys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_factory_products_item_provider_regions_FactoryId",
                table: "factory_products",
                column: "FactoryId",
                principalTable: "item_provider_regions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_default_alley_alleys_AlleyId",
                table: "default_alley");

            migrationBuilder.DropForeignKey(
                name: "FK_factory_products_item_provider_regions_FactoryId",
                table: "factory_products");

            migrationBuilder.DropColumn(
                name: "URL",
                table: "item_provider_regions");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "item_provider_regions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "dimension",
                table: "item_provider_regions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Item",
                table: "factory_products",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "FactoryId",
                table: "factory_products",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Item",
                table: "default_alley",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "AlleyId",
                table: "default_alley",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "alleys",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_default_alley_alleys_AlleyId",
                table: "default_alley",
                column: "AlleyId",
                principalTable: "alleys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_factory_products_item_provider_regions_FactoryId",
                table: "factory_products",
                column: "FactoryId",
                principalTable: "item_provider_regions",
                principalColumn: "id");
        }
    }
}
