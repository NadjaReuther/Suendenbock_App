
import React, { useState, useEffect, useCallback } from 'react';
import { PlayerStatus, DashboardAction, GameState, Quest, QuestStatus, Trophy } from './types';
import ActionCard from './components/ActionCard';
import StatusDisplay from './components/StatusDisplay';
import BattleView from './components/BattleView';
import VictoryView from './components/VictoryView';
import QuestOverview from './components/QuestOverview';
import TrophyView from './components/TrophyView';
import MapView from './components/MapView';
import { geminiService } from './services/geminiService';

const DASHBOARD_ACTIONS: DashboardAction[] = [
  { id: 'map', title: 'Karte', subtitle: 'Erkunde das Reich', icon: 'map' },
  { id: 'quests', title: 'Quests', subtitle: 'Deine Aufgaben', icon: 'history_edu' },
  { id: 'trophies', title: 'Trophäen', subtitle: 'Deine Erfolge', icon: 'skull' },
];

const ALL_CHARACTERS = ['Jewa', 'Jeremias', 'Hironimus', 'Salome', 'Jonata', 'Gabriel', 'Emma', 'Okko'];

const INITIAL_STATUS: PlayerStatus = {
  generatedPokus: 12,
  healthPercent: 85,
  openQuests: 3,
  level: 5,
  characterName: 'Sündenbock'
};

const INITIAL_QUESTS: Quest[] = [
  { id: '1', title: 'Das Erbe von Jeremias', description: 'Jeremias muss die alte Familienklinge in den Ruinen von Alt-Prag finden.', type: 'individual', assignedTo: ['Jeremias'], status: 'active' },
  { id: '2', title: 'Verteidigung des Lager', description: 'Die gesamte Gruppe muss das Nachtlager gegen die marodierenden Berserker verteidigen.', type: 'group', assignedTo: [], status: 'active' },
  { id: '3', title: 'Gabriels Vision', description: 'Gabriel hat eine vorahnung über einen Verrat in den eigenen Reihen.', type: 'individual', assignedTo: ['Gabriel'], status: 'active' },
];

const TROPHY_POOL: Trophy[] = [
  { id: 't1', name: 'Alphamännchen-Pelz', monsterType: 'Tiere', imageUrl: 'https://images.unsplash.com/photo-1557008075-7f2c5efa4cfd?q=80&w=400&auto=format&fit=crop', description: 'Vom Anführer des Schwarzwald-Rudels.', baseEffect: '+1 Initiative', slainEffect: '+3 Initiative', status: 'slain' },
  { id: 't5', name: 'Uraltes Artefakt', monsterType: 'Relikte', imageUrl: 'https://images.unsplash.com/photo-1512168474261-001099f36f9d?q=80&w=400&auto=format&fit=crop', description: 'Ein Relikt aus einer Zeit vor der Menschheit.', baseEffect: '+5 Max HP', slainEffect: '+20 Max HP', status: 'slain' },
  { id: 't12', name: 'Vampirfürsten-Zahn', monsterType: 'Vampire', imageUrl: 'https://images.unsplash.com/photo-1501705388883-4ed8a543392c?q=80&w=400&auto=format&fit=crop', description: 'Ein Relikt der ewigen Nacht.', baseEffect: '2% Lebensraub', slainEffect: '8% Lebensraub', status: 'slain' },
];

const App: React.FC = () => {
  const [gameState, setGameState] = useState<GameState>({
    status: INITIAL_STATUS,
    isReady: false,
    isRestConfirming: false,
    isCampConfirming: false,
    isLoading: false,
    lastMessage: 'Willkommen in der Welt von 1618.',
    currentView: 'dashboard'
  });

  const [quests, setQuests] = useState<Quest[]>(INITIAL_QUESTS);
  const [trophies, setTrophies] = useState<Trophy[]>(TROPHY_POOL);
  const [equippedTrophyIds, setEquippedTrophyIds] = useState<string[]>(['t1', 't12', 't5']);
  const [isTransitioning, setIsTransitioning] = useState(false);
  const [showCampSummary, setShowCampSummary] = useState(false);
  const [focusedQuestId, setFocusedQuestId] = useState<string | undefined>(undefined);
  const [isCombatPrimed, setIsCombatPrimed] = useState(false);
  
  const [characterPokus, setCharacterPokus] = useState<Record<string, number>>(
    ALL_CHARACTERS.reduce((acc, name) => ({ ...acc, [name]: 0 }), {} as Record<string, number>)
  );

  const incrementPokus = (name: string) => {
    setCharacterPokus(prev => ({ ...prev, [name]: (prev[name] || 0) + 1 }));
  };

  useEffect(() => {
    let timer: ReturnType<typeof setTimeout>;
    if (gameState.isCampConfirming && !isTransitioning) {
      timer = setTimeout(() => setGameState(prev => ({ ...prev, isCampConfirming: false })), 10000);
    }
    return () => clearTimeout(timer);
  }, [gameState.isCampConfirming, isTransitioning]);

  const fetchAiCommentary = useCallback(async () => {
    setGameState(prev => ({ ...prev, isLoading: true }));
    const message = await geminiService.generateStatusCommentary(gameState.status);
    setGameState(prev => ({ ...prev, lastMessage: message, isLoading: false }));
  }, [gameState.status]);

  const handleAction = (actionId: string) => {
    setFocusedQuestId(undefined);
    if (actionId === 'quests') setGameState(prev => ({ ...prev, currentView: 'quests' }));
    else if (actionId === 'trophies') setGameState(prev => ({ ...prev, currentView: 'trophies' }));
    else if (actionId === 'map') setGameState(prev => ({ ...prev, currentView: 'map' }));
  };

  const handleNavigateToQuest = (questId: string) => {
    setFocusedQuestId(questId);
    setGameState(prev => ({ ...prev, currentView: 'quests' }));
  };

  const handleCombatClick = () => {
    if (!isCombatPrimed) {
      setIsCombatPrimed(true);
    } else {
      setIsCombatPrimed(false);
      setGameState(prev => ({ ...prev, currentView: 'battle' }));
    }
  };

  const handleCampClick = () => {
    if (!gameState.isCampConfirming) {
      setGameState(prev => ({ ...prev, isCampConfirming: true }));
    } else {
      setShowCampSummary(true);
      setGameState(prev => ({ ...prev, isCampConfirming: false }));
    }
  };

  const confirmRestAndReset = () => {
    setShowCampSummary(false);
    setIsTransitioning(true);
    setTimeout(() => {
      setGameState(prev => ({
        ...prev,
        status: { ...prev.status, healthPercent: Math.min(100, prev.status.healthPercent + 25) }
      }));
      setCharacterPokus(ALL_CHARACTERS.reduce((acc, name) => ({ ...acc, [name]: 0 }), {} as Record<string, number>));
      setIsTransitioning(false);
      fetchAiCommentary();
    }, 1500);
  };

  const navigateBack = () => {
    setGameState(prev => ({ ...prev, currentView: 'dashboard' }));
  };

  if (gameState.currentView === 'victory') return <VictoryView onReturn={navigateBack} />;
  if (gameState.currentView === 'battle') return (
    <BattleView 
      onBack={navigateBack} 
      onVictory={() => setGameState(prev => ({ ...prev, currentView: 'victory' }))} 
      characterPokus={characterPokus}
      onCastMagic={incrementPokus}
    />
  );
  if (gameState.currentView === 'quests') return (
    <QuestOverview 
      onBack={navigateBack} 
      quests={quests} 
      initialQuestId={focusedQuestId}
      onAddQuest={q => setQuests([q, ...quests])} 
      onUpdateQuestStatus={(id, s) => setQuests(quests.map(q => q.id === id ? {...q, status: s} : q))} 
    />
  );
  if (gameState.currentView === 'trophies') return <TrophyView onBack={navigateBack} trophies={trophies} equippedIds={equippedTrophyIds} onEquip={id => setEquippedTrophyIds(prev => prev.includes(id) ? prev.filter(x => x !== id) : prev.length < 3 ? [...prev, id] : prev)} onToggleStatus={id => setTrophies(trophies.map(t => t.id === id ? {...t, status: t.status === 'bought' ? 'slain' : 'bought'} : t))} />;
  if (gameState.currentView === 'map') return <MapView onBack={navigateBack} quests={quests} onNavigateToQuest={handleNavigateToQuest} />;

  return (
    <div className="flex flex-col min-h-screen animate-in fade-in duration-700">
      {isTransitioning && (
        <div className="fixed inset-0 z-[100] bg-black/80 flex flex-col items-center justify-center animate-in fade-in duration-300">
          <span className="material-symbols-outlined text-secondary text-6xl animate-spin mb-4">hourglass_empty</span>
          <p className="font-display text-white text-xl uppercase tracking-[0.3em]">Die Zeit vergeht...</p>
        </div>
      )}

      {showCampSummary && (
        <div className="fixed inset-0 z-[150] bg-black/95 flex items-center justify-center p-4 backdrop-blur-md animate-in fade-in duration-500">
          <div className="bg-parchment-card w-full max-w-lg rounded-sm shadow-2xl border-4 border-primary relative overflow-hidden">
            <div className="absolute inset-0 bg-paper-texture opacity-30 pointer-events-none"></div>
            <header className="bg-primary p-6 text-center">
              <span className="material-symbols-outlined text-white text-5xl mb-2 animate-pulse">fireplace</span>
              <h2 className="font-display text-white text-3xl uppercase tracking-widest">Das Nachtlager</h2>
              <p className="font-medieval text-white/60 italic">Resümee der magischen Taten</p>
            </header>
            <div className="p-8 space-y-6 relative z-10">
              <div className="bg-white/40 border border-primary/20 rounded-md p-6 shadow-inner">
                <h3 className="font-display text-[10px] uppercase tracking-[0.4em] text-ink/60 mb-6 text-center">Gesammelter Pokus</h3>
                <div className="grid grid-cols-2 gap-y-4 gap-x-8">
                  {ALL_CHARACTERS.map(char => (
                    <div key={char} className="flex justify-between items-center border-b border-primary/10 pb-1">
                      <span className="font-medieval text-lg text-ink">{char}</span>
                      <div className="flex items-center space-x-2">
                        <span className="font-display font-bold text-primary">{characterPokus[char]}</span>
                        <span className="material-symbols-outlined text-[14px] text-primary/40">auto_fix_high</span>
                      </div>
                    </div>
                  ))}
                </div>
                <div className="mt-8 pt-4 border-t-2 border-primary/20 flex justify-between items-center">
                  <span className="font-display text-xs uppercase tracking-widest font-bold">Gruppen-Gesamt</span>
                  <span className="text-2xl font-bold text-primary">
                    {Object.values(characterPokus).reduce((a: number, b: number) => a + b, 0)}
                  </span>
                </div>
              </div>
              <div className="flex flex-col space-y-3">
                <button onClick={confirmRestAndReset} className="w-full bg-crimson text-white font-display py-4 rounded-sm shadow-lg hover:scale-[1.02] transition-transform active:scale-95 uppercase tracking-[0.3em] text-sm">Rast beginnen & Kräfte sammeln</button>
                <button onClick={() => setShowCampSummary(false)} className="w-full text-ink/40 font-display text-[10px] uppercase tracking-widest hover:text-ink transition-colors py-2">Zurück zum Dashboard</button>
              </div>
            </div>
          </div>
        </div>
      )}

      <nav className="w-full px-8 py-6 flex justify-between items-center border-b border-parchment-border">
        <div className="text-[10px] uppercase tracking-[0.3em] font-bold text-ink/50">Sündenbock 1618</div>
        <div className="text-[10px] uppercase tracking-[0.3em] font-bold text-ink/50">Dashboard</div>
      </nav>

      <main className="flex-grow flex flex-col items-center py-16 px-4">
        <header className="text-center mb-16">
          <h1 className="text-5xl md:text-7xl font-display text-ink mb-4 text-shadow-sm">Spielmodus</h1>
          <p className="text-sm font-body italic text-ink/60 max-w-lg mx-auto">"{gameState.lastMessage}"</p>
        </header>

        <div className="w-full max-w-5xl flex flex-col items-center space-y-16">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8 w-full">
            {DASHBOARD_ACTIONS.map(action => (
              <ActionCard key={action.id} action={action} onClick={() => handleAction(action.id)} />
            ))}
          </div>

          <div className="w-full flex justify-center">
            {/* Fixierte Dimensionen: h-24 stellt sicher, dass der Button nicht springt */}
            <button 
              onClick={handleCombatClick} 
              className={`relative group w-full md:w-[70%] h-24 rounded-lg shadow-xl px-10 flex items-center justify-center space-x-6 overflow-hidden transition-all duration-500 border-2 ${
                isCombatPrimed 
                  ? 'bg-crimson border-white/40 shadow-[0_0_30px_rgba(128,0,32,0.4)]' 
                  : 'bg-secondary border-transparent'
              }`}
            >
               {/* Rotation um 180 Grad */}
               <span className={`material-symbols-outlined text-4xl transition-all duration-500 transform ${
                 isCombatPrimed ? 'text-white rotate-180' : 'text-black rotate-0'
               }`}>swords</span>
               
               <span className={`font-display text-3xl font-bold uppercase tracking-[0.2em] transition-all duration-500 ${
                 isCombatPrimed ? 'text-white' : 'text-black'
               }`}>
                 {isCombatPrimed ? 'Bereit zum Kampf' : 'Kampfbereit Machen'}
               </span>
               <div className="absolute top-0 -inset-full h-full w-1/2 z-5 block transform -skew-x-12 bg-gradient-to-r from-transparent via-white/10 to-transparent group-hover:animate-shine"></div>
            </button>
          </div>

          <div className="w-full flex flex-col items-center">
            <StatusDisplay status={gameState.status} />
            <div className="flex flex-col md:flex-row gap-4 mt-8 w-full max-w-4xl px-6 justify-center">
              <button onClick={() => setGameState(prev => ({...prev, status: {...prev.status, healthPercent: Math.min(100, prev.status.healthPercent + 5)}}))} className="w-full md:w-56 bg-secondary px-6 py-5 rounded-md shadow-lg font-display font-bold uppercase tracking-wider text-sm flex items-center justify-center space-x-3 hover:scale-105 transition-transform">
                <span className="material-symbols-outlined">restaurant</span>
                <span>Kurze Rast</span>
              </button>
              <button onClick={handleCampClick} className={`w-full md:w-56 px-6 py-5 rounded-md shadow-lg font-display font-bold uppercase tracking-wider text-sm flex flex-col items-center justify-center transition-all duration-300 hover:scale-105 active:scale-95 ${gameState.isCampConfirming ? 'bg-indigo-900 text-white' : 'bg-secondary text-ink'}`}>
                <div className="flex items-center space-x-3">
                  <span className="material-symbols-outlined">{gameState.isCampConfirming ? 'bedtime' : 'fireplace'}</span>
                  <span>{gameState.isCampConfirming ? 'Gute Nacht' : 'Nachtlager'}</span>
                </div>
                {gameState.isCampConfirming && <span className="text-[8px] mt-1 opacity-70">Bestätigen</span>}
              </button>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
};

export default App;
