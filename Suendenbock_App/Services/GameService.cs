using Suendenbock_App.Data;
using Suendenbock_App.Models;
using Microsoft.EntityFrameworkCore;

namespace Suendenbock_App.Services
{
    public class GameService
    {
        private readonly ApplicationDbContext _context;

        public GameService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== CHARACTER MANAGEMENT =====

        /// <summary>
        /// Schaden auf Character anwenden
        /// </summary>
        public async Task ApplyDamageAsync(int characterId, int damage)
        {
            var character = await _context.Characters.FindAsync(characterId);
            if (character != null)
            {
                character.CurrentHealth = Math.Max(0, character.CurrentHealth - damage);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Character heilen
        /// </summary>
        public async Task HealCharacterAsync(int characterId, int amount)
        {
            var character = await _context.Characters.FindAsync(characterId);
            if (character != null)
            {
                character.CurrentHealth = Math.Min(
                    character.CurrentHealth + amount,
                    character.BaseMaxHealth
                );
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Pokus erhöhen (wenn Magie genutzt)
        /// </summary>
        public async Task IncrementPokusAsync(int characterId)
        {
            var character = await _context.Characters.FindAsync(characterId);
            if (character != null)
            {
                character.CurrentPokus++;
                await _context.SaveChangesAsync();
            }
        }

        // ===== RAST =====

        /// <summary>
        /// Rast durchführen mit ausgewähltem Essen
        /// </summary>
        public async Task<string> ApplyRestAsync(List<int> characterIds, int foodId)
        {
            var food = await _context.RestFoods.FindAsync(foodId);
            if (food == null) return "Essen nicht gefunden!";

            var characters = await _context.Characters
                .Where(c => characterIds.Contains(c.Id))
                .ToListAsync();

            foreach (var character in characters)
            {
                // Heilen
                character.CurrentHealth = Math.Min(
                    character.CurrentHealth + food.HealthBonus,
                    character.BaseMaxHealth
                );

                // Pokus zurücksetzen
                character.CurrentPokus = 0;

                // Zeitstempel
                character.LastRestAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return $"Rast abgeschlossen! Alle Characters +{food.HealthBonus} HP geheilt, Pokus zurückgesetzt.";
        }

        // ===== QUESTS =====

        /// <summary>
        /// Neue Quest erstellen
        /// </summary>
        public async Task<Quest> CreateQuestAsync(
            string title,
            string description,
            string type,
            int? characterId = null)
        {
            var quest = new Quest
            {
                Title = title,
                Description = description,
                Type = type,
                CharacterId = type == "individual" ? characterId : null,
                Status = "active"
            };

            _context.Quests.Add(quest);
            await _context.SaveChangesAsync();
            return quest;
        }

        /// <summary>
        /// Quest abschließen
        /// </summary>
        public async Task CompleteQuestAsync(int questId)
        {
            var quest = await _context.Quests.FindAsync(questId);
            if (quest != null)
            {
                quest.Status = "completed";
                quest.CompletedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // ===== MONSTER =====

        /// <summary>
        /// Monster als besiegt markieren
        /// </summary>
        public async Task SlayMonsterAsync(int monsterId)
        {
            var monster = await _context.Monsters.FindAsync(monsterId);
            if (monster != null)
            {
                monster.HasSlainTrophy = true;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Monster ausrüsten (max 3)
        /// </summary>
        public async Task<bool> EquipMonsterAsync(int monsterId)
        {
            var equippedCount = await _context.Monsters
                .CountAsync(m => m.IsEquipped);

            if (equippedCount >= 3)
                return false; // Schon 3 ausgerüstet

            var monster = await _context.Monsters.FindAsync(monsterId);
            if (monster != null && monster.Status != "none")
            {
                monster.IsEquipped = true;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Monster-Ausrüstung entfernen
        /// </summary>
        public async Task UnequipMonsterAsync(int monsterId)
        {
            var monster = await _context.Monsters.FindAsync(monsterId);
            if (monster != null)
            {
                monster.IsEquipped = false;
                await _context.SaveChangesAsync();
            }
        }

        // ===== AKTUELLER AKT =====

        /// <summary>
        /// Aktuellen Akt mit Karte laden
        /// </summary>
        public async Task<Act?> GetCurrentActAsync()
        {
            var act = await _context.Acts
                .FirstOrDefaultAsync(a => a.IsActive);

            if (act != null)
            {
                // Lade Weltkarte mit Markers und Quests
                act.Map = await _context.Maps
                    .Where(m => m.ActId == act.Id && m.IsWorldMap)
                    .Include(m => m.Markers)
                        .ThenInclude(mm => mm.Quests)
                    .FirstOrDefaultAsync();
            }

            return act;
        }
    }
}