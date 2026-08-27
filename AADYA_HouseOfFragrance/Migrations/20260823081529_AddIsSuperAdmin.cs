using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AADYA_HouseOfFragrance.Migrations
{
    /// <summary>
    /// Adds the IsSuperAdmin flag to the Users table to enable hierarchical admin roles.
    /// Super Admins have the ability to promote and demote other users to/from admin status.
    /// </summary>
    public partial class AddIsSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // ADD IsSuperAdmin COLUMN TO USERS TABLE
            // ============================================

            // Add a boolean flag to identify super admin users.
            // Super Admins have elevated privileges, including the ability
            // to promote/demote other admin users.
            // Default value is false (regular user or admin without super privileges).
            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdmin",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // REMOVE IsSuperAdmin COLUMN FROM USERS TABLE
            // ============================================

            // Revert the migration by dropping the IsSuperAdmin column.
            migrationBuilder.DropColumn(
                name: "IsSuperAdmin",
                table: "Users");
        }
    }
}