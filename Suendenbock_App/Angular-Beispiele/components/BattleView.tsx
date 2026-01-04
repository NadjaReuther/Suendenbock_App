
import React, { useState, useEffect } from 'react';

type SaveStatus = 'none' | 'success' | 'fail';
type WoundLevel = 'Einfach' | 'Kompliziert' | 'Schwer' | 'Tödlich';
type BattlePhase = 'initiative' | 'grid';

const CONDITION_CONFIG: Record<string, { color: string, border: string, text: string }> = {
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
const SAVE_KEYS: Array<keyof DownedState> = ['Handeln', 'Wissen', 'Soziales'];

interface DownedState {
  Handeln: SaveStatus;
  Wissen: SaveStatus;
  Soziales: SaveStatus;
}

interface Participant {
  name: string;
  initiative: number;
  type: 'player' | 'companion' | 'enemy';
  currentHealth: number;
  maxHealth: number;
  tempHealth: number;
  isDead: boolean;
  isFallen: boolean; 
  activeConditions: string[];
  conditionCounters: Record<string, number>;
  conditionLevels: Record<string, number>;
  conditionStartRounds: Record<string, number>;
  downedSaves: DownedState;
}

interface BattleViewProps {
  onBack: () => void;
  onVictory: () => void;
  characterPokus: Record<string, number>;
  onCastMagic: (name: string) => void;
}

const PLAYER_NAMES = ['Jewa', 'Jeremias', 'Hironimus', 'Salome', 'Jonata', 'Gabriel'];
const COMPANION_NAMES = ['Emma', 'Okko'];

const BattleView: React.FC<BattleViewProps> = ({ onBack, onVictory, characterPokus, onCastMagic }) => {
  const [phase, setPhase] = useState<BattlePhase>('initiative');
  const [initiatives, setInitiatives] = useState<Record<string, string>>(
    [...PLAYER_NAMES, ...COMPANION_NAMES].reduce((acc, name) => ({ ...acc, [name]: '' }), {})
  );
  const [turnOrder, setTurnOrder] = useState<Participant[] | null>(null);
  const [currentTurnIndex, setCurrentTurnIndex] = useState<number>(0);
  const [currentRound, setCurrentRound] = useState<number>(1);
  const [expandedConditions, setExpandedConditions] = useState<Record<number, boolean>>({});
  const [healthInputs, setHealthInputs] = useState<Record<number, string>>({});

  const getWoundsByHp = (current: number, max: number): WoundLevel[] => {
    const pct = (current / max) * 100;
    const wounds: WoundLevel[] = [];
    if (current <= 0) return wounds;
    if (pct < 10) wounds.push('Tödlich');
    else if (pct < 25) wounds.push('Schwer');
    else if (pct < 50) wounds.push('Kompliziert');
    else if (pct < 75) wounds.push('Einfach');
    return wounds;
  };

  const calcConditionDamage = (p: Participant, type: 'Vergiftet' | 'Brennend', globalRound: number) => {
    if (!p.activeConditions.includes(type)) return 0;
    const startRound = p.conditionStartRounds[type] || globalRound;
    const duration = globalRound - startRound + 1;
    const level = p.conditionLevels[type] || 1;
    const step = type === 'Vergiftet' ? 2 : 4;
    return (duration + level - 1) * step;
  };

  const sortParticipants = (participants: Participant[]) => {
    return [...participants].sort((a, b) => {
      const aInv = a.activeConditions.includes('Unsichtbar');
      const bInv = b.activeConditions.includes('Unsichtbar');
      if (aInv && !bInv) return -1;
      if (!aInv && bInv) return 1;
      return a.initiative - b.initiative;
    });
  };

  const startSkirmish = () => {
    let list: Participant[] = [];
    const DEFAULT_HP = 150;
    [...PLAYER_NAMES, ...COMPANION_NAMES].forEach(name => {
      const initValue = initiatives[name];
      if (initValue !== '') {
        list.push({
          name,
          initiative: parseInt(initValue),
          type: COMPANION_NAMES.includes(name) ? 'companion' : 'player',
          currentHealth: DEFAULT_HP,
          maxHealth: DEFAULT_HP,
          tempHealth: 0,
          isDead: false,
          isFallen: false,
          activeConditions: [],
          conditionCounters: {},
          conditionLevels: {},
          conditionStartRounds: {},
          downedSaves: { Handeln: 'none', Wissen: 'none', Soziales: 'none' }
        });
      }
    });
    for (let i = 1; i <= 2; i++) {
      list.push({
        name: `Berserker ${i}`,
        initiative: 50 + i,
        type: 'enemy',
        currentHealth: DEFAULT_HP,
        maxHealth: DEFAULT_HP,
        tempHealth: 0,
        isDead: false,
        isFallen: false,
        activeConditions: [],
        conditionCounters: {},
        conditionLevels: {},
        conditionStartRounds: {},
        downedSaves: { Handeln: 'none', Wissen: 'none', Soziales: 'none' }
      });
    }
    if (list.length === 0) return;
    setTurnOrder(sortParticipants(list));
    setCurrentTurnIndex(0);
    setCurrentRound(1);
    setPhase('grid');
  };

  const applyAutoDamage = (p: Participant, round: number): Participant => {
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
  };

  const triggerNextTurn = (manualList?: Participant[]) => {
    if (!turnOrder) return;
    let listToProcess = manualList || [...turnOrder];
    
    let nextIdxInCurrentList = (currentTurnIndex + 1) % listToProcess.length;
    let tempRound = currentRound;
    if (nextIdxInCurrentList === 0) tempRound++;
    const nextActorName = listToProcess[nextIdxInCurrentList].name;

    const finishedP = { ...listToProcess[currentTurnIndex] };
    if (finishedP.activeConditions.includes('Übergebend')) {
      finishedP.conditionCounters['Übergebend']--;
      if (finishedP.conditionCounters['Übergebend'] <= 0) {
        finishedP.activeConditions = finishedP.activeConditions.filter(c => c !== 'Übergebend');
        delete finishedP.conditionCounters['Übergebend'];
      }
      listToProcess[currentTurnIndex] = finishedP;
    }

    let sortedList = sortParticipants(listToProcess);
    let newTurnIdx = sortedList.findIndex(p => p.name === nextActorName);

    sortedList[newTurnIdx] = applyAutoDamage(sortedList[newTurnIdx], tempRound);

    const checkSkip = (idx: number): boolean => {
      const p = sortedList[idx];
      if (p.isDead) return true;
      if (p.activeConditions.includes('Liegend') && !p.isFallen) {
        sortedList[idx] = { ...p, activeConditions: p.activeConditions.filter(c => c !== 'Liegend') };
        return true; 
      }
      if (p.activeConditions.includes('Ohnmächtig')) return true;
      return false;
    };

    let attempts = 0;
    while (checkSkip(newTurnIdx) && attempts < sortedList.length) {
      newTurnIdx = (newTurnIdx + 1) % sortedList.length;
      if (newTurnIdx === 0) tempRound++;
      attempts++;
    }

    setTurnOrder(sortedList);
    setCurrentTurnIndex(newTurnIdx);
    setCurrentRound(tempRound);
    setExpandedConditions({}); 
  };

  const toggleCondition = (pIdx: number, condition: string) => {
    if (!turnOrder) return;
    let newList = [...turnOrder];
    let p = { ...newList[pIdx] };
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
          p.conditionStartRounds[condition] = currentRound;
      }
      if (condition === 'Liegend' && pIdx === currentTurnIndex && !p.isFallen) autoEndTurn = true;
    }

    newList[pIdx] = p;
    if (autoEndTurn) triggerNextTurn(newList);
    else setTurnOrder(newList);
  };

  const setConditionLevel = (pIdx: number, condition: string, level: number) => {
    if (!turnOrder) return;
    let newList = [...turnOrder];
    newList[pIdx].conditionLevels[condition] = level;
    setTurnOrder(newList);
  };

  const cycleDownedStatus = (pIdx: number, key: keyof DownedState) => {
    if (!turnOrder) return;
    let newList = [...turnOrder];
    let p = { ...newList[pIdx] };
    
    // Enforcement of Order: Handeln -> Wissen -> Soziales
    const keyIndex = SAVE_KEYS.indexOf(key);
    if (keyIndex > 0) {
      const prevKey = SAVE_KEYS[keyIndex - 1];
      if (p.downedSaves[prevKey] === 'none') return;
    }

    const current = p.downedSaves[key];
    let next: SaveStatus = 'none';
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

    // Check for stabilization or death
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
      if (pIdx === currentTurnIndex) mustEndTurn = true;
    } else if (fails >= 2) {
      p.isDead = true;
      p.isFallen = false;
      if (pIdx === currentTurnIndex) mustEndTurn = true;
    }

    newList[pIdx] = p;
    if (mustEndTurn) {
        triggerNextTurn(newList);
    } else {
        setTurnOrder(newList);
    }
  };

  const applyAction = (index: number, mode: 'damage' | 'heal' | 'temp') => {
    if (!turnOrder) return;
    const inputVal = parseInt(healthInputs[index]) || 0;
    const newList = [...turnOrder];
    const p = { ...newList[index] };

    if (mode === 'damage') {
      let totalDamage = inputVal;
      if (p.activeConditions.includes('Vergiftet')) {
          totalDamage += calcConditionDamage(p, 'Vergiftet', currentRound);
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

    newList[index] = p;
    setTurnOrder(newList);
    setHealthInputs(prev => ({ ...prev, [index]: '' }));

    const aliveEnemies = newList.filter(part => part.type === 'enemy' && !part.isDead);
    if (newList.some(part => part.type === 'enemy') && aliveEnemies.length === 0) {
      setTimeout(() => onVictory(), 800);
    }
  };

  return (
    <div className="min-h-screen flex flex-col items-center p-8 bg-[#0a0a0a] text-parchment-light overflow-x-hidden relative custom-scrollbar font-body">
      <div className="fixed inset-0 bg-[radial-gradient(circle_at_center,_var(--tw-gradient-stops))] from-crimson/10 via-black to-black opacity-90"></div>

      <main className="relative z-10 w-full max-w-5xl flex flex-col items-center">
        {phase === 'initiative' ? (
          <section className="w-full flex flex-col items-center animate-in slide-in-from-bottom-4 duration-500">
             <header className="text-center mb-10">
               <span className="material-symbols-outlined text-crimson text-6xl mb-4">swords</span>
               <h1 className="text-5xl font-display uppercase tracking-[0.2em] text-secondary text-shadow-sm mb-2">Initiative festlegen</h1>
               <p className="font-medieval text-white/40 italic">Bestimme die Reihenfolge der Einheiten (0-99)</p>
             </header>
             <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 w-full">
                {[...PLAYER_NAMES, ...COMPANION_NAMES].map(name => (
                    <div key={name} className="bg-white/5 border border-white/10 p-4 rounded flex flex-col hover:border-secondary/40 transition-colors">
                        <label className="text-[10px] uppercase tracking-widest text-secondary mb-2 font-bold">{name}</label>
                        <input 
                            type="number" 
                            min="0"
                            max="99"
                            placeholder="0-99"
                            value={initiatives[name]} 
                            onChange={e => setInitiatives(prev => ({...prev, [name]: e.target.value}))}
                            className="bg-black/60 border border-white/10 rounded p-2 text-2xl text-center font-bold text-white outline-none focus:border-secondary transition-all"
                        />
                    </div>
                ))}
                <div className="col-span-full flex flex-col items-center mt-10 space-y-4">
                    <button onClick={startSkirmish} className="bg-secondary text-black px-16 py-6 font-display font-bold uppercase tracking-widest text-xl shadow-2xl hover:scale-105 transition-all active:scale-95">Gefecht starten</button>
                    <button onClick={onBack} className="text-[10px] text-white/20 uppercase tracking-widest font-bold hover:text-white transition-colors">Abbruch</button>
                </div>
             </div>
          </section>
        ) : (
          <section className="w-full space-y-6 animate-in fade-in duration-700">
            <header className="text-center mb-6">
              <h1 className="text-4xl font-display uppercase tracking-[0.2em] text-secondary text-shadow-sm">Runde {currentRound}</h1>
            </header>

            <div className="bg-white/5 border border-white/10 rounded shadow-2xl overflow-hidden backdrop-blur-sm">
                <div className="p-4 space-y-4 max-h-[72vh] overflow-y-auto custom-scrollbar">
                    {turnOrder?.map((p, idx) => {
                        const isActive = idx === currentTurnIndex;
                        const isExpanded = expandedConditions[idx];
                        const isInvisible = p.activeConditions.includes('Unsichtbar');
                        const isDowned = p.isFallen;
                        
                        const visualTotal = Math.max(p.maxHealth, p.currentHealth + p.tempHealth);
                        const healthPerc = (p.currentHealth / visualTotal) * 100;
                        const tempPerc = (p.tempHealth / visualTotal) * 100;
                        
                        const wounds = getWoundsByHp(p.currentHealth, p.maxHealth);
                        const pokusCount = characterPokus[p.name] || 0;

                        let borderClasses = 'bg-black/40 border-white/5';
                        if (isActive) {
                          if (isInvisible) {
                            borderClasses = 'bg-slate-500/10 border-slate-300 shadow-[0_0_15px_rgba(203,213,225,0.2)] ring-1 ring-slate-300/50';
                          } else {
                            borderClasses = 'bg-secondary/10 border-secondary shadow-[0_0_15px_rgba(212,175,55,0.1)] ring-1 ring-secondary/40';
                          }
                        }

                        return (
                            <div key={idx} className={`flex flex-col rounded border-2 transition-all duration-500 relative ${borderClasses} ${p.isDead ? 'opacity-20 grayscale cursor-not-allowed' : ''}`}>
                                <div className="p-4 flex items-center justify-between gap-4 flex-wrap md:flex-nowrap min-h-[120px]">
                                    <div className="flex items-center space-x-4">
                                        <div className={`w-8 h-8 rounded-full flex items-center justify-center font-bold text-xs ${isActive ? (isInvisible ? 'bg-slate-300 text-black animate-pulse' : 'bg-secondary text-black animate-pulse') : 'bg-white/10 text-white/40'}`}>
                                            {p.initiative}
                                        </div>
                                        <div>
                                            <p className={`font-medieval text-xl ${p.type === 'enemy' ? 'text-red-400' : 'text-white'}`}>{p.name}</p>
                                            <div className="flex flex-wrap gap-1 mt-1">
                                                {!p.isDead && wounds.map((w, wIdx) => (
                                                  <span key={wIdx} className="text-[8px] bg-red-600/60 text-white border border-red-400 px-1.5 py-0.5 rounded uppercase font-bold tracking-tighter flex items-center space-x-1 shadow-[0_0_8px_rgba(220,38,38,0.5)]">
                                                    <span className="material-symbols-outlined text-[10px]">water_drop</span>
                                                    <span>{w}</span>
                                                  </span>
                                                ))}
                                                {p.isDead && <span className="text-[10px] bg-crimson text-white px-2 py-0.5 rounded uppercase font-black tracking-widest">Gefallen</span>}
                                                {!p.isDead && p.activeConditions.map(c => {
                                                    const cfg = CONDITION_CONFIG[c] || { color: 'bg-white/20', border: 'border-white/40', text: 'text-white' };
                                                    const level = p.conditionLevels[c];
                                                    return (
                                                        <span key={c} className={`text-[8px] ${cfg.color} ${cfg.text} border ${cfg.border} px-1.5 py-0.5 rounded uppercase font-bold tracking-tighter flex items-center gap-1`}>
                                                            {c} {level ? `(S${level})` : ''} {c === 'Übergebend' ? `(${p.conditionCounters[c]})` : ''}
                                                        </span>
                                                    );
                                                })}
                                            </div>
                                        </div>
                                    </div>

                                    <div className="flex items-center space-x-6 ml-auto md:ml-0 flex-grow justify-end">
                                        {!p.isDead && (
                                            isDowned ? (
                                                <div className="flex items-center space-x-6 animate-in fade-in duration-500">
                                                    <div className="flex flex-col items-end mr-2">
                                                        <span className="text-[10px] uppercase font-display font-bold text-crimson animate-pulse tracking-widest">Todesrettung</span>
                                                        <span className="text-[7px] uppercase text-white/30 tracking-widest">Reihenfolge einhalten</span>
                                                    </div>
                                                    
                                                    <div className="flex gap-4">
                                                        {SAVE_KEYS.map((key, kIdx) => {
                                                            const status = p.downedSaves[key];
                                                            const prevStatus = kIdx > 0 ? p.downedSaves[SAVE_KEYS[kIdx - 1]] : 'success';
                                                            const isDisabled = prevStatus === 'none';

                                                            return (
                                                                <div key={key} className={`flex flex-col items-center space-y-1 transition-opacity ${isDisabled ? 'opacity-20 grayscale' : 'opacity-100'}`}>
                                                                    <span className="text-[7px] uppercase tracking-tighter text-white/40 font-bold">{key}</span>
                                                                    <button 
                                                                        onClick={() => !isDisabled && cycleDownedStatus(idx, key)}
                                                                        disabled={isDisabled}
                                                                        className={`w-14 h-14 border-2 rounded flex items-center justify-center transition-all duration-300 relative ${
                                                                            status === 'success' ? 'bg-green-700/60 border-green-400 shadow-[0_0_15px_rgba(74,222,128,0.4)]' :
                                                                            status === 'fail' ? 'bg-crimson/70 border-red-500 shadow-[0_0_15px_rgba(128,0,32,0.5)]' :
                                                                            'bg-parchment-card/5 border-white/20 hover:border-white/40'
                                                                        }`}
                                                                    >
                                                                        {status === 'success' && <span className="material-symbols-outlined text-white text-3xl font-black drop-shadow-md">check</span>}
                                                                        {status === 'fail' && <span className="material-symbols-outlined text-white text-3xl font-black drop-shadow-md">close</span>}
                                                                    </button>
                                                                </div>
                                                            );
                                                        })}
                                                    </div>

                                                    <div className="ml-6 flex items-center space-x-2 bg-black/40 p-1.5 rounded border border-white/10">
                                                        <input 
                                                            type="number" 
                                                            placeholder="HP" 
                                                            value={healthInputs[idx] || ''} 
                                                            onChange={e => setHealthInputs(prev => ({...prev, [idx]: e.target.value}))} 
                                                            className="w-10 bg-white/5 border border-white/10 rounded text-center text-xs outline-none text-white font-bold" 
                                                        />
                                                        <button onClick={() => applyAction(idx, 'heal')} className="w-8 h-8 flex items-center justify-center rounded bg-green-700/40 hover:bg-green-600 text-white transition-all">
                                                            <span className="material-symbols-outlined text-sm">healing</span>
                                                        </button>
                                                    </div>
                                                </div>
                                            ) : (
                                                <>
                                                    <div className="flex flex-col items-stretch min-w-[240px]">
                                                        <div className="flex justify-between items-end mb-1 gap-4">
                                                            {p.type !== 'enemy' ? (
                                                              <button 
                                                                  onClick={() => onCastMagic(p.name)}
                                                                  className="group flex items-center space-x-2 bg-teal-900/40 hover:bg-teal-700/60 border border-teal-500/40 rounded px-3 py-1 transition-all active:scale-95"
                                                              >
                                                                  <span className="material-symbols-outlined text-teal-300 text-[14px] animate-pulse">auto_fix_high</span>
                                                                  <span className="text-[9px] uppercase font-bold tracking-widest text-teal-200">Magie wirken</span>
                                                                  <span className="bg-teal-500 text-black text-[10px] font-bold px-1.5 rounded-full">{pokusCount}</span>
                                                              </button>
                                                            ) : <div className="flex-grow"></div>}

                                                            <div className="flex items-baseline space-x-1">
                                                              <span className="text-xl font-bold font-medieval text-white">
                                                                  {p.currentHealth}
                                                              </span>
                                                              {p.tempHealth > 0 && <span className="text-sky-400 text-sm font-bold">+{p.tempHealth}</span>}
                                                              <span className="text-[10px] text-white/20 ml-1">/ {p.maxHealth}</span>
                                                            </div>
                                                        </div>

                                                        <div className="w-full h-2.5 bg-black/80 rounded-sm overflow-hidden relative border border-white/10 shadow-inner">
                                                            <div 
                                                              className="absolute h-full bg-gradient-to-r from-crimson to-red-500 transition-all duration-700 shadow-[0_0_10px_rgba(128,0,32,0.4)]" 
                                                              style={{width: `${healthPerc}%`}}
                                                            ></div>
                                                            <div 
                                                              className="absolute h-full bg-gradient-to-r from-sky-600 to-sky-400 transition-all duration-700 shadow-[0_0_12px_rgba(56,189,248,0.5)] border-l border-white/30" 
                                                              style={{width: `${tempPerc}%`, left: `${healthPerc}%`}}
                                                            ></div>
                                                            <div className="absolute inset-0 bg-gradient-to-b from-white/10 to-transparent pointer-events-none"></div>
                                                        </div>
                                                    </div>
                                                    
                                                    <div className="flex bg-black/60 rounded-lg border border-white/10 p-1 space-x-1.5">
                                                        <input 
                                                            type="number" 
                                                            placeholder="0" 
                                                            value={healthInputs[idx] || ''} 
                                                            onChange={e => setHealthInputs(prev => ({...prev, [idx]: e.target.value}))} 
                                                            className="w-10 bg-white/5 border border-white/10 rounded text-center text-xs outline-none text-white font-bold" 
                                                        />
                                                        <button onClick={() => applyAction(idx, 'damage')} className="w-8 h-8 flex items-center justify-center rounded bg-crimson/40 hover:bg-crimson text-white transition-all shadow-lg">
                                                            <span className="material-symbols-outlined text-sm">skull</span>
                                                        </button>
                                                        <button onClick={() => applyAction(idx, 'heal')} className="w-8 h-8 flex items-center justify-center rounded bg-green-700/40 hover:bg-green-600 text-white transition-all shadow-lg">
                                                            <span className="material-symbols-outlined text-sm">healing</span>
                                                        </button>
                                                        <button onClick={() => applyAction(idx, 'temp')} className="w-8 h-8 flex items-center justify-center rounded bg-sky-700/40 hover:bg-sky-600 text-white transition-all shadow-lg" title="Schild (tempLP)">
                                                            <span className="material-symbols-outlined text-sm">shield_moon</span>
                                                        </button>
                                                        <button onClick={() => setExpandedConditions(prev => ({...prev, [idx]: !prev[idx]}))} className={`w-8 h-8 flex items-center justify-center rounded transition-all ${isExpanded ? 'bg-secondary text-black' : 'bg-white/10 text-white/40 hover:bg-white/20'}`}>
                                                            <span className="material-symbols-outlined text-sm">settings_accessibility</span>
                                                        </button>
                                                    </div>
                                                </>
                                            )
                                        )}
                                    </div>
                                </div>

                                {isExpanded && !isDowned && !p.isDead && (
                                    <div className="bg-black/60 border-t border-white/5 p-4 animate-in slide-in-from-top-2">
                                        <h4 className="text-[10px] uppercase tracking-widest text-secondary font-bold mb-2 flex items-center gap-1">
                                          <span className="material-symbols-outlined text-xs">psychology</span> Zustände verwalten
                                        </h4>
                                        <div className="grid grid-cols-3 md:grid-cols-6 gap-2">
                                            {CONDITIONS.map(c => {
                                                const isActive = p.activeConditions.includes(c);
                                                const hasLevel = c === 'Vergiftet' || c === 'Brennend';
                                                
                                                return (
                                                  <div key={c} className="flex flex-col gap-1">
                                                    <button 
                                                      onClick={() => toggleCondition(idx, c)} 
                                                      className={`text-[9px] py-2 rounded border uppercase font-bold tracking-tighter transition-all ${isActive ? `${CONDITION_CONFIG[c].color} ${CONDITION_CONFIG[c].text} ${CONDITION_CONFIG[c].border}` : 'bg-white/5 border-white/10 text-white/30 hover:border-white/30'}`}
                                                    >
                                                        {c}
                                                    </button>
                                                    {isActive && hasLevel && (
                                                      <div className="flex items-center gap-1 bg-black/40 rounded px-1 py-0.5 border border-white/5">
                                                        <span className="text-[7px] text-white/40 uppercase font-bold">Stufe</span>
                                                        <select 
                                                          value={p.conditionLevels[c] || 1}
                                                          onChange={(e) => setConditionLevel(idx, c, parseInt(e.target.value))}
                                                          className="bg-transparent text-[8px] text-secondary font-bold outline-none"
                                                        >
                                                          {[1,2,3,4,5,6].map(lv => <option key={lv} value={lv}>{lv}</option>)}
                                                        </select>
                                                      </div>
                                                    )}
                                                  </div>
                                                );
                                            })}
                                        </div>
                                    </div>
                                )}
                            </div>
                        );
                    })}
                </div>
            </div>

            <div className="flex flex-col items-center pt-6">
                <button onClick={() => triggerNextTurn()} className="group relative bg-secondary text-black px-24 py-6 font-display font-bold uppercase tracking-[0.4em] text-xl shadow-2xl hover:scale-105 transition-all active:scale-95">
                    <div className="absolute inset-0 border-2 border-white/20 m-1 rounded-sm pointer-events-none"></div>
                    <span>Nächster Zug</span>
                </button>
                <button onClick={onBack} className="mt-8 text-[10px] text-white/20 uppercase tracking-[0.3em] font-bold hover:text-secondary transition-colors">Gefecht beenden</button>
            </div>
          </section>
        )}
      </main>
    </div>
  );
};

export default BattleView;
