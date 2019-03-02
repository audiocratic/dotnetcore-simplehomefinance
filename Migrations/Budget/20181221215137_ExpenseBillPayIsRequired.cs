using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleBillPay.Migrations.Budget
{
    public partial class ExpenseBillPayIsRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_BillPay_BillPayID",
                table: "Expenses");

            migrationBuilder.AlterColumn<int>(
                name: "BillPayID",
                table: "Expenses",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_BillPay_BillPayID",
                table: "Expenses",
                column: "BillPayID",
                principalTable: "BillPay",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_BillPay_BillPayID",
                table: "Expenses");

            migrationBuilder.AlterColumn<int>(
                name: "BillPayID",
                table: "Expenses",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_BillPay_BillPayID",
                table: "Expenses",
                column: "BillPayID",
                principalTable: "BillPay",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
