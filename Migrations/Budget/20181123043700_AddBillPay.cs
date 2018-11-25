using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleBillPay.Migrations.Budget
{
    public partial class AddBillPay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillPayID",
                table: "Payments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateConfirmed",
                table: "Payments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BillPay",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BillPayDate = table.Column<DateTime>(nullable: false),
                    StartingAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillPay", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BillPayID",
                table: "Payments",
                column: "BillPayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_BillPay_BillPayID",
                table: "Payments",
                column: "BillPayID",
                principalTable: "BillPay",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_BillPay_BillPayID",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "BillPay");

            migrationBuilder.DropIndex(
                name: "IX_Payments_BillPayID",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BillPayID",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DateConfirmed",
                table: "Payments");
        }
    }
}
