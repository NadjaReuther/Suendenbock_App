// ===== BATTLE SYSTEM - VOLLSTÄNDIGE IMPLEMENTIERUNG =====

// ===== CONDITION CONFIGURATION =====
const CONDITION_CONFIG = {
    'Vergiftet': { color: 'bg-green-900/60', border: 'border-green-500', text: 'text-green-200' },
    'Brennend': { color: 'bg-orange-900/60', border: 'border-orange-500', text: 'text-orange-200' },
    'Liegend': { color: 'bg-amber-900/40', border: 'border-amber-700', text: 'text-amber-200' },
    'Ohnmächtig': { color: 'bg-purple-950/60', border: 'border-purple-800', text: 'text-purple-300' },
    'Verwirrt': { color: 'bg-fuchsia-900/40', border: 'border-fuchsia-700', text: 'text-fuchsia-200' },
    'Erschöpft': { color: 'bg-zinc-700/40', border: 'border-zinc-500', text: 'text-zinc-300' },
    'Sensibel': { color: 'bg-cyan-900/40', border: 'border-cyan-700', text: 'text-cyan-200' },
    'Verängstigt': { color: 'bg-yellow-200/10', border: 'border-yellow-500/40', text: 'text-yellow-200' },
    'Blutend': { color: 'bg-red-700/40', border: 'border-red-500', text: 'text-red-200' },
    'Verwundbar': { color: 'bg-orange-700/40', border: 'border-orange-500', text: 'text-orange-200' },
    'Übergebend': { color: 'bg-lime-900/40', border: 'border-lime-700', text: 'text-lime-200' },
    'Ergriffen': { color: 'bg-stone-800/60', border: 'border-stone-600', text: 'text-stone-300' },
    'Betrunken': { color: 'bg-amber-600/30', border: 'border-amber-500', text: 'text-amber-100' },
    'Rasend': { color: 'bg-red-950/60', border: 'border-red-600', text: 'text-red-400' },
    'Pokus fokussiert': { color: 'bg-teal-900/40', border: 'border-teal-600', text: 'text-teal-200' },
    'Unsichtbar': { color: 'bg-sky-900/30', border: 'border-sky-400', text: 'text-sky-200' },
    'Verflucht': { color: 'bg-indigo-950/60', border: 'border-indigo-700', text: 'text-indigo-300' },
    'Gesegnet': { color: 'bg-yellow-600/30', border: 'border-yellow-500', text: 'text-yellow-200' },
    'Taktisch': { color: 'bg-blue-900/40', border: 'border-blue-600', text: 'text-blue-200' },
    'Heldenmut': { color: 'bg-white/10', border: 'border-white/60', text: 'text-white' }
};

const CONDITIONS = Object.keys(CONDITION_CONFIG);
const SAVE_KEYS = ['Handeln', 'Wissen', 'Soziales'];

// ===== GLOBAL STATE =====
let battleState = {
    participants: [],
    currentTurnIndex: 0,
    currentRound: 1,
    isAnimating: false,
    expandedConditions: {},
    sessionId: null
};

// ===== SIGNALR CONNECTION =====
let signalRConnection = null;
let isConnected = false;

// ===== INITIALIZATION =====
document.addEventListener('DOMContentLoaded', () => {
    initBattle();
});

// ===== SIGNALR SETUP =====
async function setupSignalR(sessionId) {
    if (!sessionId) {
        console.warn('Keine SessionId vorhanden - Battle läuft ohne Real-time Sync');
        return;
    }

    try {
        // SignalR Connection erstellen
        signalRConnection = new signalR.HubConnectionBuilder()
            .withUrl("/battlehub")
            .withAutomaticReconnect()
            .build();

        // Event: Battle State Update empfangen
        signalRConnection.on("ReceiveBattleState", (data) => {
            console.log('Battle State Update empfangen:', data);

            // Nur updaten, wenn es nicht von diesem Client kommt
            if (data.sessionId === battleState.sessionId) {
                const newSetup = JSON.parse(data.battleStateJson);

                // State aktualisieren, aber bestehende Participants-Struktur beibehalten
                if (newSetup.participants) {
                    battleState.participants = newSetup.participants;
                }
                battleState.currentRound = data.currentRound;
                battleState.currentTurnIndex = data.currentTurnIndex;

                // UI neu rendern
                renderBattleGrid();
                updateTurnIndicator();
            }
        });

        // Event: Kampf beendet
        signalRConnection.on("CombatEnded", (data) => {
            console.log('Kampf beendet:', data);
            showResultScreen(data.result);
        });

        // Event: Quick Action empfangen
        signalRConnection.on("ReceiveAction", (data) => {
            console.log('Action empfangen:', data);
            // Optional: Hier könntest du Animationen triggern
        });

        // Verbindung starten
        await signalRConnection.start();
        console.log('SignalR verbunden');
        isConnected = true;

        // Combat Session beitreten
        await signalRConnection.invoke("JoinCombat", sessionId);
        console.log(`Combat Session ${sessionId} beigetreten`);

    } catch (error) {
        console.error('SignalR Fehler:', error);
        // Battle läuft trotzdem weiter, nur ohne Real-time Sync
    }
}

// ===== SIGNALR STATE SYNC =====
async function syncBattleState() {
    if (!isConnected || !signalRConnection || !battleState.sessionId) {
        return; // Kein SignalR - lokaler Modus
    }

    try {
        const battleStateJson = JSON.stringify({
            participants: battleState.participants,
            expandedConditions: battleState.expandedConditions
        });

        await signalRConnection.invoke(
            "UpdateBattleState",
            battleState.sessionId,
            battleStateJson,
            battleState.currentRound,
            battleState.currentTurnIndex
        );
    } catch (error) {
        console.error('Fehler beim State Sync:', error);
    }
}

async function initBattle() {
    // Daten aus localStorage laden
    const setup = JSON.parse(localStorage.getItem('battleSetup') || '{}');

    if (!setup.monsters || !setup.characterIds || !setup.initiatives) {
        alert('Keine Battle-Daten gefunden! Bitte starte den Kampf über Combat Setup.');
        window.location.href = '/Spielmodus/CombatSetup';
        return;
    }

    // DEBUG: BATTLE_MODEL_DATA prüfen
    console.log('=== BATTLE_MODEL_DATA ===', BATTLE_MODEL_DATA);
    console.log('Characters:', BATTLE_MODEL_DATA.characters);
    console.log('Monsters:', BATTLE_MODEL_DATA.monsters);

    // SessionId speichern (falls vorhanden)
    if (setup.sessionId) {
        battleState.sessionId = setup.sessionId;
    }

    // Participants erstellen
    buildParticipants(setup);

    // Battle Grid rendern
    renderBattleGrid();

    // Erste Runde starten
    updateTurnIndicator();

    // SignalR verbinden (falls sessionId vorhanden)
    if (battleState.sessionId) {
        await setupSignalR(battleState.sessionId);
    }
}

// ===== PARTICIPANTS ERSTELLEN =====
function buildParticipants(setup) {
    const participants = [];

    // Characters hinzufügen
    setup.characterIds.forEach(charId => {
        const char = BATTLE_MODEL_DATA.characters.find(c => c.id === charId);
        if (!char) {
            console.warn(`Character mit ID ${charId} nicht gefunden!`);
            return;
        }

        // DEBUG: Character-Daten prüfen
        console.log(`=== Character ${char.name} ===`);
        console.log('Full char object:', char);
        console.log('currentHealth:', char.currentHealth);
        console.log('maxHealth:', char.maxHealth);
        console.log('currentPokus:', char.currentPokus);
        console.log('maxPokus:', char.maxPokus);

        const initKey = `char-${charId}`;
        const initiative = setup.initiatives[initKey] || 50;

        // Bei Kampfstart: Wenn CurrentHealth 0 ist, auf MaxHealth setzen
        const maxHP = char.maxHealth || 150;
        const currentHP = (char.currentHealth && char.currentHealth > 0) ? char.currentHealth : maxHP;

        const participant = {
            id: `char-${charId}`,
            name: char.name,
            initiative: parseInt(initiative),
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
            downedSaves: { Handeln: 'none', Wissen: 'none', Soziales: 'none' }
        };

        console.log('Created participant:', participant);
        participants.push(participant);
    });

    // Monster hinzufügen
    setup.monsters.forEach((monsterSetup, index) => {
        const monster = BATTLE_MODEL_DATA.monsters.find(m => m.id === monsterSetup.monsterId);
        if (!monster) return;

        const initKey = `monster-${index}`;
        const initiative = setup.initiatives[initKey] || 50;

        participants.push({
            id: `monster-${index}`,
            name: monster.name,
            initiative: parseInt(initiative),
            type: 'enemy',
            currentHealth: monsterSetup.health,
            maxHealth: monsterSetup.health,
            currentPokus: monsterSetup.pokus || 0,
            maxPokus: monsterSetup.pokus || 0,
            tempHealth: 0,
            isDead: false,
            isFallen: false,
            activeConditions: [],
            conditionCounters: {},
            conditionLevels: {},
            conditionStartRounds: {},
            downedSaves: { Handeln: 'none', Wissen: 'none', Soziales: 'none' }
        });
    });

    // Nach Initiative sortieren (Unsichtbar zuerst)
    battleState.participants = sortParticipants(participants);
}

function sortParticipants(participants) {
    return [...participants].sort((a, b) => {
        const aInv = a.activeConditions.includes('Unsichtbar');
        const bInv = b.activeConditions.includes('Unsichtbar');
        if (aInv && !bInv) return -1;
        if (!aInv && bInv) return 1;
        return a.initiative - b.initiative;
    });
}

// ===== BATTLE GRID RENDERN =====
function renderBattleGrid() {
    const container = document.getElementById('participantsContainer');
    if (!container) return;

    container.innerHTML = battleState.participants.map((p, idx) => {
        return renderParticipantCard(p, idx);
    }).join('');
}

function renderParticipantCard(p, idx) {
    const isActive = idx === battleState.currentTurnIndex;
    const isExpanded = battleState.expandedConditions[idx];
    const isInvisible = p.activeConditions.includes('Unsichtbar');
    const isDowned = p.isFallen;

    const healthPercent = (p.currentHealth / Math.max(p.maxHealth, p.currentHealth + p.tempHealth)) * 100;
    const tempPercent = (p.tempHealth / Math.max(p.maxHealth, p.currentHealth + p.tempHealth)) * 100;

    const wounds = getWoundsByHp(p.currentHealth, p.maxHealth);

    let borderClasses = 'bg-black/40 border-white/5';
    if (isActive) {
        if (isInvisible) {
            borderClasses = 'bg-slate-500/10 border-slate-300 shadow-[0_0_15px_rgba(203,213,225,0.2)] ring-1 ring-slate-300/50';
        } else {
            borderClasses = 'bg-amber-500/10 border-amber-500 shadow-[0_0_15px_rgba(245,158,11,0.3)] ring-1 ring-amber-500/40';
        }
    }

    return `
        <div class="flex flex-col rounded border-2 transition-all duration-500 relative ${borderClasses} ${p.isDead ? 'opacity-20 grayscale cursor-not-allowed' : ''}"
             data-participant-index="${idx}">
            <!-- Main Info Row -->
            <div class="p-4 flex items-center justify-between gap-4 flex-wrap md:flex-nowrap min-h-[120px]">
                <!-- Initiative & Name -->
                <div class="flex items-center space-x-4">
                    <div class="${isActive ? (isInvisible ? 'bg-slate-300 text-black animate-pulse' : 'bg-amber-500 text-black animate-pulse') : 'bg-white/10 text-white/40'} w-12 h-12 rounded-full flex items-center justify-center font-bold text-base">
                        ${p.initiative}
                    </div>
                    <div>
                        <p class="${p.type === 'enemy' ? 'text-red-400' : 'text-white'} font-['MedievalSharp'] text-2xl">${p.name}</p>
                        <div class="flex flex-wrap gap-1 mt-1">
                            ${!p.isDead ? wounds.map(w => `
                                <span class="text-[10px] bg-red-600/60 text-white border border-red-400 px-2 py-1 rounded uppercase font-bold tracking-tighter flex items-center space-x-1 shadow-[0_0_8px_rgba(220,38,38,0.5)]">
                                    <span class="material-symbols-outlined text-[12px]">water_drop</span>
                                    <span>${w}</span>
                                </span>
                            `).join('') : ''}
                            ${p.isDead ? '<span class="text-xs bg-[#800020] text-white px-2 py-1 rounded uppercase font-black tracking-widest">Gefallen</span>' : ''}
                            ${!p.isDead ? p.activeConditions.map(c => {
                                const cfg = CONDITION_CONFIG[c] || { color: 'bg-white/20', border: 'border-white/40', text: 'text-white' };
                                const level = p.conditionLevels[c];
                                return `
                                    <span class="text-[10px] ${cfg.color} ${cfg.text} border ${cfg.border} px-2 py-1 rounded uppercase font-bold tracking-tighter flex items-center gap-1">
                                        ${c} ${level ? `(S${level})` : ''} ${c === 'Übergebend' ? `(${p.conditionCounters[c]})` : ''}
                                    </span>
                                `;
                            }).join('') : ''}
                        </div>
                    </div>
                </div>

                <!-- HP & Actions -->
                <div class="flex items-center space-x-6 ml-auto md:ml-0 flex-grow justify-end">
                    ${!p.isDead ? (isDowned ? renderDownedSaves(p, idx) : renderNormalActions(p, idx, healthPercent, tempPercent)) : ''}
                </div>
            </div>

            <!-- Expanded Conditions -->
            ${isExpanded && !isDowned && !p.isDead ? renderExpandedConditions(p, idx) : ''}
        </div>
    `;
}

function renderDownedSaves(p, idx) {
    return `
        <div class="flex items-center space-x-6 animate-in fade-in duration-500">
            <div class="flex flex-col items-end mr-2">
                <span class="text-sm uppercase font-display font-bold text-[#800020] animate-pulse tracking-widest">Todesrettung</span>
                <span class="text-xs uppercase text-white/30 tracking-widest">Reihenfolge einhalten</span>
            </div>

            <div class="flex gap-4">
                ${SAVE_KEYS.map((key, kIdx) => {
                    const status = p.downedSaves[key];
                    const prevStatus = kIdx > 0 ? p.downedSaves[SAVE_KEYS[kIdx - 1]] : 'success';
                    const isDisabled = prevStatus === 'none';

                    return `
                        <div class="flex flex-col items-center space-y-1 transition-opacity ${isDisabled ? 'opacity-20 grayscale' : 'opacity-100'}">
                            <span class="text-xs uppercase tracking-tighter text-white/40 font-bold">${key}</span>
                            <button
                                onclick="${!isDisabled ? `cycleDownedStatus(${idx}, '${key}')` : ''}"
                                ${isDisabled ? 'disabled' : ''}
                                class="w-16 h-16 border-2 rounded flex items-center justify-center transition-all duration-300 relative ${
                                    status === 'success' ? 'bg-green-700/60 border-green-400 shadow-[0_0_15px_rgba(74,222,128,0.4)]' :
                                    status === 'fail' ? 'bg-[#800020]/70 border-red-500 shadow-[0_0_15px_rgba(128,0,32,0.5)]' :
                                    'bg-gray-800/20 border-white/20 hover:border-white/40'
                                }">
                                ${status === 'success' ? '<span class="material-symbols-outlined text-white text-4xl font-black drop-shadow-md">check</span>' : ''}
                                ${status === 'fail' ? '<span class="material-symbols-outlined text-white text-4xl font-black drop-shadow-md">close</span>' : ''}
                            </button>
                        </div>
                    `;
                }).join('')}
            </div>

            <div class="ml-6 flex items-center space-x-2 bg-black/40 p-1.5 rounded border border-white/10">
                <input
                    type="number"
                    placeholder="HP"
                    id="healthInput-${idx}"
                    class="w-12 bg-white/5 border border-white/10 rounded text-center text-sm outline-none text-white font-bold"
                />
                <button onclick="applyAction(${idx}, 'heal')" class="w-10 h-10 flex items-center justify-center rounded bg-green-700/40 hover:bg-green-600 text-white transition-all">
                    <span class="material-symbols-outlined text-base">healing</span>
                </button>
            </div>
        </div>
    `;
}

function renderNormalActions(p, idx, healthPercent, tempPercent) {
    return `
        <div class="flex flex-col items-stretch min-w-[280px]">
            <div class="flex justify-between items-end mb-1 gap-4">
                ${p.type !== 'enemy' ? `
                    <button
                        onclick="castMagic(${idx})"
                        class="group flex items-center space-x-2 bg-teal-900/40 hover:bg-teal-700/60 border border-teal-500/40 rounded px-4 py-2 transition-all active:scale-95">
                        <span class="material-symbols-outlined text-teal-300 text-base animate-pulse">auto_fix_high</span>
                        <span class="text-xs uppercase font-bold tracking-widest text-teal-200">Magie wirken</span>
                        <span class="bg-teal-500 text-black text-xs font-bold px-2 py-0.5 rounded-full">${p.currentPokus}</span>
                    </button>
                ` : '<div class="flex-grow"></div>'}

                <div class="flex items-baseline space-x-1">
                    <span class="text-2xl font-bold font-['MedievalSharp'] text-white">${p.currentHealth}</span>
                    ${p.tempHealth > 0 ? `<span class="text-sky-400 text-lg font-bold">+${p.tempHealth}</span>` : ''}
                    <span class="text-sm text-white/20 ml-1">/ ${p.maxHealth}</span>
                </div>
            </div>

            <div class="w-full h-3 bg-black/80 rounded-sm overflow-hidden relative border border-white/10 shadow-inner">
                <div class="absolute h-full bg-gradient-to-r from-[#800020] to-red-500 transition-all duration-700 shadow-[0_0_10px_rgba(128,0,32,0.4)]"
                     style="width: ${healthPercent}%"></div>
                <div class="absolute h-full bg-gradient-to-r from-sky-600 to-sky-400 transition-all duration-700 shadow-[0_0_12px_rgba(56,189,248,0.5)] border-l border-white/30"
                     style="width: ${tempPercent}%; left: ${healthPercent}%"></div>
                <div class="absolute inset-0 bg-gradient-to-b from-white/10 to-transparent pointer-events-none"></div>
            </div>
        </div>

        <div class="flex bg-black/60 rounded-lg border border-white/10 p-2 space-x-2">
            <input
                type="number"
                placeholder="0"
                id="healthInput-${idx}"
                class="w-12 bg-white/5 border border-white/10 rounded text-center text-sm outline-none text-white font-bold"
            />
            <button onclick="applyAction(${idx}, 'damage')" class="w-10 h-10 flex items-center justify-center rounded bg-[#800020]/40 hover:bg-[#800020] text-white transition-all shadow-lg">
                <span class="material-symbols-outlined text-base">skull</span>
            </button>
            <button onclick="applyAction(${idx}, 'heal')" class="w-10 h-10 flex items-center justify-center rounded bg-green-700/40 hover:bg-green-600 text-white transition-all shadow-lg">
                <span class="material-symbols-outlined text-base">healing</span>
            </button>
            <button onclick="applyAction(${idx}, 'temp')" class="w-10 h-10 flex items-center justify-center rounded bg-sky-700/40 hover:bg-sky-600 text-white transition-all shadow-lg" title="Schild (tempLP)">
                <span class="material-symbols-outlined text-base">shield_moon</span>
            </button>
            <button onclick="toggleConditionsPanel(${idx})" class="${battleState.expandedConditions[idx] ? 'bg-amber-500 text-black' : 'bg-white/10 text-white/40 hover:bg-white/20'} w-10 h-10 flex items-center justify-center rounded transition-all">
                <span class="material-symbols-outlined text-base">settings_accessibility</span>
            </button>
        </div>
    `;
}

function renderExpandedConditions(p, idx) {
    return `
        <div class="bg-black/60 border-t border-white/5 p-4 animate-in slide-in-from-top-2">
            <h4 class="text-xs uppercase tracking-widest text-amber-500 font-bold mb-3 flex items-center gap-1">
                <span class="material-symbols-outlined text-sm">psychology</span> Zustände verwalten
            </h4>
            <div class="grid grid-cols-3 md:grid-cols-6 gap-2">
                ${CONDITIONS.map(c => {
                    const isActive = p.activeConditions.includes(c);
                    const hasLevel = c === 'Vergiftet' || c === 'Brennend';
                    const cfg = CONDITION_CONFIG[c];

                    return `
                        <div class="flex flex-col gap-1">
                            <button
                                onclick="toggleCondition(${idx}, '${c}')"
                                class="text-xs py-2.5 rounded border uppercase font-bold tracking-tighter transition-all ${
                                    isActive ? `${cfg.color} ${cfg.text} ${cfg.border}` : 'bg-white/5 border-white/10 text-white/30 hover:border-white/30'
                                }">
                                ${c}
                            </button>
                            ${isActive && hasLevel ? `
                                <div class="flex items-center gap-1 bg-black/40 rounded px-1 py-0.5 border border-white/5">
                                    <span class="text-xs text-white/40 uppercase font-bold">Stufe</span>
                                    <select
                                        value="${p.conditionLevels[c] || 1}"
                                        onchange="setConditionLevel(${idx}, '${c}', this.value)"
                                        class="bg-transparent text-xs text-amber-500 font-bold outline-none">
                                        ${[1,2,3,4,5,6].map(lv => `<option value="${lv}">${lv}</option>`).join('')}
                                    </select>
                                </div>
                            ` : ''}
                        </div>
                    `;
                }).join('')}
            </div>
        </div>
    `;
}

// ===== HELPER FUNCTIONS =====
function getWoundsByHp(current, max) {
    const pct = (current / max) * 100;
    const wounds = [];
    if (current <= 0) return wounds;
    if (pct < 10) wounds.push('Tödlich');
    else if (pct < 25) wounds.push('Schwer');
    else if (pct < 50) wounds.push('Kompliziert');
    else if (pct < 75) wounds.push('Einfach');
    return wounds;
}

function calcConditionDamage(p, type, globalRound) {
    if (!p.activeConditions.includes(type)) return 0;
    const startRound = p.conditionStartRounds[type] || globalRound;
    const duration = globalRound - startRound + 1;
    const level = p.conditionLevels[type] || 1;
    const step = type === 'Vergiftet' ? 2 : 4;
    return (duration + level - 1) * step;
}

function applyAutoDamage(p, round) {
    let updated = { ...p };
    let totalDam = 0;

    if (updated.activeConditions.includes('Vergiftet')) {
        totalDam += calcConditionDamage(updated, 'Vergiftet', round);
    }
    if (updated.activeConditions.includes('Brennend')) {
        totalDam += calcConditionDamage(updated, 'Brennend', round);
    }

    if (totalDam > 0) {
        let rem = totalDam;
        if (updated.tempHealth > 0) {
            const consumed = Math.min(updated.tempHealth, rem);
            updated.tempHealth -= consumed;
            rem -= consumed;
        }
        updated.currentHealth = Math.max(0, updated.currentHealth - rem);
        if (updated.currentHealth === 0) {
            if (updated.type === 'enemy') updated.isDead = true;
            else updated.isFallen = true;
        }
    }
    return updated;
}

// ===== ACTIONS =====
function applyAction(index, mode) {
    if (battleState.isAnimating) return;

    const inputVal = parseInt(document.getElementById(`healthInput-${index}`).value) || 0;
    const p = battleState.participants[index];

    if (mode === 'damage') {
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

function castMagic(index) {
    const p = battleState.participants[index];
    if (p.currentPokus <= 0) {
        alert('Keine Pokuspunkte mehr!');
        return;
    }

    const cost = parseInt(prompt(`${p.name} wirkt Magie! Pokus-Kosten (verfügbar: ${p.currentPokus}):`, '1'));
    if (!cost || cost <= 0) return;

    if (cost > p.currentPokus) {
        alert('Nicht genug Pokuspunkte!');
        return;
    }

    p.currentPokus -= cost;
    renderBattleGrid();

    // SignalR: State synchronisieren
    syncBattleState();
}

function toggleConditionsPanel(index) {
    battleState.expandedConditions[index] = !battleState.expandedConditions[index];
    renderBattleGrid();
}

function toggleCondition(pIdx, condition) {
    const p = battleState.participants[pIdx];
    const hasCondition = p.activeConditions.includes(condition);
    let autoEndTurn = false;

    if (hasCondition) {
        p.activeConditions = p.activeConditions.filter(c => c !== condition);
        delete p.conditionCounters[condition];
        delete p.conditionLevels[condition];
        delete p.conditionStartRounds[condition];
    } else {
        p.activeConditions = [...p.activeConditions, condition];
        if (condition === 'Übergebend') p.conditionCounters[condition] = 3;
        if (condition === 'Vergiftet' || condition === 'Brennend') {
            p.conditionLevels[condition] = 1;
            p.conditionStartRounds[condition] = battleState.currentRound;
        }
        if (condition === 'Liegend' && pIdx === battleState.currentTurnIndex && !p.isFallen) autoEndTurn = true;
    }

    if (autoEndTurn) nextTurn();
    else {
        renderBattleGrid();
        // SignalR: State synchronisieren
        syncBattleState();
    }
}

function setConditionLevel(pIdx, condition, level) {
    battleState.participants[pIdx].conditionLevels[condition] = parseInt(level);
    renderBattleGrid();

    // SignalR: State synchronisieren
    syncBattleState();
}

function cycleDownedStatus(pIdx, key) {
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
        p.isFallen = false;
        p.currentHealth = 10;
        p.activeConditions = Array.from(new Set([...p.activeConditions, 'Liegend']));
        p.downedSaves = { Handeln: 'none', Wissen: 'none', Soziales: 'none' };
        if (pIdx === battleState.currentTurnIndex) mustEndTurn = true;
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

// ===== TURN MANAGEMENT =====
function nextTurn() {
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
    if (nextIdxInCurrentList === 0) tempRound++;

    const nextActorName = battleState.participants[nextIdxInCurrentList].name;

    // Sortieren (falls Unsichtbar geändert wurde)
    battleState.participants = sortParticipants(battleState.participants);
    let newTurnIdx = battleState.participants.findIndex(p => p.name === nextActorName);

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

function updateTurnIndicator() {
    document.getElementById('currentRound').textContent = battleState.currentRound;
    const current = battleState.participants[battleState.currentTurnIndex];
    document.getElementById('turnIndicator').textContent = `${current.name} ist am Zug`;
}

// ===== VICTORY/DEFEAT =====
function checkVictoryDefeat() {
    const aliveEnemies = battleState.participants.filter(p => p.type === 'enemy' && !p.isDead);
    const aliveAllies = battleState.participants.filter(p => p.type !== 'enemy' && !p.isDead);

    if (aliveEnemies.length === 0) {
        setTimeout(() => showResult('victory', '🏆 Sieg!', 'Alle Gegner wurden besiegt!'), 800);
    } else if (aliveAllies.length === 0) {
        setTimeout(() => showResult('defeat', '💀 Niederlage!', 'Die Gruppe wurde besiegt...'), 800);
    }
}

function showResult(type, title, message) {
    document.getElementById('battleGridScreen').classList.add('hidden');
    document.getElementById('resultScreen').classList.remove('hidden');

    document.getElementById('resultIcon').textContent = type === 'victory' ? '🏆' : '💀';
    document.getElementById('resultTitle').textContent = title;
    document.getElementById('resultMessage').textContent = message;

    // LocalStorage löschen
    localStorage.removeItem('battleSetup');

    // TODO: HP/Pokus Updates an Backend senden
}

async function endBattle() {
    if (!confirm('Gefecht wirklich beenden?')) return;

    // SignalR: Kampf beenden
    if (isConnected && signalRConnection && battleState.sessionId) {
        try {
            await signalRConnection.invoke("EndCombat", battleState.sessionId, "manual");
        } catch (error) {
            console.error('Fehler beim Beenden des Kampfes:', error);
        }
    }

    window.location.href = '/Spielmodus/Dashboard';
}
