using Microsoft.EntityFrameworkCore.Migrations;

namespace TestApp.Migrations
{
    public partial class IncreaseLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SSN",
                table: "Users",
                type: "character varying(90)",
                maxLength: 90,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(87)",
                oldMaxLength: 87,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SSN",
                table: "Users",
                type: "character varying(87)",
                maxLength: 87,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(90)",
                oldMaxLength: 90,
                oldNullable: true);
        }
    }
}
