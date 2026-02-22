// ===== BATTLE SYSTEM - CONDITION MANAGEMENT =====

import { battleState } from './battle-state.js';
import { CONDITION_CONFIG, IS_GOTT_ROLE } from './battle-config.js';
import { renderBattleGrid } from './battle-rendering.js';
import { syncBattleState } from './battle-signalr.js';

// Import next turn function - will be set from battle-actions
let nextTurn;

export function setConditionDependencies(deps) {
    nextTurn = deps.nextTurn;
}

// Make functions available globally for onclick handlers
if (typeof window !== 'undefined') {
    window.toggleCondition = toggleCondition;
    window.setConditionLevel = setConditionLevel;
    window.resetInitiative = resetInitiative;
    window.openConditionsPanel = openConditionsPanel;
    window.toggleConditionsPanel = toggleConditionsPanel;
}

export function toggleConditionsSelection(index) {
    battleState.expandedConditions[index] = !battleState.expandedConditions[index];
    renderBattleGrid();
}

export function toggleCondition(pIdx, condition) {
    if (!IS_GOTT_ROLE) {
        return;
    }
    const p = battleState.participants[pIdx];
    const hasCondition = p.activeConditions.includes(condition);
    let autoEndTurn = false;

    // Heldenmut ist ein globaler Buff für ALLE Verbündeten
    if (condition === 'Heldenmut') {
        battleState.participants.forEach((participant, idx) => {
            // Nur Spieler und Begleiter, keine Gegner
            if (participant.type !== 'enemy') {
                if (hasCondition) {
                    // Heldenmut von ALLEN entfernen
                    participant.activeConditions = participant.activeConditions.filter(c => c !== 'Heldenmut');
                } else {
                    // Heldenmut für ALLE aktivieren
                    if (!participant.activeConditions.includes('Heldenmut')) {
                        participant.activeConditions = [...participant.activeConditions, 'Heldenmut'];
                    }
                }
            }
        });

        renderBattleGrid();
        syncBattleState();
        return;
    }

    if (hasCondition) {
        p.activeConditions = p.activeConditions.filter(c => c !== condition);
        delete p.conditionCounters[condition];
        delete p.conditionLevels[condition];
        delete p.conditionStartRounds[condition];

        // Unsichtbar entfernt → Initiative zurücksetzen
        if (condition === 'Unsichtbar') {
            p.initiative = p.originalInitiative;
        }
    } else {
        p.activeConditions = [...p.activeConditions, condition];
        if (condition === 'Übergebend') p.conditionCounters[condition] = 3;
        if (condition === 'Vergiftet' || condition === 'Brennend') {
            p.conditionLevels[condition] = 1;
            p.conditionStartRounds[condition] = battleState.currentRound;
        }
        if (condition === 'Liegend' && pIdx === battleState.currentTurnIndex && !p.isFallen) autoEndTurn = true;
        if (condition === 'Ohnmächtig' && pIdx === battleState.currentTurnIndex) autoEndTurn = true;

        // Unsichtbar aktiviert → auf Position 1 setzen (Initiative = 0)
        if (condition === 'Unsichtbar') {
            p.initiative = 0;
        }
    }

    if (autoEndTurn) nextTurn();
    else {
        renderBattleGrid();
        // SignalR: State synchronisieren
        syncBattleState();
    }
}

export function setConditionLevel(pIdx, condition, level) {
    if (!IS_GOTT_ROLE) {
        return;
    }
    battleState.participants[pIdx].conditionLevels[condition] = parseInt(level);
    renderBattleGrid();

    // SignalR: State synchronisieren
    syncBattleState();
}

export async function resetInitiative(pIdx) {
    if (!IS_GOTT_ROLE) {
        return;
    }
    const p = battleState.participants[pIdx];

    // Bei Unsichtbar nur warnen, nicht zurücksetzen
    if (p.activeConditions.includes('Unsichtbar')) {
        await Swal.fire({
            icon: 'warning',
            title: 'Initiative blockiert',
            text: 'Unsichtbare Charaktere können ihre Initiative nicht manuell zurücksetzen. Entferne zuerst den Zustand "Unsichtbar".',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    p.initiative = p.originalInitiative;
    renderBattleGrid();

    // SignalR: State synchronisieren
    syncBattleState();
}

// Open conditions panel and render all conditions
export function openConditionsPanel() {
    renderConditionsList();
    toggleConditionsPanel();
}

// Toggle conditions panel
export function toggleConditionsPanel() {
    const panel = document.getElementById('conditionsPanel');
    const overlay = document.getElementById('conditionsPanelOverlay');

    if (!panel || !overlay) return;

    const isOpen = panel.classList.contains('panel-open');

    if (isOpen) {
        // Panel schließen
        panel.classList.remove('panel-open');
        overlay.classList.remove('overlay-active');
        setTimeout(() => {
            overlay.classList.add('hidden');
        }, 300);
    } else {
        // Panel öffnen
        overlay.classList.remove('hidden');
        setTimeout(() => {
            overlay.classList.add('overlay-active');
        }, 10);
        panel.classList.add('panel-open');
    }
}

// Render all conditions with descriptions
function renderConditionsList() {
    const container = document.getElementById('conditionsListContainer');
    if (!container) return;

    const html = Object.entries(CONDITION_CONFIG).map(([name, config]) => `
        <div class="${config.color} border-2 ${config.border} rounded-lg p-4">
            <div class="flex items-center gap-2 mb-2">
                <span class="${config.text} font-bold text-sm uppercase tracking-wide">${name}</span>
                ${name === 'Vergiftet' || name === 'Brennend' ? '<span class="text-xs text-white/50">(Stufen 1-6)</span>' : ''}
            </div>
            <p class="text-gray-300 text-xs leading-relaxed">${config.description}</p>
        </div>
    `).join('');

    container.innerHTML = html;
}
