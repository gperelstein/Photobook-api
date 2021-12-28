using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Photobook.Data.Migrations
{
    public partial class AlterTablesAddAuditableFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Profiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Profiles",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Profiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Profiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_CreatedBy",
                table: "Profiles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_LastModifiedBy",
                table: "Profiles",
                column: "LastModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_AspNetUsers_CreatedBy",
                table: "Profiles",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_AspNetUsers_LastModifiedBy",
                table: "Profiles",
                column: "LastModifiedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_AspNetUsers_CreatedBy",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_AspNetUsers_LastModifiedBy",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_CreatedBy",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_LastModifiedBy",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Profiles");
        }
    }
}
