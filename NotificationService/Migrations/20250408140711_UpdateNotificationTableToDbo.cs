using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationTableToDbo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "NotificationMessages",
                schema: "Administrator",
                newName: "NotificationMessages",
                newSchema: "dbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Administrator");

            migrationBuilder.RenameTable(
                name: "NotificationMessages",
                schema: "dbo",
                newName: "NotificationMessages",
                newSchema: "Administrator");
        }
    }
}
