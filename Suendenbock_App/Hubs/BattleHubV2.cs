using Microsoft.AspNetCore.SignalR;
using Suendenbock_App.Data;
using Suendenbock_App.Models;
using Microsoft.EntityFrameworkCore;

namespace Suendenbock_App.Hubs
{
    /// <summary>
    /// SignalR Hub für das neue Kampfsystem (BattleV2)
    ///
    /// WICHTIG: Komplett getrennt vom alten BattleHub
    /// Verwendet die gleichen Datenbank-Tabellen (CombatSession)
    ///
    /// NEUE FEATURES:
    /// - Neuer Rundenablauf (wird Schritt-für-Schritt implementiert)
    /// - Angepasste Kampfmechaniken
    /// - Optimierte Broadcasting-Strategie
    ///
    /// FUNKTIONSWEISE:
    /// 1. Clients verbinden sich mit JoinCombatV2(sessionId)
    /// 2. Alle Clients einer Session sind in einer Group: "combat-v2-session-{id}"
    /// 3. Aktionen werden via BroadcastActionV2 an alle gesendet
    /// 4. Battle State Updates via UpdateBattleStateV2
    /// </summary>
    public class BattleHubV2 : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<GameHub> _gameHub;

        public BattleHubV2(ApplicationDbContext context, IHubContext<GameHub> gameHub)
        {
            _context = context;
            _gameHub = gameHub;
        }

        /// <summary>
        /// Ein Client tritt dem neuen Kampfsystem bei
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        public async Task JoinCombatV2(int combatSessionId)
        {
            string groupName = $"combat-v2-session-{combatSessionId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            Console.WriteLine($"[BattleHubV2] Client {Context.ConnectionId} joined session {combatSessionId}");

            // Sende den aktuellen Battle State an den neuen Client
            var session = await _context.CombatSessions
                .FirstOrDefaultAsync(cs => cs.Id == combatSessionId && cs.IsActive);

            if (session != null)
            {
                await Clients.Caller.SendAsync("ReceiveBattleStateV2", new
                {
                    sessionId = session.Id,
                    currentRound = session.CurrentRound,
                    currentTurnIndex = session.CurrentTurnIndex,
                    battleStateJson = session.BattleStateJson
                });

                Console.WriteLine($"[BattleHubV2] Sent initial state to client {Context.ConnectionId}");
            }
        }

        /// <summary>
        /// Ein Client verlässt das neue Kampfsystem
        /// </summary>
        public async Task LeaveCombatV2(int combatSessionId)
        {
            string groupName = $"combat-v2-session-{combatSessionId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            Console.WriteLine($"[BattleHubV2] Client {Context.ConnectionId} left session {combatSessionId}");
        }

        /// <summary>
        /// Battle State Update (Neues System)
        /// WICHTIG: Hier können später neue Rundenmechaniken eingebaut werden
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        /// <param name="battleStateJson">Kompletter Battle State als JSON</param>
        /// <param name="currentRound">Aktuelle Runde</param>
        /// <param name="currentTurnIndex">Index des aktuellen Zugs</param>
        public async Task UpdateBattleStateV2(int combatSessionId, string battleStateJson, int currentRound, int currentTurnIndex)
        {
            var session = await _context.CombatSessions
                .FirstOrDefaultAsync(cs => cs.Id == combatSessionId && cs.IsActive);

            if (session != null)
            {
                // State in DB aktualisieren
                session.BattleStateJson = battleStateJson;
                session.CurrentRound = currentRound;
                session.CurrentTurnIndex = currentTurnIndex;
                await _context.SaveChangesAsync();

                // An alle Teilnehmer broadcasten
                string groupName = $"combat-v2-session-{combatSessionId}";
                await Clients.Group(groupName).SendAsync("ReceiveBattleStateV2", new
                {
                    sessionId = session.Id,
                    currentRound = session.CurrentRound,
                    currentTurnIndex = session.CurrentTurnIndex,
                    battleStateJson = session.BattleStateJson
                });

                Console.WriteLine($"[BattleHubV2] Battle state updated for session {combatSessionId}, Round: {currentRound}, Turn: {currentTurnIndex}");
            }
        }

        /// <summary>
        /// Action Broadcasting (Neues System)
        /// WICHTIG: Hier können später neue Aktionstypen hinzugefügt werden
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        /// <param name="actionType">Art der Action: "damage", "heal", "effect", etc.</param>
        /// <param name="actionData">JSON-Daten der Action</param>
        public async Task BroadcastActionV2(int combatSessionId, string actionType, string actionData)
        {
            string groupName = $"combat-v2-session-{combatSessionId}";
            await Clients.Group(groupName).SendAsync("ReceiveActionV2", new
            {
                actionType,
                actionData,
                timestamp = DateTime.UtcNow
            });

            Console.WriteLine($"[BattleHubV2] Action broadcasted to session {combatSessionId}: {actionType}");
        }

        /// <summary>
        /// Neue Runde starten (Neues System)
        /// PLACEHOLDER: Hier kommen später die neuen Rundenmechaniken rein
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        /// <param name="roundNumber">Neue Rundennummer</param>
        public async Task StartNewRoundV2(int combatSessionId, int roundNumber)
        {
            var session = await _context.CombatSessions
                .FirstOrDefaultAsync(cs => cs.Id == combatSessionId && cs.IsActive);

            if (session != null)
            {
                session.CurrentRound = roundNumber;
                session.CurrentTurnIndex = 0;
                await _context.SaveChangesAsync();

                string groupName = $"combat-v2-session-{combatSessionId}";
                await Clients.Group(groupName).SendAsync("NewRoundStartedV2", new
                {
                    sessionId = session.Id,
                    roundNumber = roundNumber
                });

                Console.WriteLine($"[BattleHubV2] New round started for session {combatSessionId}: Round {roundNumber}");
            }
        }

        /// <summary>
        /// Kampf beenden (Neues System)
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        /// <param name="result">"victory" oder "defeat"</param>
        public async Task EndCombatV2(int combatSessionId, string result)
        {
            var session = await _context.CombatSessions
                .FirstOrDefaultAsync(cs => cs.Id == combatSessionId && cs.IsActive);

            if (session != null)
            {
                session.IsActive = false;
                session.EndedAt = DateTime.Now;
                session.Result = result;
                await _context.SaveChangesAsync();

                // An alle Teilnehmer IM BATTLE senden
                string battleGroupName = $"combat-v2-session-{combatSessionId}";
                await Clients.Group(battleGroupName).SendAsync("CombatEndedV2", new
                {
                    sessionId = session.Id,
                    result = session.Result,
                    endedAt = session.EndedAt
                });

                // An alle im ACT senden (für Dashboard-Banner)
                var act = await _context.Acts.FindAsync(session.ActId);
                if (act != null)
                {
                    string actGroupName = $"act-{act.ActNumber}";
                    await _gameHub.Clients.Group(actGroupName).SendAsync("CombatEndedV2", new
                    {
                        actId = session.ActId,
                        sessionId = session.Id,
                        result = session.Result,
                        endedAt = session.EndedAt
                    });

                    Console.WriteLine($"[BattleHubV2] Combat ended broadcast sent to Act {act.ActNumber}, Result: {result}");
                }
            }
        }

        /// <summary>
        /// NEUES FEATURE: Initiative Roll Broadcasting
        /// Wird verwendet, wenn neue Initiative-Mechanik implementiert wird
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        /// <param name="initiativeData">JSON mit Initiative-Daten</param>
        public async Task BroadcastInitiativeRollV2(int combatSessionId, string initiativeData)
        {
            string groupName = $"combat-v2-session-{combatSessionId}";
            await Clients.Group(groupName).SendAsync("ReceiveInitiativeRollV2", new
            {
                initiativeData,
                timestamp = DateTime.UtcNow
            });

            Console.WriteLine($"[BattleHubV2] Initiative roll broadcasted to session {combatSessionId}");
        }

        /// <summary>
        /// NEUES FEATURE: Turn Phase Update
        /// Für neue Rundenmechanik mit verschiedenen Phasen (z.B. Move Phase, Action Phase, etc.)
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        /// <param name="phase">Phase-Name: "move", "action", "reaction", etc.</param>
        /// <param name="phaseData">JSON mit Phase-Daten</param>
        public async Task UpdateTurnPhaseV2(int combatSessionId, string phase, string phaseData)
        {
            string groupName = $"combat-v2-session-{combatSessionId}";
            await Clients.Group(groupName).SendAsync("ReceiveTurnPhaseV2", new
            {
                phase,
                phaseData,
                timestamp = DateTime.UtcNow
            });

            Console.WriteLine($"[BattleHubV2] Turn phase update broadcasted to session {combatSessionId}: {phase}");
        }

        /// <summary>
        /// Override: Wird aufgerufen, wenn ein Client disconnected
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"[BattleHubV2] Client {Context.ConnectionId} disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Override: Wird aufgerufen, wenn ein Client connected
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[BattleHubV2] Client {Context.ConnectionId} connected");
            await base.OnConnectedAsync();
        }
    }
}
