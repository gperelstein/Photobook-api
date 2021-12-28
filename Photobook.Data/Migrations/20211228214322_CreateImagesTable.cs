using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Photobook.Data.Migrations
{
    public partial class CreateImagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Profiles");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileImageId",
                table: "Profiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Path = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Images_AspNetUsers_LastModifiedBy",
                        column: x => x.LastModifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_ProfileImageId",
                table: "Profiles",
                column: "ProfileImageId",
                unique: true,
                filter: "[ProfileImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Images_CreatedBy",
                table: "Images",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Images_LastModifiedBy",
                table: "Images",
                column: "LastModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Images_ProfileImageId",
                table: "Profiles",
                column: "ProfileImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Images_ProfileImageId",
                table: "Profiles");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_ProfileImageId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "ProfileImageId",
                table: "Profiles");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Profiles",
                type: "nvarchar(200)",
                nullable: true);
        }
    }
}
