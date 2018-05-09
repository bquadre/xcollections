using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Softmax.XCollections.Data.Migrations
{
    public partial class _EntitiesModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guarantors");

            migrationBuilder.DropTable(
                name: "Withdrawals");

            migrationBuilder.DropColumn(
                name: "ApprovalCode",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "ApprovalCode",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ApprovalCode",
                table: "Deposits");

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "Refunds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "Deposits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "Deposits");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalCode",
                table: "Refunds",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalCode",
                table: "Loans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalCode",
                table: "Deposits",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Guarantors",
                columns: table => new
                {
                    GuarantorId = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    CustomerId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    IsAcceptable = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LoanId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    StateCode = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guarantors", x => x.GuarantorId);
                    table.ForeignKey(
                        name: "FK_Guarantors_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guarantors_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Withdrawals",
                columns: table => new
                {
                    WithdrawalId = table.Column<string>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    ApprovalCode = table.Column<int>(nullable: false),
                    CustomerId = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    StatusCode = table.Column<int>(nullable: false),
                    WhenRequested = table.Column<DateTime>(nullable: false),
                    WhenTreated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdrawals", x => x.WithdrawalId);
                    table.ForeignKey(
                        name: "FK_Withdrawals_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_CustomerId",
                table: "Guarantors",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_LoanId",
                table: "Guarantors",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Withdrawals_CustomerId",
                table: "Withdrawals",
                column: "CustomerId");
        }
    }
}
