using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bold.Integration.Base.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addindexbykind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SyncStates_Kind",
                table: "SyncStates",
                column: "Kind");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SyncStates_Kind",
                table: "SyncStates");
        }
    }
}
