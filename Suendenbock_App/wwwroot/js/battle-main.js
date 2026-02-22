// ===== BATTLE SYSTEM - MAIN ENTRY POINT =====
// This file imports all modules and initializes the battle system

// Import all modules
import { initBattle } from './battle-init.js';
import { setRenderingDependencies, toggleConditionsSelection } from './battle-rendering.js';
import { setConditionDependencies } from './battle-conditions.js';
import { applyAction, castMagic, removeWound, cycleDownedStatus, confirmRevive, nextTurn, endBattle } from './battle-actions.js';
import { toggleCondition, setConditionLevel, resetInitiative, openConditionsPanel, toggleConditionsPanel } from './battle-conditions.js';
import { handleDragStart, handleDragOver, handleDragLeave, handleDrop, handleDragEnd, moveParticipantUp, moveParticipantDown } from './battle-drag-drop.js';
import { toggleDefeatedPanel } from './battle-utils.js';
import {
    openEffectModal,
    closeEffectModal,
    switchEffectTab,
    addFieldEffect,
    setBiom,
    removeBiom,
    closeFieldEffectPanel,
    openBiomDescriptionPanel,
    closeBiomPanel
} from './battle-field-effects.js';
import {
    openAddParticipantModal,
    closeAddParticipantModal,
    switchAddParticipantTab,
    addCharacterToBattle,
    addMonsterToBattle,
    addCustomParticipantToBattle
} from './battle-participants.js';

// Replace the placeholder functions with actual implementations
// Store implementations with _impl suffix
if (typeof window !== 'undefined') {
    // Actions
    window.nextTurn_impl = nextTurn;
    window.endBattle_impl = endBattle;
    window.applyAction_impl = applyAction;
    window.castMagic_impl = castMagic;
    window.removeWound_impl = removeWound;
    window.cycleDownedStatus_impl = cycleDownedStatus;
    window.confirmRevive_impl = confirmRevive;

    // Conditions
    window.toggleCondition_impl = toggleCondition;
    window.setConditionLevel_impl = setConditionLevel;
    window.resetInitiative_impl = resetInitiative;
    window.openConditionsPanel_impl = openConditionsPanel;
    window.toggleConditionsPanel_impl = toggleConditionsPanel;
    window.toggleConditionsSelection_impl = toggleConditionsSelection;

    // Drag & Drop
    window.handleDragStart_impl = handleDragStart;
    window.handleDragOver_impl = handleDragOver;
    window.handleDragLeave_impl = handleDragLeave;
    window.handleDrop_impl = handleDrop;
    window.handleDragEnd_impl = handleDragEnd;
    window.moveParticipantUp_impl = moveParticipantUp;
    window.moveParticipantDown_impl = moveParticipantDown;

    // Utils
    window.toggleDefeatedPanel_impl = toggleDefeatedPanel;

    // Field Effects & Biomes
    window.openEffectModal_impl = openEffectModal;
    window.closeEffectModal_impl = closeEffectModal;
    window.switchEffectTab_impl = switchEffectTab;
    window.addFieldEffect_impl = addFieldEffect;
    window.setBiom_impl = setBiom;
    window.removeBiom_impl = removeBiom;
    window.closeFieldEffectPanel_impl = closeFieldEffectPanel;
    window.openBiomDescriptionPanel_impl = openBiomDescriptionPanel;
    window.closeBiomPanel_impl = closeBiomPanel;

    // Participants
    window.openAddParticipantModal_impl = openAddParticipantModal;
    window.closeAddParticipantModal_impl = closeAddParticipantModal;
    window.switchAddParticipantTab_impl = switchAddParticipantTab;
    window.addCharacterToBattle_impl = addCharacterToBattle;
    window.addMonsterToBattle_impl = addMonsterToBattle;
    window.addCustomParticipantToBattle_impl = addCustomParticipantToBattle;

    // Mark globals as ready
    window.battleGlobalsReady = true;

    // Process any queued calls
    if (window.battleCallQueue && window.battleCallQueue.length > 0) {
        console.log(`Processing ${window.battleCallQueue.length} queued battle calls...`);
        window.battleCallQueue.forEach(call => {
            if (window[call.fnName + '_impl']) {
                window[call.fnName + '_impl'].apply(null, call.args);
            }
        });
        window.battleCallQueue = [];
    }
}

// Set up dependencies between modules
// The rendering module needs references to action functions
setRenderingDependencies({
    applyAction,
    castMagic,
    removeWound,
    resetInitiative,
    cycleDownedStatus,
    confirmRevive,
    openConditionsPanel,
    handleDragStart,
    handleDragOver,
    handleDragLeave,
    handleDrop,
    handleDragEnd,
    moveParticipantUp,
    moveParticipantDown
});

// The conditions module needs reference to nextTurn
setConditionDependencies({
    nextTurn
});

// Initialize battle on DOM ready
document.addEventListener('DOMContentLoaded', () => {
    initBattle();
});

// Event listeners for field effect and biom select dropdowns
document.addEventListener('DOMContentLoaded', function() {
    const fieldEffectSelect = document.getElementById('fieldEffectSelect');
    if (fieldEffectSelect) {
        fieldEffectSelect.addEventListener('change', function() {
            const selectedOption = this.options[this.selectedIndex];
            const description = selectedOption.getAttribute('data-description');
            const descContainer = document.getElementById('fieldEffectDescription');
            const descText = document.getElementById('fieldEffectDescriptionText');

            if (description && description.trim()) {
                descText.innerHTML = description;
                descContainer.classList.remove('hidden');
            } else {
                descContainer.classList.add('hidden');
            }
        });
    }

    const biomSelect = document.getElementById('biomSelect');
    if (biomSelect) {
        biomSelect.addEventListener('change', function() {
            const selectedOption = this.options[this.selectedIndex];
            const description = selectedOption.getAttribute('data-description');
            const descContainer = document.getElementById('biomDescription');
            const descText = document.getElementById('biomDescriptionText');

            if (description && description.trim()) {
                descText.innerHTML = description;
                descContainer.classList.remove('hidden');
            } else {
                descContainer.classList.add('hidden');
            }
        });
    }
});
