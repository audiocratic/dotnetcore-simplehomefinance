using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleBillPay.Migrations.Budget
{
    public partial class BillInstanceNameRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_BillInstance_BillInstanceID",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_BillInstanceID",
                table: "Payments",
                newName: "IX_Payments_BillInstanceID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BillInstance",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_BillInstance_BillInstanceID",
                table: "Payments",
                column: "BillInstanceID",
                principalTable: "BillInstance",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_BillInstance_BillInstanceID",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_BillInstanceID",
                table: "Payment",
                newName: "IX_Payment_BillInstanceID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BillInstance",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_BillInstance_BillInstanceID",
                table: "Payment",
                column: "BillInstanceID",
                principalTable: "BillInstance",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
