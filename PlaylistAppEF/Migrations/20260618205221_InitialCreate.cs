using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PlaylistAppEF.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chansons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Artiste = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Album = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DureeSecondes = table.Column<int>(type: "INTEGER", nullable: false),
                    Genre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Annee = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<int>(type: "INTEGER", nullable: false),
                    AjouteLe = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chansons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreeLe = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifieLe = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistChansons",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChansonId = table.Column<int>(type: "INTEGER", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    AjouteLe = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistChansons", x => new { x.PlaylistId, x.ChansonId });
                    table.ForeignKey(
                        name: "FK_PlaylistChansons_Chansons_ChansonId",
                        column: x => x.ChansonId,
                        principalTable: "Chansons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaylistChansons_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Chansons",
                columns: new[] { "Id", "AjouteLe", "Album", "Annee", "Artiste", "DureeSecondes", "Genre", "Note", "Titre" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A Night at the Opera", 1975, "Queen", 354, "Rock", 5, "Bohemian Rhapsody" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hotel California", 1977, "Eagles", 391, "Rock", 5, "Hotel California" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "After Hours", 2019, "The Weeknd", 200, "Pop", 4, "Blinding Lights" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "÷ (Divide)", 2017, "Ed Sheeran", 234, "Pop", 4, "Shape of You" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "8 Mile Soundtrack", 2002, "Eminem", 326, "Rap", 5, "Lose Yourself" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Scorpion", 2018, "Drake", 198, "Rap", 4, "God's Plan" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nevermind", 1991, "Nirvana", 301, "Rock", 5, "Smells Like Teen Spirit" },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "21", 2010, "Adele", 228, "Soul", 5, "Rolling in the Deep" },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thriller", 1982, "Michael Jackson", 294, "Pop", 5, "Billie Jean" },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Discovery", 2000, "Daft Punk", 321, "Électro", 5, "One More Time" },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Random Access Memories", 2013, "Daft Punk", 369, "Électro", 4, "Get Lucky" },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saturday Night Fever", 1977, "Bee Gees", 245, "Disco", 4, "Stayin' Alive" }
                });

            migrationBuilder.InsertData(
                table: "Playlists",
                columns: new[] { "Id", "CreeLe", "Description", "ModifieLe", "Nom" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Les incontournables du rock", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rock Classics" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Meilleures chansons pop", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pop Hits 2010-2020" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pour danser toute la nuit", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Électro Vibes" }
                });

            migrationBuilder.InsertData(
                table: "PlaylistChansons",
                columns: new[] { "ChansonId", "PlaylistId", "AjouteLe", "Position" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 7, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 3, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 4, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 8, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 9, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 10, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 11, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 12, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chansons_Artiste",
                table: "Chansons",
                column: "Artiste");

            migrationBuilder.CreateIndex(
                name: "IX_Chansons_Genre",
                table: "Chansons",
                column: "Genre");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistChansons_ChansonId",
                table: "PlaylistChansons",
                column: "ChansonId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_Nom",
                table: "Playlists",
                column: "Nom",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistChansons");

            migrationBuilder.DropTable(
                name: "Chansons");

            migrationBuilder.DropTable(
                name: "Playlists");
        }
    }
}
