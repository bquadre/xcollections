using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Softmax.XCollections.Data.Migrations
{
    public partial class _ModelsModfied : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guarantors_LoanRequest_LoanRequestId",
                table: "Guarantors");

            migrationBuilder.DropTable(
                name: "LoanRefunds");

            migrationBuilder.DropTable(
                name: "LoanRequest");

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    LoanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateApproved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Interest = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.LoanId);
                    table.ForeignKey(
                        name: "FK_Loans_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    RefundId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LoanRequestId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.RefundId);
                    table.ForeignKey(
                        name: "FK_Refunds_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Loans_LoanRequestId",
                        column: x => x.LoanRequestId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_CustomerId",
                table: "Loans",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_EmployeeId",
                table: "Loans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ProductId",
                table: "Loans",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_CustomerId",
                table: "Refunds",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_EmployeeId",
                table: "Refunds",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_LoanRequestId",
                table: "Refunds",
                column: "LoanRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guarantors_Loans_LoanRequestId",
                table: "Guarantors",
                column: "LoanRequestId",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guarantors_Loans_LoanRequestId",
                table: "Guarantors");

            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.CreateTable(
                name: "LoanRequest",
                columns: table => new
                {
                    LoanRequestId = table.Column<string>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    CustomerId = table.Column<string>(nullable: true),
                    DateApproved = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DueDate = table.Column<DateTime>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    Interest = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ProductId = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    StatusCode = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRequest", x => x.LoanRequestId);
                    table.ForeignKey(
                        name: "FK_LoanRequest_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanRequest_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanRequest_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanRefunds",
                columns: table => new
                {
                    LoanRefundId = table.Column<string>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    Balance = table.Column<int>(nullable: false),
                    CustomerId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LoanRequestId = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRefunds", x => x.LoanRefundId);
                    table.ForeignKey(
                        name: "FK_LoanRefunds_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanRefunds_Employee_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanRefunds_LoanRequest_LoanRequestId",
                        column: x => x.LoanRequestId,
                        principalTable: "LoanRequest",
                        principalColumn: "LoanRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanRefunds_CustomerId",
                table: "LoanRefunds",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRefunds_EmployeeId",
                table: "LoanRefunds",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRefunds_LoanRequestId",
                table: "LoanRefunds",
                column: "LoanRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequest_CustomerId",
                table: "LoanRequest",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequest_EmployeeId",
                table: "LoanRequest",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequest_ProductId",
                table: "LoanRequest",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guarantors_LoanRequest_LoanRequestId",
                table: "Guarantors",
                column: "LoanRequestId",
                principalTable: "LoanRequest",
                principalColumn: "LoanRequestId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
