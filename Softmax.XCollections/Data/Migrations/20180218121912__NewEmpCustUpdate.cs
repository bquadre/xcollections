using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Softmax.XCollections.Data.Migrations
{
    public partial class _NewEmpCustUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Customers",
                nullable: true);
        }
    }
}
