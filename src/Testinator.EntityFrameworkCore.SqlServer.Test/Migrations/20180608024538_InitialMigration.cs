using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Testinator.EntityFrameworkCore.SqlServer.Test.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Widgets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Widgets_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreWidget",
                columns: table => new
                {
                    StoreId = table.Column<int>(nullable: false),
                    WidgetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreWidget", x => new { x.StoreId, x.WidgetId });
                    table.ForeignKey(
                        name: "FK_StoreWidget_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreWidget_Widgets_WidgetId",
                        column: x => x.WidgetId,
                        principalTable: "Widgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "Blue Widgets" },
                    { 2, "Red Widgets" },
                    { 3, "Other Widgets" }
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Address", "Name" },
                values: new object[,]
                {
                    { 1, "1 Credibility Street, Upper Whingeing", "The Widget Haus" },
                    { 2, "On every street corner", "Widgets R Us" },
                    { 3, "23 Red Street", "Red Widget Specialists" }
                });

            migrationBuilder.InsertData(
                table: "Widgets",
                columns: new[] { "Id", "CategoryId", "Description", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Blue with Spots", 1.0m },
                    { 2, 1, "Blue with Lines", 1.0m },
                    { 3, 2, "Red all over", 1.0m },
                    { 4, 3, "Green and Purple", 1.0m }
                });

            migrationBuilder.InsertData(
                table: "StoreWidget",
                columns: new[] { "StoreId", "WidgetId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 2, 3 },
                    { 2, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreWidget_WidgetId",
                table: "StoreWidget",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_CategoryId",
                table: "Widgets",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreWidget");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Widgets");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
