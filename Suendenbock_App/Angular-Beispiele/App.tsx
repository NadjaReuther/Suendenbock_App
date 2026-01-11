
import React, { useState, useEffect } from 'react';
import { Hero } from './components/Hero';
import { NewsSection } from './components/NewsSection';
import { PollsWidget } from './components/PollsWidget';
import { EventsWidget } from './components/EventsWidget';
import { SupportSection } from './components/SupportSection';
import { Sidebar } from './components/Sidebar';
import { Footer } from './components/Footer';
import { PollsPage } from './components/PollsPage';
import { EventsPage } from './components/EventsPage';
import { NewsPage } from './components/NewsPage';
import { ForumPage } from './components/ForumPage';
import { ForumWidget } from './components/ForumWidget';
import { ThreadDetailPage } from './components/ThreadDetailPage';

export interface NewsComment {
  id: number;
  author: string;
  text: string;
  time: string;
}

export interface NewsItem {
  id: number;
  icon: string;
  title: string;
  content: string;
  excerpt: string;
  author: string;
  date: string;
  category: string;
  comments: NewsComment[];
}

export interface ThreadReply {
  id: number;
  author: string;
  text: string;
  time: string;
}

export interface Thread {
  id: number;
  title: string;
  content: string;
  author: string;
  category: string;
  time: string;
  repliesCount: number;
  replies: ThreadReply[];
  icon: string;
}

const initialNews: NewsItem[] = [
  {
    id: 1,
    icon: 'campaign',
    title: 'Update 2.4: Die Schatten über Salzburg',
    excerpt: 'Werte Reisende, das neueste Update bringt Änderungen am Magiesystem und neue Quests...',
    content: 'Werte Reisende, das neueste Update bringt massive Änderungen am Magiesystem. Die Schattenlande-Expedition hat begonnen und neue Quests in der Altstadt warten auf euch. Die Gilden-Steuern wurden angepasst, um den Wiederaufbau der Taverne zu finanzieren.',
    author: 'Admin',
    date: new Date().toLocaleDateString('de-DE'),
    category: 'Spiel-Update',
    comments: [
      { id: 101, author: 'Jeremias', text: 'Endlich mehr Magie! Die Taverne sieht auch schon besser aus.', time: '14:20 Uhr' },
      { id: 102, author: 'Salome', text: 'Die Quests in der Altstadt sind ziemlich knackig, nehmt genug Tränke mit.', time: '15:45 Uhr' }
    ]
  },
  {
    id: 2,
    icon: 'history_edu',
    title: 'Serverwartung am kommenden Freitag',
    excerpt: 'Zur Stabilisierung der Gilden-Datenbanken werden wir eine kurze Wartung durchführen.',
    content: 'Zur Stabilisierung der Gilden-Datenbanken und zur Optimierung des Marktplatzes werden wir am Freitag zwischen 02:00 und 04:00 Uhr morgens eine Wartung durchführen. In dieser Zeit wird das Reich nicht betretbar sein.',
    author: 'Tech-Priest',
    date: '28.10.1618',
    category: 'Technik',
    comments: [
      { id: 201, author: 'Jewa', text: 'Hoffentlich wird der Lag beim Marktplatz dadurch besser.', time: '09:12 Uhr' }
    ]
  },
  {
    id: 3,
    icon: 'military_tech',
    title: 'Der Sieger des Turniers steht fest!',
    content: 'Nach drei Tagen voller Schweiß und Stahl hat Jeremias den Titel des Reichschampions errungen. Ein Festmahl zu seinen Ehren wird am kommenden Markttag abgehalten.',
    excerpt: 'Jeremias hat den Titel des Reichschampions errungen. Ein Festmahl wird abgehalten.',
    author: 'Herold',
    date: '25.10.1618',
    category: 'Events',
    comments: []
  }
];

const initialThreads: Thread[] = [
  { 
    id: 1, 
    title: 'Geheimnisse der Schattenlande', 
    content: 'Ich habe in den alten Ruinen südlich von Salzburg ein Pergament gefunden, das von einem geheimen Tunnelnetzwerk unter der Stadt spricht. Wer weiß mehr darüber?', 
    author: 'Jeremias', 
    category: 'Lore', 
    time: 'Vor 5 Min.', 
    repliesCount: 1, 
    replies: [
      { id: 1, author: 'Salome', text: 'Die alten Tunnel sind gefährlich, man sagt dort lauern Nachtmahre!', time: 'Vor 2 Min.' }
    ], 
    icon: 'auto_stories' 
  },
  { 
    id: 2, 
    title: 'Beste Tränke gegen Werwölfe', 
    content: 'Silberdistel und Mondscheinwasser scheinen am besten zu wirken. Hat jemand Erfahrungen mit Alraunwurzel-Beimischung?',
    author: 'Salome', 
    category: 'Handwerk', 
    time: 'Vor 24 Min.', 
    repliesCount: 0, 
    replies: [], 
    icon: 'colorize' 
  },
  { 
    id: 3, 
    title: 'Kriegsrat: Angriff auf Salzburg', 
    content: 'Wir müssen unsere Verteidigung an den Westtoren verstärken. Die Vorräte an Öl und Pfeilen sind knapp.',
    author: 'Jewa', 
    category: 'Taktik', 
    time: 'Vor 1 Std.', 
    repliesCount: 0, 
    replies: [], 
    icon: 'shield' 
  },
];

type View = 'home' | 'polls' | 'events' | 'news' | 'forum' | 'thread-detail';

const App: React.FC = () => {
  const [currentView, setCurrentView] = useState<View>('home');
  const [selectedThreadId, setSelectedThreadId] = useState<number | null>(null);
  const [news, setNews] = useState<NewsItem[]>(initialNews);
  const [threads, setThreads] = useState<Thread[]>(initialThreads);
  const [darkMode, setDarkMode] = useState<boolean>(() => {
    if (typeof window !== 'undefined') {
      const savedTheme = localStorage.getItem('theme');
      return savedTheme === 'dark';
    }
    return false;
  });

  useEffect(() => {
    if (darkMode) {
      document.documentElement.classList.add('dark');
      localStorage.setItem('theme', 'dark');
    } else {
      document.documentElement.classList.remove('dark');
      localStorage.setItem('theme', 'light');
    }
  }, [darkMode]);

  const toggleDarkMode = () => setDarkMode(!darkMode);

  const navigateTo = (view: View, id?: number) => {
    if (view === 'thread-detail' && id) {
      setSelectedThreadId(id);
    }
    setCurrentView(view);
    window.scrollTo(0, 0);
  };

  const selectedThread = threads.find(t => t.id === selectedThreadId);

  return (
    <div className="min-h-screen flex flex-col relative transition-colors duration-300">
      <button 
        onClick={toggleDarkMode}
        className="fixed bottom-6 right-6 z-50 p-3 rounded-full bg-surface-light dark:bg-surface-dark border border-border-light dark:border-border-dark shadow-lg hover:shadow-glow transition-all text-primary"
        aria-label="Design-Modus umschalten"
      >
        <span className="material-icons-outlined">
          {darkMode ? 'light_mode' : 'dark_mode'}
        </span>
      </button>

      {currentView === 'home' && (
        <main className="flex-grow max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 w-full animate-in fade-in duration-500">
          <Hero />
          
          <div className="space-y-12">
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-12">
              <div className="lg:col-span-2 space-y-12">
                <NewsSection news={news} onViewAll={() => navigateTo('news')} />
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                  <PollsWidget onViewAll={() => navigateTo('polls')} />
                  <EventsWidget onViewAll={() => navigateTo('events')} />
                </div>
              </div>
              
              <aside className="space-y-12">
                <Sidebar />
                <SupportSection />
              </aside>
            </div>

            <ForumWidget 
              threads={threads} 
              setThreads={setThreads} 
              onViewAllForum={() => navigateTo('forum')}
              onOpenThread={(id) => navigateTo('thread-detail', id)}
            />
          </div>
        </main>
      )}

      {currentView === 'polls' && (
        <PollsPage onBack={() => navigateTo('home')} />
      )}

      {currentView === 'events' && (
        <EventsPage onBack={() => navigateTo('home')} />
      )}

      {currentView === 'news' && (
        <NewsPage news={news} setNews={setNews} onBack={() => navigateTo('home')} />
      )}

      {currentView === 'forum' && (
        <ForumPage 
          threads={threads} 
          setThreads={setThreads} 
          onBack={() => navigateTo('home')}
          onOpenThread={(id) => navigateTo('thread-detail', id)}
        />
      )}

      {currentView === 'thread-detail' && selectedThread && (
        <ThreadDetailPage 
          thread={selectedThread} 
          onBack={() => navigateTo('forum')} 
          setThreads={setThreads}
        />
      )}

      <Footer />
    </div>
  );
};

export default App;
