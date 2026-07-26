/**
 * battle2-globals.js
 * Globale Funktionen und Stubs für BattleV2
 *
 * WICHTIG: Wird vor battle2-main.js geladen (non-module)
 * Definiert globale Funktionen, die von der View aufgerufen werden können
 */

// Globale Placeholder-Funktionen
// Diese werden später von battle2-main.js überschrieben

window.openEffectModalV2 = function(type) {
    console.log('[BattleV2] openEffectModalV2 called:', type);
};

window.closeEffectModalV2 = function() {
    console.log('[BattleV2] closeEffectModalV2 called');
};

window.removeBiomV2 = function() {
    console.log('[BattleV2] removeBiomV2 called');
};

window.openBiomDescriptionPanelV2 = function() {
    console.log('[BattleV2] openBiomDescriptionPanelV2 called');
};

window.closeBiomPanelV2 = function() {
    console.log('[BattleV2] closeBiomPanelV2 called');
};

window.openAddParticipantModalV2 = function() {
    console.log('[BattleV2] openAddParticipantModalV2 called');
};

window.closeAddParticipantModalV2 = function() {
    console.log('[BattleV2] closeAddParticipantModalV2 called');
};

window.toggleDefeatedPanelV2 = function() {
    console.log('[BattleV2] toggleDefeatedPanelV2 called');
};

window.toggleViewModeV2 = function(mode) {
    console.log('[BattleV2] toggleViewModeV2 called:', mode);

    // Simple implementation
    const gridBtn = document.getElementById('btnViewGrid');
    const listBtn = document.getElementById('btnViewList');
    const container = document.getElementById('participantsContainerV2');

    if (mode === 'grid') {
        gridBtn?.classList.add('bg-purple-500', 'text-white');
        gridBtn?.classList.remove('text-gray-400');
        listBtn?.classList.remove('bg-purple-500', 'text-white');
        listBtn?.classList.add('text-gray-400');

        container?.classList.remove('grid-cols-1');
        container?.classList.add('grid-cols-1', 'md:grid-cols-2', 'lg:grid-cols-3');
    } else {
        listBtn?.classList.add('bg-purple-500', 'text-white');
        listBtn?.classList.remove('text-gray-400');
        gridBtn?.classList.remove('bg-purple-500', 'text-white');
        gridBtn?.classList.add('text-gray-400');

        container?.classList.remove('md:grid-cols-2', 'lg:grid-cols-3');
        container?.classList.add('grid-cols-1');
    }
};

window.nextTurnV2 = function() {
    console.log('[BattleV2] nextTurnV2 called');
};

window.endBattleV2 = function() {
    console.log('[BattleV2] endBattleV2 called');
};

// State Management (wird von battle2-main.js gefüllt)
window.battleStateV2 = {
    sessionId: null,
    currentRound: 1,
    currentTurnIndex: 0,
    participants: [],
    defeatedEnemies: [],
    activeBiom: null,
    activeFieldEffects: [],
    isInitialized: false
};

console.log('[BattleV2 Globals] Globale Funktionen registriert');
