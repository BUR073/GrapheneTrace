using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrapheneTrace.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserRoleToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE AspNetRoles SET Name = 'Patient', NormalizedName = 'PATIENT' WHERE Name = 'User'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE AspNetRoles SET Name = 'User', NormalizedName = 'USER' WHERE Name = 'Patient'");
        }
    }
}
