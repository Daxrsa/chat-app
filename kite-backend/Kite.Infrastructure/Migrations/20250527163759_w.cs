using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class w : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequest_AspNetUsers_ReceiverId",
                schema: "kite",
                table: "FriendRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequest_AspNetUsers_SenderId",
                schema: "kite",
                table: "FriendRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_FriendRequest_FriendRequestId",
                schema: "kite",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_ApplicationUserId",
                schema: "kite",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_ReceiverId",
                schema: "kite",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_SenderId",
                schema: "kite",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                schema: "kite",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendship",
                schema: "kite",
                table: "Friendship");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendRequest",
                schema: "kite",
                table: "FriendRequest");

            migrationBuilder.RenameTable(
                name: "Notification",
                schema: "kite",
                newName: "Notifications",
                newSchema: "kite");

            migrationBuilder.RenameTable(
                name: "Friendship",
                schema: "kite",
                newName: "Friendships",
                newSchema: "kite");

            migrationBuilder.RenameTable(
                name: "FriendRequest",
                schema: "kite",
                newName: "FriendRequests",
                newSchema: "kite");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_SenderId",
                schema: "kite",
                table: "Notifications",
                newName: "IX_Notifications_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_ReceiverId",
                schema: "kite",
                table: "Notifications",
                newName: "IX_Notifications_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_ApplicationUserId",
                schema: "kite",
                table: "Notifications",
                newName: "IX_Notifications_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_FriendRequestId",
                schema: "kite",
                table: "Friendships",
                newName: "IX_Friendships_FriendRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequest_SenderId",
                schema: "kite",
                table: "FriendRequests",
                newName: "IX_FriendRequests_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequest_ReceiverId",
                schema: "kite",
                table: "FriendRequests",
                newName: "IX_FriendRequests_ReceiverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                schema: "kite",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendships",
                schema: "kite",
                table: "Friendships",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendRequests",
                schema: "kite",
                table: "FriendRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_AspNetUsers_ReceiverId",
                schema: "kite",
                table: "FriendRequests",
                column: "ReceiverId",
                principalSchema: "kite",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_AspNetUsers_SenderId",
                schema: "kite",
                table: "FriendRequests",
                column: "SenderId",
                principalSchema: "kite",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_FriendRequests_FriendRequestId",
                schema: "kite",
                table: "Friendships",
                column: "FriendRequestId",
                principalSchema: "kite",
                principalTable: "FriendRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_ApplicationUserId",
                schema: "kite",
                table: "Notifications",
                column: "ApplicationUserId",
                principalSchema: "kite",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_ReceiverId",
                schema: "kite",
                table: "Notifications",
                column: "ReceiverId",
                principalSchema: "kite",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderId",
                schema: "kite",
                table: "Notifications",
                column: "SenderId",
                principalSchema: "kite",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_AspNetUsers_ReceiverId",
                schema: "kite",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_AspNetUsers_SenderId",
                schema: "kite",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_FriendRequests_FriendRequestId",
                schema: "kite",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_ApplicationUserId",
                schema: "kite",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_ReceiverId",
                schema: "kite",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderId",
                schema: "kite",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                schema: "kite",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendships",
                schema: "kite",
                table: "Friendships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendRequests",
                schema: "kite",
                table: "FriendRequests");

            migrationBuilder.RenameTable(
                name: "Notifications",
                schema: "kite",
                newName: "Notification",
                newSchema: "kite");

            migrationBuilder.RenameTable(
                name: "Friendships",
                schema: "kite",
                newName: "Friendship",
                newSchema: "kite");

            migrationBuilder.RenameTable(
                name: "FriendRequests",
                schema: "kite",
                newName: "FriendRequest",
                newSchema: "kite");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_SenderId",
                schema: "kite",
                table: "Notification",
                newName: "IX_Notification_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ReceiverId",
                schema: "kite",
                table: "Notification",
                newName: "IX_Notification_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ApplicationUserId",
                schema: "kite",
                table: "Notification",
                newName: "IX_Notification_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_FriendRequestId",
                schema: "kite",
                table: "Friendship",
                newName: "IX_Friendship_FriendRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequests_SenderId",
                schema: "kite",
                table: "FriendRequest",
                newName: "IX_FriendRequest_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequests_ReceiverId",
                schema: "kite",
                table: "FriendRequest",
                newName: "IX_FriendRequest_ReceiverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                schema: "kite",
                table: "Notification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendship",
                schema: "kite",
                table: "Friendship",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendRequest",
                schema: "kite",
                table: "FriendRequest",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequest_AspNetUsers_ReceiverId",
                schema: "kite",
                table: "FriendRequest",
                column: "ReceiverId",
                principalSchema: "kite",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequest_AspNetUsers_SenderId",
                schema: "kite",
                table: "FriendRequest",
                column: "SenderId",
                principalSchema: "kite",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_FriendRequest_FriendRequestId",
                schema: "kite",
                table: "Friendship",
                column: "FriendRequestId",
                principalSchema: "kite",
                principalTable: "FriendRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_ApplicationUserId",
                schema: "kite",
                table: "Notification",
                column: "ApplicationUserId",
                principalSchema: "kite",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_ReceiverId",
                schema: "kite",
                table: "Notification",
                column: "ReceiverId",
                principalSchema: "kite",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_SenderId",
                schema: "kite",
                table: "Notification",
                column: "SenderId",
                principalSchema: "kite",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
