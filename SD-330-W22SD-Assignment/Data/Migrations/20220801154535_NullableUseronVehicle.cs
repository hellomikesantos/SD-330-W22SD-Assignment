using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SD_330_W22SD_Assignment.Data.Migrations
{
    public partial class NullableUseronVehicle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_AspNetUsers_UserId",
                table: "Vehicle");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Vehicle",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_AspNetUsers_UserId",
                table: "Vehicle",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_AspNetUsers_UserId",
                table: "Vehicle");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Vehicle",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_AspNetUsers_UserId",
                table: "Vehicle",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
