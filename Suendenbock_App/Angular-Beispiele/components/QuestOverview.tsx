
import React, { useState, useMemo, useEffect } from 'react';
import { Quest, QuestType, QuestStatus } from '../types';

// Extend the tab type to include 'all'
type ExtendedTabStatus = QuestStatus | 'all';

interface QuestOverviewProps {
  onBack: () => void;
  quests: Quest[];
  onAddQuest: (quest: Quest) => void;
  onUpdateQuestStatus: (id: string, status: QuestStatus) => void;
  initialQuestId?: string;
}

const CHARACTERS = ['Jewa', 'Jeremias', 'Hironimus', 'Salome', 'Jonata', 'Gabriel', 'Emma', 'Okko'];

const QuestOverview: React.FC<QuestOverviewProps> = ({ onBack, quests, onAddQuest, onUpdateQuestStatus, initialQuestId }) => {
  const [selectedChars, setSelectedChars] = useState<string[]>([]);
  const [showAddForm, setShowAddForm] = useState(false);
  const [expandedQuestId, setExpandedQuestId] = useState<string | null>(null);
  const [currentTab, setCurrentTab] = useState<ExtendedTabStatus>('active');
  
  // Handling initial quest focus from Map navigation
  useEffect(() => {
    if (initialQuestId) {
      setCurrentTab('all'); // Ensure it's visible regardless of status
      setExpandedQuestId(initialQuestId);
      
      // Scroll to the quest element after a short delay to allow rendering
      setTimeout(() => {
        const element = document.getElementById(`quest-${initialQuestId}`);
        if (element) {
          element.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
      }, 100);
    }
  }, [initialQuestId]);

  // Form State
  const [newQuest, setNewQuest] = useState<Partial<Quest>>({
    title: '',
    description: '',
    type: 'individual',
    assignedTo: [],
    status: 'active'
  });

  const toggleCharFilter = (char: string) => {
    setSelectedChars(prev => 
      prev.includes(char) ? prev.filter(c => c !== char) : [...prev, char]
    );
  };

  const toggleQuestExpand = (id: string) => {
    setExpandedQuestId(prev => prev === id ? null : id);
  };

  const filteredQuests = useMemo(() => {
    let list = quests.filter(q => {
      // Filter by status tab (if not 'all')
      if (currentTab !== 'all' && q.status !== currentTab) return false;
      
      // Filter by characters
      if (selectedChars.length === 0) return true;
      if (q.type === 'group' && q.assignedTo.length === 0) return true;
      return q.assignedTo.some(char => selectedChars.includes(char));
    });

    // Sorting Logic:
    // 1. Status: active (0) -> completed (1) -> failed (2)
    // 2. Type: group (0) -> individual (1)
    return list.sort((a, b) => {
      const statusWeight = { active: 0, completed: 1, failed: 2 };
      const typeWeight = { group: 0, individual: 1 };

      if (statusWeight[a.status] !== statusWeight[b.status]) {
        return statusWeight[a.status] - statusWeight[b.status];
      }
      return typeWeight[a.type] - typeWeight[b.type];
    });
  }, [quests, selectedChars, currentTab]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!newQuest.title) return;
    
    onAddQuest({
      ...newQuest,
      id: Date.now().toString(),
    } as Quest);
    
    setShowAddForm(false);
    setNewQuest({ title: '', description: '', type: 'individual', assignedTo: [], status: 'active' });
  };

  const getQuestStyles = (quest: Quest, isExpanded: boolean) => {
    const isActive = quest.status === 'active';
    const isCompleted = quest.status === 'completed';
    const isFailed = quest.status === 'failed';
    const isGroup = quest.type === 'group';

    let bgColor = 'bg-parchment-card/60';
    let borderColor = 'border-parchment-border';
    let accentBorder = '';

    if (isActive) {
      bgColor = isGroup ? 'bg-blue-50/60' : 'bg-white/80';
      borderColor = isExpanded ? 'border-secondary' : 'border-primary/20';
      accentBorder = isGroup ? 'border-l-4 border-l-primary' : '';
    } else if (isCompleted) {
      bgColor = isGroup ? 'bg-secondary/15' : 'bg-secondary/10';
      borderColor = isExpanded ? 'border-secondary' : 'border-secondary/30';
      accentBorder = isGroup ? 'border-l-4 border-l-secondary' : '';
    } else if (isFailed) {
      bgColor = isGroup ? 'bg-crimson/10' : 'bg-crimson/5';
      borderColor = isExpanded ? 'border-crimson' : 'border-crimson/20';
      accentBorder = isGroup ? 'border-l-4 border-l-crimson' : '';
    }

    return `${bgColor} ${borderColor} ${accentBorder}`;
  };

  const tabs: {id: ExtendedTabStatus, label: string, icon: string}[] = [
    { id: 'all', label: 'Alle', icon: 'auto_stories' },
    { id: 'active', label: 'Aktuell', icon: 'history_edu' },
    { id: 'completed', label: 'Abgeschlossen', icon: 'verified' },
    { id: 'failed', label: 'Gescheitert', icon: 'heart_broken' }
  ];

  return (
    <div className="min-h-screen bg-parchment-light bg-paper-texture p-4 md:p-8 animate-in fade-in duration-500">
      {/* Header Section - Perfectly Centered */}
      <div className="max-w-4xl mx-auto mb-12 border-b-2 border-primary/20 pb-8 relative">
        <div className="flex justify-center items-center">
            <h1 className="text-4xl md:text-5xl font-display text-ink whitespace-nowrap">Quest-Chronik</h1>
        </div>
        
        {/* "Add" button stays in header but moved to far right to avoid interference */}
        <div className="absolute right-0 top-1/2 -translate-y-1/2">
            <button 
              onClick={() => setShowAddForm(true)}
              className="w-10 h-10 md:w-12 md:h-12 rounded-full bg-secondary text-black flex items-center justify-center shadow-lg hover:scale-110 transition-transform active:scale-95"
              title="Neue Quest anlegen"
            >
              <span className="material-symbols-outlined font-bold">add</span>
            </button>
        </div>
      </div>

      {/* Tabs Navigation */}
      <div className="max-w-4xl mx-auto mb-8 flex justify-center flex-wrap gap-2">
        {tabs.map(tab => (
          <button
            key={tab.id}
            onClick={() => { setCurrentTab(tab.id); setExpandedQuestId(null); }}
            className={`flex items-center space-x-2 px-4 md:px-6 py-3 rounded-t-lg transition-all border-b-2 font-display uppercase tracking-widest text-[10px] md:text-xs ${
              currentTab === tab.id 
                ? 'bg-parchment-card border-secondary text-ink font-bold shadow-sm' 
                : 'bg-transparent border-transparent text-ink/40 hover:text-ink/70 hover:bg-white/20'
            }`}
          >
            <span className="material-symbols-outlined text-lg">{tab.icon}</span>
            <span>{tab.label}</span>
          </button>
        ))}
      </div>

      <div className="max-w-4xl mx-auto grid grid-cols-1 lg:grid-cols-4 gap-8">
        {/* Sidebar / Filters */}
        <aside className="lg:col-span-1 space-y-8">
          <section className="bg-parchment-card p-6 rounded shadow-sm border border-parchment-border">
            <h3 className="font-medieval text-xl mb-4 border-b border-primary/10 pb-2 text-center lg:text-left">Helden-Filter</h3>
            <div className="flex flex-wrap lg:flex-col gap-2">
              {CHARACTERS.map(char => (
                <button
                  key={char}
                  onClick={() => toggleCharFilter(char)}
                  className={`px-3 py-2 rounded text-[10px] font-display uppercase tracking-wider transition-all border text-left flex justify-between items-center ${
                    selectedChars.includes(char) 
                      ? 'bg-secondary border-secondary text-black shadow-md' 
                      : 'bg-white/50 border-primary/20 text-ink/60 hover:border-primary'
                  }`}
                >
                  {char}
                  {selectedChars.includes(char) && <span className="material-symbols-outlined text-[12px]">check_circle</span>}
                </button>
              ))}
            </div>
            {selectedChars.length > 0 && (
              <button 
                onClick={() => setSelectedChars([])}
                className="mt-4 w-full text-[10px] uppercase tracking-tighter text-crimson font-bold hover:underline"
              >
                Filter löschen
              </button>
            )}
          </section>
        </aside>

        {/* Main Quest List */}
        <main className="lg:col-span-3">
          <div className="flex flex-col space-y-4">
            {filteredQuests.map(quest => {
              const isExpanded = expandedQuestId === quest.id;
              const isFailed = quest.status === 'failed';
              const isCompleted = quest.status === 'completed';
              const isActive = quest.status === 'active';
              
              const questClasses = getQuestStyles(quest, isExpanded);
              
              return (
                <div 
                  key={quest.id} 
                  id={`quest-${quest.id}`}
                  className={`overflow-hidden rounded-sm shadow-sm border transition-all duration-300 ${
                    isExpanded ? 'scale-[1.01] shadow-lg ring-1 ring-primary/10' : ''
                  } ${questClasses}`}
                >
                  <div 
                    className="p-4 flex items-center justify-between cursor-pointer select-none"
                    onClick={() => toggleQuestExpand(quest.id)}
                  >
                    <div className="flex items-center space-x-3">
                      <span className={`material-symbols-outlined text-xl transition-transform duration-300 ${
                        isExpanded ? 'text-secondary rotate-90' : 'text-primary/40'
                      }`}>
                        {isExpanded ? 'expand_more' : 'chevron_right'}
                      </span>
                      
                      {isActive && <span className="material-symbols-outlined text-primary text-xl" title="Aktuell">history_edu</span>}
                      {isCompleted && <span className="material-symbols-outlined text-secondary text-xl" title="Abgeschlossen">verified</span>}
                      {isFailed && <span className="material-symbols-outlined text-crimson text-xl" title="Gescheitert">heart_broken</span>}

                      <h2 className={`font-medieval text-xl tracking-wide ${
                        isFailed ? 'line-through opacity-60' : ''
                      } ${isExpanded ? 'text-ink' : 'text-ink/80'}`}>
                        {quest.title}
                      </h2>
                    </div>
                    
                    <div className="flex items-center space-x-3">
                      <span className={`hidden md:inline-block px-2 py-0.5 text-[8px] font-display uppercase tracking-widest rounded-full border ${
                        quest.type === 'group' ? 'bg-secondary/20 border-secondary text-secondary font-bold' : 'bg-primary/10 border-primary/40 text-primary'
                      }`}>
                        {quest.type === 'group' ? 'Gruppenquest' : 'Personal'}
                      </span>
                    </div>
                  </div>

                  <div className={`transition-all duration-500 ease-in-out ${isExpanded ? 'max-h-[600px] opacity-100' : 'max-h-0 opacity-0 overflow-hidden'}`}>
                    <div className="p-6 pt-0 border-t border-black/5 bg-white/20">
                      <p className="text-sm text-ink/80 font-body leading-relaxed mb-6 italic pt-4">
                        {quest.description || "Keine Beschreibung hinterlegt."}
                      </p>
                      
                      <div className="flex flex-col space-y-6">
                        <div className="flex flex-wrap gap-2">
                          <span className="text-[9px] uppercase font-bold text-ink/40 tracking-widest w-full">Zugeordnete Helden:</span>
                          {quest.assignedTo.length > 0 ? (
                            quest.assignedTo.map(c => (
                              <span key={c} className="bg-primary text-white text-[9px] px-3 py-1 rounded shadow-sm font-bold uppercase tracking-tighter">
                                {c}
                              </span>
                            ))
                          ) : (
                            <span className="text-[9px] bg-secondary text-black px-3 py-1 rounded font-bold uppercase italic tracking-widest">
                              Alle Gefährten der Gruppe
                            </span>
                          )}
                        </div>
                        
                        <div className="border-t border-primary/10 pt-4">
                           <span className="text-[9px] uppercase font-bold text-ink/40 tracking-widest block mb-3 text-center">Admin: Quest Status ändern</span>
                           <div className="grid grid-cols-3 gap-2">
                              <button onClick={() => onUpdateQuestStatus(quest.id, 'active')} className={`flex items-center justify-center space-x-1 py-2 rounded text-[9px] font-display uppercase tracking-widest border transition-all ${quest.status === 'active' ? 'bg-ink text-white border-ink' : 'bg-white/50 border-ink/20 text-ink/60 hover:bg-ink hover:text-white'}`}>
                                <span className="material-symbols-outlined text-xs">history_edu</span>
                                <span>Aktiv</span>
                              </button>
                              <button onClick={() => onUpdateQuestStatus(quest.id, 'completed')} className={`flex items-center justify-center space-x-1 py-2 rounded text-[9px] font-display uppercase tracking-widest border transition-all ${quest.status === 'completed' ? 'bg-secondary text-black border-secondary' : 'bg-white/50 border-secondary/20 text-secondary hover:bg-secondary hover:text-black'}`}>
                                <span className="material-symbols-outlined text-xs">verified</span>
                                <span>Erfolg</span>
                              </button>
                              <button onClick={() => onUpdateQuestStatus(quest.id, 'failed')} className={`flex items-center justify-center space-x-1 py-2 rounded text-[9px] font-display uppercase tracking-widest border transition-all ${quest.status === 'failed' ? 'bg-crimson text-white border-crimson' : 'bg-white/50 border-crimson/20 text-crimson hover:bg-crimson hover:text-white'}`}>
                                <span className="material-symbols-outlined text-xs">heart_broken</span>
                                <span>Gescheitert</span>
                              </button>
                           </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              );
            })}

            {filteredQuests.length === 0 && (
              <div className="py-20 text-center border-2 border-dashed border-primary/10 rounded-lg">
                <span className="material-symbols-outlined text-5xl text-primary/10 mb-4">history_edu</span>
                <p className="font-medieval text-xl text-ink/30">Keine Quests in dieser Kategorie gefunden.</p>
              </div>
            )}
          </div>
          
          {/* Back Navigation at the bottom */}
          <div className="flex flex-col items-center pt-16 border-t border-primary/10 mt-12">
            <button 
                onClick={onBack} 
                className="group flex flex-col items-center space-y-4 px-12 py-6 rounded-lg bg-white/60 border border-primary/20 hover:border-secondary/40 hover:bg-white transition-all duration-300"
            >
                <span className="material-symbols-outlined text-primary text-4xl group-hover:-translate-y-1 transition-transform">fireplace</span>
                <div className="text-center">
                    <span className="block font-display uppercase tracking-[0.4em] text-sm text-ink group-hover:text-secondary transition-colors">Zurück zum Lager</span>
                    <span className="block text-[10px] font-medieval text-ink/40 italic">Die Chronik wird fortgesetzt</span>
                </div>
            </button>
          </div>
        </main>
      </div>

      {/* Admin Modal */}
      {showAddForm && (
        <div className="fixed inset-0 z-[300] bg-black/80 flex items-center justify-center p-4 backdrop-blur-sm animate-in fade-in duration-300">
          <div className="bg-parchment-light bg-paper-texture w-full max-w-lg rounded-lg shadow-2xl border-2 border-secondary overflow-hidden">
            <div className="bg-secondary p-4 flex justify-between items-center">
              <h3 className="font-display uppercase tracking-widest font-bold">Neue Quest anlegen</h3>
              <button onClick={() => setShowAddForm(false)} className="text-black hover:scale-125 transition-transform">
                <span className="material-symbols-outlined">close</span>
              </button>
            </div>
            
            <form onSubmit={handleSubmit} className="p-8 space-y-6">
              <div>
                <label className="block text-[10px] uppercase tracking-widest font-bold mb-2">Titel der Quest</label>
                <input autoFocus required type="text" value={newQuest.title} onChange={e => setNewQuest(prev => ({...prev, title: e.target.value}))} className="w-full bg-white border border-parchment-border rounded p-3 font-medieval text-lg outline-none focus:border-secondary transition-colors" />
              </div>
              <div>
                <label className="block text-[10px] uppercase tracking-widest font-bold mb-2">Beschreibung</label>
                <textarea rows={3} value={newQuest.description} onChange={e => setNewQuest(prev => ({...prev, description: e.target.value}))} className="w-full bg-white border border-parchment-border rounded p-3 font-body text-sm outline-none focus:border-secondary transition-colors" />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-[10px] uppercase tracking-widest font-bold mb-2">Quest-Typ</label>
                  <select value={newQuest.type} onChange={e => setNewQuest(prev => ({...prev, type: e.target.value as QuestType}))} className="w-full bg-white border border-parchment-border rounded p-3 outline-none focus:border-secondary transition-colors font-body uppercase text-xs">
                    <option value="individual">Personengebunden</option>
                    <option value="group">Gruppenquest</option>
                  </select>
                </div>
                <div>
                  <label className="block text-[10px] uppercase tracking-widest font-bold mb-2">Status</label>
                  <select value={newQuest.status} onChange={e => setNewQuest(prev => ({...prev, status: e.target.value as QuestStatus}))} className="w-full bg-white border border-parchment-border rounded p-3 outline-none focus:border-secondary transition-colors font-body uppercase text-xs">
                    <option value="active">Aktuell</option>
                    <option value="completed">Abgeschlossen</option>
                    <option value="failed">Gescheitert</option>
                  </select>
                </div>
              </div>
              <div>
                <label className="block text-[10px] uppercase tracking-widest font-bold mb-2">Zuweisung</label>
                <div className="h-[60px] overflow-y-auto border border-parchment-border rounded bg-white p-2 custom-scrollbar">
                  <div className="flex flex-wrap gap-1">
                    {CHARACTERS.map(char => (
                      <button key={char} type="button" onClick={() => { const current = newQuest.assignedTo || []; setNewQuest(prev => ({...prev, assignedTo: current.includes(char) ? current.filter(c => c !== char) : [...current, char]})); }} className={`px-2 py-0.5 rounded-[2px] text-[8px] font-bold uppercase border transition-colors ${newQuest.assignedTo?.includes(char) ? 'bg-secondary border-secondary text-black' : 'bg-gray-100 border-gray-300 text-gray-400'}`}>
                        {char}
                      </button>
                    ))}
                  </div>
                </div>
              </div>
              <button type="submit" className="w-full bg-crimson text-white font-display py-4 rounded shadow-lg hover:scale-[1.02] transition-transform active:scale-95 uppercase tracking-[0.2em]">
                In die Chronik aufnehmen
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default QuestOverview;
