using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AADYA_HouseOfFragrance.Migrations
{
    /// <summary>
    /// Adds payment fields to the Orders table to support the dummy payment gateway and COD functionality.
    /// - PaymentMethod: Stores the selected payment method (Card, UPI, COD).
    /// - PaymentId: Stores the transaction ID from the payment gateway (or generated dummy ID).
    /// </summary>
    /// <remarks>
    /// This migration does NOT modify seed data. The Up method only adds new columns.
    /// The Down method only drops the added columns.
    /// </remarks>
    public partial class AddPaymentFieldsToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // ADD PAYMENT FIELDS TO ORDERS TABLE
            // ============================================

            // Add PaymentMethod column to store Card, UPI, or COD
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "longtext",
                nullable: true);

            // Add PaymentId column to store transaction ID
            // For dummy payments: "PAY-DUMMY-{ticks}" or "COD-{ticks}"
            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "Orders",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // REMOVE PAYMENT FIELDS FROM ORDERS TABLE
            // ============================================

            // Drop columns in reverse order of addition
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");
        }
    }
}