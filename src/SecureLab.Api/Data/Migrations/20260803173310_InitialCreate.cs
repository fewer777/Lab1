using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureLab.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "study_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    display_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "incidents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    severity = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incidents", x => x.id);
                    table.ForeignKey(
                        name: "FK_incidents_study_users_owner_user_id",
                        column: x => x.owner_user_id,
                        principalTable: "study_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "incident_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    incident_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    is_internal = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incident_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_incident_comments_incidents_incident_id",
                        column: x => x.incident_id,
                        principalTable: "incidents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_incident_comments_study_users_author_user_id",
                        column: x => x.author_user_id,
                        principalTable: "study_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "incident_status_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    incident_id = table.Column<Guid>(type: "uuid", nullable: false),
                    changed_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    old_status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    new_status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incident_status_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_incident_status_history_incidents_incident_id",
                        column: x => x.incident_id,
                        principalTable: "incidents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_incident_status_history_study_users_changed_by_user_id",
                        column: x => x.changed_by_user_id,
                        principalTable: "study_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_incident_comments_author_user_id",
                table: "incident_comments",
                column: "author_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_incident_comments_incident_id",
                table: "incident_comments",
                column: "incident_id");

            migrationBuilder.CreateIndex(
                name: "IX_incident_status_history_changed_by_user_id",
                table: "incident_status_history",
                column: "changed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_incident_status_history_incident_id",
                table: "incident_status_history",
                column: "incident_id");

            migrationBuilder.CreateIndex(
                name: "IX_incidents_owner_user_id",
                table: "incidents",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_incidents_severity",
                table: "incidents",
                column: "severity");

            migrationBuilder.CreateIndex(
                name: "IX_incidents_status",
                table: "incidents",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_study_users_email",
                table: "study_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_study_users_user_name",
                table: "study_users",
                column: "user_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "incident_comments");

            migrationBuilder.DropTable(
                name: "incident_status_history");

            migrationBuilder.DropTable(
                name: "incidents");

            migrationBuilder.DropTable(
                name: "study_users");
        }
    }
}
