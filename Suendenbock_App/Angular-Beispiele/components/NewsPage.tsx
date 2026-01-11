
import React, { useState } from 'react';
import { NewsItem, NewsComment } from '../App';

interface NewsPageProps {
  news: NewsItem[];
  setNews: React.Dispatch<React.SetStateAction<NewsItem[]>>;
  onBack: () => void;
}

export const NewsPage: React.FC<NewsPageProps> = ({ news, setNews, onBack }) => {
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [editingItem, setEditingItem] = useState<NewsItem | null>(null);
  const [expandedComments, setExpandedComments] = useState<Record<number, boolean>>({});
  const [newCommentTexts, setNewCommentTexts] = useState<Record<number, string>>({});
  
  const [formData, setFormData] = useState({
    title: '',
    content: '',
    category: 'Spiel-Update'
  });

  const isOlderThanOneMonth = (dateStr: string) => {
    const parts = dateStr.split('.');
    if (parts.length !== 3) return false;
    
    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10) - 1;
    const year = parseInt(parts[2], 10);
    const postDate = new Date(year, month, day);
    
    const now = new Date();
    const oneMonthAgo = new Date();
    oneMonthAgo.setMonth(now.getMonth() - 1);
    
    return postDate < oneMonthAgo;
  };

  const toggleComments = (id: number) => {
    setExpandedComments(prev => ({ ...prev, [id]: !prev[id] }));
  };

  const handleCreateNews = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.title || !formData.content) return;

    const post: NewsItem = {
      id: Date.now(),
      icon: formData.category === 'Spiel-Update' ? 'campaign' : (formData.category === 'Technik' ? 'history_edu' : 'military_tech'),
      title: formData.title,
      content: formData.content,
      excerpt: formData.content.length > 100 ? formData.content.substring(0, 97) + '...' : formData.content,
      author: 'Du (Bürger)',
      date: new Date().toLocaleDateString('de-DE'),
      category: formData.category,
      comments: []
    };

    setNews(prev => [post, ...prev]);
    setIsCreateModalOpen(false);
    resetForm();
  };

  const handleDelete = (id: number) => {
    if (window.confirm('Möchtest du diese Botschaft wirklich für immer aus der Chronik tilgen?')) {
      setNews(prev => prev.filter(item => item.id !== id));
    }
  };

  const handleAddComment = (newsId: number) => {
    const text = newCommentTexts[newsId];
    if (!text || !text.trim()) return;

    const newComment: NewsComment = {
      id: Date.now(),
      author: 'Bürger',
      text: text.trim(),
      time: new Date().toLocaleTimeString('de-DE', { hour: '2-digit', minute: '2-digit' }) + ' Uhr'
    };

    setNews(prev => prev.map(item => {
      if (item.id === newsId) {
        return { ...item, comments: [...item.comments, newComment] };
      }
      return item;
    }));

    setNewCommentTexts(prev => ({ ...prev, [newsId]: '' }));
  };

  const handleDeleteComment = (newsId: number, commentId: number) => {
    if (window.confirm('Soll dieser Kommentar aus der Versammlung entfernt werden?')) {
      setNews(prev => prev.map(item => {
        if (item.id === newsId) {
          return {
            ...item,
            comments: item.comments.filter(c => c.id !== commentId)
          };
        }
        return item;
      }));
    }
  };

  const startEdit = (item: NewsItem) => {
    setEditingItem(item);
    setFormData({
      title: item.title,
      content: item.content,
      category: item.category
    });
  };

  const handleUpdate = (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingItem || !formData.title || !formData.content) return;

    setNews(prev => prev.map(item => 
      item.id === editingItem.id 
        ? { 
            ...item, 
            title: formData.title, 
            content: formData.content, 
            category: formData.category,
            excerpt: formData.content.length > 100 ? formData.content.substring(0, 97) + '...' : formData.content,
            icon: formData.category === 'Spiel-Update' ? 'campaign' : (formData.category === 'Technik' ? 'history_edu' : 'military_tech'),
          } 
        : item
    ));
    setEditingItem(null);
    resetForm();
  };

  const resetForm = () => {
    setFormData({ title: '', content: '', category: 'Spiel-Update' });
  };

  return (
    <div className="flex-grow max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-12 w-full animate-in zoom-in-95 duration-500">
      <div className="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div>
          <button 
            onClick={onBack}
            className="flex items-center gap-2 text-primary hover:text-secondary transition-colors mb-4 group"
          >
            <span className="material-icons-outlined group-hover:-translate-x-1 transition-transform">arrow_back</span>
            <span className="text-xs font-bold uppercase tracking-widest">Zurück zur Halle</span>
          </button>
          <h1 className="font-display text-4xl text-primary tracking-wide">Archiv der Verkündigungen</h1>
          <p className="text-sm opacity-60 italic mt-2">Jedes Pergament erzählt eine Geschichte des Reiches.</p>
        </div>
        
        <button 
          onClick={() => { resetForm(); setIsCreateModalOpen(true); }}
          className="bg-[#6A5ACD] hover:bg-[#5A4BBD] text-white font-bold py-3 px-8 rounded shadow-lg transition-all active:scale-95 flex items-center justify-center gap-2 text-sm uppercase tracking-widest"
        >
          <span className="material-icons-outlined">post_add</span>
          Neuigkeit verkünden
        </button>
      </div>

      <div className="space-y-8">
        {news.map((item) => {
          const isArchived = isOlderThanOneMonth(item.date);
          const isExpanded = expandedComments[item.id];
          
          return (
            <article 
              key={item.id} 
              className={`bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark rounded-xl overflow-hidden shadow-soft transition-all relative ${
                isArchived ? 'opacity-50 grayscale-[0.4] scale-[0.98]' : 'hover:shadow-glow'
              }`}
            >
              <div className={`p-1 bg-gradient-to-r ${isArchived ? 'from-gray-400 to-transparent' : 'from-primary/20 to-transparent'}`}></div>
              
              <div className="p-8">
                <div className="flex flex-wrap items-center justify-between gap-3 mb-4">
                  <div className="flex flex-wrap items-center gap-3">
                    <span className={`px-3 py-1 rounded-full text-[10px] font-bold uppercase tracking-widest border ${
                      isArchived ? 'bg-gray-200 text-gray-600 border-gray-300' : 'bg-primary/10 text-primary border-primary/20'
                    }`}>
                      {item.category}
                    </span>
                    <span className="text-[10px] opacity-40 font-bold uppercase tracking-widest flex items-center gap-1">
                      <span className="material-icons-outlined text-sm">schedule</span>
                      {item.date}
                    </span>
                    <span className="text-[10px] opacity-40 font-bold uppercase tracking-widest flex items-center gap-1">
                      <span className="material-icons-outlined text-sm">person</span>
                      {item.author}
                    </span>
                  </div>
                  
                  <div className="flex items-center gap-3">
                    {isArchived && (
                      <span className="flex items-center gap-1 text-gray-500 font-display text-sm italic opacity-60">
                        <span className="material-icons-outlined text-sm">inventory_2</span>
                        Archiv
                      </span>
                    )}
                    <button 
                      onClick={() => startEdit(item)}
                      className="text-primary hover:text-secondary opacity-40 hover:opacity-100 transition-all"
                      title="Bearbeiten"
                    >
                      <span className="material-icons-outlined text-lg">edit</span>
                    </button>
                    <button 
                      onClick={() => handleDelete(item.id)}
                      className="text-red-500 hover:text-red-700 opacity-40 hover:opacity-100 transition-all"
                      title="Löschen"
                    >
                      <span className="material-icons-outlined text-lg">delete_forever</span>
                    </button>
                  </div>
                </div>

                <div className="flex gap-6 items-start">
                  <div className={`hidden sm:flex p-4 rounded-full border items-center justify-center shrink-0 ${
                    isArchived ? 'bg-gray-100 text-gray-400 border-gray-200' : 'bg-primary/5 text-primary border-primary/10'
                  }`}>
                    <span className="material-icons-outlined text-4xl">{item.icon}</span>
                  </div>
                  <div className="flex-grow">
                    <h2 className={`font-display text-3xl mb-4 leading-tight ${
                      isArchived ? 'text-gray-500' : 'text-text-main-light dark:text-text-main-dark'
                    }`}>
                      {item.title}
                    </h2>
                    <p className="text-base opacity-90 leading-relaxed text-justify indent-8">
                      {item.content}
                    </p>
                  </div>
                </div>

                <div className="mt-8 pt-6 border-t border-border-light/30 dark:border-border-dark/30 flex justify-between items-center">
                  <button 
                    onClick={() => toggleComments(item.id)}
                    className={`flex items-center gap-2 text-xs font-bold uppercase tracking-widest transition-all group ${
                      isArchived ? 'text-gray-400 cursor-default' : 'text-primary hover:text-secondary'
                    }`}
                  >
                    <span className={`material-icons-outlined transition-transform duration-300 ${isExpanded ? 'rotate-180 text-secondary' : 'group-hover:scale-110'}`}>
                      {isExpanded ? 'expand_less' : 'forum'}
                    </span>
                    {isExpanded ? 'Diskussion verbergen' : 'Diskussion öffnen'}
                  </button>
                </div>

                {/* Expandable Comments Section */}
                {isExpanded && (
                  <div className="mt-6 pt-6 border-t border-border-light/20 dark:border-border-dark/20 animate-in slide-in-from-top-4 duration-300">
                    <div className="space-y-4 mb-8">
                      {item.comments.length > 0 ? (
                        item.comments.map((comment) => (
                          <div key={comment.id} className="bg-background-light/40 dark:bg-background-dark/40 p-4 rounded-lg border border-border-light/30 dark:border-border-dark/30 relative group/comment">
                            <div className="flex justify-between items-start mb-2">
                              <span className="font-bold text-xs text-primary">{comment.author}</span>
                              <div className="flex items-start gap-3">
                                <div className="flex flex-col items-end text-right leading-none">
                                  <span className="text-[10px] opacity-50 uppercase font-bold tracking-tighter">{comment.time}</span>
                                  <span className="text-[8px] opacity-30 uppercase font-bold tracking-[0.15em] mt-1">ANNO 1618</span>
                                </div>
                                <button 
                                  onClick={() => handleDeleteComment(item.id, comment.id)}
                                  className="text-red-500 opacity-0 group-hover/comment:opacity-40 hover:!opacity-100 transition-all pt-0.5"
                                  title="Kommentar entfernen"
                                >
                                  <span className="material-icons-outlined text-sm">close</span>
                                </button>
                              </div>
                            </div>
                            <p className="text-sm opacity-90 italic">"{comment.text}"</p>
                          </div>
                        ))
                      ) : (
                        <p className="text-center text-xs opacity-40 py-4 italic">Noch herrscht Stille in dieser Runde.</p>
                      )}
                    </div>

                    {/* New Comment Form */}
                    <div className="relative">
                      <textarea
                        value={newCommentTexts[item.id] || ''}
                        onChange={(e) => setNewCommentTexts(prev => ({ ...prev, [item.id]: e.target.value }))}
                        placeholder="Hinterlasst eine Nachricht an das Volk..."
                        className="w-full bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark rounded p-3 text-sm focus:ring-1 focus:ring-primary focus:border-primary resize-none pr-12"
                        rows={2}
                      />
                      <button 
                        onClick={() => handleAddComment(item.id)}
                        disabled={!newCommentTexts[item.id]?.trim()}
                        className="absolute right-2 bottom-2 p-2 text-primary hover:text-secondary disabled:opacity-30 disabled:hover:text-primary transition-colors"
                        title="Nachricht senden"
                      >
                        <span className="material-icons-outlined">send</span>
                      </button>
                    </div>
                  </div>
                )}
              </div>
            </article>
          );
        })}
      </div>

      {(isCreateModalOpen || editingItem) && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-in fade-in duration-200">
          <div className="bg-surface-light dark:bg-surface-dark border-2 border-primary rounded-lg shadow-glow w-full max-w-lg overflow-hidden relative animate-in zoom-in-95 duration-200">
            <div className="absolute top-0 left-0 w-full h-1 bg-primary"></div>
            
            <form onSubmit={editingItem ? handleUpdate : handleCreateNews} className="p-8 space-y-6">
              <div className="flex justify-between items-center mb-2">
                <h3 className="font-display text-2xl text-primary font-bold">
                  {editingItem ? 'Botschaft anpassen' : 'Botschaft verfassen'}
                </h3>
                <button 
                  type="button" 
                  onClick={() => { setIsCreateModalOpen(false); setEditingItem(null); }} 
                  className="text-gray-400 hover:text-primary transition-colors"
                >
                  <span className="material-icons-outlined">close</span>
                </button>
              </div>

              <div className="space-y-4">
                <div className="space-y-1">
                  <label className="text-[10px] font-bold uppercase tracking-widest opacity-60">Kategorie</label>
                  <select 
                    value={formData.category}
                    onChange={(e) => setFormData({...formData, category: e.target.value})}
                    className="w-full bg-background-light dark:bg-background-dark border-border-light dark:border-border-dark rounded focus:ring-primary focus:border-primary text-sm"
                  >
                    <option value="Spiel-Update">Spiel-Update</option>
                    <option value="Technik">Technik</option>
                    <option value="Events">Events</option>
                  </select>
                </div>

                <div className="space-y-1">
                  <label className="text-[10px] font-bold uppercase tracking-widest opacity-60">Überschrift</label>
                  <input 
                    type="text"
                    required
                    value={formData.title}
                    onChange={(e) => setFormData({...formData, title: e.target.value})}
                    className="w-full bg-background-light dark:bg-background-dark border-border-light dark:border-border-dark rounded focus:ring-primary focus:border-primary text-sm"
                  />
                </div>

                <div className="space-y-1">
                  <label className="text-[10px] font-bold uppercase tracking-widest opacity-60">Inhalt der Botschaft</label>
                  <textarea 
                    required
                    rows={6}
                    value={formData.content}
                    onChange={(e) => setFormData({...formData, content: e.target.value})}
                    className="w-full bg-background-light dark:bg-background-dark border-border-light dark:border-border-dark rounded focus:ring-primary focus:border-primary text-sm resize-none"
                  />
                </div>
              </div>

              <div className="flex gap-4 pt-4">
                <button 
                  type="button" 
                  onClick={() => { setIsCreateModalOpen(false); setEditingItem(null); }} 
                  className="flex-1 px-4 py-3 border border-border-light dark:border-border-dark text-[10px] font-bold uppercase tracking-widest rounded transition-colors hover:bg-gray-100 dark:hover:bg-gray-800"
                >
                  Abbrechen
                </button>
                <button 
                  type="submit" 
                  className="flex-1 px-4 py-3 bg-[#6A5ACD] hover:bg-[#5A4BBD] text-white text-[10px] font-bold uppercase tracking-widest rounded shadow-lg transition-all active:scale-95"
                >
                  {editingItem ? 'Änderungen besiegeln' : 'Besiegeln & Senden'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
