
import React, { useState, useEffect, useMemo } from 'react';
import { LoginView } from './views/LoginView';
import { DashboardView } from './views/DashboardView';
import { GuildView } from './views/GuildView';
import { ProfileView } from './views/ProfileView';
import { NotificationSettingsView } from './views/NotificationSettingsView';
import { SecuritySettingsView } from './views/SecuritySettingsView';
import { PrivacySettingsView } from './views/PrivacySettingsView';
import { ChangePasswordView } from './views/ChangePasswordView';
import { ChatListView } from './views/ChatListView';
import { ChatDetailView } from './views/ChatDetailView';
import { NewChatView } from './views/NewChatView';
import { DiceTrayView } from './views/DiceTrayView';
import { NewsView } from './views/NewsView';
import { GodModeView } from './views/GodModeView';
import { ViewType, ChatConversation, Character, Message, DiceResult } from './types';

const INITIAL_MEMBERS: Character[] = [
  { id: 'm1', name: 'Jeremias', class: 'Kleriker', level: 12, status: 'active', imageUrl: 'https://i.pravatar.cc/150?u=jeremias' },
  { id: 'm2', name: 'Jewa', class: 'Bürgerin', level: 16, status: 'active', imageUrl: 'https://suna.home64.de/images/characters/jewa_brand_20251116183650.jpeg', wikiUrl: 'https://suna.home64.de/Character/CharacterSheet/16?searchTerm=jewa' },
  { id: 'm3', name: 'Salome', class: 'Adlige', level: 20, status: 'active', imageUrl: 'https://i.pravatar.cc/150?u=salome' },
  { id: 'm4', name: 'Hironimus', class: 'Gelehrter', level: 15, status: 'active', imageUrl: 'https://i.pravatar.cc/150?u=hironimus' },
  { id: 'm5', name: 'Gabriel', class: 'Wächter', level: 18, status: 'active', imageUrl: 'https://i.pravatar.cc/150?u=gabriel' },
  { id: 'm6', name: 'Jonata', class: 'Handwerkerin', level: 10, status: 'active', imageUrl: 'https://i.pravatar.cc/150?u=jonata' },
  { id: 'm7', name: 'NPC', class: 'Admin', level: 99, status: 'active', imageUrl: 'https://i.pravatar.cc/150?u=npc' },
];

export const MEMBERS = INITIAL_MEMBERS;

const INITIAL_CONVERSATIONS: ChatConversation[] = [
  { id: 'c1', participant: INITIAL_MEMBERS[0], initiatorName: 'Jewa', title: 'Kräuterkunde', lastMessage: 'Habt Dank für die Kräuter!', time: '14:20' },
  { id: 'c3', participant: INITIAL_MEMBERS[1], initiatorName: 'NPC', title: 'Geheime Quest', lastMessage: 'Ein neuer Brief von Okko...', time: 'Jetzt', npcMask: 'Okko' },
  { id: 'c4', participant: INITIAL_MEMBERS[2], initiatorName: 'Jeremias', title: 'Testament', lastMessage: 'Ich werde darüber nachdenken.', time: 'Gestern' },
];

const INITIAL_MESSAGES: Record<string, Message[]> = {
  'c1': [
    { id: '1', sender: INITIAL_MEMBERS[0].name, text: `Gott zum Gruße! Zum Thema 'Kräuterkunde' wollte ich noch etwas anmerken...`, timestamp: '14:00', isMe: false },
    { id: '2', sender: 'Ich', text: 'Sprecht frei heraus, Jeremias. Ich höre zu.', timestamp: '14:15', isMe: true },
    { id: '3', sender: INITIAL_MEMBERS[0].name, text: 'Habt Dank für die Kräuter!', timestamp: '14:20', isMe: false },
  ],
  'c3': [
    { id: '4', sender: 'Okko', text: 'Ein neuer Brief von Okko...', timestamp: 'Jetzt', isMe: false }
  ],
  'c4': [
    { id: '5', sender: 'Jeremias', text: 'Salome, wir müssen über das Testament sprechen.', timestamp: 'Gestern', isMe: false },
    { id: '6', sender: 'Salome', text: 'Ich werde darüber nachdenken.', timestamp: 'Gestern', isMe: true }
  ]
};

const INITIAL_UNREAD: Record<string, Record<string, number>> = {
  'Jewa': { 'c3': 1 },
  'NPC': {}
};

const NEWS_ITEMS = [
  { id: 'n1', title: "Das Siegel der Gilde", date: "Vor 2 Tagen", content: "Ein neues Update für unsere Kommunikationswege wurde veröffentlicht." },
  { id: 'n2', title: "Sieg im Kräutergarten", date: "Vor 1 Stunde", content: "Jewa hat heute durch geschicktes Handeln den Wettbewerb gewonnen. Gratulation an die Bürgerin!" },
  { id: 'n3', title: "Briefe an den Rat", date: "Heute", content: "Neue Verordnungen zur Sicherheit in den Gassen wurden erlassen." }
];

const App: React.FC = () => {
  const [currentView, setCurrentView] = useState<ViewType>('LOGIN');
  const [username, setUsername] = useState<string>('');
  const [password, setPassword] = useState<string>('123456');
  const [isInitialized, setIsInitialized] = useState(false);
  const [selectedChatId, setSelectedChatId] = useState<string | null>(null);
  const [isUserOnlineVisible, setIsUserOnlineVisible] = useState(true);
  const [newsSeen, setNewsSeen] = useState<Record<string, boolean>>({});
  const [chatSourceIsGodMode, setChatSourceIsGodMode] = useState(false);
  
  const [members, setMembers] = useState<Character[]>(INITIAL_MEMBERS);
  const [conversations, setConversations] = useState<ChatConversation[]>(INITIAL_CONVERSATIONS);
  const [allMessages, setAllMessages] = useState<Record<string, Message[]>>(INITIAL_MESSAGES);
  const [unreadCounts, setUnreadCounts] = useState<Record<string, Record<string, number>>>(INITIAL_UNREAD);
  const [globalRolls, setGlobalRolls] = useState<DiceResult[]>([]);

  const processedMembers = useMemo(() => {
    return members.map(member => {
      const isOnline = member.name === username && isUserOnlineVisible;
      return { ...member, isOnline };
    });
  }, [members, username, isUserOnlineVisible]);

  const newsMentionsCount = useMemo(() => {
    if (!username || newsSeen[username]) return 0;
    return NEWS_ITEMS.filter(item => 
      item.content.toLowerCase().includes(username.toLowerCase())
    ).length;
  }, [username, newsSeen]);

  const stats = useMemo(() => {
    const chatCount = conversations.filter(c => c.initiatorName === username || c.participant.name === username).length;
    let messageCount = 0;
    let rollCount = globalRolls.length;

    Object.entries(allMessages).forEach(([cid, msgs]) => {
      const chat = conversations.find(c => c.id === cid);
      if (chat && (chat.initiatorName === username || chat.participant.name === username)) {
        messageCount += (msgs as Message[]).length;
      }
    });
    
    return { chatCount, messageCount, rollCount };
  }, [conversations, allMessages, globalRolls, username]);

  const totalUnread = useMemo(() => {
    if (!username || !unreadCounts[username]) return 0;
    return (Object.values(unreadCounts[username]) as number[]).reduce((a, b) => a + b, 0);
  }, [username, unreadCounts]);

  useEffect(() => {
    const timer = setTimeout(() => setIsInitialized(true), 100);
    return () => clearTimeout(timer);
  }, []);

  const handleLogin = (name: string) => {
    setUsername(name);
    setCurrentView('DASHBOARD');
  };

  const handleLogout = () => {
    setUsername('');
    setSelectedChatId(null);
    setCurrentView('LOGIN');
  };

  const navigate = (view: ViewType) => {
    if (view !== 'CHAT_DETAIL') {
      setSelectedChatId(null);
      setChatSourceIsGodMode(false);
    }
    if (view === 'NEWS' && username) {
      setNewsSeen(prev => ({ ...prev, [username]: true }));
    }
    setCurrentView(view);
  };

  const handleUpdateMemberImage = (newImageUrl: string) => {
    setMembers(prev => prev.map(m => 
      m.name === username ? { ...m, imageUrl: newImageUrl } : m
    ));
    setConversations(prev => prev.map(c => {
      if (c.participant.name === username) {
        return { ...c, participant: { ...c.participant, imageUrl: newImageUrl } };
      }
      return c;
    }));
  };

  const handleSelectChat = (chat: ChatConversation) => {
    if (username) {
      setUnreadCounts(prev => ({
        ...prev,
        [username]: { ...(prev[username] || {}), [chat.id]: 0 }
      }));
    }
    const exists = conversations.some(c => c.id === chat.id);
    if (!exists) {
      setConversations(prev => [chat, ...prev]);
      if (!allMessages[chat.id]) {
        setAllMessages(prev => ({ ...prev, [chat.id]: [] }));
      }
    }

    // Determine if we are entering this chat from God Mode
    if (currentView === 'GOD_MODE') {
      setChatSourceIsGodMode(true);
    } else {
      setChatSourceIsGodMode(false);
    }

    setSelectedChatId(chat.id);
    setCurrentView('CHAT_DETAIL');
  };

  const handleDeleteChat = (chatId: string) => {
    setConversations(prev => prev.filter(c => c.id !== chatId));
    setAllMessages(prev => {
      const next = { ...prev };
      delete next[chatId];
      return next;
    });
  };

  const handleNewMessage = (chatId: string, message: Message) => {
    const chat = conversations.find(c => c.id === chatId);
    if (!chat) return;
    let processedMessage = { ...message };
    if (username === 'NPC' && message.isMe && chat.npcMask) {
      processedMessage.sender = chat.npcMask;
    } else if (message.isMe) {
      processedMessage.sender = username;
    }
    
    if (processedMessage.diceResult) {
      setGlobalRolls(prev => [processedMessage.diceResult!, ...prev]);
    }

    setAllMessages(prev => ({ ...prev, [chatId]: [...(prev[chatId] || []), processedMessage] }));
    setConversations(prev => prev.map(c => c.id === chatId ? { ...c, lastMessage: processedMessage.text || (processedMessage.diceResult ? 'Würfelwurf' : 'Bild'), time: processedMessage.timestamp } : c));
    
    const recipientName = message.isMe ? (chat.participant.name === username ? chat.initiatorName : chat.participant.name) : username;
    if (message.isMe) {
      setUnreadCounts(prev => ({ ...prev, [recipientName]: { ...(prev[recipientName] || {}), [chatId]: (prev[recipientName]?.[chatId] || 0) + 1 } }));
    }
  };

  const activeChat = useMemo(() => conversations.find(c => c.id === selectedChatId), [conversations, selectedChatId]);
  
  const filteredConversations = useMemo(() => {
    if (currentView === 'GOD_MODE' && username === 'NPC') return conversations;
    return conversations.filter(c => c.initiatorName === username || c.participant.name === username);
  }, [conversations, username, currentView]);

  const getUnreadForChat = (chatId: string) => unreadCounts[username]?.[chatId] || 0;

  if (!isInitialized) return <div className="min-h-screen bg-background-light"></div>;

  return (
    <div className="max-w-md mx-auto min-h-screen relative shadow-2xl bg-background-light overflow-x-hidden font-sans text-black text-shadow-none">
      {currentView === 'LOGIN' && <LoginView onLogin={handleLogin} correctPasswordHash={password} />}
      {currentView === 'DASHBOARD' && (
        <DashboardView 
          username={username} 
          onNavigate={navigate} 
          totalUnread={totalUnread} 
          members={members} 
          newsMentions={newsMentionsCount}
        />
      )}
      {currentView === 'GOD_MODE' && username === 'NPC' && (
        <GodModeView 
          onNavigate={navigate} 
          conversations={conversations} 
          onSelectChat={handleSelectChat} 
          processedMembers={processedMembers} 
        />
      )}
      {currentView === 'GUILD' && <GuildView onNavigate={navigate} totalUnread={totalUnread} />}
      {currentView === 'NEWS' && <NewsView onNavigate={navigate} totalUnread={totalUnread} newsItems={NEWS_ITEMS} />}
      {currentView === 'DICE_TRAY' && <DiceTrayView onNavigate={navigate} rolls={globalRolls} onNewRoll={(r) => setGlobalRolls(p => [r, ...p])} totalUnread={totalUnread} />}
      {currentView === 'PROFILE' && (
        <ProfileView onNavigate={navigate} onLogout={handleLogout} stats={stats} username={username} totalUnread={totalUnread} members={members} onUpdateImage={handleUpdateMemberImage} />
      )}
      {currentView === 'NOTIFICATION_SETTINGS' && <NotificationSettingsView onNavigate={navigate} totalUnread={totalUnread} />}
      {currentView === 'SECURITY_SETTINGS' && <SecuritySettingsView onNavigate={navigate} onLogout={handleLogout} totalUnread={totalUnread} />}
      {currentView === 'PRIVACY_SETTINGS' && <PrivacySettingsView onNavigate={navigate} isOnlineVisible={isUserOnlineVisible} onToggleOnlineVisibility={setIsUserOnlineVisible} totalUnread={totalUnread} />}
      {currentView === 'CHANGE_PASSWORD' && <ChangePasswordView onNavigate={navigate} currentPasswordHash={password} onChangePassword={(p) => setPassword(p)} />}
      {currentView === 'CHAT_LIST' && <ChatListView onNavigate={navigate} onDeleteChat={handleDeleteChat} onSelectChat={handleSelectChat} isUserOnlineVisible={isUserOnlineVisible} conversations={filteredConversations.map(c => ({...c, unread: getUnreadForChat(c.id)}))} currentUser={username} totalUnread={totalUnread} processedMembers={processedMembers} />}
      {currentView === 'CHAT_DETAIL' && activeChat && (
        <ChatDetailView 
          chat={activeChat} 
          messages={allMessages[activeChat.id] || []} 
          onSendMessage={(msg) => handleNewMessage(activeChat.id, msg)} 
          onNavigate={navigate} 
          currentUser={username} 
          processedMembers={processedMembers}
          isPeeking={chatSourceIsGodMode}
        />
      )}
      {currentView === 'NEW_CHAT' && <NewChatView onNavigate={navigate} currentUser={username} processedMembers={processedMembers} onStartChat={(member, title, npcIdentity) => {
        const chatId = `c-new-${member.id}-${Date.now()}`;
        handleSelectChat({ id: chatId, participant: member, initiatorName: username, title, lastMessage: 'Ein neuer Brief wurde verfasst...', time: 'Jetzt', npcMask: npcIdentity });
      }} />}
    </div>
  );
};

export default App;
