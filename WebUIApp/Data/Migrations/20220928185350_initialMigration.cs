using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebUIApp.Data.Migrations
{
    public partial class initialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "buysell",
                columns: table => new
                {
                    BuyselId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuySellStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_buysell", x => x.BuyselId);
                });

            migrationBuilder.CreateTable(
                name: "messageinfo",
                columns: table => new
                {
                    MessageID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MessageName = table.Column<string>(nullable: false),
                    MessageCode = table.Column<string>(nullable: true),
                    CreateDate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messageinfo", x => x.MessageID);
                });

            migrationBuilder.CreateTable(
                name: "serviceStatus",
                columns: table => new
                {
                    StatusId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActiveStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serviceStatus", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "TradeviewAlert_API",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    alertmessage = table.Column<string>(nullable: true),
                    createdate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeviewAlert_API", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotInfo",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    botid = table.Column<string>(nullable: true),
                    baseordersize = table.Column<int>(nullable: false),
                    delay = table.Column<int>(nullable: false),
                    createdate = table.Column<string>(nullable: true),
                    BuyselId = table.Column<int>(nullable: false),
                    buySellBuyselId = table.Column<int>(nullable: true),
                    MessageID = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotInfo", x => x.id);
                    table.ForeignKey(
                        name: "FK_BotInfo_messageinfo_MessageID",
                        column: x => x.MessageID,
                        principalTable: "messageinfo",
                        principalColumn: "MessageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotInfo_serviceStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "serviceStatus",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotInfo_buysell_buySellBuyselId",
                        column: x => x.buySellBuyselId,
                        principalTable: "buysell",
                        principalColumn: "BuyselId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RuleInfo",
                columns: table => new
                {
                    RuleID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RuleName = table.Column<string>(nullable: false),
                    CreateDate = table.Column<string>(nullable: true),
                    Target = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleInfo", x => x.RuleID);
                    table.ForeignKey(
                        name: "FK_RuleInfo_serviceStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "serviceStatus",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlertInfo",
                columns: table => new
                {
                    alertId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NoOFTrigger = table.Column<int>(nullable: false),
                    NoOFAlert = table.Column<int>(nullable: false),
                    LastTrueDate = table.Column<string>(nullable: true),
                    createdate = table.Column<string>(nullable: true),
                    RuleID = table.Column<int>(nullable: false),
                    MessageID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertInfo", x => x.alertId);
                    table.ForeignKey(
                        name: "FK_AlertInfo_messageinfo_MessageID",
                        column: x => x.MessageID,
                        principalTable: "messageinfo",
                        principalColumn: "MessageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertInfo_RuleInfo_RuleID",
                        column: x => x.RuleID,
                        principalTable: "RuleInfo",
                        principalColumn: "RuleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertInfo_MessageID",
                table: "AlertInfo",
                column: "MessageID");

            migrationBuilder.CreateIndex(
                name: "IX_AlertInfo_RuleID",
                table: "AlertInfo",
                column: "RuleID");

            migrationBuilder.CreateIndex(
                name: "IX_BotInfo_MessageID",
                table: "BotInfo",
                column: "MessageID");

            migrationBuilder.CreateIndex(
                name: "IX_BotInfo_StatusId",
                table: "BotInfo",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BotInfo_buySellBuyselId",
                table: "BotInfo",
                column: "buySellBuyselId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleInfo_StatusId",
                table: "RuleInfo",
                column: "StatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertInfo");

            migrationBuilder.DropTable(
                name: "BotInfo");

            migrationBuilder.DropTable(
                name: "TradeviewAlert_API");

            migrationBuilder.DropTable(
                name: "RuleInfo");

            migrationBuilder.DropTable(
                name: "messageinfo");

            migrationBuilder.DropTable(
                name: "buysell");

            migrationBuilder.DropTable(
                name: "serviceStatus");
        }
    }
}
