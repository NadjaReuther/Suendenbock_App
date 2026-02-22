// ===== BATTLE SYSTEM - ADD PARTICIPANTS DURING COMBAT =====

import { battleState } from './battle-state.js';
import { renderBattleGrid } from './battle-rendering.js';
import { syncBattleState } from './battle-signalr.js';

// Make functions available globally for onclick handlers
if (typeof window !== 'undefined') {
    window.openAddParticipantModal = openAddParticipantModal;
    window.closeAddParticipantModal = closeAddParticipantModal;
    window.switchAddParticipantTab = switchAddParticipantTab;
    window.addCharacterToBattle = addCharacterToBattle;
    window.addMonsterToBattle = addMonsterToBattle;
    window.addCustomParticipantToBattle = addCustomParticipantToBattle;
}

export function openAddParticipantModal() {
    const modal = document.getElementById('addParticipantModal');
    modal.classList.remove('hidden');
    switchAddParticipantTab('character');
}

export function closeAddParticipantModal() {
    const modal = document.getElementById('addParticipantModal');
    modal.classList.add('hidden');

    // Reset form fields
    document.getElementById('addCharacterSelect').value = '';
    document.getElementById('addCharacterInitiative').value = '50';
    document.getElementById('addMonsterSelect').value = '';
    document.getElementById('addMonsterInitiative').value = '50';
    document.getElementById('addCustomName').value = '';
    document.getElementById('addCustomHealth').value = '';
    document.getElementById('addCustomInitiative').value = '50';
}

export function switchAddParticipantTab(tab) {
    // Update tabs
    const tabs = {
        'character': { button: 'tabAddCharacter', content: 'addCharacterTab' },
        'monster': { button: 'tabAddMonster', content: 'addMonsterTab' },
        'custom': { button: 'tabAddCustom', content: 'addCustomTab' }
    };

    Object.keys(tabs).forEach(key => {
        const button = document.getElementById(tabs[key].button);
        const content = document.getElementById(tabs[key].content);

        if (key === tab) {
            button.classList.add('border-blue-500', 'text-blue-300');
            button.classList.remove('border-transparent', 'text-gray-400');
            content.classList.remove('hidden');
        } else {
            button.classList.remove('border-blue-500', 'text-blue-300');
            button.classList.add('border-transparent', 'text-gray-400');
            content.classList.add('hidden');
        }
    });
}

export async function addCharacterToBattle() {
    const select = document.getElementById('addCharacterSelect');
    const selectedOption = select.options[select.selectedIndex];
    const initiative = parseInt(document.getElementById('addCharacterInitiative').value);

    if (!selectedOption.value) {
        await Swal.fire({
            title: 'Fehler',
            text: 'Bitte wähle einen Charakter aus.',
            icon: 'error'
        });
        return;
    }

    if (!initiative || initiative < 0) {
        await Swal.fire({
            title: 'Fehler',
            text: 'Bitte gib eine gültige Initiative ein.',
            icon: 'error'
        });
        return;
    }

    const characterId = parseInt(selectedOption.value);
    const name = selectedOption.getAttribute('data-name');
    const maxHealth = parseInt(selectedOption.getAttribute('data-health'));
    const maxPokus = parseInt(selectedOption.getAttribute('data-pokus'));

    // Create new participant
    const newParticipant = {
        id: `character-${characterId}-${Date.now()}`,
        characterId: characterId,
        name: name,
        initiative: initiative,
        originalInitiative: initiative,
        type: 'player',
        currentHealth: maxHealth,
        maxHealth: maxHealth,
        currentPokus: maxPokus,
        maxPokus: maxPokus,
        tempHealth: 0,
        isDead: false,
        isFallen: false,
        activeConditions: [],
        conditionCounters: {},
        conditionLevels: {},
        conditionStartRounds: {},
        downedSaves: { Handeln: 'none', Wissen: 'none', Soziales: 'none' },
        wounds: [],
        readyToRevive: false
    };

    // Remember current participant
    const currentParticipant = battleState.participants[battleState.currentTurnIndex];

    // Add to battle state
    battleState.participants.push(newParticipant);

    // Re-sort by initiative (lower number = higher priority)
    battleState.participants.sort((a, b) => a.initiative - b.initiative);

    // Update currentTurnIndex to point to the same participant
    if (currentParticipant) {
        battleState.currentTurnIndex = battleState.participants.findIndex(p => p.id === currentParticipant.id);
        if (battleState.currentTurnIndex === -1) {
            battleState.currentTurnIndex = 0;
        }
    }

    // Re-render
    renderBattleGrid();

    // Sync via SignalR
    await syncBattleState();

    await Swal.fire({
        title: 'Erfolgreich',
        text: `${name} wurde dem Kampf hinzugefügt!`,
        icon: 'success',
        timer: 2000,
        showConfirmButton: false
    });

    closeAddParticipantModal();
}

export async function addMonsterToBattle() {
    const select = document.getElementById('addMonsterSelect');
    const selectedOption = select.options[select.selectedIndex];
    const initiative = parseInt(document.getElementById('addMonsterInitiative').value);

    if (!selectedOption.value) {
        await Swal.fire({
            title: 'Fehler',
            text: 'Bitte wähle ein Monster aus.',
            icon: 'error'
        });
        return;
    }

    if (!initiative || initiative < 0) {
        await Swal.fire({
            title: 'Fehler',
            text: 'Bitte gib eine gültige Initiative ein.',
            icon: 'error'
        });
        return;
    }

    const monsterId = parseInt(selectedOption.value);
    const baseName = selectedOption.getAttribute('data-name');
    const maxHealth = parseInt(selectedOption.getAttribute('data-health'));
    const imagePath = selectedOption.getAttribute('data-image');

    // Count existing monsters with this base name
    const existingCount = battleState.participants.filter(p =>
        p.baseName === baseName && p.type === 'enemy'
    ).length;

    const monsterNumber = existingCount + 1;
    const name = existingCount > 0 ? `${baseName} ${monsterNumber}` : baseName;

    // Create new participant
    const newParticipant = {
        id: `monster-${monsterId}-${Date.now()}`,
        monsterId: monsterId,
        name: name,
        baseName: baseName,
        initiative: initiative,
        originalInitiative: initiative,
        type: 'enemy', // Monster sind immer Gegner (rot dargestellt, sterben bei 0 HP)
        currentHealth: maxHealth,
        maxHealth: maxHealth,
        tempHealth: 0,
        imagePath: imagePath,
        isDead: false,
        isFallen: false,
        activeConditions: [],
        conditionCounters: {},
        conditionLevels: {},
        conditionStartRounds: {},
        wounds: []
    };

    // Remember current participant
    const currentParticipant = battleState.participants[battleState.currentTurnIndex];

    // Add to battle state
    battleState.participants.push(newParticipant);

    // Re-sort by initiative (lower number = higher priority)
    battleState.participants.sort((a, b) => a.initiative - b.initiative);

    // Update currentTurnIndex to point to the same participant
    if (currentParticipant) {
        battleState.currentTurnIndex = battleState.participants.findIndex(p => p.id === currentParticipant.id);
        if (battleState.currentTurnIndex === -1) {
            battleState.currentTurnIndex = 0;
        }
    }

    // Re-render
    renderBattleGrid();

    // Sync via SignalR
    await syncBattleState();

    await Swal.fire({
        title: 'Erfolgreich',
        text: `${name} wurde dem Kampf hinzugefügt!`,
        icon: 'success',
        timer: 2000,
        showConfirmButton: false
    });

    closeAddParticipantModal();
}

export async function addCustomParticipantToBattle() {
    const name = document.getElementById('addCustomName').value.trim();
    const maxHealth = parseInt(document.getElementById('addCustomHealth').value);
    const initiative = parseInt(document.getElementById('addCustomInitiative').value);

    // Get selected type (enemy or extra)
    const typeRadio = document.querySelector('input[name="customType"]:checked');
    const participantType = typeRadio ? typeRadio.value : 'extra';

    if (!name) {
        await Swal.fire({
            title: 'Fehler',
            text: 'Bitte gib einen Namen ein.',
            icon: 'error'
        });
        return;
    }

    if (!maxHealth || maxHealth <= 0) {
        await Swal.fire({
            title: 'Fehler',
            text: 'Bitte gib gültige Lebenspunkte ein.',
            icon: 'error'
        });
        return;
    }

    if (!initiative || initiative < 0) {
        await Swal.fire({
            title: 'Fehler',
            text: 'Bitte gib eine gültige Initiative ein.',
            icon: 'error'
        });
        return;
    }

    // Create new participant
    const newParticipant = {
        id: `custom-${participantType}-${Date.now()}`,
        name: name,
        baseName: name,
        initiative: initiative,
        originalInitiative: initiative,
        type: participantType, // 'enemy' or 'extra'
        currentHealth: maxHealth,
        maxHealth: maxHealth,
        tempHealth: 0,
        imagePath: '/images/default-participant.png',
        isDead: false,
        isFallen: false,
        activeConditions: [],
        conditionCounters: {},
        conditionLevels: {},
        conditionStartRounds: {},
        wounds: []
    };

    // Remember current participant
    const currentParticipant = battleState.participants[battleState.currentTurnIndex];

    // Add to battle state
    battleState.participants.push(newParticipant);

    // Re-sort by initiative (lower number = higher priority)
    battleState.participants.sort((a, b) => a.initiative - b.initiative);

    // Update currentTurnIndex to point to the same participant
    if (currentParticipant) {
        battleState.currentTurnIndex = battleState.participants.findIndex(p => p.id === currentParticipant.id);
        if (battleState.currentTurnIndex === -1) {
            battleState.currentTurnIndex = 0;
        }
    }

    // Re-render
    renderBattleGrid();

    // Sync via SignalR
    await syncBattleState();

    const typeText = participantType === 'enemy' ? 'Gegner' : 'Teilnehmer';
    await Swal.fire({
        title: 'Erfolgreich',
        text: `${name} wurde als ${typeText} dem Kampf hinzugefügt!`,
        icon: 'success',
        timer: 2000,
        showConfirmButton: false
    });

    closeAddParticipantModal();
}
