using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Softmax.XCollections.Data.Migrations
{
    public partial class _modelupdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guarantors_Loans_LoanRequestId",
                table: "Guarantors");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Loans_LoanRequestId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_LoanRequestId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Guarantors_LoanRequestId",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "LoanRequestId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "LoanRequestId",
                table: "Guarantors");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalCode",
                table: "Withdrawals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalCode",
                table: "Refunds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LoanId",
                table: "Refunds",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalCode",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalCode",
                table: "Deposits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LoanId",
                table: "Guarantors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_LoanId",
                table: "Refunds",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_LoanId",
                table: "Guarantors",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guarantors_Loans_LoanId",
                table: "Guarantors",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Loans_LoanId",
                table: "Refunds",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guarantors_Loans_LoanId",
                table: "Guarantors");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Loans_LoanId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_LoanId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Guarantors_LoanId",
                table: "Guarantors");

            migrationBuilder.DropColumn(
                name: "ApprovalCode",
                table: "Withdrawals");

            migrationBuilder.DropColumn(
                name: "ApprovalCode",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "LoanId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "ApprovalCode",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ApprovalCode",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "LoanId",
                table: "Guarantors");

            migrationBuilder.AddColumn<string>(
                name: "LoanRequestId",
                table: "Refunds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoanRequestId",
                table: "Guarantors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_LoanRequestId",
                table: "Refunds",
                column: "LoanRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Guarantors_LoanRequestId",
                table: "Guarantors",
                column: "LoanRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guarantors_Loans_LoanRequestId",
                table: "Guarantors",
                column: "LoanRequestId",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Loans_LoanRequestId",
                table: "Refunds",
                column: "LoanRequestId",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
