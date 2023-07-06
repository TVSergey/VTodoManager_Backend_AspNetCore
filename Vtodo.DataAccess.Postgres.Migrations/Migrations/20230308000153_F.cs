using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vtodo.DataAccess.Postgres.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class F : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountProject");

            migrationBuilder.AddColumn<string>(
                name: "ImageHeaderPath",
                table: "Boards",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageHeaderPath",
                table: "Boards");

            migrationBuilder.CreateTable(
                name: "AccountProject",
                columns: table => new
                {
                    MemberInProjectsId = table.Column<int>(type: "integer", nullable: false),
                    MembersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountProject", x => new { x.MemberInProjectsId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_AccountProject_Accounts_MembersId",
                        column: x => x.MembersId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountProject_Projects_MemberInProjectsId",
                        column: x => x.MemberInProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountProject_MembersId",
                table: "AccountProject",
                column: "MembersId");
        }
    }
}
