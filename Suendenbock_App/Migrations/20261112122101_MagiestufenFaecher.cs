using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Suendenbock_App.Migrations
{
    /// <inheritdoc />
    public partial class MagiestufenFaecher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Beschreibung",
                schema: "stamm",
                table: "Faecher",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HafenText",
                schema: "stamm",
                table: "Faecher",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerkAnger",
                schema: "stamm",
                table: "Faecher",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerkDom",
                schema: "stamm",
                table: "Faecher",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerkGarten",
                schema: "stamm",
                table: "Faecher",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PerkHafen",
                schema: "stamm",
                table: "Faecher",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "stamm",
                table: "Faecher",
                keyColumn: "Id",
                keyValue: 0,
                columns: new[] { "Beschreibung", "HafenText", "PerkAnger", "PerkDom", "PerkGarten", "PerkHafen" },
                values: new object[] { "Die Kunst, Zauberkraft direkt in Stoff und Rüstung einzuweben. Wer hier lernt, veredelt die eigene Robe mit jeder Stufe zu einem widerstandsfähigeren, magisch durchwirkten Gewand.", null, "Robe für Verbündete", "Robe der Zuflucht", "Lebende Robe", "Eigene Robe" });

            migrationBuilder.UpdateData(
                schema: "stamm",
                table: "Faecher",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Beschreibung", "HafenText", "PerkAnger", "PerkDom", "PerkGarten", "PerkHafen" },
                values: new object[] { "Lehre der magischen Schwingungen. Resonanzkundige lernen, fremde Kräfte zu bündeln (Auritieren) oder zu bannen (Silizieren) — ein Fach für alle, die Magie eher lenken als bekämpfen wollen.", null, "Resonanzfeld", "Stille", "Resonanzfeld", "Schöpfen" });

            migrationBuilder.UpdateData(
                schema: "stamm",
                table: "Faecher",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Beschreibung", "HafenText", "PerkAnger", "PerkDom", "PerkGarten", "PerkHafen" },
                values: new object[] { "Einklang mit den Elementen stärkt Körper und Widerstandskraft. Wer die Harmonie beherrscht, gewinnt entweder an Lebenskraft oder an Rüstung — und mit der Zeit an echter Resistenz.", null, "Wundverschiebung (-20)", "Unversehrt", "Elementwechsel", "Wundverschiebung (-10)" });

            migrationBuilder.UpdateData(
                schema: "stamm",
                table: "Faecher",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Beschreibung", "HafenText", "PerkAnger", "PerkDom", "PerkGarten", "PerkHafen" },
                values: new object[] { "Das Lesen der Sterne schärft die Wahrnehmung für Magie und für das Spürbare zugleich. Astronomen orientieren sich nicht nur im Raum, sondern auch im Unsichtbaren.", null, "Sternenweg", "Himmelskarte", "Vorzeichen", "Liminales Sehen" });

            migrationBuilder.UpdateData(
                schema: "stamm",
                table: "Faecher",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Beschreibung", "HafenText", "PerkAnger", "PerkDom", "PerkGarten", "PerkHafen" },
                values: new object[] { "Die Kunde der Karten und Wege. Kartopäden kennen jede Abkürzung und jeden Rückweg — das verschafft ihnen mehr Reichweite und macht sie zu verlässlichen Helfern im Team.", null, "Kartograph der Akademie", "Geheimgang", "Wegmarke", "Schneller oder Präziser" });

            migrationBuilder.UpdateData(
                schema: "stamm",
                table: "Faecher",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Beschreibung", "HafenText", "PerkAnger", "PerkDom", "PerkGarten", "PerkHafen" },
                values: new object[] { "Wer den richtigen Moment erkennt, trifft öfter ins Schwarze. Chronotaktiker studieren Timing und Rhythmus des Gefechts, was sich in höheren Kritischen Erfolgen niederschlägt.", "Schaltet Meisterschaften frei: ab jetzt dürfen in allen Talenten Meisterschaften gewählt werden.", "Taktgeber", "Stillstand", "Zeitschleife", "Meisterschaft" });

            migrationBuilder.UpdateData(
                schema: "stamm",
                table: "Faecher",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Beschreibung", "HafenText", "PerkAnger", "PerkDom", "PerkGarten", "PerkHafen" },
                values: new object[] { "Fechtkunst mit ätherisch geladenen Klingen. Ätherfechter trainieren Präzision und Talentwert im Kampf gegen sichtbare wie unsichtbare Gegner.", "Gebundene Waffe: eine feste Ätherwaffe aus einer Kategorie. Würfeln bleibt erlaubt.", "Zweite Form", "Klingentanz", "Klinge leihen", "Form der Waffe" });

            migrationBuilder.UpdateData(
                schema: "stamm",
                table: "Faecher",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Beschreibung", "HafenText", "PerkAnger", "PerkDom", "PerkGarten", "PerkHafen" },
                values: new object[] { "Theorie der Schwellen und Übergänge — zwischen den Welten, aber auch zwischen Aktion und Reaktion. Wer sie meistert, reagiert schneller und sicherer auf plötzliche Gefahr.", null, "Schwellenschritt", "Schwelle öffnen", "Zwischenraum", "Gelegenheitsangriff" });

            migrationBuilder.UpdateData(
                schema: "stamm",
                table: "Faecher",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Beschreibung", "HafenText", "PerkAnger", "PerkDom", "PerkGarten", "PerkHafen" },
                values: new object[] { "Alchemie an der Grenze des Machbaren. Grenzchemiker brauen Tränke, die weiter reichen als gewöhnlich, und verbessern nebenbei ihre Widerstandskraft gegen Verderbnis.", null, "Reisebraukit", "Grenztrank", "Eigenes Rezept", "Erstes Braukit" });

            migrationBuilder.UpdateData(
                schema: "stamm",
                table: "Faecher",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Beschreibung", "HafenText", "PerkAnger", "PerkDom", "PerkGarten", "PerkHafen" },
                values: new object[] { "Die Grundlagenlehre der Zauberei selbst. Magiatur schärft das Verständnis für die eigene Kraft: Mit jeder Stufe lässt sich wählen, ob man lieber einen neuen Zauber erlernt oder die Zahl der Zauberslots erweitert.", null, "Zauberbuch", "Eigener Zauber", "3 Slots", "2 Slots" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Beschreibung",
                schema: "stamm",
                table: "Faecher");

            migrationBuilder.DropColumn(
                name: "HafenText",
                schema: "stamm",
                table: "Faecher");

            migrationBuilder.DropColumn(
                name: "PerkAnger",
                schema: "stamm",
                table: "Faecher");

            migrationBuilder.DropColumn(
                name: "PerkDom",
                schema: "stamm",
                table: "Faecher");

            migrationBuilder.DropColumn(
                name: "PerkGarten",
                schema: "stamm",
                table: "Faecher");

            migrationBuilder.DropColumn(
                name: "PerkHafen",
                schema: "stamm",
                table: "Faecher");
        }
    }
}
