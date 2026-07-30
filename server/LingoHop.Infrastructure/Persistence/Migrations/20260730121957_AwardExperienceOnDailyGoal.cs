using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingoHop.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AwardExperienceOnDailyGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "daily_progress_pending_experience",
                schema: "lingohop",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "daily_progress_pending_experience",
                schema: "lingohop",
                table: "users");
        }
    }
}
