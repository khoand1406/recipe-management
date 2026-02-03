using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipentMgt.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class ExpandDishRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "DishStatistics",
                columns: table => new
                {
                    DishId = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BookmarkCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RecipeCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishStatistics", x => x.DishId);
                    table.ForeignKey(
                        name: "FK_DishStatistics_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "DishId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RelatedDishes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DishId = table.Column<int>(type: "int", nullable: false),
                    RelatedDishId = table.Column<int>(type: "int", nullable: false),
                    RelationType = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedDishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatedDishes_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "DishId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RelatedDishes_Dishes_RelatedDishId",
                        column: x => x.RelatedDishId,
                        principalTable: "Dishes",
                        principalColumn: "DishId",
                        onDelete: ReferentialAction.Restrict);
                });


            migrationBuilder.CreateIndex(
                name: "IX_RelatedDishes_DishId_RelatedDishId",
                table: "RelatedDishes",
                columns: new[] { "DishId", "RelatedDishId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelatedDishes_RelatedDishId",
                table: "RelatedDishes",
                column: "RelatedDishId");

            

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            

            migrationBuilder.DropTable(
                name: "DishStatistics");

            migrationBuilder.DropTable(
                name: "RelatedDishes");

            

            migrationBuilder.DropIndex(
                name: "IX_Dishes_StatisticDishId",
                table: "Dishes");

           
            migrationBuilder.DropColumn(
                name: "StatisticDishId",
                table: "Dishes");
        }
    }
}
