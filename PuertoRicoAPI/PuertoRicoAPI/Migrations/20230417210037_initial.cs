using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuertoRicoAPI.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsRoleInProgress = table.Column<bool>(type: "bit", nullable: false),
                    CurrentPlayerIndex = table.Column<int>(type: "int", nullable: false),
                    PrivilegeIndex = table.Column<int>(type: "int", nullable: false),
                    GovernorIndex = table.Column<int>(type: "int", nullable: false),
                    VictoryPointSupply = table.Column<int>(type: "int", nullable: false),
                    ColonistsSupply = table.Column<int>(type: "int", nullable: false),
                    ColonistsOnShip = table.Column<int>(type: "int", nullable: false),
                    QuarryCount = table.Column<int>(type: "int", nullable: false),
                    CornSupply = table.Column<int>(type: "int", nullable: false),
                    IndigoSupply = table.Column<int>(type: "int", nullable: false),
                    SugarSupply = table.Column<int>(type: "int", nullable: false),
                    TobaccoSupply = table.Column<int>(type: "int", nullable: false),
                    CoffeeSupply = table.Column<int>(type: "int", nullable: false),
                    CurrentRole = table.Column<int>(type: "int", nullable: false),
                    CaptainPlayableIndexes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaptainFirstShipment = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataGameStateId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buildings_Games_DataGameStateId",
                        column: x => x.DataGameStateId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataGameStateId = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Doubloons = table.Column<int>(type: "int", nullable: false),
                    Colonists = table.Column<int>(type: "int", nullable: false),
                    VictoryPoints = table.Column<int>(type: "int", nullable: false),
                    CanUseHacienda = table.Column<bool>(type: "bit", nullable: false),
                    CanUseWharf = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Games_DataGameStateId",
                        column: x => x.DataGameStateId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataGameStateId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<int>(type: "int", nullable: false),
                    IsPlayable = table.Column<bool>(type: "bit", nullable: false),
                    Bounty = table.Column<int>(type: "int", nullable: false),
                    IsFirstIteration = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Games_DataGameStateId",
                        column: x => x.DataGameStateId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataGameStateId = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Load = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ships_Games_DataGameStateId",
                        column: x => x.DataGameStateId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeHouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataGameStateId = table.Column<int>(type: "int", nullable: false),
                    Goods = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeHouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeHouses_Games_DataGameStateId",
                        column: x => x.DataGameStateId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataGameStateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Games_DataGameStateId",
                        column: x => x.DataGameStateId,
                        principalTable: "Games",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataPlayerId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goods_Players_DataPlayerId",
                        column: x => x.DataPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerBuildings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataPlayerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBuildings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerBuildings_Players_DataPlayerId",
                        column: x => x.DataPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsOccupied = table.Column<bool>(type: "bit", nullable: false),
                    DataBuildingId = table.Column<int>(type: "int", nullable: true),
                    DataPlayerBuildingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slots_Buildings_DataBuildingId",
                        column: x => x.DataBuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Slots_PlayerBuildings_DataPlayerBuildingId",
                        column: x => x.DataPlayerBuildingId,
                        principalTable: "PlayerBuildings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Plantations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataGameStateId = table.Column<int>(type: "int", nullable: false),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    IsExposed = table.Column<bool>(type: "bit", nullable: false),
                    IsDiscarded = table.Column<bool>(type: "bit", nullable: false),
                    Good = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plantations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plantations_Games_DataGameStateId",
                        column: x => x.DataGameStateId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plantations_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPlantations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataPlayerId = table.Column<int>(type: "int", nullable: false),
                    SlotId = table.Column<int>(type: "int", nullable: false),
                    Good = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPlantations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerPlantations_Players_DataPlayerId",
                        column: x => x.DataPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerPlantations_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_DataGameStateId",
                table: "Buildings",
                column: "DataGameStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_DataPlayerId",
                table: "Goods",
                column: "DataPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Plantations_DataGameStateId",
                table: "Plantations",
                column: "DataGameStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Plantations_SlotId",
                table: "Plantations",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBuildings_DataPlayerId",
                table: "PlayerBuildings",
                column: "DataPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPlantations_DataPlayerId",
                table: "PlayerPlantations",
                column: "DataPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPlantations_SlotId",
                table: "PlayerPlantations",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_DataGameStateId",
                table: "Players",
                column: "DataGameStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_DataGameStateId",
                table: "Roles",
                column: "DataGameStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_DataGameStateId",
                table: "Ships",
                column: "DataGameStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_DataBuildingId",
                table: "Slots",
                column: "DataBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_DataPlayerBuildingId",
                table: "Slots",
                column: "DataPlayerBuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeHouses_DataGameStateId",
                table: "TradeHouses",
                column: "DataGameStateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DataGameStateId",
                table: "Users",
                column: "DataGameStateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "Plantations");

            migrationBuilder.DropTable(
                name: "PlayerPlantations");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "TradeHouses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Slots");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "PlayerBuildings");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
