using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaqAssistant.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdatedRatingToSeperateEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Rating",
            table: "Faqs");

        migrationBuilder.CreateTable(
            name: "Ratings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                FaqId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                IsUpvote = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Ratings", x => x.Id);
                table.ForeignKey(
                    name: "FK_Ratings_Faqs_FaqId",
                    column: x => x.FaqId,
                    principalTable: "Faqs",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Ratings_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Rating_FaqId_UserId",
            table: "Ratings",
            columns: new[] { "FaqId", "UserId" });

        migrationBuilder.CreateIndex(
            name: "IX_Ratings_UserId",
            table: "Ratings",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Ratings");

        migrationBuilder.AddColumn<int>(
            name: "Rating",
            table: "Faqs",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }
}
