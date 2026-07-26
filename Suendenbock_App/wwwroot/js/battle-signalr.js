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
        console.warn("[SignalR] Keine Session-ID - kein SignalR Setup");
        return;
    }

    console.log(`[SignalR] Starte Verbindungsaufbau für Session ${sessionId}...`);

    try {
        // SignalR Connection erstellen
        signalRConnection = new signalR.HubConnectionBuilder()
            .withUrl("/battlehub", {
                transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling,
                skipNegotiation: false // Wichtig für Docker!
            })
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: retryContext => {
                    // Exponentielles Backoff: 0s, 2s, 10s, 30s
                    const delays = [0, 2000, 10000, 30000];
                    return delays[Math.min(retryContext.previousRetryCount, delays.length - 1)];
                }
            })
            .configureLogging(signalR.LogLevel.Information) // Logging aktivieren
            .build();

        // Connection Events
        signalRConnection.onclose((error) => {
            isConnected = false;
            if (error) {
                console.error("[SignalR] Verbindung geschlossen mit Fehler:", error);
            } else {
                console.log("[SignalR] Verbindung sauber geschlossen");
            }
        });

        signalRConnection.onreconnecting((error) => {
            console.warn("[SignalR] Versuche Reconnect...", error);
            isConnected = false;
        });

        signalRConnection.onreconnected((connectionId) => {
            console.log(`[SignalR] Erfolgreich reconnected! Connection-ID: ${connectionId}`);
            isConnected = true;
            // Nach Reconnect: Rejoin Combat Session
            signalRConnection.invoke("JoinCombat", sessionId)
                .catch(err => console.error("[SignalR] Fehler beim Rejoin:", err));
        });

        // Event: Battle State Update empfangen
        signalRConnection.on("ReceiveBattleState", (data) => {
            console.log("[SignalR] ReceiveBattleState empfangen:", data);
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
            console.log("[SignalR] CombatEnded empfangen:", data);
            await showResultScreen(data.result);
        });

        // Event: Quick Action empfangen
        signalRConnection.on("ReceiveAction", (data) => {
            console.log("[SignalR] ReceiveAction empfangen:", data);
            // Optional: Hier könntest du Animationen triggern
        });

        // Verbindung starten
        console.log("[SignalR] Starte Verbindung...");
        await signalRConnection.start();
        isConnected = true;
        console.log(`[SignalR] ✅ Verbindung erfolgreich! Connection-ID: ${signalRConnection.connectionId}`);

        // Combat Session beitreten
        console.log(`[SignalR] Trete Combat Session ${sessionId} bei...`);
        await signalRConnection.invoke("JoinCombat", sessionId);
        console.log("[SignalR] ✅ Combat Session beigetreten!");

    } catch (error) {
        console.error("[SignalR] ❌ FEHLER beim Verbindungsaufbau:", error);
        console.error("[SignalR] Details:", error.message, error.stack);
        // Battle läuft trotzdem weiter, nur ohne Real-time Sync
        alert("⚠️ SignalR-Verbindung fehlgeschlagen. Der Kampf läuft nur lokal.\n\nFehler: " + error.message);
    }
}

// ===== SIGNALR STATE SYNC =====
export async function syncBattleState() {
    if (!isConnected || !signalRConnection || !battleState.sessionId) {
        console.log("[SignalR] Sync übersprungen: Nicht verbunden oder keine Session-ID");
        return; // Kein SignalR - lokaler Modus
    }

    try {
        const battleStateJson = JSON.stringify({
            participants: battleState.participants,
            expandedConditions: battleState.expandedConditions,
            activeFieldEffects: battleState.activeFieldEffects
        });

        console.log(`[SignalR] Sende Battle State Update für Session ${battleState.sessionId}...`);

        await signalRConnection.invoke(
            "UpdateBattleState",
            battleState.sessionId,
            battleStateJson,
            battleState.currentRound,
            battleState.currentTurnIndex
        );

        console.log("[SignalR] ✅ Battle State Update gesendet");
    } catch (error) {
        console.error("[SignalR] ❌ Fehler beim State Sync:", error);
        // Fehler stillschweigend behandeln - Battle läuft lokal weiter
    }
}

// Export connection state checkers
export function getIsConnected() {
    return isConnected;
}

export function getSignalRConnection() {
    return signalRConnection;
}
