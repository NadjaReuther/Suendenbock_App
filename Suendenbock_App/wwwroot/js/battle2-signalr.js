/**
 * battle2-signalr.js
 * SignalR Connection Manager für BattleV2
 *
 * Verbindet sich mit dem BattleHubV2 Hub
 * Handhabt alle Real-time Events für das neue Kampfsystem
 */

let connectionV2 = null;
let isConnectedV2 = false;
let reconnectAttemptsV2 = 0;
const MAX_RECONNECT_ATTEMPTS_V2 = 5;

/**
 * Initialisiert die SignalR-Verbindung zum BattleHubV2
 */
export async function initializeSignalRConnectionV2(sessionId) {
    try {
        console.log('[BattleV2 SignalR] Initialisiere Verbindung...');

        connectionV2 = new signalR.HubConnectionBuilder()
            .withUrl('/battlehubv2')
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: retryContext => {
                    // Exponential backoff: 0s, 2s, 10s, 30s, dann jede Minute
                    if (retryContext.previousRetryCount === 0) {
                        return 0;
                    }
                    if (retryContext.previousRetryCount === 1) {
                        return 2000;
                    }
                    if (retryContext.previousRetryCount === 2) {
                        return 10000;
                    }
                    if (retryContext.previousRetryCount === 3) {
                        return 30000;
                    }
                    return 60000;
                }
            })
            .configureLogging(signalR.LogLevel.Information)
            .build();

        setupEventHandlersV2();

        await connectionV2.start();
        console.log('[BattleV2 SignalR] Verbindung erfolgreich hergestellt');

        isConnectedV2 = true;
        reconnectAttemptsV2 = 0;

        // Join Combat Session
        if (sessionId) {
            await connectionV2.invoke('JoinCombatV2', sessionId);
            console.log(`[BattleV2 SignalR] Combat Session ${sessionId} beigetreten`);
        }

        return true;

    } catch (error) {
        console.error('[BattleV2 SignalR] Verbindungsfehler:', error);
        isConnectedV2 = false;

        if (reconnectAttemptsV2 < MAX_RECONNECT_ATTEMPTS_V2) {
            reconnectAttemptsV2++;
            console.log(`[BattleV2 SignalR] Reconnect Versuch ${reconnectAttemptsV2}/${MAX_RECONNECT_ATTEMPTS_V2}...`);
            setTimeout(() => initializeSignalRConnectionV2(sessionId), 3000);
        } else {
            showConnectionErrorV2();
        }

        return false;
    }
}

/**
 * Setup Event Handlers für SignalR Events
 */
function setupEventHandlersV2() {
    // Battle State Updates
    connectionV2.on('ReceiveBattleStateV2', (data) => {
        console.log('[BattleV2 SignalR] Received Battle State:', data);
        handleBattleStateUpdateV2(data);
    });

    // Action Updates (schnellere Updates ohne kompletten State)
    connectionV2.on('ReceiveActionV2', (data) => {
        console.log('[BattleV2 SignalR] Received Action:', data);
        handleActionUpdateV2(data);
    });

    // New Round Started
    connectionV2.on('NewRoundStartedV2', (data) => {
        console.log('[BattleV2 SignalR] New Round Started:', data);
        handleNewRoundV2(data);
    });

    // Combat Ended
    connectionV2.on('CombatEndedV2', (data) => {
        console.log('[BattleV2 SignalR] Combat Ended:', data);
        handleCombatEndedV2(data);
    });

    // Initiative Roll (Neues Feature)
    connectionV2.on('ReceiveInitiativeRollV2', (data) => {
        console.log('[BattleV2 SignalR] Initiative Roll:', data);
        handleInitiativeRollV2(data);
    });

    // Turn Phase Update (Neues Feature)
    connectionV2.on('ReceiveTurnPhaseV2', (data) => {
        console.log('[BattleV2 SignalR] Turn Phase Update:', data);
        handleTurnPhaseUpdateV2(data);
    });

    // Connection Events
    connectionV2.onreconnecting((error) => {
        console.warn('[BattleV2 SignalR] Reconnecting...', error);
        isConnectedV2 = false;
        showReconnectingNotificationV2();
    });

    connectionV2.onreconnected((connectionId) => {
        console.log('[BattleV2 SignalR] Reconnected:', connectionId);
        isConnectedV2 = true;
        hideReconnectingNotificationV2();
    });

    connectionV2.onclose((error) => {
        console.error('[BattleV2 SignalR] Connection closed:', error);
        isConnectedV2 = false;
        showConnectionErrorV2();
    });
}

/**
 * Sendet einen Battle State Update zum Server
 */
export async function sendBattleStateUpdateV2(sessionId, battleState, currentRound, currentTurnIndex) {
    if (!isConnectedV2) {
        console.warn('[BattleV2 SignalR] Nicht verbunden, kann State nicht senden');
        return false;
    }

    try {
        await connectionV2.invoke('UpdateBattleStateV2',
            sessionId,
            JSON.stringify(battleState),
            currentRound,
            currentTurnIndex
        );
        return true;
    } catch (error) {
        console.error('[BattleV2 SignalR] Fehler beim Senden des Battle States:', error);
        return false;
    }
}

/**
 * Broadcastet eine Action an alle Clients
 */
export async function broadcastActionV2(sessionId, actionType, actionData) {
    if (!isConnectedV2) {
        console.warn('[BattleV2 SignalR] Nicht verbunden, kann Action nicht senden');
        return false;
    }

    try {
        await connectionV2.invoke('BroadcastActionV2',
            sessionId,
            actionType,
            JSON.stringify(actionData)
        );
        return true;
    } catch (error) {
        console.error('[BattleV2 SignalR] Fehler beim Broadcasten der Action:', error);
        return false;
    }
}

/**
 * Startet eine neue Runde
 */
export async function startNewRoundV2(sessionId, roundNumber) {
    if (!isConnectedV2) {
        console.warn('[BattleV2 SignalR] Nicht verbunden');
        return false;
    }

    try {
        await connectionV2.invoke('StartNewRoundV2', sessionId, roundNumber);
        return true;
    } catch (error) {
        console.error('[BattleV2 SignalR] Fehler beim Starten der neuen Runde:', error);
        return false;
    }
}

/**
 * Beendet den Kampf
 */
export async function endCombatV2(sessionId, result) {
    if (!isConnectedV2) {
        console.warn('[BattleV2 SignalR] Nicht verbunden');
        return false;
    }

    try {
        await connectionV2.invoke('EndCombatV2', sessionId, result);
        return true;
    } catch (error) {
        console.error('[BattleV2 SignalR] Fehler beim Beenden des Kampfes:', error);
        return false;
    }
}

/**
 * Broadcastet Initiative Roll (Neues Feature)
 */
export async function broadcastInitiativeRollV2(sessionId, initiativeData) {
    if (!isConnectedV2) {
        console.warn('[BattleV2 SignalR] Nicht verbunden');
        return false;
    }

    try {
        await connectionV2.invoke('BroadcastInitiativeRollV2',
            sessionId,
            JSON.stringify(initiativeData)
        );
        return true;
    } catch (error) {
        console.error('[BattleV2 SignalR] Fehler beim Broadcasten der Initiative:', error);
        return false;
    }
}

/**
 * Sendet Turn Phase Update (Neues Feature)
 */
export async function updateTurnPhaseV2(sessionId, phase, phaseData) {
    if (!isConnectedV2) {
        console.warn('[BattleV2 SignalR] Nicht verbunden');
        return false;
    }

    try {
        await connectionV2.invoke('UpdateTurnPhaseV2',
            sessionId,
            phase,
            JSON.stringify(phaseData)
        );
        return true;
    } catch (error) {
        console.error('[BattleV2 SignalR] Fehler beim Senden des Phase Updates:', error);
        return false;
    }
}

// ===== Event Handler Funktionen (werden von anderen Modulen implementiert) =====

function handleBattleStateUpdateV2(data) {
    // Wird von battle2-main.js überschrieben
    console.log('handleBattleStateUpdateV2 called (default)');
}

function handleActionUpdateV2(data) {
    // Wird von battle2-main.js überschrieben
    console.log('handleActionUpdateV2 called (default)');
}

function handleNewRoundV2(data) {
    // Wird von battle2-main.js überschrieben
    console.log('handleNewRoundV2 called (default)');
}

function handleCombatEndedV2(data) {
    // Wird von battle2-main.js überschrieben
    console.log('handleCombatEndedV2 called (default)');
}

function handleInitiativeRollV2(data) {
    // Wird von battle2-main.js überschrieben
    console.log('handleInitiativeRollV2 called (default)');
}

function handleTurnPhaseUpdateV2(data) {
    // Wird von battle2-main.js überschrieben
    console.log('handleTurnPhaseUpdateV2 called (default)');
}

// ===== UI Notification Functions =====

function showReconnectingNotificationV2() {
    console.log('[BattleV2 UI] Zeige Reconnecting Notification');
    // TODO: Implement UI notification
}

function hideReconnectingNotificationV2() {
    console.log('[BattleV2 UI] Verstecke Reconnecting Notification');
    // TODO: Implement UI notification
}

function showConnectionErrorV2() {
    console.error('[BattleV2 UI] Zeige Connection Error');
    // TODO: Implement UI notification with reload option
    alert('Verbindung zum Server verloren. Bitte lade die Seite neu.');
}

/**
 * Disconnected von SignalR
 */
export async function disconnectV2() {
    if (connectionV2) {
        try {
            await connectionV2.stop();
            console.log('[BattleV2 SignalR] Verbindung getrennt');
        } catch (error) {
            console.error('[BattleV2 SignalR] Fehler beim Trennen:', error);
        }
    }
}

/**
 * Überprüft ob verbunden
 */
export function isConnectedToHubV2() {
    return isConnectedV2;
}

// Export connection für direkten Zugriff falls nötig
export function getConnectionV2() {
    return connectionV2;
}
