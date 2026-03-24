using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipentMgt.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class ModifyDishTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirm",
                table: "Dishes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirm",
                table: "Dishes");
        }
    }
}
