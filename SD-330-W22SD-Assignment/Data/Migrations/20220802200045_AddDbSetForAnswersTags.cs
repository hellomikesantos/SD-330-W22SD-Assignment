using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_330_W22SD_Assignment.Data.Migrations
{
    public partial class AddDbSetForAnswersTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTag_Question_QuestionId",
                table: "QuestionTag");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "QuestionTag",
                newName: "QuestionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTag_Question_QuestionsId",
                table: "QuestionTag",
                column: "QuestionsId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTag_Question_QuestionsId",
                table: "QuestionTag");

            migrationBuilder.RenameColumn(
                name: "QuestionsId",
                table: "QuestionTag",
                newName: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTag_Question_QuestionId",
                table: "QuestionTag",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
