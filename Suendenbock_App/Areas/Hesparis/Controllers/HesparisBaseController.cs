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
            var akademie = await _context.Kampagnen
                .OrderBy(k => k.Id)
                .Select(k => new { k.Spieltag, k.BrauenGesperrt })
                .FirstOrDefaultAsync();
            if (akademie != null)
            {
                ViewBag.Spieltag = (DateOnly?)akademie.Spieltag;
                // Im Menü steht dann "gesperrt" neben der Trankküche
                ViewBag.BrauenGesperrt = akademie.BrauenGesperrt;
            }

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

        /// <summary>Figurenknöpfe mit Farbe aus dem Sonderzauber (JS: mdPlayers / plCol). Aus dem MagieController hierher verschoben, das Tagebuch braucht die Farben auch.</summary>
        protected async Task<List<FigurChip>> FigurChipsAsync(int kampagneId, int? aktivId = null)
        {
            var figuren = await _context.Spielfiguren
                .Where(f => f.KampagneId == kampagneId && f.HesparisCharakter != null)
                .Include(f => f.HesparisCharakter)
                .OrderBy(f => f.Position)
                .ToListAsync();
            var sonderzauber = await _context.Sonderzauber.ToListAsync();

            return figuren.Select(f =>
            {
                var name = f.HesparisCharakter!.Anzeigename;
                var zauber = sonderzauber.FirstOrDefault(z => z.Person == name);
                return new FigurChip
                {
                    Id = f.Id,
                    Name = name,
                    Aussehen = HesparisSigille.Fuer(zauber?.Code),
                    Aktiv = f.Id == aktivId,
                };
            }).ToList();
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

        // =====================================================================
        // Talentbogen (aus dem FigurController hierher verschoben, damit auch
        // die Trankküche den Metallurgie-Wert ausrechnen kann)
        // =====================================================================

        protected Task<Spielfigur?> FigurMitTalentenLadenAsync(int? id) => FigurLadenAsync(id, q => q
            .Include(f => f.Talente)
            .Include(f => f.TalentZiele)
            .Include(f => f.FachStufen)
            .Include(f => f.StufenWahlen)
            .Include(f => f.Meisterschaften)
            .Include(f => f.Gegenstaende)
            .Include(f => f.Wirkungen));

        /// <summary>
        /// Rechnet den kompletten Talentbogen aus (JS: talCalc): Punkte, Kopfwerte und alle Boni.
        /// Wird von der Talente-Seite, beim Wählen einer Meisterschaft und in der Trankküche benutzt.
        /// aktion = Seite, auf die die Figurenknöpfe im Kopf verlinken.
        /// </summary>
        protected async Task<TalenteViewModel> TalenteBerechnenAsync(Spielfigur figur, string aktion = "Talente")
        {
            var kampagne = figur.Kampagne;
            var begabungen = await _context.Begabungen
                .Include(b => b.Talente).ThenInclude(t => t.Meisterschaften)
                .AsSplitQuery()
                .ToListAsync();
            var faecher = await _context.Faecher.Include(f => f.Stufen).ToListAsync();

            // Alle Boni sammeln: Magiestufen, AGs, Tasche, Wirkungen (JS: talCalc)
            var boni = HesparisRegeln.FachBoni(figur, faecher);
            boni.AddRange(await AgBoniAsync(figur));
            var (tascheBoni, vorteile) = await TascheBoniAsync(figur, kampagne);
            boni.AddRange(tascheBoni);

            var talentNamen = begabungen.SelectMany(b => b.Talente).ToDictionary(t => t.Code, t => t.Name);
            var punkte = figur.Talente.ToDictionary(t => t.TalentCode, t => t.Wert);
            var gewaehlt = figur.Meisterschaften.Select(m => m.MeisterschaftDefinitionId).ToHashSet();

            var model = new TalenteViewModel
            {
                Kopf = await KopfFuerFigurAsync(figur, aktion),
                Boni = boni,
                Vorteile = vorteile,
                TalentNamen = talentNamen,
                MeisterschaftenFreigeschaltet =
                    (figur.FachStufen.FirstOrDefault(s => s.FachId == HesparisRegeln.FachChronotaktik)?.Stufe ?? 0) >= 3,
            };

            foreach (var b in begabungen.OrderBy(b => "hws".IndexOf(b.Code)))
            {
                var talente = b.Talente.OrderBy(t => t.Position).ToList();
                int summe = talente.Sum(t => punkte.GetValueOrDefault(t.Code));
                int kopfwert = HesparisRegeln.Kopfwert(summe);

                var zeile = new BegabungZeile
                {
                    Code = b.Code,
                    Name = b.Name,
                    Farbe = HesparisRegeln.BegabungFarbe.GetValueOrDefault(b.Code, "#e7d6a3"),
                    Summe = summe,
                    Kopfwert = kopfwert,
                    Geistesblitze = HesparisRegeln.Geistesblitze(kopfwert),
                };

                foreach (var t in talente)
                {
                    var talentBoni = boni.Where(x => x.Ziel == t.Code).ToList();
                    int bonus = talentBoni.Sum(x => x.Betrag);
                    int p = punkte.GetValueOrDefault(t.Code);
                    zeile.Talente.Add(new TalentZeile
                    {
                        Code = t.Code,
                        Name = t.Name,
                        Punkte = p,
                        Bonus = bonus,
                        Boni = talentBoni,
                        Gesamt = p + kopfwert + bonus,
                        Meisterschaften = t.Meisterschaften.OrderBy(m => m.Position).Select(m => new MeisterschaftZeile
                        {
                            Id = m.Id,
                            Name = m.Name,
                            Beschreibung = m.Beschreibung,
                            Gewaehlt = gewaehlt.Contains(m.Id),
                        }).ToList(),
                    });
                }

                model.Verteilt += summe;
                model.Begabungen.Add(zeile);
            }

            return model;
        }

        /// <summary>Boni aus AGs, in denen die Figur Mitglied ist (JS: agBoni).</summary>
        protected async Task<List<TalentBonus>> AgBoniAsync(Spielfigur figur)
        {
            var ags = await _context.AgMitglieder
                .Where(m => m.SpielfigurId == figur.Id && m.AgEintrag.KampagneId == figur.KampagneId)
                .Select(m => m.AgEintrag.Ag)
                .Where(a => a.BonusTalentCode != null && a.BonusWert != 0)
                .ToListAsync();

            return ags.Select(a => new TalentBonus
            {
                Quelle = a.Name,
                Herkunft = "AG",
                Betrag = a.BonusWert,
                Beschriftung = "+" + a.BonusWert,
                Kandidaten = new List<string> { a.BonusTalentCode! },
                Ziel = a.BonusTalentCode,
            }).ToList();
        }

        /// <summary>
        /// Boni und Vorteile aus der Tasche (Bücher, Ausrüstung, Sonstiges) und aus laufenden Wirkungen
        /// (JS: fxActive / itemBoni). Gegenstände ohne eigene Wirkung übernehmen die Wirkung der
        /// gleichnamigen Ware im Schulladen.
        /// </summary>
        protected async Task<(List<TalentBonus> Boni, List<TalentBonus> Vorteile)> TascheBoniAsync(Spielfigur figur, Kampagne kampagne)
        {
            var laden = await _context.LadenArtikel.Where(l => l.KampagneId == figur.KampagneId).ToListAsync();
            var aktiv = new List<(string Name, Effekt Fx, Wirkung? Wirkung, string Herkunft)>();

            foreach (var g in figur.Gegenstaende.Where(g => g.Menge > 0 && HesparisRegeln.PassiveKategorien.Contains(g.Kategorie)))
            {
                var fx = HesparisRegeln.EffektGueltig(g.Effekt) ? g.Effekt : laden.FirstOrDefault(l => l.Name == g.Name)?.Effekt;
                if (HesparisRegeln.EffektGueltig(fx)) aktiv.Add((g.Name, fx!, null, "in der Tasche"));
            }
            foreach (var w in figur.Wirkungen.Where(w => HesparisRegeln.WirkungAktiv(w, kampagne.Spieltag)))
                aktiv.Add((w.Name, w.Effekt, w, "wirkt gerade"));

            var boni = new List<TalentBonus>();
            var vorteile = new List<TalentBonus>();
            foreach (var (name, fx, wirkung, herkunft) in aktiv)
            {
                var bonus = new TalentBonus
                {
                    Quelle = name,
                    Herkunft = herkunft,
                    WirkungId = wirkung?.Id,
                    Anwendungen = fx.DauerArt == EffektDauerArt.Anwendungen,
                    Dauer = wirkung != null ? HesparisRegeln.WirkungRest(wirkung, kampagne.Spieltag) : null,
                };
                if (fx.Art == EffektArt.Talent)
                {
                    bonus.Betrag = fx.Wert ?? 0;
                    bonus.Kandidaten = new List<string> { fx.TalentCode! };
                    bonus.Ziel = fx.TalentCode;
                    boni.Add(bonus);
                }
                else
                {
                    bonus.Beschriftung = "Vorteil auf " + fx.VorteilAuf;
                    vorteile.Add(bonus);
                }
            }
            return (boni, vorteile);
        }
    }
}
