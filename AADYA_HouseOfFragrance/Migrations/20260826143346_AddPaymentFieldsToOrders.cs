using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AADYA_HouseOfFragrance.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFieldsToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "Orders",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7686));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7717));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7721));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7743));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7974));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7979));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7745));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7753));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7755));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7984));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7987));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7939));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7724));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7942));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7945));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7990));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7993));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7948));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7997));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7951));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7954));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7957));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7960));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7962));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7999));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(8002));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7965));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(8005));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7968));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7728));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7732));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7971));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7734));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7737));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 24, 14, 19, 12, 753, DateTimeKind.Local).AddTicks(7740));
        }
    }
}
