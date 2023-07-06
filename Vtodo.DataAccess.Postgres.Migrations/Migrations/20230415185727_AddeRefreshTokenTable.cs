using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Vtodo.DataAccess.Postgres.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddeRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectAccountsRoles",
                table: "ProjectAccountsRoles");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAccountsRoles_ProjectId",
                table: "ProjectAccountsRoles");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ProjectAccountsRoles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "ProjectAccountsRoles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectAccountsRoles",
                table: "ProjectAccountsRoles",
                columns: new[] { "ProjectId", "AccountId", "ProjectRole" });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Ip = table.Column<string>(type: "text", nullable: false),
                    Device = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectAccountsRoles",
                table: "ProjectAccountsRoles");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "ProjectAccountsRoles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ProjectAccountsRoles",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectAccountsRoles",
                table: "ProjectAccountsRoles",
                column: "ProjectRole");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccountsRoles_ProjectId",
                table: "ProjectAccountsRoles",
                column: "ProjectId");
        }
    }
}
