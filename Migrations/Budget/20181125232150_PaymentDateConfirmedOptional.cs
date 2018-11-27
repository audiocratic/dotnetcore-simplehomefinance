using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleBillPay.Migrations.Budget
{
    public partial class PaymentDateConfirmedOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateConfirmed",
                table: "Payments",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateConfirmed",
                table: "Payments",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
