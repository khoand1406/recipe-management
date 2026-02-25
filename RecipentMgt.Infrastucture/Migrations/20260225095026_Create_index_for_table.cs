using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipentMgt.Infrastucture.Migrations
{
    /// <inheritdoc />
    public partial class Create_index_for_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_User_Session_Time", 
                table: "UserActivityLogs",
                columns: new[] { "UserId", "SessionId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserActivityLogs_User_Session_Time",
                table: "UserActivityLogs");
        }
    }
}
