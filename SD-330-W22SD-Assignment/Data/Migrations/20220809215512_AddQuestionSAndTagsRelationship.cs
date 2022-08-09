using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_330_W22SD_Assignment.Data.Migrations
{
    public partial class AddQuestionSAndTagsRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Tag_Question_QuestionId",
            //    table: "Tag");

            //migrationBuilder.DropIndex(
            //    name: "IX_Tag_QuestionId",
            //    table: "Tag");

            //migrationBuilder.DropColumn(
            //    name: "QuestionId",
            //    table: "Tag");

            migrationBuilder.CreateTable(
                name: "QuestionTag",
                columns: table => new
                {
                    QuestionsId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTag", x => new { x.QuestionsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_QuestionTag_Question_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Question",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionTag_Tag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTag_TagsId",
                table: "QuestionTag",
                column: "TagsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionTag");

            //migrationBuilder.AddColumn<int>(
            //    name: "QuestionId",
            //    table: "Tag",
            //    type: "int",
            //    nullable: true);

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
    }
}
