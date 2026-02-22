// ===== BATTLE SYSTEM - COMBAT ACTIONS =====

import { battleState } from './battle-state.js';
import { IS_GOTT_ROLE, SAVE_KEYS } from './battle-config.js';
import { getWoundTierByPercent, getWoundNameByTier, calcConditionDamage, applyAutoDamage, sortParticipants } from './battle-utils.js';
import { renderBattleGrid, updateTurnIndicator } from './battle-rendering.js';
import { syncBattleState, getIsConnected, getSignalRConnection } from './battle-signalr.js';

// Make functions available globally for onclick handlers
if (typeof window !== 'undefined') {
    window.applyAction = applyAction;
    window.castMagic = castMagic;
    window.removeWound = removeWound;
    window.cycleDownedStatus = cycleDownedStatus;
    window.confirmRevive = confirmRevive;
    window.nextTurn = nextTurn;
    window.endBattle = endBattle;
}

// ===== ACTIONS =====
export function applyAction(index, mode) {
    if (!IS_GOTT_ROLE) {
        return;
    }
    if (battleState.isAnimating) return;

    const inputVal = parseInt(document.getElementById(`healthInput-${index}`).value) || 0;
    const p = battleState.participants[index];

    if (mode === 'damage') {
        // Vorher HP% und Wundstufe
        const beforePercent = (p.currentHealth / p.maxHealth) * 100;
        const beforeTier = getWoundTierByPercent(beforePercent);

        let totalDamage = inputVal;
        if (p.activeConditions.includes('Vergiftet')) {
            totalDamage += calcConditionDamage(p, 'Vergiftet', battleState.currentRound);
        }

        let remainingDamage = totalDamage;
        if (p.tempHealth > 0) {
            const consumedTemp = Math.min(p.tempHealth, remainingDamage);
            p.tempHealth -= consumedTemp;
            remainingDamage -= consumedTemp;
        }
        p.currentHealth = Math.max(0, p.currentHealth - remainingDamage);

        // Nachher HP% und Wundstufe
        const afterPercent = (p.currentHealth / p.maxHealth) * 100;
        const afterTier = getWoundTierByPercent(afterPercent);

        // Wunden hinzufügen (NUR für Spieler-Charaktere, NICHT für Begleiter oder Monster)
        if (p.type === 'player' && afterTier > beforeTier) {
            // Eine Wunde für die erreichte Stufe
            if (afterTier > 0) {
                p.wounds.push(getWoundNameByTier(afterTier));
            }
        }

        if (p.currentHealth === 0) {
            if (p.type === 'enemy') p.isDead = true;
            else p.isFallen = true;
        }
    } else if (mode === 'heal') {
        p.currentHealth = Math.min(p.maxHealth, p.currentHealth + inputVal);
    } else if (mode === 'temp') {
        p.tempHealth += inputVal;
    }

    document.getElementById(`healthInput-${index}`).value = '';
    renderBattleGrid();
    checkVictoryDefeat();

    // SignalR: State synchronisieren
    syncBattleState();
}

export function removeWound(pIdx, woundIdx) {
    if (!IS_GOTT_ROLE) {
        return;
    }
    const p = battleState.participants[pIdx];
    p.wounds.splice(woundIdx, 1);
    renderBattleGrid();

    // SignalR: State synchronisieren
    syncBattleState();
}

export async function castMagic(index) {
    if (!IS_GOTT_ROLE) {
        return;
    }
    const p = battleState.participants[index];

    if (p.activeConditions.includes('Blutend')) {
        await Swal.fire({
            icon: 'warning',
            title: 'Magie blockiert',
            text: 'Blutende Charaktere können keine Magie wirken!',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    // Zauber-Zähler erhöhen
    p.currentPokus += 1;
    renderBattleGrid();

    // SignalR: State synchronisieren
    syncBattleState();
}

export function cycleDownedStatus(pIdx, key) {
    if (!IS_GOTT_ROLE) {
        return;
    }
    const p = battleState.participants[pIdx];

    const keyIndex = SAVE_KEYS.indexOf(key);
    if (keyIndex > 0) {
        const prevKey = SAVE_KEYS[keyIndex - 1];
        if (p.downedSaves[prevKey] === 'none') return;
    }

    const current = p.downedSaves[key];
    let next = 'none';
    if (current === 'none') next = 'success';
    else if (current === 'success') next = 'fail';
    else next = 'none';

    const updatedSaves = { ...p.downedSaves, [key]: next };
    if (next === 'none') {
        for (let i = keyIndex + 1; i < SAVE_KEYS.length; i++) {
            updatedSaves[SAVE_KEYS[i]] = 'none';
        }
    }

    p.downedSaves = updatedSaves;

    let successes = 0;
    let fails = 0;
    SAVE_KEYS.forEach(k => {
        if (p.downedSaves[k] === 'success') successes++;
        if (p.downedSaves[k] === 'fail') fails++;
    });

    let mustEndTurn = false;
    if (successes >= 2) {
        // Bestätigung erforderlich vor Rettung
        p.readyToRevive = true;
    } else if (fails >= 2) {
        p.isDead = true;
        p.isFallen = false;
        if (pIdx === battleState.currentTurnIndex) mustEndTurn = true;
    }

    if (mustEndTurn) nextTurn();
    else {
        renderBattleGrid();
        // SignalR: State synchronisieren
        syncBattleState();
    }
}

export function confirmRevive(pIdx) {
    if (!IS_GOTT_ROLE) {
        return;
    }
    const p = battleState.participants[pIdx];

    p.isFallen = false;
    p.currentHealth = 10;
    p.activeConditions = Array.from(new Set([...p.activeConditions, 'Liegend']));
    p.downedSaves = { Handeln: 'none', Wissen: 'none', Soziales: 'none' };
    p.readyToRevive = false;

    if (pIdx === battleState.currentTurnIndex) {
        nextTurn();
    } else {
        renderBattleGrid();
        syncBattleState();
    }
}

// ===== TURN MANAGEMENT =====
export function nextTurn() {
    if (!IS_GOTT_ROLE) {
        return;
    }
    if (battleState.isAnimating) return;

    // Übergebend-Counter reduzieren
    const finishedP = battleState.participants[battleState.currentTurnIndex];
    if (finishedP.activeConditions.includes('Übergebend')) {
        finishedP.conditionCounters['Übergebend']--;
        if (finishedP.conditionCounters['Übergebend'] <= 0) {
            finishedP.activeConditions = finishedP.activeConditions.filter(c => c !== 'Übergebend');
            delete finishedP.conditionCounters['Übergebend'];
        }
    }

    // Nächster Index
    let nextIdxInCurrentList = (battleState.currentTurnIndex + 1) % battleState.participants.length;
    let tempRound = battleState.currentRound;
    let isNewRound = false;
    if (nextIdxInCurrentList === 0) {
        tempRound++;
        isNewRound = true;
    }

    let newTurnIdx;

    if (isNewRound) {
        // Beim Rundenwechsel: Alle Initiativen zurücksetzen (außer Unsichtbar)
        battleState.participants.forEach(p => {
            if (!p.activeConditions.includes('Unsichtbar')) {
                p.initiative = p.originalInitiative;
            }
        });

        // Sortieren nach Initiative
        battleState.participants = sortParticipants(battleState.participants);

        // WICHTIG: Bei Rundenwechsel IMMER bei Index 0 beginnen (kleinste Initiative)
        newTurnIdx = 0;
    } else {
        // Kein Rundenwechsel: Merke die ID des nächsten Spielers VOR dem Sortieren!
        const nextActorId = battleState.participants[nextIdxInCurrentList].id;

        // Sortieren (falls Unsichtbar geändert wurde)
        battleState.participants = sortParticipants(battleState.participants);

        // Finde den nächsten Spieler nach ID
        newTurnIdx = battleState.participants.findIndex(p => p.id === nextActorId);
    }

    // Auto-Damage anwenden
    battleState.participants[newTurnIdx] = applyAutoDamage(battleState.participants[newTurnIdx], tempRound);

    // Skip-Check
    const checkSkip = (idx) => {
        const p = battleState.participants[idx];
        if (p.isDead) return true;
        if (p.activeConditions.includes('Liegend') && !p.isFallen) {
            battleState.participants[idx].activeConditions = p.activeConditions.filter(c => c !== 'Liegend');
            return true;
        }
        if (p.activeConditions.includes('Ohnmächtig')) return true;
        return false;
    };

    let attempts = 0;
    while (checkSkip(newTurnIdx) && attempts < battleState.participants.length) {
        newTurnIdx = (newTurnIdx + 1) % battleState.participants.length;
        if (newTurnIdx === 0) tempRound++;
        attempts++;
    }

    battleState.currentTurnIndex = newTurnIdx;
    battleState.currentRound = tempRound;
    battleState.expandedConditions = {};

    renderBattleGrid();
    updateTurnIndicator();
    checkVictoryDefeat();

    // SignalR: State synchronisieren
    syncBattleState();
}

// ===== VICTORY/DEFEAT =====
async function checkVictoryDefeat() {
    const aliveEnemies = battleState.participants.filter(p => p.type === 'enemy' && !p.isDead);
    const aliveAllies = battleState.participants.filter(p => p.type !== 'enemy' && !p.isDead);

    if (aliveEnemies.length === 0) {
        // Sieg!
        if (IS_GOTT_ROLE && getIsConnected() && getSignalRConnection() && battleState.sessionId) {
            try {
                await getSignalRConnection().invoke("EndCombat", battleState.sessionId, "victory");
            } catch (error) {
                // Fehler stillschweigend behandeln
            }
        }
        setTimeout(async () => await showResultScreen('victory'), 800);
    } else if (aliveAllies.length === 0) {
        // Niederlage!
        if (IS_GOTT_ROLE && getIsConnected() && getSignalRConnection() && battleState.sessionId) {
            try {
                await getSignalRConnection().invoke("EndCombat", battleState.sessionId, "defeat");
            } catch (error) {
                // Fehler stillschweigend behandeln
            }
        }
        setTimeout(async () => await showResultScreen('defeat'), 800);
    }
}

// Speichert Charakterdaten in DB nach Kampfende
async function saveCharacterData() {
    const characterData = battleState.participants
        .filter(p => p.type === 'player') // NUR Spieler (keine Begleiter, keine Monster)
        .map(p => ({
            characterId: p.characterId,
            type: p.type,
            currentHealth: p.currentHealth,
            currentPokus: p.currentPokus
        }));

    try {
        await fetch('/api/battle/save-character-data', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ characters: characterData })
        });
    } catch (error) {
        console.error('[saveCharacterData] Error saving character data:', error);
    }
}

export async function showResultScreen(result) {
    // Charakterdaten speichern (nur für Gott, um Duplikate zu vermeiden)
    if (IS_GOTT_ROLE) {
        await saveCharacterData();
    }

    let icon, title, message;

    switch(result) {
        case 'victory':
            icon = '🏆';
            title = 'Sieg!';
            message = 'Alle Gegner wurden besiegt!';
            break;
        case 'defeat':
            icon = '💀';
            title = 'Niederlage!';
            message = 'Die Gruppe wurde besiegt...';
            break;
        case 'aborted':
        case 'manual':
            icon = '⚔️';
            title = 'Gefecht beendet';
            message = 'Der Kampf wurde vom Gott abgebrochen.';
            break;
        default:
            icon = '⚔️';
            title = 'Kampf beendet';
            message = 'Der Kampf ist vorbei.';
    }

    document.getElementById('battleGridScreen').classList.add('hidden');
    document.getElementById('resultScreen').classList.remove('hidden');

    document.getElementById('resultIcon').textContent = icon;
    document.getElementById('resultTitle').textContent = title;
    document.getElementById('resultMessage').textContent = message;

    // LocalStorage löschen
    localStorage.removeItem('battleSetup');
}

export async function endBattle() {
    if (!IS_GOTT_ROLE) {
        await Swal.fire({
            icon: 'warning',
            title: 'Keine Berechtigung',
            text: 'Nur der Gott kann den Kampf beenden.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    const result = await Swal.fire({
        title: 'Gefecht beenden?',
        text: 'Möchtest du das Gefecht wirklich beenden?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#d97706',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Ja, beenden',
        cancelButtonText: 'Abbrechen'
    });

    if (!result.isConfirmed) return;

    // SignalR: Kampf beenden (an alle Teilnehmer senden)
    if (getIsConnected() && getSignalRConnection() && battleState.sessionId) {
        try {
            await getSignalRConnection().invoke("EndCombat", battleState.sessionId, "aborted");
        } catch (error) {
            // Fehler stillschweigend behandeln
        }
    }

    // Gott sieht auch den Result Screen
    await showResultScreen("aborted");
}
