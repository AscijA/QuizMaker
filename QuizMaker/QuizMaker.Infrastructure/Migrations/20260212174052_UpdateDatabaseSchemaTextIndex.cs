using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizMaker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseSchemaTextIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Questions_Text",
                table: "Questions",
                column: "Text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questions_Text",
                table: "Questions");
        }
    }
}
