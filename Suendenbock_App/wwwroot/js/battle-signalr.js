// ===== BATTLE SYSTEM - SIGNALR INTEGRATION =====

import { battleState } from './battle-state.js';
import { renderBattleGrid, updateTurnIndicator } from './battle-rendering.js';
import { renderFieldEffects } from './battle-field-effects.js';
import { showResultScreen } from './battle-actions.js';

// ===== SIGNALR CONNECTION =====
let signalRConnection = null;
let isConnected = false;

// ===== SIGNALR SETUP =====
export async function setupSignalR(sessionId) {
    if (!sessionId) {
        return;
    }

    try {
        // SignalR Connection erstellen
        signalRConnection = new signalR.HubConnectionBuilder()
            .withUrl("/battlehub")
            .withAutomaticReconnect()
            .build();

        // Event: Battle State Update empfangen
        signalRConnection.on("ReceiveBattleState", (data) => {
            // Nur updaten, wenn es nicht von diesem Client kommt
            if (data.sessionId === battleState.sessionId) {
                const newSetup = JSON.parse(data.battleStateJson);

                // State aktualisieren, aber bestehende Participants-Struktur beibehalten
                if (newSetup.participants) {
                    battleState.participants = newSetup.participants;
                }
                if (newSetup.activeFieldEffects) {
                    battleState.activeFieldEffects = newSetup.activeFieldEffects;
                }
                battleState.currentRound = data.currentRound;
                battleState.currentTurnIndex = data.currentTurnIndex;

                // UI neu rendern
                renderBattleGrid();
                updateTurnIndicator();
                renderFieldEffects();
            }
        });

        // Event: Kampf beendet
        signalRConnection.on("CombatEnded", async (data) => {
            await showResultScreen(data.result);
        });

        // Event: Quick Action empfangen
        signalRConnection.on("ReceiveAction", (data) => {
            // Optional: Hier könntest du Animationen triggern
        });

        // Verbindung starten
        await signalRConnection.start();
        isConnected = true;

        // Combat Session beitreten
        await signalRConnection.invoke("JoinCombat", sessionId);

    } catch (error) {
        // Battle läuft trotzdem weiter, nur ohne Real-time Sync
    }
}

// ===== SIGNALR STATE SYNC =====
export async function syncBattleState() {
    if (!isConnected || !signalRConnection || !battleState.sessionId) {
        return; // Kein SignalR - lokaler Modus
    }

    try {
        const battleStateJson = JSON.stringify({
            participants: battleState.participants,
            expandedConditions: battleState.expandedConditions,
            activeFieldEffects: battleState.activeFieldEffects
        });

        await signalRConnection.invoke(
            "UpdateBattleState",
            battleState.sessionId,
            battleStateJson,
            battleState.currentRound,
            battleState.currentTurnIndex
        );
    } catch (error) {
        // Fehler stillschweigend behandeln
    }
}

// Export connection state checkers
export function getIsConnected() {
    return isConnected;
}

export function getSignalRConnection() {
    return signalRConnection;
}
