using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipentMgt.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class Add_Dish_Creator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Dishes",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Users_AuthorId",
                table: "Dishes",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_AuthorId",
                table: "Dishes",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Users_AuthorId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_AuthorId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Dishes");
        }
    }
}
