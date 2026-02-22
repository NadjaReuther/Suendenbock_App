// ===== BATTLE SYSTEM - STATE MANAGEMENT =====

// ===== GLOBAL STATE =====
export let battleState = {
    participants: [],
    currentTurnIndex: 0,
    currentRound: 1,
    isAnimating: false,
    expandedConditions: {},
    sessionId: null,
    activeFieldEffects: [], // Array of { feldEffektId, name, colorCode, schwere }
    activeBiom: null // Object { biomId, name, colorCode, description } - nur ein Biom gleichzeitig
};

// Export a setter to allow other modules to update the entire state if needed
export function setBattleState(newState) {
    battleState = newState;
}
