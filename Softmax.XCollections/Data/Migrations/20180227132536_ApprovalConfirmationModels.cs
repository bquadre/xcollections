using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Softmax.XCollections.Data.Migrations
{
    public partial class ApprovalConfirmationModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "RefundConfirms",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "LoanApprovals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "DepositConfirms",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefundConfirms_CustomerId",
                table: "RefundConfirms",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApprovals_CustomerId",
                table: "LoanApprovals",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_DepositConfirms_CustomerId",
                table: "DepositConfirms",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepositConfirms_Customers_CustomerId",
                table: "DepositConfirms",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApprovals_Customers_CustomerId",
                table: "LoanApprovals",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefundConfirms_Customers_CustomerId",
                table: "RefundConfirms",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepositConfirms_Customers_CustomerId",
                table: "DepositConfirms");

            migrationBuilder.DropForeignKey(
                name: "FK_LoanApprovals_Customers_CustomerId",
                table: "LoanApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundConfirms_Customers_CustomerId",
                table: "RefundConfirms");

            migrationBuilder.DropIndex(
                name: "IX_RefundConfirms_CustomerId",
                table: "RefundConfirms");

            migrationBuilder.DropIndex(
                name: "IX_LoanApprovals_CustomerId",
                table: "LoanApprovals");

            migrationBuilder.DropIndex(
                name: "IX_DepositConfirms_CustomerId",
                table: "DepositConfirms");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "RefundConfirms");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "LoanApprovals");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "DepositConfirms");
        }
    }
}
