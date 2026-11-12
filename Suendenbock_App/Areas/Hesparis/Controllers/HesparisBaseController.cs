using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Controllers;
using Suendenbock_App.Data;
using Suendenbock_App.Data.Hesparis;
using Suendenbock_App.Models.Domain.Hesparis;
using Suendenbock_App.Models.ViewModels.Hesparis;

namespace Suendenbock_App.Areas.Hesparis.Controllers
{
    /// <summary>
    /// Gemeinsame Grundlage aller Hesparis-Seiten (Area "Hesparis", erreichbar unter /Hesparis).
    /// Rechte: Ansehen alle Eingeloggten, Spielercharaktere bearbeiten alle Spieler und Gott,
    /// Verwaltung (Charaktere, Noten, Spieltag) nur Gott ("Gott-Modus").
    /// </summary>
    [Area("Hesparis")]
    [Authorize]
    public abstract class HesparisBaseController : BaseController
    {
        protected HesparisBaseController(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>Für das Layout: der eigene Hesparis-Charakter (oben rechts) und der Spieltag (Kopfleiste).</summary>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.Spieltag = await _context.Kampagnen
                .OrderBy(k => k.Id)
                .Select(k => (DateOnly?)k.Spieltag)
                .FirstOrDefaultAsync();

            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                ViewBag.MeinHesparisCharakter = await _context.HesparisCharaktere
                    .FirstOrDefaultAsync(h => h.UserId == userId);
            }
            await base.OnActionExecutionAsync(context, next);
        }

        /// <summary>Darf Spielercharaktere bearbeiten: jeder Spieler (alle Figuren) und Gott.</summary>
        protected bool DarfBearbeiten() => IsGott() || User.IsInRole("Spieler");

        /// <summary>
        /// Die Akademie (es gibt genau eine) und dazu passende Spielwerte für jeden Spielercharakter.
        /// </summary>
        protected async Task<Kampagne> AkademieLadenAsync()
        {
            var akademie = await HesparisAkademie.HolenOderAnlegenAsync(_context);
            await SpielfigurenAbgleichenAsync(akademie);
            return akademie;
        }

        /// <summary>
        /// Jeder Hesparis-Charakter der Art "Spieler" bekommt Spielwerte (Spielfigur).
        /// Leere Spielwerte ohne passenden Spielercharakter (z.B. Art auf Lehrkraft geändert) werden entfernt;
        /// Spielwerte mit Einträgen bleiben sicherheitshalber stehen.
        /// </summary>
        private async Task SpielfigurenAbgleichenAsync(Kampagne akademie)
        {
            var leereFremde = await _context.Spielfiguren
                .Where(f => f.KampagneId == akademie.Id)
                .Where(f => f.HesparisCharakter == null || f.HesparisCharakter.CharakterartId != Charakterart.Spieler)
                .Where(f => f.Geld == 0 && !f.Talente.Any() && !f.Noten.Any() && !f.FachStufen.Any()
                            && !f.Gegenstaende.Any() && !f.Wirkungen.Any() && !f.Geldbuchungen.Any())
                .ToListAsync();
            if (leereFremde.Count > 0)
            {
                _context.Spielfiguren.RemoveRange(leereFremde);
                await _context.SaveChangesAsync();
            }

            var ohneFigur = await _context.HesparisCharaktere
                .Where(h => h.CharakterartId == Charakterart.Spieler && h.Spielfigur == null)
                .OrderBy(h => h.Id)
                .ToListAsync();
            if (ohneFigur.Count == 0) return;

            var belegt = await _context.Spielfiguren
                .Where(f => f.KampagneId == akademie.Id)
                .Select(f => f.Position)
                .ToListAsync();

            foreach (var charakter in ohneFigur)
            {
                // Die fünf aus der hesparis.html bekommen ihren alten Platz (Elma 0, Levi 1 …), alle anderen kommen dahinter
                int position = HesparisAkademie.PositionVon(charakter.Vorname, charakter.Nachname) ?? HesparisAkademie.Spielfiguren.Length;
                while (belegt.Contains(position))
                {
                    position++;
                }
                belegt.Add(position);

                _context.Spielfiguren.Add(new Spielfigur
                {
                    KampagneId = akademie.Id,
                    HesparisCharakterId = charakter.Id,
                    Position = position,
                });
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Lädt eine Figur samt Akademie und Hesparis-Charakter. Ohne Id: die eigene Figur, sonst die erste.
        /// </summary>
        protected async Task<Spielfigur?> FigurLadenAsync(int? id, Func<IQueryable<Spielfigur>, IQueryable<Spielfigur>> includes)
        {
            var akademie = await AkademieLadenAsync();

            if (!id.HasValue)
            {
                var userId = GetUserId();
                id = await _context.Spielfiguren
                    .Where(f => f.KampagneId == akademie.Id && f.HesparisCharakter != null)
                    .OrderByDescending(f => f.HesparisCharakter!.UserId == userId)
                    .ThenBy(f => f.Position)
                    .Select(f => (int?)f.Id)
                    .FirstOrDefaultAsync();
                if (!id.HasValue) return null;
            }

            var query = _context.Spielfiguren
                .Include(f => f.Kampagne)
                .Include(f => f.HesparisCharakter)
                .Where(f => f.HesparisCharakter != null)
                .AsSplitQuery();
            return await includes(query).FirstOrDefaultAsync(f => f.Id == id.Value);
        }

        protected IActionResult KeineFigur() => View("KeineKampagne");

        protected Spielfigur StandardFigur(List<Spielfigur> figuren)
        {
            // Die eigene Figur, sonst die erste
            var userId = GetUserId();
            return figuren.FirstOrDefault(f => f.HesparisCharakter!.UserId == userId) ?? figuren[0];
        }

        protected FigurKopfViewModel KopfErstellen(Kampagne kampagne, List<Spielfigur> figuren, Spielfigur aktiv, string aktion)
        {
            var userId = GetUserId();
            return new FigurKopfViewModel
            {
                Kampagne = kampagne,
                AktiveFigurId = aktiv.Id,
                Aktion = aktion,
                IstSpielleitung = IsGott(),
                DarfBearbeiten = DarfBearbeiten(),
                Figuren = figuren.Select(f => new FigurKurz
                {
                    Id = f.Id,
                    Name = f.HesparisCharakter!.Anzeigename,
                    Bild = f.HesparisCharakter.BildPfad,
                    Eigene = f.HesparisCharakter.UserId == userId,
                }).ToList(),
            };
        }

        protected async Task<FigurKopfViewModel> KopfFuerFigurAsync(Spielfigur figur, string aktion)
        {
            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == figur.KampagneId && f.HesparisCharakter != null)
                .Include(f => f.HesparisCharakter)
                .OrderBy(f => f.Position)
                .ToListAsync();
            return KopfErstellen(figur.Kampagne, figuren, figuren.First(f => f.Id == figur.Id), aktion);
        }
    }
}
