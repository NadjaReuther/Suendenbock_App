using Microsoft.AspNetCore.SignalR;
using Suendenbock_App.Data;
using Suendenbock_App.Models;
using Microsoft.EntityFrameworkCore;

namespace Suendenbock_App.Hubs
{
    /// <summary>
    /// SignalR Hub für Real-time Combat
    ///
    /// FUNKTIONSWEISE:
    /// 1. Gott startet einen Kampf → CombatSession wird erstellt
    /// 2. Alle beteiligten Spieler verbinden sich mit dem Hub
    /// 3. Clients treten einer "Group" bei (z.B. "combat-session-123")
    /// 4. Wenn jemand eine Action macht, wird sie an alle in der Group gesendet
    ///
    /// WICHTIGE KONZEPTE:
    /// - Groups: Ermöglichen Broadcasting an eine Teilmenge von Clients
    /// - Clients.Group("name").SendAsync(): Sendet an alle in der Group
    /// - Context.ConnectionId: Eindeutige ID für jeden verbundenen Client
    /// </summary>
    public class BattleHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<GameHub> _gameHub;

        public BattleHub(ApplicationDbContext context, IHubContext<GameHub> gameHub)
        {
            _context = context;
            _gameHub = gameHub;
        }

        /// <summary>
        /// Ein Client tritt einem Kampf bei.
        /// Er wird der SignalR-Group für diese CombatSession hinzugefügt.
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        public async Task JoinCombat(int combatSessionId)
        {
            string groupName = $"combat-session-{combatSessionId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Sende den aktuellen Battle State an den neuen Client
            var session = await _context.CombatSessions
                .FirstOrDefaultAsync(cs => cs.Id == combatSessionId && cs.IsActive);

            if (session != null)
            {
                await Clients.Caller.SendAsync("ReceiveBattleState", new
                {
                    sessionId = session.Id,
                    currentRound = session.CurrentRound,
                    currentTurnIndex = session.CurrentTurnIndex,
                    battleStateJson = session.BattleStateJson
                });
            }
        }

        /// <summary>
        /// Ein Client verlässt einen Kampf.
        /// </summary>
        public async Task LeaveCombat(int combatSessionId)
        {
            string groupName = $"combat-session-{combatSessionId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Gott (oder ein Client) aktualisiert den Battle State.
        /// Der neue State wird in der DB gespeichert und an alle Teilnehmer gesendet.
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        /// <param name="battleStateJson">Kompletter Battle State als JSON</param>
        /// <param name="currentRound">Aktuelle Runde</param>
        /// <param name="currentTurnIndex">Index des aktuellen Zugs</param>
        public async Task UpdateBattleState(int combatSessionId, string battleStateJson, int currentRound, int currentTurnIndex)
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
                string groupName = $"combat-session-{combatSessionId}";
                await Clients.Group(groupName).SendAsync("ReceiveBattleState", new
                {
                    sessionId = session.Id,
                    currentRound = session.CurrentRound,
                    currentTurnIndex = session.CurrentTurnIndex,
                    battleStateJson = session.BattleStateJson
                });
            }
        }

        /// <summary>
        /// Schnelle Action-Broadcasting (z.B. Damage, Heal).
        /// Sendet nur die Action, nicht den kompletten State.
        /// Der empfangende Client aktualisiert seinen lokalen State.
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        /// <param name="actionType">Art der Action: "damage", "heal", "temp", "condition", etc.</param>
        /// <param name="actionData">JSON-Daten der Action</param>
        public async Task BroadcastAction(int combatSessionId, string actionType, string actionData)
        {
            string groupName = $"combat-session-{combatSessionId}";
            await Clients.Group(groupName).SendAsync("ReceiveAction", new
            {
                actionType,
                actionData,
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Kampf beenden.
        /// Setzt IsActive = false und EndedAt.
        /// </summary>
        /// <param name="combatSessionId">ID der CombatSession</param>
        /// <param name="result">"victory" oder "defeat"</param>
        public async Task EndCombat(int combatSessionId, string result)
        {
            var session = await _context.CombatSessions
                .FirstOrDefaultAsync(cs => cs.Id == combatSessionId && cs.IsActive);

            if (session != null)
            {
                session.IsActive = false;
                session.EndedAt = DateTime.Now;
                session.Result = result;
                await _context.SaveChangesAsync();

                // An alle Teilnehmer IM BATTLE senden (combat-session Group)
                string battleGroupName = $"combat-session-{combatSessionId}";
                await Clients.Group(battleGroupName).SendAsync("CombatEnded", new
                {
                    sessionId = session.Id,
                    result = session.Result,
                    endedAt = session.EndedAt
                });

                // WICHTIG: Auch an alle im ACT senden (für Dashboard-Banner)
                var act = await _context.Acts.FindAsync(session.ActId);
                if (act != null)
                {
                    string actGroupName = $"act-{act.ActNumber}";
                    await _gameHub.Clients.Group(actGroupName).SendAsync("CombatEnded", new
                    {
                        actId = session.ActId,
                        sessionId = session.Id,
                        result = session.Result,
                        endedAt = session.EndedAt
                    });

                    Console.WriteLine($"[BattleHub] Combat ended broadcast sent to Act {act.ActNumber}, Result: {result}");
                }
            }
        }

        /// <summary>
        /// Override: Wird aufgerufen, wenn ein Client disconnected.
        /// Optional: Cleanup-Logik hier einfügen.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Hier könntest du z.B. tracken, welche Clients disconnected sind
            await base.OnDisconnectedAsync(exception);
        }
    }
}
