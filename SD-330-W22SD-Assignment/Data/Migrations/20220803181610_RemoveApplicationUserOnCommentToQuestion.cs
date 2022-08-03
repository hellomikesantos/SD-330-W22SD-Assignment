using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_330_W22SD_Assignment.Data.Migrations
{
    public partial class RemoveApplicationUserOnCommentToQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentToQuestion_AspNetUsers_UserId1",
                table: "CommentToQuestion");

            migrationBuilder.DropIndex(
                name: "IX_CommentToQuestion_UserId1",
                table: "CommentToQuestion");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "CommentToQuestion");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CommentToQuestion",
                newName: "UserIdInt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserIdInt",
                table: "CommentToQuestion",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "CommentToQuestion",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentToQuestion_UserId1",
                table: "CommentToQuestion",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentToQuestion_AspNetUsers_UserId1",
                table: "CommentToQuestion",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
