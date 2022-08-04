using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_330_W22SD_Assignment.Data.Migrations
{
    public partial class RemoveQuestionICollectionPropsFromTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_Tag_TagId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Question_QuestionId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_QuestionId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Question_TagId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Question");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "Tag",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Question",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_QuestionId",
                table: "Tag",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_TagId",
                table: "Question",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Tag_TagId",
                table: "Question",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Question_QuestionId",
                table: "Tag",
                column: "QuestionId",
                principalTable: "Question",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
