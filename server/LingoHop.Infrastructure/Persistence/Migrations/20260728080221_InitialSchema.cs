using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LingoHop.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "lingohop");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "lingohop",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    experience = table.Column<int>(type: "integer", nullable: false),
                    daily_goal_cards = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    daily_progress_cards_reviewed = table.Column<int>(type: "integer", nullable: false),
                    daily_progress_date = table.Column<DateOnly>(type: "date", nullable: true),
                    first_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    language_code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    last_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    username = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    streak_current = table.Column<int>(type: "integer", nullable: false),
                    streak_last_studied_on = table.Column<DateOnly>(type: "date", nullable: true),
                    streak_longest = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "decks",
                schema: "lingohop",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    icon = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_decks", x => x.id);
                    table.ForeignKey(
                        name: "FK_decks_users_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "lingohop",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cards",
                schema: "lingohop",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    deck_id = table.Column<Guid>(type: "uuid", nullable: false),
                    term = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    translation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    part_of_speech = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    example = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    correct_streak = table.Column<int>(type: "integer", nullable: false),
                    last_reviewed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    times_known = table.Column<int>(type: "integer", nullable: false),
                    times_seen = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cards", x => x.id);
                    table.ForeignKey(
                        name: "FK_cards_decks_deck_id",
                        column: x => x.deck_id,
                        principalSchema: "lingohop",
                        principalTable: "decks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "study_sessions",
                schema: "lingohop",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deck_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    started_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    experience_earned = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_study_sessions_decks_deck_id",
                        column: x => x.deck_id,
                        principalSchema: "lingohop",
                        principalTable: "decks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_study_sessions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "lingohop",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session_cards",
                schema: "lingohop",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    known = table.Column<bool>(type: "boolean", nullable: true),
                    answered_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session_cards", x => x.id);
                    table.ForeignKey(
                        name: "FK_session_cards_study_sessions_session_id",
                        column: x => x.session_id,
                        principalSchema: "lingohop",
                        principalTable: "study_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cards_deck_id_term",
                schema: "lingohop",
                table: "cards",
                columns: new[] { "deck_id", "term" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_decks_owner_id",
                schema: "lingohop",
                table: "decks",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_cards_session_id_card_id",
                schema: "lingohop",
                table: "session_cards",
                columns: new[] { "session_id", "card_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_session_cards_session_id_position",
                schema: "lingohop",
                table: "session_cards",
                columns: new[] { "session_id", "position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_study_sessions_deck_id",
                schema: "lingohop",
                table: "study_sessions",
                column: "deck_id");

            migrationBuilder.CreateIndex(
                name: "ix_study_sessions_user_id_deck_id_status",
                schema: "lingohop",
                table: "study_sessions",
                columns: new[] { "user_id", "deck_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_users_telegram_id",
                schema: "lingohop",
                table: "users",
                column: "telegram_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cards",
                schema: "lingohop");

            migrationBuilder.DropTable(
                name: "session_cards",
                schema: "lingohop");

            migrationBuilder.DropTable(
                name: "study_sessions",
                schema: "lingohop");

            migrationBuilder.DropTable(
                name: "decks",
                schema: "lingohop");

            migrationBuilder.DropTable(
                name: "users",
                schema: "lingohop");
        }
    }
}
