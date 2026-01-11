
import React, { useState, useMemo } from 'react';
import { Thread } from '../App';

interface ForumPageProps {
  threads: Thread[];
  setThreads: React.Dispatch<React.SetStateAction<Thread[]>>;
  onBack: () => void;
  onOpenThread: (id: number) => void;
}

const threadCategories = [
  { label: 'Alle', icon: 'apps' },
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

export const ForumPage: React.FC<ForumPageProps> = ({ threads, setThreads, onBack, onOpenThread }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [activeCategory, setActiveCategory] = useState('Alle');
  const [isNewThreadModalOpen, setIsNewThreadModalOpen] = useState(false);
  
  const [newThreadData, setNewThreadData] = useState({
    title: '',
    content: '',
    category: 'Allgemein'
  });

  const filteredThreads = useMemo(() => {
    return threads.filter(thread => {
      const matchesSearch = thread.title.toLowerCase().includes(searchTerm.toLowerCase()) || 
                            thread.author.toLowerCase().includes(searchTerm.toLowerCase());
      const matchesCategory = activeCategory === 'Alle' || thread.category === activeCategory;
      return matchesSearch && matchesCategory;
    });
  }, [threads, searchTerm, activeCategory]);

  const handleCreateThread = (e: React.FormEvent) => {
    e.preventDefault();
    if (!newThreadData.title.trim() || !newThreadData.content.trim()) return;

    const catObj = threadCategories.find(c => c.label === newThreadData.category) || threadCategories[threadCategories.length - 1];

    const thread: Thread = {
      id: Date.now(),
      title: newThreadData.title,
      content: newThreadData.content,
      author: 'Du (Bürger)',
      category: newThreadData.category,
      time: 'Gerade eben',
      repliesCount: 0,
      replies: [],
      icon: catObj.icon
    };

    setThreads(prev => [thread, ...prev]);
    setIsNewThreadModalOpen(false);
    setNewThreadData({ title: '', content: '', category: 'Allgemein' });
  };

  const handleDeleteThread = (e: React.MouseEvent, id: number) => {
    e.stopPropagation();
    if (window.confirm('Möchtest du dieses Thema wirklich unwiderruflich aus der Halle tilgen?')) {
      setThreads(prev => prev.filter(t => t.id !== id));
    }
  };

  return (
    <div className="flex-grow max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-12 w-full animate-in zoom-in-95 duration-500">
      <div className="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div>
          <button 
            onClick={onBack}
            className="flex items-center gap-2 text-primary hover:text-secondary transition-colors mb-4 group"
          >
            <span className="material-icons-outlined group-hover:-translate-x-1 transition-transform">arrow_back</span>
            <span className="text-xs font-bold uppercase tracking-widest">Zurück zur Halle</span>
          </button>
          <h1 className="font-display text-4xl text-primary tracking-wide">Halle der Absprachen</h1>
        </div>
        
        <button 
          onClick={() => setIsNewThreadModalOpen(true)}
          className="bg-primary hover:bg-yellow-600 text-white font-bold py-3 px-8 rounded shadow-lg transition-all active:scale-95 flex items-center justify-center gap-2 text-sm uppercase tracking-widest"
        >
          <span className="material-icons-outlined">add_comment</span>
          Thema eröffnen
        </button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
        <div className="space-y-6">
          <div className="bg-surface-light dark:bg-surface-dark p-6 rounded-xl border border-border-light dark:border-border-dark shadow-soft">
            <h3 className="font-bold text-xs uppercase tracking-widest mb-4 opacity-60">Suche</h3>
            <div className="relative mb-6">
              <input 
                type="text"
                placeholder="Schlagwort..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full bg-background-light dark:bg-background-dark border-border-light dark:border-border-dark rounded-lg py-2 pl-10 text-sm"
              />
              <span className="material-icons-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-lg">search</span>
            </div>

            <h3 className="font-bold text-xs uppercase tracking-widest mb-4 opacity-60">Kategorien</h3>
            <div className="space-y-1">
              {threadCategories.map((cat) => (
                <button
                  key={cat.label}
                  onClick={() => setActiveCategory(cat.label)}
                  className={`w-full flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-bold transition-all ${
                    activeCategory === cat.label 
                      ? 'bg-primary text-white shadow-md' 
                      : 'hover:bg-primary/10 text-text-main-light dark:text-text-main-dark opacity-70 hover:opacity-100'
                  }`}
                >
                  <span className="material-icons-outlined text-lg">{cat.icon}</span>
                  {cat.label}
                </button>
              ))}
            </div>
          </div>
        </div>

        <div className="lg:col-span-3 space-y-4">
          {filteredThreads.map((thread) => (
            <div 
              key={thread.id} 
              onClick={() => onOpenThread(thread.id)}
              className="bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark rounded-xl p-5 shadow-soft hover:shadow-glow transition-all group relative cursor-pointer"
            >
              <div className="flex gap-5 items-start">
                <div className="bg-primary/5 p-4 rounded-xl text-primary shrink-0 group-hover:scale-105 transition-transform">
                  <span className="material-icons-outlined text-3xl">{thread.icon}</span>
                </div>
                <div className="flex-grow min-w-0 pr-12">
                  <div className="flex items-center gap-3 mb-2">
                    <span className="px-2 py-0.5 rounded-full bg-primary/10 text-primary text-[9px] font-bold uppercase tracking-widest border border-primary/20">
                      {thread.category}
                    </span>
                    <span className="text-[10px] opacity-40 uppercase font-bold">{thread.time}</span>
                  </div>
                  <h2 className="font-display text-2xl mb-2 group-hover:text-primary transition-colors truncate">
                    {thread.title}
                  </h2>
                  <div className="flex items-center gap-4 text-xs opacity-60 font-bold uppercase">
                    <span>{thread.author}</span>
                    <span>•</span>
                    <span>{thread.repliesCount} Antworten</span>
                  </div>
                </div>
                <div className="absolute right-4 top-1/2 -translate-y-1/2 flex flex-col gap-2">
                  <button 
                    onClick={(e) => handleDeleteThread(e, thread.id)}
                    className="p-3 text-red-500 hover:bg-red-50 rounded-full transition-all opacity-40 group-hover:opacity-100"
                    title="Thema löschen"
                  >
                    <span className="material-icons-outlined text-xl">delete</span>
                  </button>
                </div>
              </div>
            </div>
          ))}
          
          {filteredThreads.length === 0 && (
            <div className="text-center py-20 opacity-40 border-2 border-dashed border-border-light rounded-xl">
              <span className="material-icons-outlined text-6xl mb-4">history_edu</span>
              <p className="font-display text-2xl">Keine Themen gefunden.</p>
            </div>
          )}
        </div>
      </div>

      {isNewThreadModalOpen && (
        <div className="fixed inset-0 z-[120] flex items-center justify-center p-4 bg-black/70 backdrop-blur-md">
          <div className="bg-surface-light dark:bg-surface-dark border-2 border-primary rounded-lg shadow-glow w-full max-w-md p-8">
            <h3 className="font-display text-2xl text-primary font-bold mb-6">Neues Thema</h3>
            <form onSubmit={handleCreateThread} className="space-y-4">
              <select 
                value={newThreadData.category}
                onChange={(e) => setNewThreadData({...newThreadData, category: e.target.value})}
                className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm p-2"
              >
                {threadCategories.filter(c => c.label !== 'Alle').map(cat => (
                  <option key={cat.label} value={cat.label}>{cat.label}</option>
                ))}
              </select>
              <input 
                type="text" required
                value={newThreadData.title}
                onChange={(e) => setNewThreadData({...newThreadData, title: e.target.value})}
                placeholder="Titel..."
                className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm p-2"
              />
              <textarea 
                required
                value={newThreadData.content}
                onChange={(e) => setNewThreadData({...newThreadData, content: e.target.value})}
                placeholder="Eure Nachricht..."
                rows={4}
                className="w-full bg-background-light dark:bg-background-dark border-border-light rounded text-sm p-2 resize-none"
              />
              <div className="flex gap-4 pt-4">
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
