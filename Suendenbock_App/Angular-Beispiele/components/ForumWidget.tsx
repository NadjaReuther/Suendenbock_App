
import React, { useState } from 'react';
import { Thread } from '../App';

interface ForumWidgetProps {
  threads: Thread[];
  setThreads: React.Dispatch<React.SetStateAction<Thread[]>>;
  onViewAllForum: () => void;
  onOpenThread: (id: number) => void;
}

const threadCategories = [
  { label: 'Lore', icon: 'auto_stories' },
  { label: 'Handwerk', icon: 'colorize' },
  { label: 'Taktik', icon: 'shield' },
  { label: 'Gilde', icon: 'groups' },
  { label: 'Löhne', icon: 'payments' },
  { label: 'Monster', icon: 'visibility' },
  { label: 'Brauerei', icon: 'sports_bar' },
  { label: 'Bewerbungen', icon: 'assignment_ind' },
  { label: 'Astronomikon', icon: 'star' },
  { label: 'Allgemein', icon: 'forum' },
];

export const ForumWidget: React.FC<ForumWidgetProps> = ({ threads, setThreads, onViewAllForum, onOpenThread }) => {
  const [isNewThreadModalOpen, setIsNewThreadModalOpen] = useState(false);
  const [newThread, setNewThread] = useState({ title: '', content: '', category: 'Allgemein' });

  const handleCreateThread = (e: React.FormEvent) => {
    e.preventDefault();
    if (!newThread.title.trim() || !newThread.content.trim()) return;
    const catObj = threadCategories.find(c => c.label === newThread.category) || threadCategories[threadCategories.length - 1];
    const thread: Thread = {
      id: Date.now(),
      title: newThread.title,
      content: newThread.content,
      author: 'Du (Bürger)',
      category: newThread.category,
      time: 'Gerade eben',
      repliesCount: 0,
      replies: [],
      icon: catObj.icon
    };
    setThreads(prev => [thread, ...prev]);
    setIsNewThreadModalOpen(false);
    setNewThread({ title: '', content: '', category: 'Allgemein' });
  };

  const deleteThread = (e: React.MouseEvent, id: number) => {
    e.stopPropagation();
    if (window.confirm('Soll dieses Thema wirklich unwiderruflich aus den Aufzeichnungen entfernt werden?')) {
      setThreads(prev => prev.filter(t => t.id !== id));
    }
  };

  return (
    <div className="flex flex-col w-full animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div className="flex items-center justify-between border-b-2 border-primary pb-2 mb-4 h-[40px] leading-none">
        <div className="flex items-center gap-3">
          <h2 className="font-display text-2xl text-primary flex items-center gap-2">
            <span className="material-icons-outlined">forum</span>
            Forum & Absprachen
          </h2>
          <span className="hidden sm:inline-block text-[10px] opacity-40 uppercase font-bold tracking-[0.2em] pt-1">Versammlungshalle</span>
        </div>
        <div className="flex gap-3 shrink-0">
          <button 
            onClick={() => setIsNewThreadModalOpen(true)}
            className="flex items-center gap-2 bg-primary hover:bg-yellow-600 text-white font-bold py-1.5 px-4 rounded shadow transition-all active:scale-95 text-[10px] uppercase tracking-widest"
          >
            <span className="material-icons-outlined text-sm">add_circle</span>
            Neues Thema
          </button>
        </div>
      </div>

      <div className="bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark rounded-xl shadow-soft p-6">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {threads.slice(0, 6).length > 0 ? threads.slice(0, 6).map((thread) => (
            <div 
              key={thread.id} 
              onClick={() => onOpenThread(thread.id)}
              className="group cursor-pointer p-4 rounded-lg bg-background-light/30 dark:bg-background-dark/30 hover:bg-primary/5 border border-border-light/20 dark:border-border-dark/20 transition-all relative active:scale-[0.98] hover:shadow-sm"
            >
              <div className="flex gap-4">
                <div className="text-primary opacity-60 group-hover:opacity-100 transition-opacity pt-1">
                  <span className="material-icons-outlined text-2xl">{thread.icon}</span>
                </div>
                <div className="flex-grow min-w-0 pr-8">
                  <div className="flex items-center gap-2 mb-1">
                    <span className="text-[9px] font-bold uppercase tracking-widest text-primary bg-primary/10 px-1.5 py-0.5 rounded border border-primary/10">
                      {thread.category}
                    </span>
                    <span className="text-[9px] opacity-40 font-bold">{thread.time}</span>
                  </div>
                  <p className="font-bold text-sm leading-snug line-clamp-1 group-hover:text-primary transition-colors">
                    {thread.title}
                  </p>
                  <p className="text-[10px] opacity-50 mt-1">Von {thread.author} • {thread.repliesCount} Antw.</p>
                </div>
                <button 
                  onClick={(e) => deleteThread(e, thread.id)}
                  className="absolute right-2 top-2 p-1.5 text-red-500 opacity-0 group-hover:opacity-100 transition-opacity hover:bg-red-100 rounded-full z-10"
                  title="Thema löschen"
                >
                  <span className="material-icons-outlined text-sm">delete</span>
                </button>
              </div>
            </div>
          )) : (
            <div className="col-span-full text-center py-10 opacity-30 italic text-sm">
              Keine aktuellen Themen vorhanden.
            </div>
          )}
        </div>

        <div className="mt-8 text-center pt-6 border-t border-border-light/30 dark:border-border-dark/30">
          <button 
            onClick={onViewAllForum}
            className="inline-flex items-center gap-2 text-[10px] font-bold uppercase tracking-widest text-primary hover:text-white hover:bg-primary border border-primary px-10 py-3 rounded-lg transition-all shadow-sm hover:shadow-glow active:scale-95"
          >
            Alle Themen der Versammlungshalle anzeigen
            <span className="material-icons-outlined text-sm">arrow_forward</span>
          </button>
        </div>
      </div>

      {isNewThreadModalOpen && (
        <div className="fixed inset-0 z-[150] flex items-center justify-center p-4 bg-black/70 backdrop-blur-md">
          <div className="bg-surface-light dark:bg-surface-dark border-2 border-primary rounded-lg shadow-glow w-full max-w-md animate-in zoom-in-95 duration-200">
             <form onSubmit={handleCreateThread} className="p-8 space-y-6">
              <div className="flex justify-between items-center">
                <h3 className="font-display text-2xl text-primary font-bold">Neues Thema</h3>
                <button type="button" onClick={() => setIsNewThreadModalOpen(false)} className="text-gray-400 hover:text-primary">
                  <span className="material-icons-outlined">close</span>
                </button>
              </div>
              <div className="space-y-4">
                <div>
                  <label className="text-[10px] font-bold uppercase tracking-widest opacity-60 block mb-1">Kategorie</label>
                  <select 
                    value={newThread.category}
                    onChange={(e) => setNewThread({...newThread, category: e.target.value})}
                    className="w-full bg-background-light dark:bg-background-dark border-border-light dark:border-border-dark rounded text-sm p-2"
                  >
                    {threadCategories.map(cat => (
                      <option key={cat.label} value={cat.label}>{cat.label}</option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="text-[10px] font-bold uppercase tracking-widest opacity-60 block mb-1">Titel</label>
                  <input 
                    type="text" required autoFocus
                    value={newThread.title}
                    onChange={(e) => setNewThread({...newThread, title: e.target.value})}
                    placeholder="Worum geht es?"
                    className="w-full bg-background-light dark:bg-background-dark border-border-light dark:border-border-dark rounded text-sm p-2 mb-4"
                  />
                  <label className="text-[10px] font-bold uppercase tracking-widest opacity-60 block mb-1 mt-4">Inhalt</label>
                  <textarea 
                    required
                    value={newThread.content}
                    onChange={(e) => setNewThread({...newThread, content: e.target.value})}
                    placeholder="Eure Nachricht an die Halle..."
                    rows={4}
                    className="w-full bg-background-light dark:bg-background-dark border-border-light dark:border-border-dark rounded text-sm p-2 resize-none"
                  />
                </div>
              </div>
              <div className="flex gap-4 pt-2">
                <button type="button" onClick={() => setIsNewThreadModalOpen(false)} className="flex-1 py-3 border text-[10px] font-bold uppercase tracking-widest rounded">Abbrechen</button>
                <button type="submit" className="flex-1 py-3 bg-primary text-white text-[10px] font-bold uppercase tracking-widest rounded shadow-lg">Verkünden</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
