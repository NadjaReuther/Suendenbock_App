using Microsoft.AspNetCore.SignalR;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Suendenbock_App.Hubs
{
    /// <summary>
    /// Globaler SignalR Hub für alle Echtzeit-Kommunikation im Spiel
    ///
    /// FUNKTIONALITÄT:
    /// - Spieler verbinden sich beim Laden jeder Seite
    /// - Treten automatisch der Group für ihren aktuellen Act bei
    /// - Empfangen Broadcasts (z.B. "Kampf hat begonnen!")
    /// - Können zwischen Acts wechseln (für Gott)
    /// </summary>
    public class GameHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public GameHub(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Spieler tritt einem Act bei.
        /// Er wird der SignalR Group für diesen Act hinzugefügt.
        /// </summary>
        public async Task JoinAct(int actId, string userName)
        {
            string groupName = $"act-{actId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            Console.WriteLine($"[GameHub] {userName} joined Act {actId} (ConnectionId: {Context.ConnectionId})");
        }

        /// <summary>
        /// Spieler verlässt einen Act
        /// </summary>
        public async Task LeaveAct(int actId)
        {
            string groupName = $"act-{actId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Broadcast: Kampf wurde gestartet
        /// Sendet an alle Spieler im Act die Info, dass ein Kampf begonnen hat
        /// </summary>
        public async Task NotifyCombatStarted(int actId, int combatSessionId)
        {
            string groupName = $"act-{actId}";
            await Clients.Group(groupName).SendAsync("CombatStarted", new
            {
                actId,
                combatSessionId,
                message = "Ein Kampf hat begonnen!",
                timestamp = DateTime.UtcNow
            });

            Console.WriteLine($"[GameHub] Combat started broadcast sent to Act {actId}, SessionId: {combatSessionId}");
        }

        /// <summary>
        /// Broadcast: Kampf wurde beendet
        /// </summary>
        public async Task NotifyCombatEnded(int actId, string result)
        {
            string groupName = $"act-{actId}";
            await Clients.Group(groupName).SendAsync("CombatEnded", new
            {
                actId,
                result,
                message = $"Der Kampf ist vorbei! Ergebnis: {result}",
                timestamp = DateTime.UtcNow
            });

            Console.WriteLine($"[GameHub] Combat ended broadcast sent to Act {actId}, Result: {result}");
        }

        /// <summary>
        /// Broadcast: Generische Nachricht an alle im Act
        /// </summary>
        public async Task BroadcastToAct(int actId, string messageType, object data)
        {
            string groupName = $"act-{actId}";
            await Clients.Group(groupName).SendAsync("ReceiveMessage", new
            {
                messageType,
                data,
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Spieler fordert Nachtlager an.
        /// Sendet Benachrichtigung an den Gott mit allen Spieler-Charakteren und deren Pokus.
        /// Speichert die Anfrage in der DB damit der Gott sie auch sieht wenn er später kommt.
        /// </summary>
        public async Task RequestNightRest(int actId, string playerName)
        {
            // Prüfe ob bereits eine aktive Anfrage existiert
            var existingRequest = await _context.NightRestRequests
                .FirstOrDefaultAsync(r => r.ActId == actId && r.IsActive);

            if (existingRequest != null)
            {
                // Anfrage bereits vorhanden, ignoriere neue Anfrage
                Console.WriteLine($"[GameHub] Night rest request already pending for Act {actId}");
                return;
            }

            // Erstelle neue Anfrage in DB
            var request = new NightRestRequest
            {
                ActId = actId,
                PlayerName = playerName,
                RequestedAt = DateTime.Now,
                IsActive = true
            };

            _context.NightRestRequests.Add(request);
            await _context.SaveChangesAsync();

            // Hole NUR Spieler-Charaktere (keine Begleiter)
            var characters = await _context.Characters
                .Include(c => c.Lebensstatus)
                .Where(c => c.Lebensstatus.Name == "Lebend")
                .Where(c => c.UserId != null)
                .ToListAsync();

            var characterData = characters.Select(c => new
            {
                id = c.Id,
                name = $"{c.Vorname} {c.Nachname}".Trim(),
                currentPokus = c.CurrentPokus,
                isCompanion = false
            }).ToList();

            // Sende an Gott (alle in der Act-Group)
            string groupName = $"act-{actId}";
            await Clients.Group(groupName).SendAsync("NightRestRequested", new
            {
                actId,
                playerName,
                characters = characterData,
                timestamp = DateTime.UtcNow
            });

            Console.WriteLine($"[GameHub] Night rest requested by {playerName} in Act {actId}, saved to DB");
        }

        /// <summary>
        /// Gott wendet Nachtlager an.
        /// Heilt nur Spielercharaktere, addiert Extra-Pokus pro Charakter auf die gewirkten Zauber.
        /// Begleiter werden NICHT beeinflusst (HP/Pokus wird nach jedem Kampf zurückgesetzt).
        /// Löscht die Anfrage aus der DB.
        /// </summary>
        public async Task ApplyNightRest(int actId, Dictionary<string, int> foodPerCharacter, Dictionary<string, int> extraPokusPerCharacter)
        {
            // Null-Sicherheit: leere Dicts wenn null
            foodPerCharacter ??= new Dictionary<string, int>();
            extraPokusPerCharacter ??= new Dictionary<string, int>();

            // Prüfe ob noch eine aktive Anfrage existiert — Spieler könnte die Seite verlassen haben
            var activeRequest = await _context.NightRestRequests
                .FirstOrDefaultAsync(r => r.ActId == actId && r.IsActive);

            if (activeRequest == null)
            {
                Console.WriteLine($"[GameHub] ApplyNightRest für Act {actId} abgebrochen — keine aktive Anfrage mehr.");
                return;
            }

            // Lade alle verwendeten Nahrungen
            var usedFoodIds = foodPerCharacter.Values.Distinct().ToList();
            var foods = await _context.RestFoods
                .Where(f => usedFoodIds.Contains(f.Id))
                .ToListAsync();
            var foodDict = foods.ToDictionary(f => f.Id);

            // Hole NUR Spieler-Charaktere (keine Begleiter, keine NPCs)
            var characters = await _context.Characters
                .Include(c => c.Lebensstatus)
                .Where(c => c.Lebensstatus.Name == "Lebend")
                .Where(c => c.UserId != null)
                .ToListAsync();

            // Erfasse Ergebnisse pro Charakter für die Anzeige
            var characterResults = new List<object>();

            foreach (var character in characters)
            {
                int spellCount = character.CurrentPokus;
                int extraPokus = extraPokusPerCharacter.TryGetValue(character.Id.ToString(), out var ep) ? ep : 0;

                // Hole individuelle Mahlzeit für diesen Charakter
                var charFoodId = foodPerCharacter.TryGetValue(character.Id.ToString(), out var fid) ? fid : 0;
                var charFood = foodDict.TryGetValue(charFoodId, out var f) ? f : null;
                int healthBonus = charFood?.HealthBonus ?? 0;

                // Spieler heilen mit ihrer individuellen Mahlzeit
                character.CurrentHealth = Math.Min(
                    character.CurrentHealth + healthBonus,
                    character.BaseMaxHealth
                );

                // Zauber bleiben als Pokuspunkte + Extra von Gott
                character.CurrentPokus = spellCount + extraPokus;
                character.LastRestAt = DateTime.Now;

                characterResults.Add(new
                {
                    id = character.Id,
                    name = $"{character.Vorname} {character.Nachname}".Trim(),
                    spellCount,
                    extraPokus,
                    totalPokus = character.CurrentPokus,
                    currentHealth = character.CurrentHealth,
                    maxHealth = character.BaseMaxHealth,
                    foodName = charFood?.Name ?? "Keine Mahlzeit",
                    healthBonus = healthBonus
                });
            }

            // Lösche alle aktiven Anfragen für diesen Act
            var pendingRequests = await _context.NightRestRequests
                .Where(r => r.ActId == actId && r.IsActive)
                .ToListAsync();

            foreach (var request in pendingRequests)
            {
                request.IsActive = false;
            }

            await _context.SaveChangesAsync();

            // Sende Ergebnis an alle Spieler im Act
            string groupName = $"act-{actId}";
            await Clients.Group(groupName).SendAsync("NightRestCompleted", new
            {
                actId,
                characters = characterResults,
                timestamp = DateTime.UtcNow
            });

            Console.WriteLine($"[GameHub] Night rest applied in Act {actId}, removed pending requests");
        }

        /// <summary>
        /// Nachtlager-Anfrage abbrechen (von Gott oder Spieler).
        /// Löscht die Anfrage aus der DB und benachrichtigt alle im Act.
        /// </summary>
        public async Task CancelNightRest(int actId)
        {
            var pendingRequests = await _context.NightRestRequests
                .Where(r => r.ActId == actId && r.IsActive)
                .ToListAsync();

            foreach (var request in pendingRequests)
            {
                request.IsActive = false;
            }

            await _context.SaveChangesAsync();

            string groupName = $"act-{actId}";
            await Clients.Group(groupName).SendAsync("NightRestCancelled", new
            {
                actId,
                timestamp = DateTime.UtcNow
            });

            Console.WriteLine($"[GameHub] Night rest cancelled for Act {actId}");
        }

        /// <summary>
        /// Override: Wird aufgerufen, wenn ein Client verbunden wird
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[GameHub] Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Override: Wird aufgerufen, wenn ein Client getrennt wird
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"[GameHub] Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
