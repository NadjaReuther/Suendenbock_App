
import React, { useState, useMemo } from 'react';

interface PollOption {
  text: string;
  votes: number;
}

interface Poll {
  id: number;
  question: string;
  options: PollOption[];
  status: 'active' | 'closed';
  totalVotes: number; // This reflects total unique voters
  category: string;
  allowMultipleChoices: boolean;
}

const initialPolls: Poll[] = [
  {
    id: 1,
    question: 'Welches Event wünscht ihr euch als nächstes?',
    status: 'active',
    totalVotes: 342,
    category: 'Events',
    allowMultipleChoices: false,
    options: [
      { text: 'Großes Turnier von Salzburg', votes: 150 },
      { text: 'Raid: Die Schattenlande', votes: 120 },
      { text: 'Gilden-Wettsaufen', votes: 72 }
    ]
  },
  {
    id: 2,
    question: 'Balancing der Monsterklasse "Nachtmahr"',
    status: 'closed',
    totalVotes: 890,
    category: 'Balancing',
    allowMultipleChoices: true,
    options: [
      { text: 'Abschwächen (Nerf)', votes: 600 },
      { text: 'So lassen', votes: 200 },
      { text: 'Stärker machen (Buff)', votes: 90 }
    ]
  },
  {
    id: 3,
    question: 'Soll Gold handelbar sein?',
    status: 'active',
    totalVotes: 156,
    category: 'Wirtschaft',
    allowMultipleChoices: false,
    options: [
      { text: 'Ja, uneingeschränkt', votes: 100 },
      { text: 'Nein, niemals', votes: 40 },
      { text: 'Nur über das Auktionshaus', votes: 16 }
    ]
  }
];

export const PollsPage: React.FC<{ onBack: () => void }> = ({ onBack }) => {
  const [polls, setPolls] = useState<Poll[]>(initialPolls);
  // userVotes: map of pollId to an array of selected option indices
  const [userVotes, setUserVotes] = useState<Record<number, number[]>>({}); 
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [votingPoll, setVotingPoll] = useState<Poll | null>(null);
  const [selectedOptionIndices, setSelectedOptionIndices] = useState<number[]>([]);
  
  // New Poll Form State
  const [newPollTitle, setNewPollTitle] = useState('');
  const [newPollOptions, setNewPollOptions] = useState<string[]>(['', '']);
  const [allowMultiple, setAllowMultiple] = useState(false);

  const sortedPolls = useMemo(() => {
    return [...polls].sort((a, b) => {
      if (a.status === 'active' && b.status === 'closed') return -1;
      if (a.status === 'closed' && b.status === 'active') return 1;
      return b.id - a.id;
    });
  }, [polls]);

  const handleAddOption = () => {
    setNewPollOptions([...newPollOptions, '']);
  };

  const handleRemoveOption = (index: number) => {
    if (newPollOptions.length <= 2) return;
    setNewPollOptions(newPollOptions.filter((_, i) => i !== index));
  };

  const handleOptionChange = (index: number, value: string) => {
    const updated = [...newPollOptions];
    updated[index] = value;
    setNewPollOptions(updated);
  };

  const handleCreatePoll = (e: React.FormEvent) => {
    e.preventDefault();
    const validOptions = newPollOptions.filter(opt => opt.trim() !== '');
    if (!newPollTitle || validOptions.length < 2) return;

    const poll: Poll = {
      id: Date.now(),
      question: newPollTitle,
      status: 'active',
      totalVotes: 0,
      category: 'Community',
      allowMultipleChoices: allowMultiple,
      options: validOptions.map(text => ({ text, votes: 0 }))
    };

    setPolls([poll, ...polls]);
    setIsCreateModalOpen(false);
    resetForm();
  };

  const resetForm = () => {
    setNewPollTitle('');
    setNewPollOptions(['', '']);
    setAllowMultiple(false);
  };

  const openVoteModal = (poll: Poll) => {
    setVotingPoll(poll);
    setSelectedOptionIndices(userVotes[poll.id] || []);
  };

  const toggleOptionSelection = (idx: number) => {
    if (!votingPoll) return;
    
    if (votingPoll.allowMultipleChoices) {
      if (selectedOptionIndices.includes(idx)) {
        setSelectedOptionIndices(selectedOptionIndices.filter(i => i !== idx));
      } else {
        setSelectedOptionIndices([...selectedOptionIndices, idx]);
      }
    } else {
      setSelectedOptionIndices([idx]);
    }
  };

  const castVote = () => {
    if (!votingPoll) return;

    const pollId = votingPoll.id;
    const previousIndices = userVotes[pollId] || [];

    setPolls(prevPolls => prevPolls.map(p => {
      if (p.id !== pollId) return p;

      const newOptions = [...p.options];
      let newTotalUniqueVoters = p.totalVotes;

      // Decrement old choices
      previousIndices.forEach(idx => {
        if (newOptions[idx]) {
          newOptions[idx] = { ...newOptions[idx], votes: Math.max(0, newOptions[idx].votes - 1) };
        }
      });

      // Increment new choices
      selectedOptionIndices.forEach(idx => {
        if (newOptions[idx]) {
          newOptions[idx] = { ...newOptions[idx], votes: newOptions[idx].votes + 1 };
        }
      });

      // Update unique voters count if it's the first time
      if (previousIndices.length === 0 && selectedOptionIndices.length > 0) {
        newTotalUniqueVoters += 1;
      } else if (previousIndices.length > 0 && selectedOptionIndices.length === 0) {
        newTotalUniqueVoters -= 1;
      }

      return { ...p, options: newOptions, totalVotes: newTotalUniqueVoters };
    }));

    setUserVotes({ ...userVotes, [pollId]: selectedOptionIndices });
    setVotingPoll(null);
    setSelectedOptionIndices([]);
  };

  return (
    <div className="flex-grow max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-12 w-full animate-in slide-in-from-right duration-500">
      <div className="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div>
          <button 
            onClick={onBack}
            className="flex items-center gap-2 text-primary hover:text-secondary transition-colors mb-4 group"
          >
            <span className="material-icons-outlined group-hover:-translate-x-1 transition-transform">arrow_back</span>
            <span className="text-xs font-bold uppercase tracking-widest">Zurück zur Halle</span>
          </button>
          <h1 className="font-display text-4xl text-primary">Marktplatz der Meinungen</h1>
          <p className="text-sm opacity-60 italic mt-2">Stimme ab und forme die Zukunft des Reiches.</p>
        </div>
        
        <button 
          onClick={() => setIsCreateModalOpen(true)}
          className="bg-primary hover:bg-yellow-600 text-white font-bold py-3 px-8 rounded shadow-lg transition-all active:scale-95 flex items-center justify-center gap-2 text-sm uppercase tracking-widest"
        >
          <span className="material-icons-outlined">add_chart</span>
          Neue Umfrage starten
        </button>
      </div>

      <div className="grid gap-8">
        {sortedPolls.map((poll) => (
          <div 
            key={poll.id} 
            className={`bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark rounded-xl p-6 shadow-soft relative overflow-hidden transition-all hover:shadow-glow ${poll.status === 'closed' ? 'opacity-70 bg-gray-50/50 dark:bg-gray-900/10' : ''}`}
          >
            <div className="absolute -top-4 -right-4 opacity-5 pointer-events-none">
              <span className="material-icons-outlined text-[120px]">poll</span>
            </div>

            <div className="flex items-center gap-3 mb-4">
              <span className={`px-2 py-0.5 rounded text-[10px] font-bold uppercase tracking-widest ${poll.status === 'active' ? 'bg-green-100 text-green-700 dark:bg-green-900/40 dark:text-green-300' : 'bg-gray-100 text-gray-500 dark:bg-gray-800 dark:text-gray-400'}`}>
                {poll.status === 'active' ? 'Aktiv' : 'Beendet'}
              </span>
              <span className="text-[10px] font-bold uppercase tracking-widest opacity-40">#{poll.category}</span>
              <span className="text-[10px] font-bold uppercase tracking-widest opacity-40">
                {poll.allowMultipleChoices ? 'Mehrfachauswahl' : 'Einfachauswahl'}
              </span>
            </div>

            <h2 className="font-display text-2xl text-text-main-light dark:text-text-main-dark mb-6 pr-12">
              {poll.question}
            </h2>

            <div className="space-y-5 relative z-10">
              {poll.options.map((opt, idx) => {
                // In multiple choice, percentages might sum to > 100% of voters, which is normal
                const percentage = poll.totalVotes > 0 ? (opt.votes / poll.totalVotes) * 100 : 0;
                const isUserChoice = (userVotes[poll.id] || []).includes(idx);
                
                return (
                  <div key={idx} className="space-y-1.5 group">
                    <div className="flex justify-between text-sm font-bold">
                      <div className="flex items-center gap-2">
                        {isUserChoice && <span className="material-icons-outlined text-primary text-xs">check_circle</span>}
                        <span className={isUserChoice ? 'text-primary' : 'group-hover:text-primary transition-colors'}>
                          {opt.text}
                        </span>
                      </div>
                      <span className="opacity-60">{percentage.toFixed(0)}% ({opt.votes})</span>
                    </div>
                    <div className="w-full bg-background-light dark:bg-background-dark h-3 rounded-full border border-border-light/50 dark:border-border-dark/50 overflow-hidden">
                      <div 
                        className={`h-full transition-all duration-1000 ease-out ${poll.status === 'active' ? 'bg-primary shadow-[0_0_8px_rgba(184,158,104,0.4)]' : 'bg-gray-400 dark:bg-gray-600'}`}
                        style={{ width: `${Math.min(100, percentage)}%` }}
                      ></div>
                    </div>
                  </div>
                );
              })}
            </div>

            <div className="mt-8 pt-4 border-t border-border-light/30 dark:border-border-dark/30 flex justify-between items-center text-xs opacity-50 font-bold uppercase tracking-wider">
              <span>{poll.totalVotes} Wähler insgesamt</span>
              {poll.status === 'active' && (
                <button 
                  onClick={() => openVoteModal(poll)}
                  className="text-primary hover:text-secondary transition-colors underline underline-offset-4 flex items-center gap-1"
                >
                  <span className="material-icons-outlined text-sm">how_to_vote</span>
                  {userVotes[poll.id] && userVotes[poll.id].length > 0 ? 'Stimme ändern' : 'Stimme abgeben'}
                </button>
              )}
            </div>
          </div>
        ))}
      </div>

      {/* Vote Modal */}
      {votingPoll && (
        <div className="fixed inset-0 z-[110] flex items-center justify-center p-4 bg-black/70 backdrop-blur-md animate-in fade-in duration-200">
          <div className="bg-surface-light dark:bg-surface-dark border-2 border-primary rounded-lg shadow-glow w-full max-w-md overflow-hidden animate-in zoom-in-95 duration-200">
            <div className="p-6">
              <h3 className="font-display text-xl text-primary font-bold mb-1">{votingPoll.question}</h3>
              <p className="text-[10px] uppercase tracking-widest opacity-50 mb-4 italic">
                {votingPoll.allowMultipleChoices ? 'Du kannst mehrere Optionen wählen' : 'Wähle genau eine Option'}
              </p>
              
              <div className="space-y-3 mb-6 max-h-[40vh] overflow-y-auto pr-2 custom-scrollbar">
                {votingPoll.options.map((opt, idx) => {
                  const isSelected = selectedOptionIndices.includes(idx);
                  return (
                    <button
                      key={idx}
                      onClick={() => toggleOptionSelection(idx)}
                      className={`w-full text-left p-3 rounded border transition-all flex items-center justify-between ${
                        isSelected 
                          ? 'border-primary bg-primary/10 text-primary' 
                          : 'border-border-light dark:border-border-dark hover:border-primary/50'
                      }`}
                    >
                      <span className="text-sm font-bold">{opt.text}</span>
                      <span className="material-icons-outlined text-sm">
                        {votingPoll.allowMultipleChoices 
                          ? (isSelected ? 'check_box' : 'check_box_outline_blank')
                          : (isSelected ? 'radio_button_checked' : 'radio_button_unchecked')}
                      </span>
                    </button>
                  );
                })}
              </div>
              
              <div className="flex gap-3">
                <button 
                  onClick={() => setVotingPoll(null)}
                  className="flex-1 px-4 py-2 border border-border-light dark:border-border-dark text-[10px] font-bold uppercase tracking-widest rounded"
                >
                  Abbrechen
                </button>
                <button 
                  disabled={selectedOptionIndices.length === 0}
                  onClick={castVote}
                  className="flex-1 px-4 py-2 bg-primary disabled:opacity-50 hover:bg-yellow-600 text-white text-[10px] font-bold uppercase tracking-widest rounded shadow shadow-primary/20"
                >
                  Wahl besiegeln
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Create Poll Modal */}
      {isCreateModalOpen && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-in fade-in duration-200">
          <div className="bg-surface-light dark:bg-surface-dark border-2 border-primary rounded-lg shadow-glow w-full max-w-lg overflow-hidden relative animate-in zoom-in-95 duration-200 max-h-[90vh] flex flex-col">
            <div className="absolute top-0 left-0 w-full h-1 bg-primary"></div>
            
            <form onSubmit={handleCreatePoll} className="p-6 space-y-5 flex flex-col overflow-hidden">
              <div className="flex justify-between items-center mb-2 shrink-0">
                <h3 className="font-display text-xl text-primary font-bold">Volksbefragung starten</h3>
                <button 
                  type="button" 
                  onClick={() => setIsCreateModalOpen(false)}
                  className="text-gray-400 hover:text-primary transition-colors"
                >
                  <span className="material-icons-outlined">close</span>
                </button>
              </div>

              <div className="flex-grow overflow-y-auto pr-2 custom-scrollbar space-y-6">
                {/* Question */}
                <div className="space-y-1">
                  <label className="text-[10px] font-bold uppercase tracking-widest opacity-60">Die Frage ans Volk</label>
                  <input 
                    type="text"
                    required
                    value={newPollTitle}
                    onChange={(e) => setNewPollTitle(e.target.value)}
                    placeholder="Worum geht es?"
                    className="w-full bg-background-light dark:bg-background-dark border-border-light dark:border-border-dark rounded focus:ring-primary focus:border-primary text-sm"
                  />
                </div>

                {/* Multiple Choice Toggle */}
                <div className="flex items-center justify-between p-3 bg-primary/5 border border-primary/20 rounded-lg">
                  <div>
                    <h4 className="text-xs font-bold uppercase tracking-tight">Mehrfachauswahl erlauben?</h4>
                    <p className="text-[10px] opacity-60 italic">Können Bürger für mehrere Optionen stimmen?</p>
                  </div>
                  <button
                    type="button"
                    onClick={() => setAllowMultiple(!allowMultiple)}
                    className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors focus:outline-none ${allowMultiple ? 'bg-primary' : 'bg-gray-300 dark:bg-gray-700'}`}
                  >
                    <span className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${allowMultiple ? 'translate-x-6' : 'translate-x-1'}`} />
                  </button>
                </div>

                {/* Dynamic Options */}
                <div className="space-y-3">
                  <label className="text-[10px] font-bold uppercase tracking-widest opacity-60 block">Antwortmöglichkeiten</label>
                  {newPollOptions.map((opt, idx) => (
                    <div key={idx} className="flex gap-2 group">
                      <div className="flex-grow relative">
                        <input 
                          type="text"
                          required={idx < 2}
                          value={opt}
                          onChange={(e) => handleOptionChange(idx, e.target.value)}
                          placeholder={`Option ${idx + 1}`}
                          className="w-full bg-background-light dark:bg-background-dark border-border-light dark:border-border-dark rounded focus:ring-primary focus:border-primary text-sm pr-10"
                        />
                        {newPollOptions.length > 2 && (
                          <button
                            type="button"
                            onClick={() => handleRemoveOption(idx)}
                            className="absolute right-2 top-1/2 -translate-y-1/2 text-gray-400 hover:text-red-500 transition-colors opacity-0 group-hover:opacity-100"
                          >
                            <span className="material-icons-outlined text-sm">delete</span>
                          </button>
                        )}
                      </div>
                    </div>
                  ))}
                  <button 
                    type="button"
                    onClick={handleAddOption}
                    className="w-full py-2 border border-dashed border-primary/40 text-primary text-[10px] font-bold uppercase tracking-widest rounded hover:bg-primary/5 transition-all flex items-center justify-center gap-2"
                  >
                    <span className="material-icons-outlined text-sm">add</span>
                    Option hinzufügen
                  </button>
                </div>
              </div>

              <div className="flex gap-3 pt-4 shrink-0">
                <button 
                  type="button"
                  onClick={() => setIsCreateModalOpen(false)}
                  className="flex-1 px-4 py-2 border border-border-light dark:border-border-dark text-[10px] font-bold uppercase tracking-widest rounded"
                >
                  Abbrechen
                </button>
                <button 
                  type="submit"
                  className="flex-1 px-4 py-2 bg-primary hover:bg-yellow-600 text-white text-[10px] font-bold uppercase tracking-widest rounded shadow shadow-primary/20"
                >
                  Verkünden
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
