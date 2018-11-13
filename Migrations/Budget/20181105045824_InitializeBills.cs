using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleBillPay.Migrations.Budget
{
    public partial class InitializeBills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.CreateTable(
            //     name: "User",
            //     columns: table => new
            //     {
            //         Id = table.Column<string>(nullable: false),
            //         UserName = table.Column<string>(nullable: true),
            //         NormalizedUserName = table.Column<string>(nullable: true),
            //         Email = table.Column<string>(nullable: true),
            //         NormalizedEmail = table.Column<string>(nullable: true),
            //         EmailConfirmed = table.Column<bool>(nullable: false),
            //         PasswordHash = table.Column<string>(nullable: true),
            //         SecurityStamp = table.Column<string>(nullable: true),
            //         ConcurrencyStamp = table.Column<string>(nullable: true),
            //         PhoneNumber = table.Column<string>(nullable: true),
            //         PhoneNumberConfirmed = table.Column<bool>(nullable: false),
            //         TwoFactorEnabled = table.Column<bool>(nullable: false),
            //         LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
            //         LockoutEnabled = table.Column<bool>(nullable: false),
            //         AccessFailedCount = table.Column<int>(nullable: false),
            //         AccountCreationDateTime = table.Column<DateTime>(nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_User", x => x.Id);
            //     });

            migrationBuilder.CreateTable(
                name: "BillTemplate",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    FrequencyInMonths = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillTemplate", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BillTemplate_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillInstance",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BillTemplateID = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillInstance", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BillInstance_BillTemplate_BillTemplateID",
                        column: x => x.BillTemplateID,
                        principalTable: "BillTemplate",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillInstance_BillTemplateID",
                table: "BillInstance",
                column: "BillTemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_BillTemplate_UserId",
                table: "BillTemplate",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillInstance");

            migrationBuilder.DropTable(
                name: "BillTemplate");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
