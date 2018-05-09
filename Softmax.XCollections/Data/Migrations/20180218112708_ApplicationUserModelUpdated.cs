using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Softmax.XCollections.Data.Migrations
{
    public partial class ApplicationUserModelUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UniqueIdentifier",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Loans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Deposits",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GenderCode",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UniqueNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserCode",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ApplicationUserId",
                table: "Loans",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_ApplicationUserId",
                table: "Deposits",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_AspNetUsers_ApplicationUserId",
                table: "Deposits",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_ApplicationUserId",
                table: "Loans",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_AspNetUsers_ApplicationUserId",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Deposits_ApplicationUserId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "GenderCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UniqueNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserCode",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UniqueIdentifier",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "User",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }
    }
}
