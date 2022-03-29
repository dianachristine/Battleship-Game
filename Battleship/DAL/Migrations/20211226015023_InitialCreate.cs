using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameConfigs",
                columns: table => new
                {
                    GameConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameConfigName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BoardSizeX = table.Column<int>(type: "int", nullable: false),
                    BoardSizeY = table.Column<int>(type: "int", nullable: false),
                    EShipTouchRule = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameConfigs", x => x.GameConfigId);
                });

            migrationBuilder.CreateTable(
                name: "SavedGames",
                columns: table => new
                {
                    SavedGameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SavedGameName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CurrentPlayerNo = table.Column<int>(type: "int", nullable: false),
                    GameConfigId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedGames", x => x.SavedGameId);
                    table.ForeignKey(
                        name: "FK_SavedGames_GameConfigs_GameConfigId",
                        column: x => x.GameConfigId,
                        principalTable: "GameConfigs",
                        principalColumn: "GameConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipConfigs",
                columns: table => new
                {
                    ShipConfigId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShipSizeX = table.Column<int>(type: "int", nullable: false),
                    ShipSizeY = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    GameConfigId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipConfigs", x => x.ShipConfigId);
                    table.ForeignKey(
                        name: "FK_ShipConfigs_GameConfigs_GameConfigId",
                        column: x => x.GameConfigId,
                        principalTable: "GameConfigs",
                        principalColumn: "GameConfigId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameBoards",
                columns: table => new
                {
                    GameBoardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Board = table.Column<string>(type: "nvarchar(max)", maxLength: 21000, nullable: false),
                    SavedGameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameBoards", x => x.GameBoardId);
                    table.ForeignKey(
                        name: "FK_GameBoards_SavedGames_SavedGameId",
                        column: x => x.SavedGameId,
                        principalTable: "SavedGames",
                        principalColumn: "SavedGameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    ShipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GameBoardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.ShipId);
                    table.ForeignKey(
                        name: "FK_Ships_GameBoards_GameBoardId",
                        column: x => x.GameBoardId,
                        principalTable: "GameBoards",
                        principalColumn: "GameBoardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coordinates",
                columns: table => new
                {
                    CoordinateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    ShipId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinates", x => x.CoordinateId);
                    table.ForeignKey(
                        name: "FK_Coordinates_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "ShipId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coordinates_ShipId",
                table: "Coordinates",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_GameBoards_SavedGameId",
                table: "GameBoards",
                column: "SavedGameId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedGames_GameConfigId",
                table: "SavedGames",
                column: "GameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipConfigs_GameConfigId",
                table: "ShipConfigs",
                column: "GameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_GameBoardId",
                table: "Ships",
                column: "GameBoardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coordinates");

            migrationBuilder.DropTable(
                name: "ShipConfigs");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "GameBoards");

            migrationBuilder.DropTable(
                name: "SavedGames");

            migrationBuilder.DropTable(
                name: "GameConfigs");
        }
    }
}
