using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipentMgt.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImageLinkk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Dishes_DishId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_DishId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "Images");

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<int>(
                name: "DishId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_DishId",
                table: "Images",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Dishes_DishId",
                table: "Images",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "DishId");
        }
    }
}
