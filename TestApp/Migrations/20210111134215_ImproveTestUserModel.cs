using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApp.Migrations
{
    public partial class ImproveTestUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Users",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MotherId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SSN",
                table: "Users",
                type: "character varying(87)",
                maxLength: 87,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MotherId",
                table: "Users",
                column: "MotherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_MotherId",
                table: "Users",
                column: "MotherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_MotherId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_MotherId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MotherId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SSN",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "Surname");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                type: "character varying(109)",
                maxLength: 109,
                nullable: true);
        }
    }
}
