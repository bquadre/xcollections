using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Softmax.XCollections.Data.Migrations
{
    public partial class ApprovalandConfirmationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepositConfirms",
                columns: table => new
                {
                    DepositConfirmId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DepositId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositConfirms", x => x.DepositConfirmId);
                    table.ForeignKey(
                        name: "FK_DepositConfirms_Deposits_DepositId",
                        column: x => x.DepositId,
                        principalTable: "Deposits",
                        principalColumn: "DepositId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepositConfirms_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanApprovals",
                columns: table => new
                {
                    LoanApprovalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LoanId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApprovals", x => x.LoanApprovalId);
                    table.ForeignKey(
                        name: "FK_LoanApprovals_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanApprovals_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefundConfirms",
                columns: table => new
                {
                    RefundConfirmId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RefundId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundConfirms", x => x.RefundConfirmId);
                    table.ForeignKey(
                        name: "FK_RefundConfirms_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundConfirms_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalTable: "Refunds",
                        principalColumn: "RefundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepositConfirms_DepositId",
                table: "DepositConfirms",
                column: "DepositId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositConfirms_EmployeeId",
                table: "DepositConfirms",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApprovals_EmployeeId",
                table: "LoanApprovals",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApprovals_LoanId",
                table: "LoanApprovals",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundConfirms_EmployeeId",
                table: "RefundConfirms",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundConfirms_RefundId",
                table: "RefundConfirms",
                column: "RefundId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepositConfirms");

            migrationBuilder.DropTable(
                name: "LoanApprovals");

            migrationBuilder.DropTable(
                name: "RefundConfirms");
        }
    }
}
