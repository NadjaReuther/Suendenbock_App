// ===== BATTLE SYSTEM - GLOBAL FUNCTION STUBS =====
// This file provides immediate global function stubs that will be replaced
// by the actual implementations once the ES6 modules are loaded.
// This ensures onclick attributes work even before modules finish loading.

// Placeholder functions that will be replaced by battle-main.js
window.battleGlobalsReady = false;

// Queue for calls made before modules are ready
window.battleCallQueue = [];

// Helper to queue or execute calls
function queueOrExecute(fnName, args) {
    if (window.battleGlobalsReady && window[fnName + '_impl']) {
        return window[fnName + '_impl'].apply(null, args);
    } else {
        console.warn(`Function ${fnName} called before modules loaded, queueing...`);
        window.battleCallQueue.push({ fnName, args });
    }
}

// Actions
window.nextTurn = function() { return queueOrExecute('nextTurn', arguments); };
window.endBattle = function() { return queueOrExecute('endBattle', arguments); };
window.applyAction = function() { return queueOrExecute('applyAction', arguments); };
window.castMagic = function() { return queueOrExecute('castMagic', arguments); };
window.removeWound = function() { return queueOrExecute('removeWound', arguments); };
window.cycleDownedStatus = function() { return queueOrExecute('cycleDownedStatus', arguments); };
window.confirmRevive = function() { return queueOrExecute('confirmRevive', arguments); };

// Conditions
window.toggleCondition = function() { return queueOrExecute('toggleCondition', arguments); };
window.setConditionLevel = function() { return queueOrExecute('setConditionLevel', arguments); };
window.resetInitiative = function() { return queueOrExecute('resetInitiative', arguments); };
window.openConditionsPanel = function() { return queueOrExecute('openConditionsPanel', arguments); };
window.toggleConditionsPanel = function() { return queueOrExecute('toggleConditionsPanel', arguments); };
window.toggleConditionsSelection = function() { return queueOrExecute('toggleConditionsSelection', arguments); };

// Drag & Drop
window.handleDragStart = function() { return queueOrExecute('handleDragStart', arguments); };
window.handleDragOver = function() { return queueOrExecute('handleDragOver', arguments); };
window.handleDragLeave = function() { return queueOrExecute('handleDragLeave', arguments); };
window.handleDrop = function() { return queueOrExecute('handleDrop', arguments); };
window.handleDragEnd = function() { return queueOrExecute('handleDragEnd', arguments); };
window.moveParticipantUp = function() { return queueOrExecute('moveParticipantUp', arguments); };
window.moveParticipantDown = function() { return queueOrExecute('moveParticipantDown', arguments); };

// Utils
window.toggleDefeatedPanel = function() { return queueOrExecute('toggleDefeatedPanel', arguments); };

// Field Effects & Biomes
window.openEffectModal = function() { return queueOrExecute('openEffectModal', arguments); };
window.closeEffectModal = function() { return queueOrExecute('closeEffectModal', arguments); };
window.switchEffectTab = function() { return queueOrExecute('switchEffectTab', arguments); };
window.addFieldEffect = function() { return queueOrExecute('addFieldEffect', arguments); };
window.setBiom = function() { return queueOrExecute('setBiom', arguments); };
window.removeBiom = function() { return queueOrExecute('removeBiom', arguments); };
window.closeFieldEffectPanel = function() { return queueOrExecute('closeFieldEffectPanel', arguments); };
window.openBiomDescriptionPanel = function() { return queueOrExecute('openBiomDescriptionPanel', arguments); };
window.closeBiomPanel = function() { return queueOrExecute('closeBiomPanel', arguments); };

// Participants
window.openAddParticipantModal = function() { return queueOrExecute('openAddParticipantModal', arguments); };
window.closeAddParticipantModal = function() { return queueOrExecute('closeAddParticipantModal', arguments); };
window.switchAddParticipantTab = function() { return queueOrExecute('switchAddParticipantTab', arguments); };
window.addCharacterToBattle = function() { return queueOrExecute('addCharacterToBattle', arguments); };
window.addMonsterToBattle = function() { return queueOrExecute('addMonsterToBattle', arguments); };
window.addCustomParticipantToBattle = function() { return queueOrExecute('addCustomParticipantToBattle', arguments); };
