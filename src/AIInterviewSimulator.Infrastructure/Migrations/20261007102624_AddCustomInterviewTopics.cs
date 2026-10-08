using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIInterviewSimulator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomInterviewTopics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SessionTopics_SessionId_Topic",
                table: "SessionTopics");

            migrationBuilder.AlterColumn<int>(
                name: "Topic",
                table: "SessionTopics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CustomTopic",
                table: "SessionTopics",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Topic",
                table: "InterviewQuestions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "CustomTopic",
                table: "InterviewQuestions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionTopics_SessionId_CustomTopic",
                table: "SessionTopics",
                columns: new[] { "SessionId", "CustomTopic" },
                unique: true,
                filter: "[CustomTopic] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SessionTopics_SessionId_Topic",
                table: "SessionTopics",
                columns: new[] { "SessionId", "Topic" },
                unique: true,
                filter: "[Topic] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SessionTopics_SessionId_CustomTopic",
                table: "SessionTopics");

            migrationBuilder.DropIndex(
                name: "IX_SessionTopics_SessionId_Topic",
                table: "SessionTopics");

            migrationBuilder.DropColumn(
                name: "CustomTopic",
                table: "SessionTopics");

            migrationBuilder.DropColumn(
                name: "CustomTopic",
                table: "InterviewQuestions");

            migrationBuilder.AlterColumn<int>(
                name: "Topic",
                table: "SessionTopics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Topic",
                table: "InterviewQuestions",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionTopics_SessionId_Topic",
                table: "SessionTopics",
                columns: new[] { "SessionId", "Topic" },
                unique: true);
        }
    }
}
