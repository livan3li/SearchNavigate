using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchNavigate.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ViewHistoryAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserViewHistory",
                schema: "DBO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewSourse = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserViewHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserViewHistory_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "DBO",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserViewHistory_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "DBO",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserViewHistory_ProductId",
                schema: "DBO",
                table: "UserViewHistory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserViewHistory_UserId",
                schema: "DBO",
                table: "UserViewHistory",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserViewHistory",
                schema: "DBO");
        }
    }
}
