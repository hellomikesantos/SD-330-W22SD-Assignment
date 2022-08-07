using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_330_W22SD_Assignment.Data.Migrations
{
    public partial class SetForeignKeyInCorrectAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CorrectAnswer",
                table: "CorrectAnswer");

            migrationBuilder.DropIndex(
                name: "IX_CorrectAnswer_QuestionId",
                table: "CorrectAnswer");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CorrectAnswer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CorrectAnswer",
                table: "CorrectAnswer",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CorrectAnswer",
                table: "CorrectAnswer");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CorrectAnswer",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CorrectAnswer",
                table: "CorrectAnswer",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CorrectAnswer_QuestionId",
                table: "CorrectAnswer",
                column: "QuestionId",
                unique: true);
        }
    }
}
