using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;
using Suendenbock_App.Services;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Gott-Modus: Hesparis-Charaktere anlegen, bearbeiten, löschen (/Hesparis/Charakter).
    /// Ein Charakter kann mit einem Sündenbock-Character verknüpft werden (dieselbe Person in beiden Welten).
    /// </summary>
    [Authorize(Roles = "Gott")]
    public class CharakterController : HesparisBaseController
    {
        private readonly IImageUploadService _imageUploadService;

        public CharakterController(ApplicationDbContext context, IImageUploadService imageUploadService) : base(context)
        {
            _imageUploadService = imageUploadService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var charaktere = await _context.HesparisCharaktere
                .Include(h => h.Charakterart)
                .Include(h => h.User)
                .Include(h => h.Verknuepfung).ThenInclude(v => v!.Character)
                .OrderBy(h => h.CharakterartId)
                .ThenBy(h => h.Vorname)
                .ToListAsync();

            var model = new CharakterIndexViewModel
            {
                Arten = await _context.Charakterarten.OrderBy(a => a.Id).ToListAsync(),
                Charaktere = charaktere,
            };
            return View(model);
        }

        /// <summary>
        /// Neuer Charakter. Mit characterId (aus dem Sündenbock-Charakterbogen) werden Name und Bild
        /// vorausgefüllt und der Charakter gleich verknüpft.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Neu(int? characterId)
        {
            var formular = new CharakterFormular { CharakterartId = Charakterart.Mitschueler };

            if (characterId.HasValue)
            {
                var vorhanden = await _context.CharakterVerknuepfungen.FirstOrDefaultAsync(v => v.CharacterId == characterId.Value);
                if (vorhanden != null)
                {
                    // Schon verknüpft → direkt bearbeiten
                    return RedirectToAction(nameof(Bearbeiten), new { id = vorhanden.HesparisCharakterId });
                }

                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == characterId.Value);
                if (character != null)
                {
                    formular.Vorname = character.Vorname;
                    formular.Nachname = character.Nachname;
                    formular.Rufname = character.Rufname;
                    formular.BildPfad = character.ImagePath;
                    formular.CharacterId = character.Id;
                }
            }

            return View("Formular", await FormularModellAsync(formular));
        }

        [HttpGet]
        public async Task<IActionResult> Bearbeiten(int id)
        {
            var charakter = await _context.HesparisCharaktere
                .Include(h => h.Verknuepfung)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (charakter == null) return NotFound();

            var formular = new CharakterFormular
            {
                Id = charakter.Id,
                Vorname = charakter.Vorname,
                Nachname = charakter.Nachname,
                Rufname = charakter.Rufname,
                BildPfad = charakter.BildPfad,
                CharakterartId = charakter.CharakterartId,
                UserId = charakter.UserId,
                CharacterId = charakter.Verknuepfung?.CharacterId,
            };
            return View("Formular", await FormularModellAsync(formular));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Speichern(CharakterFormular formular, IFormFile? bild)
        {
            if (string.IsNullOrWhiteSpace(formular.Vorname) || !await _context.Charakterarten.AnyAsync(a => a.Id == formular.CharakterartId))
            {
                TempData["Error"] = "Bitte mindestens Vorname und Art angeben.";
                return View("Formular", await FormularModellAsync(formular));
            }

            // Sündenbock-Character darf nur mit einem Hesparis-Charakter verknüpft sein
            if (formular.CharacterId.HasValue)
            {
                bool schonVergeben = await _context.CharakterVerknuepfungen
                    .AnyAsync(v => v.CharacterId == formular.CharacterId.Value && v.HesparisCharakterId != formular.Id);
                if (schonVergeben)
                {
                    TempData["Error"] = "Dieser Sündenbock-Character ist schon mit einem anderen Hesparis-Charakter verknüpft.";
                    return View("Formular", await FormularModellAsync(formular));
                }
            }

            HesparisCharakter charakter;
            if (formular.Id.HasValue)
            {
                var vorhanden = await _context.HesparisCharaktere
                    .Include(h => h.Verknuepfung)
                    .FirstOrDefaultAsync(h => h.Id == formular.Id.Value);
                if (vorhanden == null) return NotFound();
                charakter = vorhanden;
            }
            else
            {
                charakter = new HesparisCharakter();
                _context.HesparisCharaktere.Add(charakter);
            }

            charakter.Vorname = formular.Vorname.Trim();
            charakter.Nachname = formular.Nachname?.Trim();
            charakter.Rufname = formular.Rufname?.Trim();
            charakter.BildPfad = formular.BildPfad;
            charakter.CharakterartId = formular.CharakterartId;
            // Spieler nur bei Spielercharakteren
            if (formular.CharakterartId == Charakterart.Spieler)
            {
                charakter.UserId = string.IsNullOrEmpty(formular.UserId) ? null : formular.UserId;
            }
            else
            {
                charakter.UserId = null;
            }

            if (bild != null && bild.Length > 0)
            {
                try
                {
                    charakter.BildPfad = await _imageUploadService.UploadImageAsync(bild, "hesparis", $"{charakter.Vorname}_{charakter.Nachname}_{DateTime.Now:yyyyMMddHHmmss}");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Bild-Upload fehlgeschlagen: {ex.Message}";
                }
            }

            // Verknüpfung mit Sündenbock setzen, ändern oder entfernen
            if (formular.CharacterId.HasValue)
            {
                if (charakter.Verknuepfung == null)
                {
                    charakter.Verknuepfung = new CharakterVerknuepfung { CharacterId = formular.CharacterId.Value };
                }
                else
                {
                    charakter.Verknuepfung.CharacterId = formular.CharacterId.Value;
                }
            }
            else if (charakter.Verknuepfung != null)
            {
                _context.CharakterVerknuepfungen.Remove(charakter.Verknuepfung);
            }

            await _context.SaveChangesAsync();

            if (TempData["Error"] == null)
            {
                TempData["Success"] = $"{charakter.VollerName} gespeichert.";
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Charakter löschen. Bei Spielercharakteren verschwinden auch Talente, Noten, Tasche …
        /// Der verknüpfte Sündenbock-Character bleibt erhalten.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Loeschen(int id)
        {
            var charakter = await _context.HesparisCharaktere.FirstOrDefaultAsync(h => h.Id == id);
            if (charakter == null) return NotFound();

            // Lehrkraft-/Mitschüler-Zusatzdaten nur lösen, nicht löschen
            var lehrer = await _context.Lehrer.Where(l => l.HesparisCharakterId == id).ToListAsync();
            foreach (var l in lehrer) l.HesparisCharakterId = null;
            var mitschueler = await _context.Mitschueler.Where(m => m.HesparisCharakterId == id).ToListAsync();
            foreach (var m in mitschueler) m.HesparisCharakterId = null;

            _context.HesparisCharaktere.Remove(charakter);
            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = $"{charakter.VollerName} gelöscht.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Der Charakter hängt noch an anderen Einträgen (z. B. Nachsitzen, Klassenarbeiten) und kann nicht gelöscht werden.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<CharakterFormularViewModel> FormularModellAsync(CharakterFormular formular)
        {
            // Spieler = Benutzer mit Rolle "Spieler"
            var spielerRolle = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Spieler");
            var spielerIds = new List<string>();
            if (spielerRolle != null)
            {
                spielerIds = await _context.UserRoles.Where(ur => ur.RoleId == spielerRolle.Id).Select(ur => ur.UserId).ToListAsync();
            }

            // Sündenbock-Characters, die noch frei sind (oder schon mit diesem verknüpft)
            var vergeben = await _context.CharakterVerknuepfungen
                .Where(v => v.HesparisCharakterId != formular.Id)
                .Select(v => v.CharacterId)
                .ToListAsync();

            return new CharakterFormularViewModel
            {
                Formular = formular,
                Arten = await _context.Charakterarten.OrderBy(a => a.Id).ToListAsync(),
                Spieler = await _context.Users.Where(u => spielerIds.Contains(u.Id)).OrderBy(u => u.UserName).ToListAsync(),
                SuendenbockCharacters = await _context.Characters
                    .Where(c => !vergeben.Contains(c.Id))
                    .OrderBy(c => c.Vorname).ThenBy(c => c.Nachname)
                    .ToListAsync(),
            };
        }
    }
}
