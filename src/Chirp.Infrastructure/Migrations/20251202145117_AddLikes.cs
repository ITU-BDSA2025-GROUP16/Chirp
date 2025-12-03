using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "Likes",
        columns: table => new
        {
            LikerId = table.Column<int>(type: "INTEGER", nullable: false),
            LikedCheepId = table.Column<int>(type: "INTEGER", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Likes", x => new { x.LikerId, x.LikedCheepId });
        });

    migrationBuilder.CreateIndex(
        name: "IX_Likes_LikedCheepId",
        table: "Likes",
        column: "LikedCheepId");

    migrationBuilder.CreateIndex(
        name: "IX_Likes_LikerId",
        table: "Likes",
        column: "LikerId");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropTable(name: "Likes");
}
    }
}
