using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Controllers
{
    /// <summary>
    /// Spielleitung für Hesparis: Kampagnen anlegen, einer Spielwelt zuordnen
    /// und Sündenbock-Characters als Spielfiguren hinzufügen.
    /// </summary>
    [Authorize(Roles = "Gott")]
    public class HesparisSpielleitungController : BaseController
    {
        public HesparisSpielleitungController(ApplicationDbContext context) : base(context)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new SpielleitungIndexViewModel
            {
                Welten = await _context.GameWorlds.OrderBy(w => w.SortOrder).ToListAsync(),
                Kampagnen = await _context.Kampagnen
                    .OrderByDescending(k => k.Id)
                    .Select(k => new KampagneZeile
                    {
                        Kampagne = k,
                        Welt = k.GameWorld.Name,
                        Figuren = k.Spielfiguren.Count,
                    })
                    .ToListAsync(),
            };
            model.Neu.GameWorldId = model.Welten.FirstOrDefault()?.Id ?? 0;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Anlegen(KampagneFormular formular)
        {
            if (string.IsNullOrWhiteSpace(formular.Name) || !await _context.GameWorlds.AnyAsync(w => w.Id == formular.GameWorldId))
            {
                TempData["Error"] = "Bitte Name und Spielwelt angeben.";
                return RedirectToAction(nameof(Index));
            }

            var kampagne = new Kampagne
            {
                Name = formular.Name.Trim(),
                GameWorldId = formular.GameWorldId,
                Spieltag = formular.Spieltag,
                Kalenderjahr = formular.Spieltag.Month >= 8 ? formular.Spieltag.Year : formular.Spieltag.Year - 1,
                Schuljahr = Math.Max(1, formular.Schuljahr),
            };
            _context.Kampagnen.Add(kampagne);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Kampagne „{kampagne.Name}“ angelegt. Jetzt die Spielfiguren hinzufügen.";
            return RedirectToAction(nameof(Figuren), new { id = kampagne.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Figuren(int id)
        {
            var kampagne = await _context.Kampagnen.Include(k => k.GameWorld).FirstOrDefaultAsync(k => k.Id == id);
            if (kampagne == null) return NotFound();

            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == id)
                .Include(f => f.Character)
                .OrderBy(f => f.Position)
                .ToListAsync();

            var benutzer = await _context.Users
                .Select(u => new { u.Id, u.UserName })
                .ToDictionaryAsync(u => u.Id, u => u.UserName);

            var vergeben = figuren.Select(f => f.CharacterId).ToList();
            var inWelt = await _context.CharacterGameWorlds
                .Where(cgw => cgw.GameWorldId == kampagne.GameWorldId)
                .Select(cgw => cgw.CharacterId)
                .ToListAsync();

            var characters = await _context.Characters
                .Where(c => !vergeben.Contains(c.Id))
                .OrderBy(c => c.Vorname)
                .ToListAsync();

            var model = new SpielleitungFigurenViewModel
            {
                Kampagne = kampagne,
                Welten = await _context.GameWorlds.OrderBy(w => w.SortOrder).ToListAsync(),
                Figuren = figuren.Select(f => new SpielleitungFigurZeile
                {
                    Figur = f,
                    Name = f.Character.Vorname + " " + f.Character.Nachname,
                    Spieler = f.Character.UserId != null ? benutzer.GetValueOrDefault(f.Character.UserId) : null,
                }).ToList(),
                Verfuegbar = characters
                    .Select(c => new CharacterAuswahl
                    {
                        Id = c.Id,
                        Name = (c.Vorname + " " + c.Nachname).Trim(),
                        HatSpieler = c.UserId != null,
                        InWelt = inWelt.Contains(c.Id),
                    })
                    // Spieler-Characters und Characters der Welt zuerst
                    .OrderByDescending(c => c.HatSpieler)
                    .ThenByDescending(c => c.InWelt)
                    .ThenBy(c => c.Name)
                    .ToList(),
            };
            return View(model);
        }

        /// <summary>Name, Welt, Spieltag und Schuljahr einer Kampagne ändern.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bearbeiten(KampagneFormular formular)
        {
            var kampagne = await _context.Kampagnen.FirstOrDefaultAsync(k => k.Id == formular.Id);
            if (kampagne == null) return NotFound();
            if (string.IsNullOrWhiteSpace(formular.Name) || !await _context.GameWorlds.AnyAsync(w => w.Id == formular.GameWorldId))
            {
                TempData["Error"] = "Bitte Name und Spielwelt angeben.";
                return RedirectToAction(nameof(Figuren), new { id = kampagne.Id });
            }

            bool weltGewechselt = kampagne.GameWorldId != formular.GameWorldId;
            kampagne.Name = formular.Name.Trim();
            kampagne.GameWorldId = formular.GameWorldId;
            kampagne.Spieltag = formular.Spieltag;
            kampagne.Schuljahr = Math.Max(1, formular.Schuljahr);

            if (weltGewechselt)
            {
                // Alle Figuren auch der neuen Welt zuordnen
                var characterIds = await _context.Spielfiguren
                    .Where(f => f.KampagneId == kampagne.Id)
                    .Select(f => f.CharacterId)
                    .ToListAsync();
                foreach (var characterId in characterIds)
                    await CharacterInWeltSicherstellenAsync(characterId, kampagne.GameWorldId);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Kampagne gespeichert.";
            return RedirectToAction(nameof(Figuren), new { id = kampagne.Id });
        }

        /// <summary>
        /// Character als Spielfigur hinzufügen. Ist er noch nicht in der Spielwelt der Kampagne,
        /// wird er dort automatisch eingetragen (CharacterGameWorld, nicht als Ursprungswelt).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FigurHinzufuegen(int id, int characterId)
        {
            var kampagne = await _context.Kampagnen.FirstOrDefaultAsync(k => k.Id == id);
            if (kampagne == null) return NotFound();
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == characterId);
            if (character == null) return NotFound();

            if (await _context.Spielfiguren.AnyAsync(f => f.KampagneId == id && f.CharacterId == characterId))
            {
                TempData["Error"] = "Dieser Character spielt schon mit.";
                return RedirectToAction(nameof(Figuren), new { id });
            }

            int position = await _context.Spielfiguren
                .Where(f => f.KampagneId == id)
                .Select(f => (int?)f.Position)
                .MaxAsync() ?? -1;

            _context.Spielfiguren.Add(new Spielfigur
            {
                KampagneId = id,
                CharacterId = characterId,
                Position = position + 1,
            });
            await CharacterInWeltSicherstellenAsync(characterId, kampagne.GameWorldId);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{FigurKurz.NameVon(character)} ist jetzt in der Akademie.";
            return RedirectToAction(nameof(Figuren), new { id });
        }

        /// <summary>
        /// Spielfigur entfernen. Noten, Talente und Tasche verschwinden mit.
        /// Klappt nicht, solange z. B. ein Nachsitzen oder eine Klassenarbeit auf die Figur zeigt.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FigurEntfernen(int id, int figurId)
        {
            var figur = await _context.Spielfiguren.FirstOrDefaultAsync(f => f.Id == figurId && f.KampagneId == id);
            if (figur == null) return NotFound();

            _context.Spielfiguren.Remove(figur);
            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Spielfigur entfernt.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Die Figur hängt noch an anderen Einträgen (z. B. Nachsitzen, Klassenarbeiten) und kann nicht entfernt werden.";
            }
            return RedirectToAction(nameof(Figuren), new { id });
        }

        /// <summary>Reihenfolge ändern: eine Figur einen Platz nach oben oder unten.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FigurVerschieben(int id, int figurId, bool nachOben)
        {
            var figuren = await _context.Spielfiguren.Where(f => f.KampagneId == id).OrderBy(f => f.Position).ToListAsync();
            int index = figuren.FindIndex(f => f.Id == figurId);
            int ziel = nachOben ? index - 1 : index + 1;
            if (index < 0 || ziel < 0 || ziel >= figuren.Count)
                return RedirectToAction(nameof(Figuren), new { id });

            // Über einen freien Zwischenwert tauschen, wegen des eindeutigen Index (KampagneId, Position)
            var a = figuren[index];
            var b = figuren[ziel];
            int posA = a.Position, posB = b.Position;
            a.Position = -1;
            await _context.SaveChangesAsync();
            b.Position = posA;
            await _context.SaveChangesAsync();
            a.Position = posB;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Figuren), new { id });
        }

        private async Task CharacterInWeltSicherstellenAsync(int characterId, int gameWorldId)
        {
            bool vorhanden = await _context.CharacterGameWorlds
                .AnyAsync(cgw => cgw.CharacterId == characterId && cgw.GameWorldId == gameWorldId);
            bool schonGeplant = _context.ChangeTracker.Entries<CharacterGameWorld>()
                .Any(e => e.State == EntityState.Added && e.Entity.CharacterId == characterId && e.Entity.GameWorldId == gameWorldId);

            if (!vorhanden && !schonGeplant)
            {
                _context.CharacterGameWorlds.Add(new CharacterGameWorld
                {
                    CharacterId = characterId,
                    GameWorldId = gameWorldId,
                    IsOriginWorld = false,
                });
            }
        }
    }
}
