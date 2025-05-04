using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationTypeEntity_NotificationTypeId",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationTypeEntity",
                table: "NotificationTypeEntity");

            migrationBuilder.RenameTable(
                name: "NotificationTypeEntity",
                newName: "NotificationTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationTypes",
                table: "NotificationTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationTypes_NotificationTypeId",
                table: "Notifications",
                column: "NotificationTypeId",
                principalTable: "NotificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationTypes_NotificationTypeId",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationTypes",
                table: "NotificationTypes");

            migrationBuilder.RenameTable(
                name: "NotificationTypes",
                newName: "NotificationTypeEntity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationTypeEntity",
                table: "NotificationTypeEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationTypeEntity_NotificationTypeId",
                table: "Notifications",
                column: "NotificationTypeId",
                principalTable: "NotificationTypeEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
