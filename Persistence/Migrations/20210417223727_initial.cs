using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suggestions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuggestedActions = table.Column<string>(nullable: true),
                    FinalSuggestion = table.Column<string>(nullable: true),
                    SuggestionMessage = table.Column<string>(nullable: true),
                    Success = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    Symbol = table.Column<string>(nullable: true),
                    CurrentRSIValue = table.Column<string>(nullable: true),
                    CurrentChaikinOSCValue = table.Column<string>(nullable: true),
                    InsertTimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suggestions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suggestions");
        }
    }
}
