// ===== BATTLE SYSTEM - INITIALIZATION =====

import { battleState } from './battle-state.js';
import { setupSignalR } from './battle-signalr.js';
import { buildParticipants } from './battle-utils.js';
import { renderBattleGrid, updateTurnIndicator } from './battle-rendering.js';
import { renderFieldEffects, renderBiomDisplay, applyBiomBackground } from './battle-field-effects.js';

// ===== BATTLE STATE VOM SERVER LADEN =====
async function loadBattleStateFromServer(sessionId) {
    try {
        // Combat Session vom Server abrufen
        const response = await fetch(`/api/battle/session/${sessionId}`);

        if (!response.ok) {
            throw new Error('Combat Session nicht gefunden');
        }

        const session = await response.json();

        // Battle State aus JSON parsen
        const savedState = JSON.parse(session.battleStateJson);

        // Wenn Participants noch nicht existieren (Setup-Daten), müssen wir sie erstellen
        if (!savedState.participants || savedState.participants.length === 0) {
            buildParticipants(savedState);
        } else {
            // Participants existieren bereits (wurde vom Gott schon erstellt)
            battleState.participants = savedState.participants;
        }

        battleState.currentRound = session.currentRound;
        battleState.currentTurnIndex = session.currentTurnIndex;

        if (savedState.expandedConditions) {
            battleState.expandedConditions = savedState.expandedConditions;
        }

        if (savedState.activeFieldEffects) {
            battleState.activeFieldEffects = savedState.activeFieldEffects;
        }

        // UI rendern
        renderBattleGrid();
        updateTurnIndicator();
        renderFieldEffects();

    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler beim Laden',
            text: 'Der Kampf konnte nicht geladen werden. Bitte versuche es erneut.',
            confirmButtonColor: '#d97706'
        });
        window.location.href = '/Spielmodus/Dashboard';
    }
}

// ===== INITIALIZATION =====
export async function initBattle() {
    // WICHTIG: SessionId aus URL-Query lesen (Spieler werden mit ?sessionId=123 weitergeleitet)
    const urlParams = new URLSearchParams(window.location.search);
    const sessionIdFromUrl = urlParams.get('sessionId');

    if (sessionIdFromUrl) {
        // Spieler kommt via Broadcast-Redirect → SessionId aus URL verwenden
        battleState.sessionId = parseInt(sessionIdFromUrl);

        // Battle State von Server laden
        await loadBattleStateFromServer(battleState.sessionId);
    } else {
        // Gott hat Kampf direkt gestartet → Daten aus localStorage
        const setup = JSON.parse(localStorage.getItem('battleSetup') || '{}');

        if (!setup.monsters || !setup.characterIds || !setup.initiatives) {
            await Swal.fire({
                icon: 'error',
                title: 'Keine Battle-Daten',
                text: 'Bitte starte den Kampf über Combat Setup.',
                confirmButtonColor: '#d97706'
            });
            window.location.href = '/Spielmodus/CombatSetup';
            return;
        }

        // SessionId speichern (falls vorhanden)
        if (setup.sessionId) {
            battleState.sessionId = setup.sessionId;
        }

        // Participants erstellen aus localStorage
        buildParticipants(setup);

        // Battle Grid rendern
        renderBattleGrid();

        // Erste Runde starten
        updateTurnIndicator();
    }

    // SignalR verbinden (falls sessionId vorhanden)
    if (battleState.sessionId) {
        await setupSignalR(battleState.sessionId);
    }

    // Feldeffekte rendern
    renderFieldEffects();

    // Biom rendern und anwenden (falls vorhanden)
    if (battleState.activeBiom) {
        renderBiomDisplay();
        applyBiomBackground();
    }
}
