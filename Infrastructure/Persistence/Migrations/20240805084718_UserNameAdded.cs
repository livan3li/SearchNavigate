using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchNavigate.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserNameAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderItemsId",
                schema: "DBO",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "DBO",
                table: "User",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "DBO",
                table: "User");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderItemsId",
                schema: "DBO",
                table: "Order",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
