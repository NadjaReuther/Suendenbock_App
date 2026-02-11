using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class FixParentMapIdForDetailMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Repariere bestehende Detail-Karten: Setze ParentMapId basierend auf MapRegions
            migrationBuilder.Sql(@"
                UPDATE m
                SET m.ParentMapId = mr.MapId,
                    m.IsWorldMap = 0
                FROM Maps m
                INNER JOIN MapRegions mr ON m.Id = mr.LinkedMapId
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
