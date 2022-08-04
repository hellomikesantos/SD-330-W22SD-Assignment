using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_330_W22SD_Assignment.Data.Migrations
{
    public partial class AddTagsICollectionInQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "Tag",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_QuestionId",
                table: "Tag",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Question_QuestionId",
                table: "Tag",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Question_QuestionId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_QuestionId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "Tag");
        }
    }
}
