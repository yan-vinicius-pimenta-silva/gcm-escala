using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscalaGcm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRegimeToEscalaItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Regime",
                table: "escala_itens",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Regime",
                table: "escala_itens");
        }
    }
}
