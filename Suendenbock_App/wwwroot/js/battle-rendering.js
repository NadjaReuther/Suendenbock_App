// ===== BATTLE SYSTEM - RENDERING FUNCTIONS =====

import { battleState } from './battle-state.js';
import { CONDITION_CONFIG, CONDITIONS, SAVE_KEYS, IS_GOTT_ROLE } from './battle-config.js';
import { getWoundsByHp } from './battle-utils.js';

// Dependencies injected by battle-main.js via setRenderingDependencies()
let applyAction, castMagic, removeWound, resetInitiative, cycleDownedStatus, confirmRevive;
let openConditionsPanel;
let handleDragStart, handleDragOver, handleDragLeave, handleDrop, handleDragEnd;
let moveParticipantUp, moveParticipantDown;

export function setRenderingDependencies(deps) {
    applyAction = deps.applyAction;
    castMagic = deps.castMagic;
    removeWound = deps.removeWound;
    resetInitiative = deps.resetInitiative;
    cycleDownedStatus = deps.cycleDownedStatus;
    confirmRevive = deps.confirmRevive;
    openConditionsPanel = deps.openConditionsPanel;
    handleDragStart = deps.handleDragStart;
    handleDragOver = deps.handleDragOver;
    handleDragLeave = deps.handleDragLeave;
    handleDrop = deps.handleDrop;
    handleDragEnd = deps.handleDragEnd;
    moveParticipantUp = deps.moveParticipantUp;
    moveParticipantDown = deps.moveParticipantDown;
}

// ===== BATTLE GRID RENDERN =====
export function renderBattleGrid() {
    const container = document.getElementById('participantsContainer');
    if (!container) return;

    // Nur LEBENDE Teilnehmer anzeigen (tote Monster ausblenden)
    const aliveParticipants = battleState.participants
        .map((p, idx) => ({ participant: p, originalIndex: idx }))
        .filter(item => !item.participant.isDead);

    container.innerHTML = aliveParticipants.map(item => {
        return renderParticipantCard(item.participant, item.originalIndex);
    }).join('');

    // Gefallene-Liste rendern
    renderDefeatedList();
}

export function renderDefeatedList() {
    const container = document.getElementById('defeatedListContainer');
    const badge = document.getElementById('defeatedCountBadge');

    if (!container) return;

    // Sammle alle toten Gegner (Monster/Enemies)
    const defeatedEnemies = battleState.participants.filter(p => p.isDead && p.type === 'enemy');

    // Update Badge
    if (badge) {
        if (defeatedEnemies.length > 0) {
            badge.textContent = defeatedEnemies.length;
            badge.classList.remove('hidden');
        } else {
            badge.classList.add('hidden');
        }
    }

    if (defeatedEnemies.length === 0) {
        container.innerHTML = '<p class="text-white/40 text-sm italic">Noch keine Gegner gefallen</p>';
        return;
    }

    // Gruppiere nach baseName und speichere alle Namen (mit Nummern)
    const defeatedGroups = {};
    defeatedEnemies.forEach(p => {
        const baseName = p.baseName || p.name;
        if (!defeatedGroups[baseName]) {
            defeatedGroups[baseName] = [];
        }
        defeatedGroups[baseName].push(p.name);
    });

    // Erstelle HTML - zeige alle individuellen Monster mit Nummern
    const html = Object.entries(defeatedGroups).map(([baseName, names]) => {
        // Sortiere Namen natürlich (damit "Monster 1", "Monster 2", etc. richtig sortiert sind)
        names.sort((a, b) => {
            const numA = parseInt(a.match(/\d+$/)?.[0] || 0);
            const numB = parseInt(b.match(/\d+$/)?.[0] || 0);
            return numA - numB;
        });

        const namesList = names.join(', ');

        return `
            <div class="px-3 py-2 bg-red-900/20 border border-red-500/30 rounded">
                <div class="flex items-center gap-2 mb-1">
                    <span class="material-symbols-outlined text-red-400 text-lg">skull</span>
                    <span class="text-red-200 font-['MedievalSharp'] font-bold">${baseName}</span>
                    <span class="text-red-300 font-bold text-sm">×${names.length}</span>
                </div>
                <div class="text-red-300/70 text-xs ml-7">
                    ${namesList}
                </div>
            </div>
        `;
    }).join('');

    container.innerHTML = html;
}

export function renderParticipantCard(p, idx) {
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
                            <div class="flex flex-col gap-1">
                                <button
                                    onclick="moveParticipantUp(${idx})"
                                    class="cursor-pointer text-white/30 hover:text-amber-400 hover:bg-amber-500/10 transition-colors p-1 rounded disabled:opacity-20 disabled:cursor-not-allowed"
                                    title="Eine Position nach oben"
                                    ${idx === 0 ? 'disabled' : ''}>
                                    <span class="material-symbols-outlined text-lg">keyboard_arrow_up</span>
                                </button>
                                <div class="cursor-move text-white/20 hover:text-white/60 transition-colors" title="Ziehen um Reihenfolge zu ändern">
                                    <span class="material-symbols-outlined text-xl">drag_indicator</span>
                                </div>
                                <button
                                    onclick="moveParticipantDown(${idx})"
                                    class="cursor-pointer text-white/30 hover:text-amber-400 hover:bg-amber-500/10 transition-colors p-1 rounded disabled:opacity-20 disabled:cursor-not-allowed"
                                    title="Eine Position nach unten"
                                    ${idx === battleState.participants.length - 1 ? 'disabled' : ''}>
                                    <span class="material-symbols-outlined text-lg">keyboard_arrow_down</span>
                                </button>
                            </div>
                        ` : ''}
                    </div>
                    <div>
                        <p class="${p.type === 'enemy' ? 'text-red-400' : 'text-white'} font-['MedievalSharp'] text-2xl">${p.name}</p>
                        <div class="flex flex-wrap gap-1 mt-1">
                            ${!p.isDead && p.wounds && p.wounds.length > 0 ? p.wounds.map((w, wIdx) => `
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
                                    <span onclick="openConditionsPanel()" class="text-[10px] ${cfg.color} ${cfg.text} border ${cfg.border} px-2 py-1 rounded uppercase font-bold tracking-tighter flex items-center gap-1 cursor-pointer hover:opacity-80 transition-opacity">
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

            <!-- Expanded Conditions (nur für Gott) -->
            ${isExpanded && !isDowned && !p.isDead && IS_GOTT_ROLE ? renderExpandedConditions(p, idx) : ''}
        </div>
    `;
}

export function renderDownedSaves(p, idx) {
    // Nur Charaktere (type='player') haben downedSaves - Monster sterben direkt
    if (p.type !== 'player' || !p.downedSaves) {
        return ''; // Monster/Enemies haben keine Todesrettungswürfe
    }

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

export function renderNormalActions(p, idx, healthPercent, tempPercent) {
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
                <button onclick="toggleConditionsSelection(${idx})" class="${battleState.expandedConditions[idx] ? 'bg-amber-500 text-black' : 'bg-white/10 text-white/40 hover:bg-white/20'} w-10 h-10 flex items-center justify-center rounded transition-all">
                    <span class="material-symbols-outlined text-base">settings_accessibility</span>
                </button>
            </div>
        ` : ''}
    `;
}

export function renderExpandedConditions(p, idx) {
    // Import the toggleCondition and setConditionLevel functions
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

export function updateTurnIndicator() {
    document.getElementById('currentRound').textContent = battleState.currentRound;
    const current = battleState.participants[battleState.currentTurnIndex];
    document.getElementById('turnIndicator').textContent = `${current.name} ist am Zug`;
}

export function toggleConditionsSelection(index) {
    battleState.expandedConditions[index] = !battleState.expandedConditions[index];
    renderBattleGrid();
}
