using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profanity.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtToProfanityWords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "profanity_words",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    word = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profanity_words", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_profanity_words_word",
                table: "profanity_words",
                column: "word",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "profanity_words");
        }
    }
}
