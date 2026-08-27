using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AADYA_HouseOfFragrance.Migrations
{
    /// <summary>
    /// Adds the IsAdmin flag to the Users table to enable role-based authorization.
    /// This flag determines which users have administrative access to the application.
    /// </summary>
    public partial class AddIsAdminToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // ADD IsAdmin COLUMN TO USERS TABLE
            // ============================================

            // Add a boolean flag to identify admin users.
            // Default value is false (regular user).
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // REMOVE IsAdmin COLUMN FROM USERS TABLE
            // ============================================

            // Revert the migration by dropping the IsAdmin column.
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Users");
        }
    }
}