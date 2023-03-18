using Microsoft.EntityFrameworkCore.Migrations;

namespace register_app.Data.Migrations
{
    public partial class AddFormId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormId",
                table: "Events",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormId",
                table: "Events");
        }
    }
}
