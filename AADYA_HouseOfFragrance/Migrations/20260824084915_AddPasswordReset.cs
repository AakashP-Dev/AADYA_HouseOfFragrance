using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AADYA_HouseOfFragrance.Migrations
{
    /// <summary>
    /// Adds the PasswordResets table to support OTP-based password reset functionality.
    /// Users can request a password reset OTP which is stored in this table with an expiry.
    /// </summary>
    /// <remarks>
    /// This migration does NOT modify the Users table. All password reset logic
    /// is handled through this separate table.
    /// </remarks>
    public partial class AddPasswordReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // CREATE PASSWORD RESETS TABLE
            // ============================================

            // The PasswordResets table stores OTP tokens for password reset requests.
            // Each reset request is linked to a user and has an expiry time.
            migrationBuilder.CreateTable(
                name: "PasswordResets",
                columns: table => new
                {
                    // Primary key
                    ResetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),

                    // Foreign key to User
                    UserId = table.Column<int>(type: "int", nullable: false),

                    // Token for the reset session (stored in session after OTP verification)
                    Token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    // OTP code sent to user's email
                    Otp = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    // Reset method ("Email" or "SMS" - currently only Email is implemented)
                    ResetMethod = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    // Timestamp when the reset request was created
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),

                    // Timestamp when the OTP expires (10 minutes after creation)
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),

                    // Flag indicating whether the OTP has been used
                    IsUsed = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResets", x => x.ResetId);
                    table.ForeignKey(
                        name: "FK_PasswordResets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // ============================================
            // CREATE INDEX FOR PERFORMANCE
            // ============================================

            // Create index on UserId for faster lookups when checking
            // if a user has an active reset request.
            migrationBuilder.CreateIndex(
                name: "IX_PasswordResets_UserId",
                table: "PasswordResets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // DROP PASSWORD RESETS TABLE
            // ============================================

            // Revert the migration by dropping the entire table.
            migrationBuilder.DropTable(
                name: "PasswordResets");
        }
    }
}