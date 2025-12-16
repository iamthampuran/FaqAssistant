using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaqAssistant.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddedMiscellaneousFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "LastUpdatedAt",
            table: "Users",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Tags",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Tags",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "LastUpdatedAt",
            table: "Tags",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Faqs",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Faqs",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "LastUpdatedAt",
            table: "Faqs",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Categories",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Categories",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "LastUpdatedAt",
            table: "Categories",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Answers",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Answers",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "LastUpdatedAt",
            table: "Answers",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.CreateIndex(
            name: "IX_Answers_UserId",
            table: "Answers",
            column: "UserId");

        migrationBuilder.AddForeignKey(
            name: "FK_Answers_Users_UserId",
            table: "Answers",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Answers_Users_UserId",
            table: "Answers");

        migrationBuilder.DropIndex(
            name: "IX_Answers_UserId",
            table: "Answers");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "LastUpdatedAt",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "LastUpdatedAt",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Faqs");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Faqs");

        migrationBuilder.DropColumn(
            name: "LastUpdatedAt",
            table: "Faqs");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Categories");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Categories");

        migrationBuilder.DropColumn(
            name: "LastUpdatedAt",
            table: "Categories");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Answers");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Answers");

        migrationBuilder.DropColumn(
            name: "LastUpdatedAt",
            table: "Answers");
    }
}
