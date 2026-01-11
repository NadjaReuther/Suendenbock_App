
import React, { useState, useRef } from 'react';
import { Thread, ThreadReply } from '../App';

interface ThreadDetailPageProps {
  thread: Thread;
  onBack: () => void;
  setThreads: React.Dispatch<React.SetStateAction<Thread[]>>;
}

export const ThreadDetailPage: React.FC<ThreadDetailPageProps> = ({ thread, onBack, setThreads }) => {
  const [replyText, setReplyText] = useState('');
  const [isEditing, setIsEditing] = useState(false);
  const [editedContent, setEditedContent] = useState(thread.content);
  const editorRef = useRef<HTMLTextAreaElement>(null);

  // Simple Markdown to HTML parser
  const renderMarkdown = (text: string) => {
    // 1. Escape HTML to prevent XSS
    let html = text
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#039;");

    // 2. Bold (**text**)
    html = html.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
    
    // 3. Italic (_text_)
    html = html.replace(/_(.*?)_/g, '<em>$1</em>');
    
    // 4. Lists (lines starting with - )
    const lines = html.split('\n');
    let inList = false;
    const processedLines = lines.map(line => {
      if (line.trim().startsWith('- ')) {
        const content = line.trim().substring(2);
        let prefix = '';
        if (!inList) {
          inList = true;
          prefix = '<ul class="list-disc ml-5 space-y-1 my-2">';
        }
        return `${prefix}<li>${content}</li>`;
      } else {
        let suffix = '';
        if (inList) {
          inList = false;
          suffix = '</ul>';
        }
        return suffix + line;
      }
    });
    
    if (inList) {
      processedLines[processedLines.length - 1] += '</ul>';
    }

    // 5. Paragraphs / Newlines
    return processedLines.join('<br />');
  };

  const handlePostReply = (e: React.FormEvent) => {
    e.preventDefault();
    if (!replyText.trim()) return;

    const newReply: ThreadReply = {
      id: Date.now(),
      author: 'Du (Bürger)',
      text: replyText.trim(),
      time: 'Gerade eben'
    };

    setThreads(prev => prev.map(t => {
      if (t.id === thread.id) {
        return {
          ...t,
          replies: [...t.replies, newReply],
          repliesCount: t.repliesCount + 1
        };
      }
      return t;
    }));

    setReplyText('');
  };

  const handleSaveEdit = () => {
    if (!editedContent.trim()) return;
    setThreads(prev => prev.map(t => t.id === thread.id ? { ...t, content: editedContent } : t));
    setIsEditing(false);
  };

  const insertFormat = (tag: string) => {
    if (!editorRef.current) return;
    const start = editorRef.current.selectionStart;
    const end = editorRef.current.selectionEnd;
    const text = editorRef.current.value;
    const before = text.substring(0, start);
    const after = text.substring(end, text.length);
    const selection = text.substring(start, end);

    let newText = "";
    if (tag === 'bold') newText = `${before}**${selection || 'fetter Text'}**${after}`;
    if (tag === 'italic') newText = `${before}_${selection || 'kursiver Text'}_${after}`;
    if (tag === 'list') newText = `${before}\n- ${selection || 'Listenpunkt'}${after}`;

    setEditedContent(newText);
    
    // Set focus back and adjust cursor position (small delay to let React update state)
    setTimeout(() => {
      if (editorRef.current) {
        editorRef.current.focus();
        const offset = tag === 'bold' ? 2 : (tag === 'italic' ? 1 : 3);
        editorRef.current.setSelectionRange(start + offset, end + offset);
      }
    }, 10);
  };

  return (
    <div className="flex-grow max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12 w-full animate-in zoom-in-95 duration-500">
      <button 
        onClick={onBack}
        className="flex items-center gap-2 text-primary hover:text-secondary transition-colors mb-8 group"
      >
        <span className="material-icons-outlined group-hover:-translate-x-1 transition-transform">arrow_back</span>
        <span className="text-xs font-bold uppercase tracking-widest">Zurück zum Forum</span>
      </button>

      <div className="space-y-8">
        {/* Main Post */}
        <div className="bg-surface-light dark:bg-surface-dark border-2 border-primary/20 rounded-xl overflow-hidden shadow-soft relative">
          <div className="absolute top-0 left-0 w-full h-1 bg-primary/40"></div>
          <div className="p-8">
            <div className="flex flex-wrap items-center justify-between gap-4 mb-6">
              <div className="flex flex-wrap items-center gap-3">
                <span className="px-3 py-1 rounded-full bg-primary/10 text-primary text-[10px] font-bold uppercase tracking-widest border border-primary/20">
                  {thread.category}
                </span>
                <span className="text-[10px] opacity-40 font-bold uppercase tracking-widest flex items-center gap-1">
                  <span className="material-icons-outlined text-sm">schedule</span>
                  {thread.time}
                </span>
                <span className="text-[10px] opacity-40 font-bold uppercase tracking-widest flex items-center gap-1">
                  <span className="material-icons-outlined text-sm">person</span>
                  {thread.author}
                </span>
              </div>
              
              {!isEditing && (
                <button 
                  onClick={() => { setEditedContent(thread.content); setIsEditing(true); }}
                  className="flex items-center gap-1 text-[10px] font-bold uppercase tracking-widest text-primary hover:text-secondary transition-colors border border-primary/20 px-3 py-1 rounded-md"
                >
                  <span className="material-icons-outlined text-sm">edit</span>
                  Bearbeiten
                </button>
              )}
            </div>

            <h1 className="font-display text-4xl text-primary mb-6 leading-tight">
              {thread.title}
            </h1>

            {isEditing ? (
              <div className="space-y-4 animate-in fade-in duration-300">
                {/* Editor Toolbar */}
                <div className="flex gap-2 border-b border-border-light dark:border-border-dark pb-2 mb-2 bg-background-light/30 dark:bg-background-dark/30 p-1 rounded-t-lg">
                  <button onClick={() => insertFormat('bold')} className="p-2 hover:bg-primary hover:text-white rounded transition-all text-primary" title="Fett (**text**)">
                    <span className="material-icons-outlined">format_bold</span>
                  </button>
                  <button onClick={() => insertFormat('italic')} className="p-2 hover:bg-primary hover:text-white rounded transition-all text-primary" title="Kursiv (_text_)">
                    <span className="material-icons-outlined">format_italic</span>
                  </button>
                  <button onClick={() => insertFormat('list')} className="p-2 hover:bg-primary hover:text-white rounded transition-all text-primary" title="Liste (- punkt)">
                    <span className="material-icons-outlined">format_list_bulleted</span>
                  </button>
                </div>
                <textarea 
                  ref={editorRef}
                  value={editedContent}
                  onChange={(e) => setEditedContent(e.target.value)}
                  rows={10}
                  className="w-full bg-background-light dark:bg-background-dark border border-primary/30 rounded-lg p-4 text-base focus:ring-2 focus:ring-primary focus:border-primary transition-all font-body leading-relaxed shadow-inner"
                  placeholder="Schreibe deine Nachricht..."
                />
                <div className="flex gap-2 justify-end">
                  <button 
                    onClick={() => setIsEditing(false)}
                    className="px-4 py-2 border border-border-light dark:border-border-dark text-[10px] font-bold uppercase tracking-widest rounded hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
                  >
                    Abbrechen
                  </button>
                  <button 
                    onClick={handleSaveEdit}
                    className="px-6 py-2 bg-primary text-white text-[10px] font-bold uppercase tracking-widest rounded shadow-md hover:bg-yellow-600 transition-all active:scale-95"
                  >
                    Speichern
                  </button>
                </div>
              </div>
            ) : (
              <div className="relative p-6 bg-background-light/10 dark:bg-background-dark/10 rounded-lg border border-border-light/10">
                {/* Prose class for proper typography rendering */}
                <div 
                  className="prose prose-stone dark:prose-invert max-w-none text-lg opacity-90 leading-relaxed font-body"
                  dangerouslySetInnerHTML={{ __html: renderMarkdown(thread.content) }}
                />
              </div>
            )}
          </div>
        </div>

        {/* Replies List */}
        <div className="space-y-6">
          <div className="flex items-center gap-4 border-b border-border-light pb-2">
            <h3 className="font-display text-2xl text-primary">Antworten ({thread.repliesCount})</h3>
            <span className="material-icons-outlined text-primary opacity-40">chat_bubble_outline</span>
          </div>

          {thread.replies.length > 0 ? (
            thread.replies.map((reply) => (
              <div 
                key={reply.id} 
                className="bg-surface-light dark:bg-surface-dark border border-border-light/50 dark:border-border-dark/50 p-6 rounded-lg shadow-soft animate-in slide-in-from-left duration-300"
              >
                <div className="flex justify-between items-center mb-4">
                  <div className="flex items-center gap-2">
                    <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center text-primary border border-primary/20">
                      <span className="material-icons-outlined text-lg">person</span>
                    </div>
                    <span className="font-bold text-sm text-primary">{reply.author}</span>
                  </div>
                  <span className="text-[10px] opacity-40 font-bold uppercase tracking-widest">{reply.time}</span>
                </div>
                {/* Render Markdown also for replies */}
                <div 
                  className="prose prose-sm prose-stone dark:prose-invert max-w-none opacity-90 leading-relaxed border-l-2 border-primary/20 pl-4 py-1"
                  dangerouslySetInnerHTML={{ __html: renderMarkdown(reply.text) }}
                />
              </div>
            ))
          ) : (
            <div className="text-center py-12 opacity-40 border-2 border-dashed border-border-light/50 rounded-xl bg-background-light/10">
              <span className="material-icons-outlined text-5xl mb-3">history_edu</span>
              <p className="font-display text-xl italic">Stille herrscht in diesem Saal. Erhebt Eure Stimme!</p>
            </div>
          )}
        </div>

        {/* Post Reply Area */}
        {!isEditing && (
          <div className="bg-surface-light dark:bg-surface-dark border-2 border-primary/10 rounded-xl p-8 shadow-glow">
            <h4 className="font-display text-xl text-primary mb-4 flex items-center gap-2">
              <span className="material-icons-outlined">edit_note</span>
              Eure Replik verfassen
            </h4>
            <form onSubmit={handlePostReply} className="space-y-4">
              <textarea
                required
                rows={4}
                value={replyText}
                onChange={(e) => setReplyText(e.target.value)}
                placeholder="Was habt Ihr zu dieser Kunde zu sagen? (Markdown unterstützt)"
                className="w-full bg-background-light dark:bg-background-dark border border-border-light dark:border-border-dark rounded-lg p-4 text-sm focus:ring-2 focus:ring-primary focus:border-primary resize-none transition-all shadow-inner"
              />
              <div className="flex justify-end">
                <button 
                  type="submit"
                  disabled={!replyText.trim()}
                  className="bg-primary hover:bg-yellow-600 disabled:opacity-50 text-white font-bold py-3 px-10 rounded-lg shadow-lg transition-all active:scale-95 flex items-center gap-2 text-xs uppercase tracking-widest"
                >
                  <span className="material-icons-outlined text-sm">send</span>
                  Absenden
                </button>
              </div>
            </form>
          </div>
        )}
      </div>
    </div>
  );
};
