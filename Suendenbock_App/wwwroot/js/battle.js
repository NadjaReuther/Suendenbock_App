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
    sessionId: null,
    activeFieldEffects: [] // Array of { feldEffektId, name, colorCode, strength }
};

// ===== ROLE-BASED ACCESS CONTROL =====
// IS_GOTT wird von Battle.cshtml gesetzt
// Falls nicht gesetzt (alte Implementierung), default auf true für Kompatibilität
const IS_GOTT_ROLE = typeof IS_GOTT !== 'undefined' ? IS_GOTT : true;

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
            // Nur updaten, wenn es nicht von diesem Client kommt
            if (data.sessionId === battleState.sessionId) {
                const newSetup = JSON.parse(data.battleStateJson);

                // State aktualisieren, aber bestehende Participants-Struktur beibehalten
                if (newSetup.participants) {
                    battleState.participants = newSetup.participants;
                }
                if (newSetup.activeFieldEffects) {
                    battleState.activeFieldEffects = newSetup.activeFieldEffects;
                }
                battleState.currentRound = data.currentRound;
                battleState.currentTurnIndex = data.currentTurnIndex;

                // UI neu rendern
                renderBattleGrid();
                updateTurnIndicator();
                renderFieldEffects();
            }
        });

        // Event: Kampf beendet
        signalRConnection.on("CombatEnded", async (data) => {
            await showResultScreen(data.result);
        });

        // Event: Quick Action empfangen
        signalRConnection.on("ReceiveAction", (data) => {
            // Optional: Hier könntest du Animationen triggern
        });

        // Verbindung starten
        await signalRConnection.start();
        isConnected = true;

        // Combat Session beitreten
        await signalRConnection.invoke("JoinCombat", sessionId);

    } catch (error) {
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
            expandedConditions: battleState.expandedConditions,
            activeFieldEffects: battleState.activeFieldEffects
        });

        await signalRConnection.invoke(
            "UpdateBattleState",
            battleState.sessionId,
            battleStateJson,
            battleState.currentRound,
            battleState.currentTurnIndex
        );
    } catch (error) {
        // Fehler stillschweigend behandeln
    }
}

async function initBattle() {
    // WICHTIG: SessionId aus URL-Query lesen (Spieler werden mit ?sessionId=123 weitergeleitet)
    const urlParams = new URLSearchParams(window.location.search);
    const sessionIdFromUrl = urlParams.get('sessionId');

    if (sessionIdFromUrl) {
        // Spieler kommt via Broadcast-Redirect → SessionId aus URL verwenden
        battleState.sessionId = parseInt(sessionIdFromUrl);

        // Battle State von Server laden
        await loadBattleStateFromServer(battleState.sessionId);
    } else {
        // Gott hat Kampf direkt gestartet → Daten aus localStorage
        const setup = JSON.parse(localStorage.getItem('battleSetup') || '{}');

        if (!setup.monsters || !setup.characterIds || !setup.initiatives) {
            await Swal.fire({
                icon: 'error',
                title: 'Keine Battle-Daten',
                text: 'Bitte starte den Kampf über Combat Setup.',
                confirmButtonColor: '#d97706'
            });
            window.location.href = '/Spielmodus/CombatSetup';
            return;
        }

        // SessionId speichern (falls vorhanden)
        if (setup.sessionId) {
            battleState.sessionId = setup.sessionId;
        }

        // Participants erstellen aus localStorage
        buildParticipants(setup);

        // Battle Grid rendern
        renderBattleGrid();

        // Erste Runde starten
        updateTurnIndicator();
    }

    // SignalR verbinden (falls sessionId vorhanden)
    if (battleState.sessionId) {
        await setupSignalR(battleState.sessionId);
    }

    // Feldeffekte rendern
    renderFieldEffects();
}

// ===== BATTLE STATE VOM SERVER LADEN =====
async function loadBattleStateFromServer(sessionId) {
    try {
        // Combat Session vom Server abrufen
        const response = await fetch(`/api/battle/session/${sessionId}`);

        if (!response.ok) {
            throw new Error('Combat Session nicht gefunden');
        }

        const session = await response.json();

        // Battle State aus JSON parsen
        const savedState = JSON.parse(session.battleStateJson);

        // Wenn Participants noch nicht existieren (Setup-Daten), müssen wir sie erstellen
        if (!savedState.participants || savedState.participants.length === 0) {
            buildParticipants(savedState);
        } else {
            // Participants existieren bereits (wurde vom Gott schon erstellt)
            battleState.participants = savedState.participants;
        }

        battleState.currentRound = session.currentRound;
        battleState.currentTurnIndex = session.currentTurnIndex;

        if (savedState.expandedConditions) {
            battleState.expandedConditions = savedState.expandedConditions;
        }

        if (savedState.activeFieldEffects) {
            battleState.activeFieldEffects = savedState.activeFieldEffects;
        }

        // UI rendern
        renderBattleGrid();
        updateTurnIndicator();
        renderFieldEffects();

    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler beim Laden',
            text: 'Der Kampf konnte nicht geladen werden. Bitte versuche es erneut.',
            confirmButtonColor: '#d97706'
        });
        window.location.href = '/Spielmodus/Dashboard';
    }
}

// ===== PARTICIPANTS ERSTELLEN =====
function buildParticipants(setup) {
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
    setup.monsters.forEach((monsterSetup, index) => {
        const monster = BATTLE_MODEL_DATA.monsters.find(m => m.id === monsterSetup.monsterId);
        if (!monster) return;

        const initKey = `monster-${index}`;
        const initiative = setup.initiatives[initKey] || 50;

        participants.push({
            id: `monster-${index}`,
            name: monster.name,
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

    // Extra-Teilnehmer hinzufügen (nur Name + HP, keine DB-Referenz, kein Pokus)
    if (setup.extraParticipants) {
        setup.extraParticipants.forEach((extra, index) => {
            const initKey = `extra-${index}`;
            const initiative = setup.initiatives[initKey] || 50;

            participants.push({
                id: `extra-${index}`,
                name: extra.name,
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
             data-participant-index="${idx}"
             draggable="${!p.isDead && IS_GOTT_ROLE}"
             ondragstart="${IS_GOTT_ROLE ? `handleDragStart(event, ${idx})` : ''}"
             ondragover="${IS_GOTT_ROLE ? `handleDragOver(event)` : ''}"
             ondragleave="${IS_GOTT_ROLE ? `handleDragLeave(event)` : ''}"
             ondrop="${IS_GOTT_ROLE ? `handleDrop(event, ${idx})` : ''}"
             ondragend="${IS_GOTT_ROLE ? `handleDragEnd(event)` : ''}">
            <!-- Main Info Row -->
            <div class="p-4 flex items-center justify-between gap-4 flex-wrap md:flex-nowrap min-h-[120px]">
                <!-- Initiative & Name -->
                <div class="flex items-center space-x-4">
                    <div class="flex items-center gap-2">
                        <div class="flex flex-col items-center gap-1">
                            <div class="${isActive ? (isInvisible ? 'bg-slate-300 text-black animate-pulse' : 'bg-amber-500 text-black animate-pulse') : 'bg-white/10 text-white/40'} w-12 h-12 rounded-full flex items-center justify-center font-bold text-base">
                                ${p.initiative}
                            </div>
                            ${p.initiative !== p.originalInitiative && IS_GOTT_ROLE ? `
                                <button
                                    onclick="resetInitiative(${idx})"
                                    class="bg-amber-500/20 hover:bg-amber-500/40 text-amber-300 text-[10px] px-2 py-0.5 rounded border border-amber-500/40 transition-all"
                                    title="Auf ${p.originalInitiative} zurücksetzen">
                                    ↻ ${p.originalInitiative}
                                </button>
                            ` : ''}
                        </div>
                        ${!p.isDead && IS_GOTT_ROLE ? `
                            <div class="cursor-move text-white/20 hover:text-white/60 transition-colors" title="Ziehen um Reihenfolge zu ändern">
                                <span class="material-symbols-outlined text-2xl">drag_indicator</span>
                            </div>
                        ` : ''}
                    </div>
                    <div>
                        <p class="${p.type === 'enemy' ? 'text-red-400' : 'text-white'} font-['MedievalSharp'] text-2xl">${p.name}</p>
                        <div class="flex flex-wrap gap-1 mt-1">
                            ${!p.isDead ? p.wounds.map((w, wIdx) => `
                                <span class="text-[10px] bg-red-600/60 text-white border border-red-400 px-2 py-1 rounded uppercase font-bold tracking-tighter flex items-center space-x-1 shadow-[0_0_8px_rgba(220,38,38,0.5)] ${IS_GOTT_ROLE ? 'group' : ''}">
                                    <span class="material-symbols-outlined text-[12px]">water_drop</span>
                                    <span>${w}</span>
                                    ${IS_GOTT_ROLE ? `<button onclick="removeWound(${idx}, ${wIdx})" class="ml-1 opacity-0 group-hover:opacity-100 transition-opacity text-white hover:text-red-200">×</button>` : ''}
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
                <span class="text-xs uppercase text-white/30 tracking-widest">${IS_GOTT_ROLE ? 'Reihenfolge einhalten' : 'Nur-Lese-Modus'}</span>
            </div>

            <div class="flex gap-4">
                ${SAVE_KEYS.map((key, kIdx) => {
                    const status = p.downedSaves[key];
                    const prevStatus = kIdx > 0 ? p.downedSaves[SAVE_KEYS[kIdx - 1]] : 'success';
                    const isDisabled = prevStatus === 'none' || !IS_GOTT_ROLE;

                    return `
                        <div class="flex flex-col items-center space-y-1 transition-opacity ${isDisabled ? 'opacity-20 grayscale' : 'opacity-100'}">
                            <span class="text-xs uppercase tracking-tighter text-white/40 font-bold">${key}</span>
                            ${IS_GOTT_ROLE ? `
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
                            ` : `
                                <div class="w-16 h-16 border-2 rounded flex items-center justify-center ${
                                    status === 'success' ? 'bg-green-700/60 border-green-400 shadow-[0_0_15px_rgba(74,222,128,0.4)]' :
                                    status === 'fail' ? 'bg-[#800020]/70 border-red-500 shadow-[0_0_15px_rgba(128,0,32,0.5)]' :
                                    'bg-gray-800/20 border-white/20'
                                }">
                                    ${status === 'success' ? '<span class="material-symbols-outlined text-white text-4xl font-black drop-shadow-md">check</span>' : ''}
                                    ${status === 'fail' ? '<span class="material-symbols-outlined text-white text-4xl font-black drop-shadow-md">close</span>' : ''}
                                </div>
                            `}
                        </div>
                    `;
                }).join('')}
            </div>

            ${IS_GOTT_ROLE ? (p.readyToRevive ? `
                <div class="ml-6 flex flex-col items-center space-y-2 bg-green-900/40 p-3 rounded border border-green-500/60 animate-pulse">
                    <span class="text-xs uppercase text-green-200 font-bold tracking-widest">Rettung möglich!</span>
                    <button onclick="confirmRevive(${idx})" class="px-6 py-3 bg-green-600 hover:bg-green-500 text-white font-bold rounded transition-all shadow-lg flex items-center space-x-2">
                        <span class="material-symbols-outlined">emergency</span>
                        <span>Charakter retten</span>
                    </button>
                </div>
            ` : `
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
            `) : (p.readyToRevive ? `
                <div class="ml-6 flex flex-col items-center space-y-2 bg-green-900/40 p-3 rounded border border-green-500/60">
                    <span class="text-xs uppercase text-green-200 font-bold tracking-widest">Rettung möglich!</span>
                    <div class="px-6 py-3 bg-green-600/60 text-white font-bold rounded flex items-center space-x-2 cursor-not-allowed">
                        <span class="material-symbols-outlined">emergency</span>
                        <span>Warte auf Gott</span>
                    </div>
                </div>
            ` : '')}
        </div>
    `;
}

function renderNormalActions(p, idx, healthPercent, tempPercent) {
    return `
        <div class="flex flex-col items-stretch min-w-[280px]">
            <div class="flex justify-between items-end mb-1 gap-4">
                ${p.type === 'player' && IS_GOTT_ROLE ? `
                    <button
                        onclick="castMagic(${idx})"
                        class="group flex items-center space-x-2 bg-teal-900/40 hover:bg-teal-700/60 border border-teal-500/40 rounded px-4 py-2 transition-all active:scale-95">
                        <span class="material-symbols-outlined text-teal-300 text-base animate-pulse">auto_fix_high</span>
                        <span class="text-xs uppercase font-bold tracking-widest text-teal-200">Zauber wirken</span>
                        <span class="bg-teal-500 text-black text-xs font-bold px-2 py-0.5 rounded-full">${p.currentPokus}</span>
                    </button>
                ` : (p.type === 'player' && !IS_GOTT_ROLE ? `
                    <div class="flex items-center space-x-2 bg-teal-900/20 border border-teal-500/20 rounded px-4 py-2">
                        <span class="material-symbols-outlined text-teal-300/40 text-base">auto_fix_high</span>
                        <span class="text-xs uppercase font-bold tracking-widest text-teal-200/40">Zauber</span>
                        <span class="bg-teal-500/40 text-white text-xs font-bold px-2 py-0.5 rounded-full">${p.currentPokus}</span>
                    </div>
                ` : '<div class="flex-grow"></div>')}

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

        ${IS_GOTT_ROLE ? `
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
        ` : ''}
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
function getWoundTierByPercent(pct) {
    if (pct < 10) return 4; // Tödlich
    if (pct < 25) return 3; // Schwer
    if (pct < 50) return 2; // Kompliziert
    if (pct < 75) return 1; // Einfach
    return 0; // Keine Wunde
}

function getWoundNameByTier(tier) {
    const names = ['Keine', 'Einfach', 'Kompliziert', 'Schwer', 'Tödlich'];
    return names[tier] || 'Keine';
}

function getWoundsByHp(current, max) {
    // Diese Funktion wird jetzt nur noch für die Anzeige verwendet
    // Tatsächliche Wunden werden im wounds Array getrackt
    return [];
}

function calcConditionDamage(p, type, globalRound) {
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

// ===== ACTIONS =====
function applyAction(index, mode) {
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

function removeWound(pIdx, woundIdx) {
    if (!IS_GOTT_ROLE) {
        return;
    }
    const p = battleState.participants[pIdx];
    p.wounds.splice(woundIdx, 1);
    renderBattleGrid();

    // SignalR: State synchronisieren
    syncBattleState();
}

async function castMagic(index) {
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

function toggleConditionsPanel(index) {
    battleState.expandedConditions[index] = !battleState.expandedConditions[index];
    renderBattleGrid();
}

function toggleCondition(pIdx, condition) {
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

function setConditionLevel(pIdx, condition, level) {
    if (!IS_GOTT_ROLE) {
        return;
    }
    battleState.participants[pIdx].conditionLevels[condition] = parseInt(level);
    renderBattleGrid();

    // SignalR: State synchronisieren
    syncBattleState();
}

async function resetInitiative(pIdx) {
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

// ===== DRAG & DROP FOR INITIATIVE =====
let draggedIndex = null;

function handleDragStart(event, index) {
    if (!IS_GOTT_ROLE) return;
    draggedIndex = index;
    event.currentTarget.style.opacity = '0.5';
    event.dataTransfer.effectAllowed = 'move';
}

function handleDragOver(event) {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';

    // Visual Feedback: Border highlighten
    const dropZone = event.currentTarget;
    dropZone.style.borderColor = 'rgb(245, 158, 11)';
    dropZone.style.borderWidth = '3px';

    return false;
}

function handleDragLeave(event) {
    // Border zurücksetzen
    const dropZone = event.currentTarget;
    dropZone.style.borderColor = '';
    dropZone.style.borderWidth = '';
}

function handleDrop(event, dropIndex) {
    if (!IS_GOTT_ROLE) return;
    event.stopPropagation();
    event.preventDefault();

    // Border zurücksetzen
    event.currentTarget.style.borderColor = '';
    event.currentTarget.style.borderWidth = '';

    if (draggedIndex === null || draggedIndex === dropIndex) return;

    // Teilnehmer verschieben
    const draggedParticipant = battleState.participants[draggedIndex];
    const targetParticipant = battleState.participants[dropIndex];

    // Initiative des gezogenen Teilnehmers anpassen
    // Wenn nach unten gezogen (höhere Position = höhere Initiative), zwischen target und nächstem setzen
    if (draggedIndex < dropIndex) {
        // Nach unten gezogen
        const nextParticipant = battleState.participants[dropIndex + 1];
        if (nextParticipant) {
            // Zwischen dropIndex und dropIndex+1
            draggedParticipant.initiative = Math.floor((targetParticipant.initiative + nextParticipant.initiative) / 2);
        } else {
            // Ans Ende
            draggedParticipant.initiative = targetParticipant.initiative + 1;
        }
    } else {
        // Nach oben gezogen
        const prevParticipant = battleState.participants[dropIndex - 1];
        if (prevParticipant) {
            // Zwischen dropIndex-1 und dropIndex
            draggedParticipant.initiative = Math.floor((prevParticipant.initiative + targetParticipant.initiative) / 2);
        } else {
            // An den Anfang
            draggedParticipant.initiative = targetParticipant.initiative - 1;
        }
    }

    renderBattleGrid();
    syncBattleState();
}

function handleDragEnd(event) {
    event.currentTarget.style.opacity = '1';
    draggedIndex = null;
}

function cycleDownedStatus(pIdx, key) {
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

function confirmRevive(pIdx) {
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
function nextTurn() {
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

    // Beim Rundenwechsel: Alle Initiativen zurücksetzen (außer Unsichtbar)
    if (isNewRound) {
        battleState.participants.forEach(p => {
            if (!p.activeConditions.includes('Unsichtbar')) {
                p.initiative = p.originalInitiative;
            }
        });
    }

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
async function checkVictoryDefeat() {
    const aliveEnemies = battleState.participants.filter(p => p.type === 'enemy' && !p.isDead);
    const aliveAllies = battleState.participants.filter(p => p.type !== 'enemy' && !p.isDead);

    if (aliveEnemies.length === 0) {
        // Sieg!
        if (IS_GOTT_ROLE && isConnected && signalRConnection && battleState.sessionId) {
            try {
                await signalRConnection.invoke("EndCombat", battleState.sessionId, "victory");
            } catch (error) {
                // Fehler stillschweigend behandeln
            }
        }
        setTimeout(async () => await showResultScreen('victory'), 800);
    } else if (aliveAllies.length === 0) {
        // Niederlage!
        if (IS_GOTT_ROLE && isConnected && signalRConnection && battleState.sessionId) {
            try {
                await signalRConnection.invoke("EndCombat", battleState.sessionId, "defeat");
            } catch (error) {
                // Fehler stillschweigend behandeln
            }
        }
        setTimeout(async () => await showResultScreen('defeat'), 800);
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

async function showResultScreen(result) {
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

// ===== FIELD EFFECTS SYSTEM =====

// Global state for modal
let selectedStrength = null;

// Render active field effects as gradient banner
function renderFieldEffects() {
    const banner = document.getElementById('fieldEffectsBanner');
    const container = document.getElementById('activeFieldEffectsContainer');

    if (!banner || !container) return;

    // Für Gott: Banner IMMER anzeigen (auch ohne aktive Effekte, damit der Button sichtbar ist)
    // Für Spieler: Nur anzeigen wenn Effekte aktiv sind
    if (battleState.activeFieldEffects.length === 0) {
        if (!IS_GOTT_ROLE) {
            banner.classList.add('hidden');
            return;
        }
        // Gott: Banner anzeigen, aber mit neutralem Hintergrund
        banner.classList.remove('hidden');
        banner.style = 'background: linear-gradient(135deg, #374151 0%, #1f2937 100%)';
        container.innerHTML = '<p class="text-gray-400 italic text-sm">Keine Feldeffekte aktiv</p>';
        return;
    }

    banner.classList.remove('hidden');

    // Create gradient from all active field effect colors
    const colors = battleState.activeFieldEffects.map(fe => fe.colorCode);
    let gradientStyle = '';

    if (colors.length === 1) {
        gradientStyle = `background: ${colors[0]}`;
    } else {
        // Create linear gradient for multiple colors
        const stops = colors.map((color, index) => {
            const percent = (index / (colors.length - 1)) * 100;
            return `${color} ${percent}%`;
        }).join(', ');
        gradientStyle = `background: linear-gradient(to right, ${stops})`;
    }

    banner.style = gradientStyle;

    // Render individual effect badges
    container.innerHTML = battleState.activeFieldEffects.map(fe => {
        const strengthColors = {
            'einfach': 'bg-green-900/60 border-green-500 text-green-200',
            'mittel': 'bg-yellow-900/60 border-yellow-500 text-yellow-200',
            'schwer': 'bg-red-900/60 border-red-500 text-red-200'
        };

        const colorClass = strengthColors[fe.strength] || 'bg-gray-700 border-gray-500 text-gray-200';

        // Beschreibung für onclick vorbereiten (escapen)
        const escapedName = (fe.name || '').replace(/'/g, "\\'").replace(/"/g, '&quot;');
        const escapedDescription = (fe.beschreibung || 'Keine Beschreibung verfügbar').replace(/'/g, "\\'").replace(/"/g, '&quot;');
        const escapedStrength = (fe.strength || '').replace(/'/g, "\\'");
        const escapedColorCode = (fe.colorCode || '').replace(/'/g, "\\'");

        return `
            <div class="flex items-center gap-2 ${colorClass} border-2 rounded-lg px-3 py-2 relative group">
                <button onclick="showFieldEffectDescription('${escapedName}', '${escapedDescription}', '${escapedStrength}', '${escapedColorCode}')"
                        class="flex items-center gap-2 hover:opacity-80 transition-opacity cursor-pointer"
                        title="Klicken für Details">
                    <span class="font-bold">${fe.name}</span>
                    <span class="text-sm opacity-80">(${fe.strength})</span>
                    <span class="text-xs opacity-60">ℹ️</span>
                </button>
                ${IS_GOTT_ROLE ? `
                    <button onclick="removeFieldEffect(${fe.feldEffektId})"
                            class="ml-2 hover:text-white transition-colors text-lg leading-none"
                            title="Entfernen">
                        ×
                    </button>
                ` : ''}
            </div>
        `;
    }).join('');
}

// Open field effect modal
function openFieldEffectModal() {
    if (!IS_GOTT_ROLE) return;

    const modal = document.getElementById('fieldEffectModal');
    if (!modal) return;

    // Reset modal state
    document.getElementById('fieldEffectSelect').value = '';
    document.getElementById('fieldEffectDescription').classList.add('hidden');
    selectedStrength = null;
    document.querySelectorAll('.strength-btn').forEach(btn => {
        btn.classList.remove('ring-2', 'ring-white');
    });

    modal.classList.remove('hidden');
}

// Close field effect modal
function closeFieldEffectModal() {
    const modal = document.getElementById('fieldEffectModal');
    if (modal) {
        modal.classList.add('hidden');
    }
}

// Handle field effect selection change
document.addEventListener('DOMContentLoaded', () => {
    const selectEl = document.getElementById('fieldEffectSelect');
    if (selectEl) {
        selectEl.addEventListener('change', function() {
            const selectedOption = this.options[this.selectedIndex];
            const description = selectedOption.getAttribute('data-description');

            const descDiv = document.getElementById('fieldEffectDescription');
            const descText = document.getElementById('fieldEffectDescriptionText');

            if (description && description !== 'null' && description !== '') {
                descText.textContent = description;
                descDiv.classList.remove('hidden');
            } else {
                descDiv.classList.add('hidden');
            }
        });
    }
});

// Select strength
function selectStrength(strength) {
    selectedStrength = strength;

    // Update button styling
    document.querySelectorAll('.strength-btn').forEach(btn => {
        if (btn.getAttribute('data-strength') === strength) {
            btn.classList.add('ring-2', 'ring-white');
        } else {
            btn.classList.remove('ring-2', 'ring-white');
        }
    });
}

// Add field effect
async function addFieldEffect() {
    if (!IS_GOTT_ROLE) return;

    const selectEl = document.getElementById('fieldEffectSelect');
    const selectedOption = selectEl.options[selectEl.selectedIndex];

    if (!selectedOption.value) {
        await Swal.fire({
            icon: 'warning',
            title: 'Keine Auswahl',
            text: 'Bitte wähle einen Feldeffekt aus.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    if (!selectedStrength) {
        await Swal.fire({
            icon: 'warning',
            title: 'Keine Stärke gewählt',
            text: 'Bitte wähle eine Stärke für den Feldeffekt.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    const feldEffektId = parseInt(selectedOption.value);
    const name = selectedOption.getAttribute('data-name');
    const colorCode = selectedOption.getAttribute('data-color');
    const beschreibung = selectedOption.getAttribute('data-description') || '';

    // Check if already active
    if (battleState.activeFieldEffects.some(fe => fe.feldEffektId === feldEffektId)) {
        await Swal.fire({
            icon: 'info',
            title: 'Bereits aktiv',
            text: 'Dieser Feldeffekt ist bereits aktiv.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    // Add to state
    battleState.activeFieldEffects.push({
        feldEffektId,
        name,
        colorCode,
        strength: selectedStrength,
        beschreibung: beschreibung
    });

    // Re-render
    renderFieldEffects();

    // Sync via SignalR
    await syncBattleState();

    // Close modal
    closeFieldEffectModal();
}

// Remove field effect
async function removeFieldEffect(feldEffektId) {
    if (!IS_GOTT_ROLE) return;

    battleState.activeFieldEffects = battleState.activeFieldEffects.filter(
        fe => fe.feldEffektId !== feldEffektId
    );

    // Re-render
    renderFieldEffects();

    // Sync via SignalR
    await syncBattleState();
}

// Show field effect description in slide-in panel (für alle sichtbar)
function showFieldEffectDescription(name, beschreibung, strength, colorCode) {
    const panel = document.getElementById('fieldEffectPanel');
    const overlay = document.getElementById('fieldEffectPanelOverlay');
    const panelTitle = document.getElementById('panelTitle');
    const panelDescription = document.getElementById('panelDescription');
    const panelStrength = document.getElementById('panelStrength');
    const panelHeader = document.getElementById('panelHeader');

    if (!panel || !overlay) return;

    // Setze Inhalt
    panelTitle.textContent = name;
    // HTML-Tags in Beschreibung rendern (innerHTML statt textContent)
    panelDescription.innerHTML = beschreibung || '<em class="text-gray-400">Keine Beschreibung verfügbar</em>';

    // Stärke Badge
    const strengthConfig = {
        'einfach': { text: 'Einfach', classes: 'bg-green-900/60 border border-green-500 text-green-200' },
        'mittel': { text: 'Mittel', classes: 'bg-yellow-900/60 border border-yellow-500 text-yellow-200' },
        'schwer': { text: 'Schwer', classes: 'bg-red-900/60 border border-red-500 text-red-200' }
    };

    const strengthInfo = strengthConfig[strength] || { text: strength, classes: 'bg-gray-700 border border-gray-500 text-gray-200' };
    panelStrength.textContent = strengthInfo.text;
    panelStrength.className = `inline-block px-3 py-1 rounded-full text-sm font-semibold ${strengthInfo.classes}`;

    // Header Border mit Feldeffekt-Farbe
    if (colorCode) {
        panelHeader.style.borderBottomColor = colorCode;
        panelHeader.style.borderBottomWidth = '3px';
        panel.style.borderLeftColor = colorCode;
    }

    // Overlay anzeigen
    overlay.classList.remove('hidden');
    setTimeout(() => {
        overlay.classList.add('overlay-active');
    }, 10);

    // Panel einsliden
    panel.classList.add('panel-open');
}

// Close field effect panel
function closeFieldEffectPanel() {
    const panel = document.getElementById('fieldEffectPanel');
    const overlay = document.getElementById('fieldEffectPanelOverlay');

    if (!panel || !overlay) return;

    // Panel aussliden
    panel.classList.remove('panel-open');

    // Overlay ausblenden
    overlay.classList.remove('overlay-active');
    setTimeout(() => {
        overlay.classList.add('hidden');
    }, 300);
}

async function endBattle() {
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
    if (isConnected && signalRConnection && battleState.sessionId) {
        try {
            await signalRConnection.invoke("EndCombat", battleState.sessionId, "aborted");
        } catch (error) {
            // Fehler stillschweigend behandeln
        }
    }

    // Gott sieht auch den Result Screen
    await showResultScreen("aborted");
}
