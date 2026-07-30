// Controllers/SpielmodusV2Controller.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models;
using Suendenbock_App.Controllers.Api;

namespace Suendenbock_App.Controllers
{
    /// <summary>
    /// SpielmodusV2Controller - Neues Kampfsystem (BattleV2)
    ///
    /// WICHTIG: Komplett getrennt vom alten SpielmodusController
    /// Verwendet aber die gleichen Datenbank-Tabellen (CombatSession, etc.)
    ///
    /// Neue Features:
    /// - Neuer Rundenablauf
    /// - Angepasste Kampfmechaniken
    /// - Komplett neues UI/UX
    /// </summary>
    [Authorize(Roles = "Spieler,Moderator,Gott")] // Nur Spieler, Moderatoren und Götter - KEINE Gäste
    public class SpielmodusV2Controller : BaseController
    {
        public SpielmodusV2Controller(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Kampf-Seite (Neues System - BattleV2)
        /// Route: /SpielmodusV2/Battle2
        /// </summary>
        public async Task<IActionResult> Battle2(int? sessionId = null)
        {
            var isGod = User.IsInRole("Gott");

            // ===== FÜR SPIELER: Automatische Weiterleitung zur aktiven Combat Session =====
            if (!isGod)
            {
                var userId = GetUserId();
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.UserId == userId);

                if (character != null)
                {
                    // Finde den aktuell aktiven Act
                    var activeAct = await _context.Acts.FirstOrDefaultAsync(a => a.IsActive);

                    if (activeAct != null)
                    {
                        // Suche aktive V2 Combat Session für den aktiven Act
                        // HINWEIS: Hier könnten wir später ein Flag "Version" oder "Type" hinzufügen
                        // um V1 und V2 Sessions zu unterscheiden
                        var activeSession = await _context.CombatSessions
                            .Where(cs => cs.IsActive && cs.ActId == activeAct.Id)
                            .OrderByDescending(cs => cs.StartedAt)
                            .FirstOrDefaultAsync();

                        if (activeSession != null)
                        {
                            if (!sessionId.HasValue || sessionId.Value != activeSession.Id)
                            {
                                return RedirectToAction("Battle2", new { sessionId = activeSession.Id });
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Aktuell läuft kein Kampf.";
                            return RedirectToAction("Dashboard", "Spielmodus");
                        }
                    }
                }
            }

            // ===== FÜR GOTT: Prüfen ob Session noch aktiv ist =====
            if (isGod && sessionId.HasValue)
            {
                var session = await _context.CombatSessions.FindAsync(sessionId.Value);
                if (session == null || !session.IsActive)
                {
                    TempData["ErrorMessage"] = "Diese Combat Session ist nicht mehr aktiv.";
                    return RedirectToAction("CombatSetup2");
                }
            }

            // Lade alle verfügbaren Charaktere (für Add Participant Modal)
            var characters = await _context.Characters
                .Where(c => c.UserId != null || c.IsCompanion)
                .OrderBy(c => c.Nachname)
                .Select(c => new BattleV2CharacterOption
                {
                    Id = c.Id,
                    Name = $"{c.Vorname} {c.Nachname}",
                    CurrentHealth = c.CurrentHealth,
                    MaxHealth = c.BaseMaxHealth,
                    CurrentPokus = c.CastedSpellsCount,
                    MaxPokus = c.BaseMaxPokus,
                    IsBegleiter = c.IsCompanion,
                    HealthPercent = c.BaseMaxHealth > 0
                        ? (int)((double)c.CurrentHealth / c.BaseMaxHealth * 100)
                        : 0
                })
                .ToListAsync();

            // Lade alle verfügbaren Monster
            var monsters = await _context.Monsters
                .Include(m => m.Monstertyp)
                .Where(m => m.meet)
                .OrderBy(m => m.Name)
                .Select(m => new BattleV2MonsterOption
                {
                    Id = m.Id,
                    Name = m.Name,
                    MonstertypName = m.Monstertyp != null ? m.Monstertyp.Name : "Unbekannt",
                    ImageUrl = m.ImagePath ?? "/images/monsters/default.png",
                    Health = m.Lebenspunkte,
                    Attack = 10,
                    Defense = 10,
                    IsBoss = false
                })
                .ToListAsync();

            // Lade Feldeffekte
            var fieldEffects = await _context.FeldEffekte
                .Include(fe => fe.LightCard)
                .OrderBy(fe => fe.Name)
                .Select(fe => new FeldEffektV2Option
                {
                    Id = fe.Id,
                    Name = fe.Name,
                    Beschreibung = fe.Beschreibung,
                    Schwere = fe.Schwere,
                    ColorCode = fe.LightCard != null ? fe.LightCard.Farbcode : "#ffffff",
                    LightCardName = fe.LightCard != null ? fe.LightCard.Bezeichnung : "Unbekannt"
                })
                .ToListAsync();

            // Lade Biome
            var biomes = await _context.Biome
                .Include(b => b.LightCard)
                .OrderBy(b => b.Name)
                .Select(b => new BiomV2Option
                {
                    Id = b.Id,
                    Name = b.Name,
                    Beschreibung = b.Beschreibung,
                    ColorCode = b.LightCard != null ? b.LightCard.Farbcode : "#ffffff",
                    LightCardName = b.LightCard != null ? b.LightCard.Bezeichnung : "Unbekannt"
                })
                .ToListAsync();

            var viewModel = new BattleV2ViewModel
            {
                Characters = characters,
                Monsters = monsters,
                AllFieldEffects = fieldEffects,
                AllBiomes = biomes,
                IsGod = isGod,
                SessionId = sessionId
            };

            ViewBag.SessionId = sessionId;

            return View(viewModel);
        }

        /// <summary>
        /// Combat-Setup Seite (Neues System)
        /// Route: /SpielmodusV2/CombatSetup2
        /// Nur für Gott zugänglich
        /// </summary>
        [Authorize(Roles = "Gott")]
        public async Task<IActionResult> CombatSetup2()
        {
            var currentAct = await _context.Acts
                .FirstOrDefaultAsync(a => a.IsActive);

            var monsters = await _context.Monsters
                .Include(m => m.Monstertyp)
                .Where(m => m.meet == true)
                .OrderBy(m => m.Name)
                .Select(m => new MonsterV2OptionViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    ImagePath = m.ImagePath,
                    MonstertypName = m.Monstertyp != null ? m.Monstertyp.Name : "Unbekannt",
                    Lebenspunkte = m.Lebenspunkte
                })
                .ToListAsync();

            var playerCharacters = await _context.Characters
                .Where(c => c.UserId != null)
                .OrderBy(c => c.Nachname)
                .Select(c => new CharacterV2OptionViewModel
                {
                    Id = c.Id,
                    Name = $"{c.Vorname} {c.Nachname}",
                    Nachname = c.Nachname,
                    CurrentHealth = c.CurrentHealth,
                    MaxHealth = c.BaseMaxHealth,
                    CurrentPokus = c.CastedSpellsCount,
                    IsBegleiter = false
                })
                .ToListAsync();

            var companions = new List<CharacterV2OptionViewModel>();
            if (currentAct != null)
            {
                var companionNames = new List<string>();
                if (!string.IsNullOrEmpty(currentAct.Companion1))
                {
                    companionNames.Add(currentAct.Companion1.Trim());
                }
                if (!string.IsNullOrEmpty(currentAct.Companion2))
                {
                    companionNames.Add(currentAct.Companion2.Trim());
                }

                if (companionNames.Any())
                {
                    var allCompanions = await _context.Characters.ToListAsync();

                    companions = allCompanions
                        .Where(c => companionNames.Contains($"{c.Vorname} {c.Nachname}"))
                        .OrderBy(c => c.Nachname)
                        .Select(c => new CharacterV2OptionViewModel
                        {
                            Id = c.Id,
                            Name = $"{c.Vorname} {c.Nachname}",
                            Nachname = c.Nachname,
                            CurrentHealth = c.CurrentHealth,
                            MaxHealth = c.BaseMaxHealth,
                            CurrentPokus = c.CastedSpellsCount,
                            IsBegleiter = true
                        })
                        .ToList();
                }
            }

            var allCharacters = playerCharacters.Concat(companions).ToList();

            var viewModel = new CombatSetupV2ViewModel
            {
                ActId = currentAct?.Id ?? 0,
                Monsters = monsters,
                Characters = allCharacters
            };

            return View(viewModel);
        }
    }

    #region ViewModels für BattleV2

    public class BattleV2ViewModel
    {
        public List<BattleV2CharacterOption> Characters { get; set; } = new();
        public List<BattleV2MonsterOption> Monsters { get; set; } = new();
        public List<FeldEffektV2Option> AllFieldEffects { get; set; } = new();
        public List<BiomV2Option> AllBiomes { get; set; } = new();
        public bool IsGod { get; set; }
        public int? SessionId { get; set; }
    }

    public class BattleV2CharacterOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentPokus { get; set; }
        public int MaxPokus { get; set; }
        public bool IsBegleiter { get; set; }
        public int HealthPercent { get; set; }
    }

    public class BattleV2MonsterOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MonstertypName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public bool IsBoss { get; set; }
    }

    public class FeldEffektV2Option
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Beschreibung { get; set; }
        public string Schwere { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
        public string LightCardName { get; set; } = string.Empty;
    }

    public class BiomV2Option
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Beschreibung { get; set; }
        public string ColorCode { get; set; } = string.Empty;
        public string LightCardName { get; set; } = string.Empty;
    }

    public class CombatSetupV2ViewModel
    {
        public int ActId { get; set; }
        public List<MonsterV2OptionViewModel> Monsters { get; set; } = new();
        public List<CharacterV2OptionViewModel> Characters { get; set; } = new();
    }

    public class MonsterV2OptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string MonstertypName { get; set; } = string.Empty;
        public int Lebenspunkte { get; set; }
    }

    public class CharacterV2OptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentPokus { get; set; }
        public bool IsBegleiter { get; set; }
    }

    #endregion
}
