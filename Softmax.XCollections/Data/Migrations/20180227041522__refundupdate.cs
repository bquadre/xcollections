using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Softmax.XCollections.Data.Migrations
{
    public partial class _refundupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Customers_CustomerId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Employee_EmployeeId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_CustomerId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_EmployeeId",
                table: "Refunds");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Refunds",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Refunds",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Refunds",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Refunds",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_CustomerId",
                table: "Refunds",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_EmployeeId",
                table: "Refunds",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Customers_CustomerId",
                table: "Refunds",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Employee_EmployeeId",
                table: "Refunds",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
