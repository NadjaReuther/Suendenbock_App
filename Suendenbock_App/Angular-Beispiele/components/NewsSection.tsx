
import React from 'react';
import { NewsItem } from '../App';

interface NewsSectionProps {
  news: NewsItem[];
  onViewAll: () => void;
}

export const NewsSection: React.FC<NewsSectionProps> = ({ news, onViewAll }) => {
  const isOlderThanOneMonth = (dateStr: string) => {
    const parts = dateStr.split('.');
    if (parts.length !== 3) return false;
    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10) - 1;
    const year = parseInt(parts[2], 10);
    const postDate = new Date(year, month, day);
    const oneMonthAgo = new Date();
    oneMonthAgo.setMonth(oneMonthAgo.getMonth() - 1);
    return postDate < oneMonthAgo;
  };

  const activeNews = news
    .filter(item => !isOlderThanOneMonth(item.date))
    .slice(0, 2);

  return (
    <div className="flex flex-col w-full">
      {/* Absolute Align Header */}
      <div className="flex items-center justify-between border-b-2 border-secondary mb-4 h-[40px] leading-none">
        <h2 className="font-display text-xl text-secondary flex items-center gap-2">
          <span className="material-icons-outlined">campaign</span>
          News & Ankündigungen
        </h2>
        <span className="material-icons-outlined text-secondary opacity-50">history_edu</span>
      </div>

      <div className="bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark rounded-lg shadow-soft p-6 flex flex-col h-[320px]">
        <div className="flex-grow overflow-hidden">
          {activeNews.length > 0 ? (
            <ul className="space-y-6">
              {activeNews.map((item) => (
                <li 
                  key={item.id} 
                  onClick={onViewAll}
                  className="flex items-start gap-4 border-b border-border-light/50 dark:border-border-dark/50 pb-4 last:border-0 last:pb-0 group/item cursor-pointer transition-all hover:bg-primary/5 p-2 -m-2 rounded"
                >
                  <div className="bg-primary/10 p-3 rounded-full text-primary shrink-0 transition-transform group-hover/item:scale-110">
                    <span className="material-icons-outlined text-2xl">{item.icon}</span>
                  </div>
                  <div className="flex-grow min-w-0">
                    <h3 className="font-display text-lg text-text-main-light dark:text-text-main-dark group-hover/item:text-primary transition-colors truncate">
                      {item.title}
                    </h3>
                    <p className="text-sm opacity-70 mt-1 line-clamp-2">
                      {item.excerpt}
                    </p>
                    <div className="flex items-center gap-4 mt-2 text-xs font-bold uppercase tracking-wider opacity-60">
                      <span className="flex items-center gap-1"><span className="material-icons-outlined text-sm">person</span> {item.author}</span>
                      <span className="flex items-center gap-1"><span className="material-icons-outlined text-sm">schedule</span> {item.date}</span>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          ) : (
            <div className="h-full flex flex-col items-center justify-center text-center py-10 opacity-40">
              <span className="material-icons-outlined text-5xl mb-3">auto_stories</span>
              <p className="font-display text-xl">Derzeit nichts Neues.</p>
            </div>
          )}
        </div>
        
        <div className="mt-auto pt-4 border-t border-border-light/30 dark:border-border-dark/30 text-center">
          <button 
            onClick={onViewAll}
            className="text-[10px] font-bold uppercase tracking-widest text-primary hover:text-white hover:bg-primary px-6 py-2 rounded border border-primary transition-all active:scale-95 inline-block"
          >
            Zum Archiv & Alle News
          </button>
        </div>
      </div>
    </div>
  );
};
