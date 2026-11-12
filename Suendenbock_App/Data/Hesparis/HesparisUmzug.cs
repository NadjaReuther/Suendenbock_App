using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Data.Hesparis;

/// <summary>
/// Einmaliger Umzug aus dem alten Tenant-System in die eigene Hesparis-Tabelle.
///
/// Für jeden Character der alten Spielwelt "Hesparis" (bzw. "1810 Season Zwei"):
///  * Es entsteht ein HesparisCharakter (Name, Bild, Spieler werden übernommen).
///  * Nur in Hesparis (Elma, Levi …): Spielwerte umhängen, Character in Sündenbock löschen.
///  * Auch in Sündenbock: bleibt dort, wird über CharakterVerknuepfung verbunden.
///
/// Läuft bei jedem Start, macht aber nur etwas, solange noch Zuordnungen zur alten Welt existieren.
/// Sobald alles umgezogen ist, fliegen GameWorld, CharacterGameWorld und diese Klasse raus.
/// </summary>
public static class HesparisUmzug
{
    public static async Task AusfuehrenAsync(ApplicationDbContext db, ILogger logger)
    {
        var hesparisWelt = await db.GameWorlds
            .FirstOrDefaultAsync(w => w.Name.Contains("Hesparis") || w.Name.Contains("1810"));
        if (hesparisWelt == null)
        {
            return;
        }

        var suendenbockWelten = await db.GameWorlds
            .Where(w => w.Id != hesparisWelt.Id)
            .Select(w => w.Id)
            .ToListAsync();

        var zuordnungen = await db.CharacterGameWorlds
            .Include(cgw => cgw.Character)
            .Where(cgw => cgw.GameWorldId == hesparisWelt.Id)
            .ToListAsync();

        foreach (var zuordnung in zuordnungen)
        {
            var character = zuordnung.Character;
            bool auchInSuendenbock = await db.CharacterGameWorlds
                .AnyAsync(o => o.CharacterId == character.Id && suendenbockWelten.Contains(o.GameWorldId));

            // Spieler: bei reinen Hesparis-Characters steht er im Character oder (seit dem letzten Umbau) an der Zuordnung.
            // Bei Characters, die auch in Sündenbock sind, gehört Character.UserId zum Sündenbock-Hauptcharakter.
            string? spielerId = zuordnung.UserId;
            if (spielerId == null && !auchInSuendenbock)
            {
                spielerId = character.UserId;
            }

            // 1. Hesparis-Charakter anlegen (oder den aus einem früheren Lauf wiederfinden) und verknüpfen
            var verknuepfung = await db.CharakterVerknuepfungen
                .Include(v => v.HesparisCharakter)
                .FirstOrDefaultAsync(v => v.CharacterId == character.Id);

            HesparisCharakter hesparisCharakter;
            if (verknuepfung != null)
            {
                hesparisCharakter = verknuepfung.HesparisCharakter;
            }
            else
            {
                int art = Charakterart.Mitschueler;
                if (spielerId != null)
                {
                    art = Charakterart.Spieler;
                }

                hesparisCharakter = new HesparisCharakter
                {
                    Vorname = character.Vorname,
                    Nachname = character.Nachname,
                    Rufname = character.Rufname,
                    BildPfad = character.ImagePath,
                    CharakterartId = art,
                    UserId = spielerId,
                };
                db.HesparisCharaktere.Add(hesparisCharakter);
                db.CharakterVerknuepfungen.Add(new CharakterVerknuepfung
                {
                    HesparisCharakter = hesparisCharakter,
                    CharacterId = character.Id,
                });
                await db.SaveChangesAsync();
                logger.LogInformation("Hesparis-Umzug: {Name} als Hesparis-Charakter angelegt", hesparisCharakter.VollerName);
            }

            // 2. Spielwerte (Talente, Noten, Tasche …) an den Hesparis-Charakter hängen
            var spielfigur = await db.Spielfiguren.FirstOrDefaultAsync(f => f.CharacterId == character.Id);
            if (spielfigur != null)
            {
                if (spielfigur.HesparisCharakterId == null)
                {
                    spielfigur.HesparisCharakterId = hesparisCharakter.Id;
                }
                spielfigur.CharacterId = null;
            }

            // Die alte Zuordnung zur Spielwelt wird nicht mehr gebraucht
            db.CharacterGameWorlds.Remove(zuordnung);

            if (auchInSuendenbock)
            {
                await db.SaveChangesAsync();
                continue;
            }

            // 3. Reiner Hesparis-Character: in Sündenbock löschen
            var benutzerMitVerweis = await db.Users.Where(u => u.CurrentCharacterId == character.Id).ToListAsync();
            foreach (var benutzer in benutzerMitVerweis)
            {
                benutzer.CurrentCharacterId = null;
            }
            var alteVerknuepfung = await db.CharakterVerknuepfungen.FirstOrDefaultAsync(v => v.CharacterId == character.Id);
            if (alteVerknuepfung != null)
            {
                db.CharakterVerknuepfungen.Remove(alteVerknuepfung);
            }
            var weitereZuordnungen = await db.CharacterGameWorlds.Where(o => o.CharacterId == character.Id && o.Id != zuordnung.Id).ToListAsync();
            db.CharacterGameWorlds.RemoveRange(weitereZuordnungen);
            db.Characters.Remove(character);

            try
            {
                await db.SaveChangesAsync();
                logger.LogInformation("Hesparis-Umzug: {Name} aus Sündenbock gelöscht", hesparisCharakter.VollerName);
            }
            catch (DbUpdateException ex)
            {
                // Hängt in Sündenbock noch etwas am Character (z.B. als Elternteil eingetragen) → stehen lassen,
                // Verknüpfung bleibt bestehen, beim nächsten Start wird es erneut versucht
                logger.LogWarning(ex, "Hesparis-Umzug: {Name} konnte in Sündenbock nicht gelöscht werden", hesparisCharakter.VollerName);
                db.ChangeTracker.Clear();
            }
        }
    }
}
