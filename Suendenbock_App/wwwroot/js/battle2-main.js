/**
 * battle2-main.js
 * Hauptmodul für BattleV2
 *
 * Koordiniert alle Battle-Logik, SignalR-Kommunikation und UI-Updates
 */

import {
    initializeSignalRConnectionV2,
    sendBattleStateUpdateV2,
    broadcastActionV2,
    startNewRoundV2,
    endCombatV2,
    isConnectedToHubV2
} from './battle2-signalr.js';

// ===== State Management =====
let battleState = {
    sessionId: SESSION_ID_V2 || null,
    currentRound: 1,
    currentTurnIndex: 0,
    participants: [],
    defeatedEnemies: [],
    activeBiom: null,
    activeFieldEffects: [],
    isInitialized: false
};

// ===== Initialization =====

/**
 * Initialisiert das BattleV2 System
 */
async function initializeBattleV2() {
    console.log('[BattleV2 Main] Initialisiere Battle System V2...');

    try {
        // 1. SignalR Connection herstellen
        if (battleState.sessionId) {
            const connected = await initializeSignalRConnectionV2(battleState.sessionId);
            if (!connected) {
                console.error('[BattleV2 Main] SignalR-Verbindung fehlgeschlagen');
                showErrorNotification('Verbindung zum Server fehlgeschlagen');
                return;
            }
        }

        // 2. Initiale Participants laden (falls noch keine Session)
        if (!battleState.sessionId) {
            console.log('[BattleV2 Main] Keine Session ID - warte auf CombatSetup');
            // Falls direkt auf Battle2 ohne Setup zugegriffen wird
            initializeEmptyBattle();
        }

        // 3. UI Event Listeners registrieren
        setupEventListeners();

        // 4. Initiale UI Render
        renderBattleUI();

        battleState.isInitialized = true;
        console.log('[BattleV2 Main] Battle System V2 erfolgreich initialisiert');

    } catch (error) {
        console.error('[BattleV2 Main] Initialisierungsfehler:', error);
        showErrorNotification('Fehler beim Laden des Kampfsystems');
    }
}

/**
 * Setup Event Listeners für UI-Interaktionen
 */
function setupEventListeners() {
    // Global function implementations
    window.nextTurnV2 = handleNextTurn;
    window.endBattleV2 = handleEndBattle;
    window.openAddParticipantModalV2 = openAddParticipantModal;
    window.closeAddParticipantModalV2 = closeAddParticipantModal;
    window.toggleDefeatedPanelV2 = toggleDefeatedPanel;

    console.log('[BattleV2 Main] Event Listeners registriert');
}

/**
 * Initialisiert einen leeren Battle State (für direkten Zugriff ohne Setup)
 */
function initializeEmptyBattle() {
    console.log('[BattleV2 Main] Initialisiere leeren Battle State');

    // Leere Participants Liste
    battleState.participants = [];
    battleState.currentRound = 1;
    battleState.currentTurnIndex = 0;

    updateRoundDisplay();
    updateTurnIndicator('Noch keine Teilnehmer - füge Charaktere/Monster hinzu');
}

// ===== Battle Logic =====

/**
 * Nächster Zug
 */
function handleNextTurn() {
    if (!IS_GOTT_V2) {
        console.warn('[BattleV2 Main] Nur Gott kann Züge steuern');
        return;
    }

    console.log('[BattleV2 Main] Nächster Zug');

    battleState.currentTurnIndex++;

    // Wenn alle Teilnehmer dran waren, nächste Runde
    if (battleState.currentTurnIndex >= battleState.participants.length) {
        battleState.currentTurnIndex = 0;
        battleState.currentRound++;

        console.log(`[BattleV2 Main] Neue Runde: ${battleState.currentRound}`);

        // Broadcast new round
        if (battleState.sessionId) {
            startNewRoundV2(battleState.sessionId, battleState.currentRound);
        }
    }

    // UI Update
    renderBattleUI();

    // State Sync mit Server
    if (battleState.sessionId) {
        sendBattleStateUpdateV2(
            battleState.sessionId,
            battleState,
            battleState.currentRound,
            battleState.currentTurnIndex
        );
    }
}

/**
 * Kampf beenden
 */
async function handleEndBattle() {
    if (!IS_GOTT_V2) {
        console.warn('[BattleV2 Main] Nur Gott kann Kämpfe beenden');
        return;
    }

    const result = await Swal.fire({
        title: 'Kampf beenden?',
        text: 'Möchtest du den Kampf wirklich beenden?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sieg',
        cancelButtonText: 'Niederlage',
        showDenyButton: true,
        denyButtonText: 'Abbrechen',
        background: '#1e293b',
        color: '#fff',
        confirmButtonColor: '#22c55e',
        cancelButtonColor: '#ef4444'
    });

    if (result.isConfirmed) {
        console.log('[BattleV2 Main] Kampf beendet - Sieg');
        if (battleState.sessionId) {
            await endCombatV2(battleState.sessionId, 'victory');
        }
        showResultScreen('victory');
    } else if (result.dismiss === Swal.DismissReason.cancel) {
        console.log('[BattleV2 Main] Kampf beendet - Niederlage');
        if (battleState.sessionId) {
            await endCombatV2(battleState.sessionId, 'defeat');
        }
        showResultScreen('defeat');
    }
}

// ===== UI Rendering =====

/**
 * Rendert die komplette Battle UI
 */
function renderBattleUI() {
    updateRoundDisplay();
    updateTurnIndicator();
    renderParticipants();
    renderActiveEffects();
    updateDefeatedCount();
}

/**
 * Update Round Display
 */
function updateRoundDisplay() {
    const roundElement = document.getElementById('currentRoundV2');
    if (roundElement) {
        roundElement.textContent = battleState.currentRound;
    }
}

/**
 * Update Turn Indicator
 */
function updateTurnIndicator(customText = null) {
    const indicator = document.getElementById('turnIndicatorV2');
    if (!indicator) return;

    if (customText) {
        indicator.textContent = customText;
        return;
    }

    if (battleState.participants.length === 0) {
        indicator.textContent = 'Keine Teilnehmer im Kampf';
        return;
    }

    const currentParticipant = battleState.participants[battleState.currentTurnIndex];
    if (currentParticipant) {
        indicator.textContent = `${currentParticipant.name} ist am Zug`;
    }
}

/**
 * Rendert alle Participants
 */
function renderParticipants() {
    const container = document.getElementById('participantsContainerV2');
    if (!container) return;

    if (battleState.participants.length === 0) {
        container.innerHTML = `
            <div class="col-span-full text-center py-12">
                <p class="text-gray-400 text-lg">Noch keine Teilnehmer im Kampf</p>
                ${IS_GOTT_V2 ? '<button onclick="openAddParticipantModalV2()" class="mt-4 bg-purple-500 hover:bg-purple-600 text-white px-6 py-3 rounded-lg font-semibold">Teilnehmer hinzufügen</button>' : ''}
            </div>
        `;
        return;
    }

    container.innerHTML = battleState.participants.map((participant, index) => {
        const isCurrentTurn = index === battleState.currentTurnIndex;
        return `
            <div class="bg-gradient-to-br from-slate-800/80 to-slate-900/80 backdrop-blur-xl border ${isCurrentTurn ? 'border-purple-500 shadow-lg shadow-purple-500/50' : 'border-purple-500/20'} rounded-xl p-4 transition-all ${isCurrentTurn ? 'scale-105' : 'hover:scale-102'}">
                <div class="flex items-center justify-between mb-3">
                    <div class="flex items-center space-x-3">
                        ${participant.imagePath ? `<img src="${participant.imagePath}" alt="${participant.name}" class="w-12 h-12 rounded-lg object-cover border border-purple-500/30" />` : ''}
                        <div>
                            <h3 class="text-white font-bold text-lg">${participant.name}</h3>
                            <p class="text-gray-400 text-sm">${participant.type === 'character' ? 'Charakter' : 'Monster'}</p>
                        </div>
                    </div>
                    ${isCurrentTurn ? '<span class="material-symbols-outlined text-purple-400 text-2xl animate-pulse">radio_button_checked</span>' : ''}
                </div>

                <!-- Health Bar -->
                <div class="mb-2">
                    <div class="flex justify-between text-sm mb-1">
                        <span class="text-gray-400">HP</span>
                        <span class="text-white font-semibold">${participant.currentHealth}/${participant.maxHealth}</span>
                    </div>
                    <div class="w-full bg-slate-950 rounded-full h-3 overflow-hidden">
                        <div class="bg-gradient-to-r from-red-500 to-pink-500 h-full rounded-full transition-all" style="width: ${(participant.currentHealth / participant.maxHealth) * 100}%"></div>
                    </div>
                </div>

                <!-- Initiative -->
                <div class="text-sm text-gray-400">
                    Initiative: <span class="text-purple-400 font-semibold">${participant.initiative || 50}</span>
                </div>
            </div>
        `;
    }).join('');
}

/**
 * Rendert aktive Effekte (Biom, Field Effects)
 */
function renderActiveEffects() {
    // Biom Banner
    const biomBanner = document.getElementById('biomBannerV2');
    if (battleState.activeBiom && biomBanner) {
        biomBanner.classList.remove('hidden');
        document.getElementById('activeBiomNameV2').textContent = battleState.activeBiom.name;
    } else if (biomBanner) {
        biomBanner.classList.add('hidden');
    }

    // Field Effects Banner
    const effectsBanner = document.getElementById('fieldEffectsBannerV2');
    const effectsContainer = document.getElementById('activeFieldEffectsContainerV2');

    if (battleState.activeFieldEffects.length > 0 && effectsBanner && effectsContainer) {
        effectsBanner.classList.remove('hidden');
        effectsContainer.innerHTML = battleState.activeFieldEffects.map(effect => `
            <div class="bg-amber-900/40 border border-amber-500/50 rounded-lg px-3 py-2 text-sm text-amber-300 font-semibold">
                ${effect.name}
            </div>
        `).join('');
    } else if (effectsBanner) {
        effectsBanner.classList.add('hidden');
    }
}

/**
 * Update Defeated Count Badge
 */
function updateDefeatedCount() {
    const badge = document.getElementById('defeatedCountBadgeV2');
    if (!badge) return;

    if (battleState.defeatedEnemies.length > 0) {
        badge.textContent = battleState.defeatedEnemies.length;
        badge.classList.remove('hidden');
    } else {
        badge.classList.add('hidden');
    }
}

// ===== Modal Functions =====

function openAddParticipantModal() {
    console.log('[BattleV2 Main] Open Add Participant Modal - TODO');
    alert('Add Participant Modal wird in einem späteren Schritt implementiert');
}

function closeAddParticipantModal() {
    console.log('[BattleV2 Main] Close Add Participant Modal - TODO');
}

function toggleDefeatedPanel() {
    console.log('[BattleV2 Main] Toggle Defeated Panel - TODO');
}

// ===== Result Screen =====

function showResultScreen(result) {
    const screen = document.getElementById('resultScreenV2');
    const icon = document.getElementById('resultIconV2');
    const title = document.getElementById('resultTitleV2');
    const message = document.getElementById('resultMessageV2');

    if (!screen || !icon || !title || !message) return;

    if (result === 'victory') {
        icon.textContent = '🎉';
        title.textContent = 'SIEG!';
        title.className = 'text-7xl font-["Orbitron"] font-black uppercase tracking-wider text-green-400';
        message.textContent = 'Die Gruppe hat den Kampf gewonnen!';
    } else {
        icon.textContent = '💀';
        title.textContent = 'NIEDERLAGE';
        title.className = 'text-7xl font-["Orbitron"] font-black uppercase tracking-wider text-red-400';
        message.textContent = 'Die Gruppe wurde besiegt...';
    }

    screen.classList.remove('hidden');
    screen.classList.add('flex');
}

// ===== Utility Functions =====

function showErrorNotification(message) {
    Swal.fire({
        title: 'Fehler',
        text: message,
        icon: 'error',
        background: '#1e293b',
        color: '#fff',
        confirmButtonColor: '#ef4444'
    });
}

// ===== Initialize on Load =====

document.addEventListener('DOMContentLoaded', () => {
    console.log('[BattleV2 Main] DOM geladen, starte Initialisierung');
    initializeBattleV2();
});

// Export für Testing/Debugging
window.battleV2Debug = {
    getState: () => battleState,
    setState: (newState) => { battleState = newState; renderBattleUI(); },
    render: renderBattleUI
};

console.log('[BattleV2 Main] Modul geladen');
