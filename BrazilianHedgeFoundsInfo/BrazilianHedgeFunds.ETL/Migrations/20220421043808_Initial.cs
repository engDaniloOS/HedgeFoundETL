using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrazilianHedgeFunds.ETL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HedgeFundRecord",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    TYPE = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    CNPJ_FUNDO = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    DT_COMPTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VL_TOTAL = table.Column<double>(type: "float", nullable: false),
                    VL_QUOTA = table.Column<double>(type: "float", nullable: false),
                    VL_PATRIM_LIQ = table.Column<double>(type: "float", nullable: false),
                    CAPTC_DIA = table.Column<double>(type: "float", nullable: false),
                    RESG_DIA = table.Column<double>(type: "float", nullable: false),
                    NR_COTST = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HedgeFundRecord", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HedgeFundRecord_CNPJ_FUNDO",
                table: "HedgeFundRecord",
                column: "CNPJ_FUNDO");

            migrationBuilder.CreateIndex(
                name: "IX_HedgeFundRecord_DT_COMPTC",
                table: "HedgeFundRecord",
                column: "DT_COMPTC");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HedgeFundRecord");
        }
    }
}
