using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_330_W22SD_Assignment.Data.Migrations
{
    public partial class AddApplicationUserAndUserIdStringOnCommentToQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CommentToQuestion",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentToQuestion_UserId",
                table: "CommentToQuestion",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentToQuestion_AspNetUsers_UserId",
                table: "CommentToQuestion",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentToQuestion_AspNetUsers_UserId",
                table: "CommentToQuestion");

            migrationBuilder.DropIndex(
                name: "IX_CommentToQuestion_UserId",
                table: "CommentToQuestion");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CommentToQuestion");
        }
    }
}
