// ===== BATTLE SYSTEM - UTILITY FUNCTIONS =====

import { battleState } from './battle-state.js';

// ===== HELPER FUNCTIONS =====
export function getWoundTierByPercent(pct) {
    if (pct < 10) return 4; // Tödlich
    if (pct < 25) return 3; // Schwer
    if (pct < 50) return 2; // Kompliziert
    if (pct < 75) return 1; // Einfach
    return 0; // Keine Wunde
}

export function getWoundNameByTier(tier) {
    const names = ['Keine', 'Einfach', 'Kompliziert', 'Schwer', 'Tödlich'];
    return names[tier] || 'Keine';
}

export function getWoundsByHp(current, max) {
    // Diese Funktion wird jetzt nur noch für die Anzeige verwendet
    // Tatsächliche Wunden werden im wounds Array getrackt
    return [];
}

export function sortParticipants(participants) {
    return [...participants].sort((a, b) => {
        const aInv = a.activeConditions.includes('Unsichtbar');
        const bInv = b.activeConditions.includes('Unsichtbar');
        if (aInv && !bInv) return -1;
        if (!aInv && bInv) return 1;
        return a.initiative - b.initiative;
    });
}

export function calcConditionDamage(p, type, globalRound) {
    if (!p.activeConditions.includes(type)) return 0;
    const startRound = p.conditionStartRounds[type] || globalRound;
    const duration = globalRound - startRound + 1; // Wie lange schon betroffen (Runde)
    const level = p.conditionLevels[type] || 1;     // Stufe (1-6)

    if (type === 'Vergiftet') {
        // Formel: Schaden = Runde × 2^(Stufe-1)
        if (level === 0) return 0;
        return duration * Math.pow(2, level - 1);
    } else if (type === 'Brennend') {
        // Formel: Schaden = Runde × 4 × 2^(Stufe-1) = Runde × 2^(Stufe+1)
        if (level === 0) return 0;
        return duration * 4 * Math.pow(2, level - 1);
    }

    return 0;
}

export function applyAutoDamage(p, round) {
    let updated = { ...p };
    let totalDam = 0;

    if (updated.activeConditions.includes('Vergiftet')) {
        totalDam += calcConditionDamage(updated, 'Vergiftet', round);
    }
    if (updated.activeConditions.includes('Brennend')) {
        totalDam += calcConditionDamage(updated, 'Brennend', round);
    }

    if (totalDam > 0) {
        // Vorher HP% und Wundstufe
        const beforePercent = (updated.currentHealth / updated.maxHealth) * 100;
        const beforeTier = getWoundTierByPercent(beforePercent);

        let rem = totalDam;
        if (updated.tempHealth > 0) {
            const consumed = Math.min(updated.tempHealth, rem);
            updated.tempHealth -= consumed;
            rem -= consumed;
        }
        updated.currentHealth = Math.max(0, updated.currentHealth - rem);

        // Nachher HP% und Wundstufe
        const afterPercent = (updated.currentHealth / updated.maxHealth) * 100;
        const afterTier = getWoundTierByPercent(afterPercent);

        // Wunden hinzufügen (NUR für Spieler-Charaktere, NICHT für Begleiter oder Monster)
        if (updated.type === 'player' && afterTier > beforeTier) {
            // Eine Wunde für die erreichte Stufe
            if (afterTier > 0) {
                updated.wounds.push(getWoundNameByTier(afterTier));
            }
        }

        if (updated.currentHealth === 0) {
            if (updated.type === 'enemy') updated.isDead = true;
            else updated.isFallen = true;
        }
    }
    return updated;
}

// ===== PARTICIPANTS ERSTELLEN =====
export function buildParticipants(setup) {
    const participants = [];

    // Characters hinzufügen
    setup.characterIds.forEach(charId => {
        const char = BATTLE_MODEL_DATA.characters.find(c => c.id === charId);
        if (!char) {
            return;
        }

        const initKey = `char-${charId}`;
        const initiative = setup.initiatives[initKey] || 50;

        // Bei Kampfstart: Wenn CurrentHealth 0 ist, auf MaxHealth setzen
        const maxHP = char.maxHealth || 150;
        const currentHP = (char.currentHealth && char.currentHealth > 0) ? char.currentHealth : maxHP;

        const participant = {
            id: `char-${charId}`,
            characterId: charId, // DB-ID für Speicherung
            name: char.name,
            initiative: parseInt(initiative),
            originalInitiative: parseInt(initiative),
            type: char.isBegleiter ? 'companion' : 'player',
            currentHealth: currentHP,
            maxHealth: maxHP,
            currentPokus: char.currentPokus || 0,
            maxPokus: char.maxPokus || 30,
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

        participants.push(participant);
    });

    // Monster hinzufügen
    // Zähle Monster vom gleichen Typ für Nummerierung
    const monsterTypeCounts = {};
    const monsterTypeCounters = {};

    // Erste Durchlauf: Zähle wie viele Monster vom gleichen Typ es gibt
    setup.monsters.forEach((monsterSetup) => {
        const monster = BATTLE_MODEL_DATA.monsters.find(m => m.id === monsterSetup.monsterId);
        if (!monster) return;

        const baseName = monster.name;
        monsterTypeCounts[baseName] = (monsterTypeCounts[baseName] || 0) + 1;
    });

    // Zweiter Durchlauf: Erstelle Monster mit Nummerierung
    setup.monsters.forEach((monsterSetup, index) => {
        const monster = BATTLE_MODEL_DATA.monsters.find(m => m.id === monsterSetup.monsterId);
        if (!monster) return;

        const initKey = `monster-${index}`;
        const initiative = setup.initiatives[initKey] || 50;

        const baseName = monster.name;
        let displayName = baseName;

        // Nummerierung nur wenn mehrere Monster vom gleichen Typ
        if (monsterTypeCounts[baseName] > 1) {
            monsterTypeCounters[baseName] = (monsterTypeCounters[baseName] || 0) + 1;
            displayName = `${baseName} ${monsterTypeCounters[baseName]}`;
        }

        participants.push({
            id: `monster-${index}`,
            name: displayName,
            baseName: baseName, // Speichere Original-Namen für spätere Referenz
            initiative: parseInt(initiative),
            originalInitiative: parseInt(initiative),
            type: 'enemy',
            currentHealth: monsterSetup.health,
            maxHealth: monsterSetup.health,
            currentPokus: 0,
            maxPokus: 0,
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
        });
    });

    // Benutzerdefinierte Gegner hinzufügen (wie Monster, aber mit eigenem Namen)
    if (setup.customEnemies) {
        setup.customEnemies.forEach((enemy, index) => {
            const initKey = `customEnemy-${index}`;
            const initiative = setup.initiatives[initKey] || 50;

            participants.push({
                id: `customEnemy-${index}`,
                name: enemy.name,
                baseName: enemy.name,
                initiative: parseInt(initiative),
                originalInitiative: parseInt(initiative),
                type: 'enemy',
                currentHealth: enemy.health,
                maxHealth: enemy.health,
                currentPokus: 0,
                maxPokus: 0,
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
            });
        });
    }

    // Extra-Teilnehmer hinzufügen (nur Name + HP, keine DB-Referenz, kein Pokus)
    if (setup.extraParticipants) {
        // Zähle Extra-Teilnehmer vom gleichen Namen für Nummerierung
        const extraTypeCounts = {};
        const extraTypeCounters = {};

        // Erste Durchlauf: Zähle wie viele Extras vom gleichen Namen es gibt
        setup.extraParticipants.forEach((extra) => {
            const baseName = extra.name;
            extraTypeCounts[baseName] = (extraTypeCounts[baseName] || 0) + 1;
        });

        // Zweiter Durchlauf: Erstelle Extras mit Nummerierung
        setup.extraParticipants.forEach((extra, index) => {
            const initKey = `extra-${index}`;
            const initiative = setup.initiatives[initKey] || 50;

            const baseName = extra.name;
            let displayName = baseName;

            // Nummerierung nur wenn mehrere Extras vom gleichen Namen
            if (extraTypeCounts[baseName] > 1) {
                extraTypeCounters[baseName] = (extraTypeCounters[baseName] || 0) + 1;
                displayName = `${baseName} ${extraTypeCounters[baseName]}`;
            }

            participants.push({
                id: `extra-${index}`,
                name: displayName,
                baseName: baseName, // Speichere Original-Namen für spätere Referenz
                initiative: parseInt(initiative),
                originalInitiative: parseInt(initiative),
                type: 'companion',
                currentHealth: extra.health,
                maxHealth: extra.health,
                currentPokus: 0,
                maxPokus: 0,
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
            });
        });
    }

    // Nach Initiative sortieren (Unsichtbar zuerst)
    battleState.participants = sortParticipants(participants);
}

export function showResult(type, title, message) {
    document.getElementById('battleGridScreen').classList.add('hidden');
    document.getElementById('resultScreen').classList.remove('hidden');

    document.getElementById('resultIcon').textContent = type === 'victory' ? '🏆' : '💀';
    document.getElementById('resultTitle').textContent = title;
    document.getElementById('resultMessage').textContent = message;

    // LocalStorage löschen
    localStorage.removeItem('battleSetup');

    // TODO: HP/Pokus Updates an Backend senden
}

export function toggleDefeatedPanel() {
    const panel = document.getElementById('defeatedPanel');
    const overlay = document.getElementById('defeatedPanelOverlay');

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
