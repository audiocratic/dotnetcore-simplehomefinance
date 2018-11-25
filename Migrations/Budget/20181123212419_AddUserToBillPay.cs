using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleBillPay.Migrations.Budget
{
    public partial class AddUserToBillPay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BillPay",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BillPay_UserId_BillPayDate",
                table: "BillPay",
                columns: new[] { "UserId", "BillPayDate" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BillPay_AspNetUsers_UserId",
                table: "BillPay",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillPay_AspNetUsers_UserId",
                table: "BillPay");

            migrationBuilder.DropIndex(
                name: "IX_BillPay_UserId_BillPayDate",
                table: "BillPay");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BillPay");
        }
    }
}
