// ===== BATTLE SYSTEM - CONFIGURATION =====

// ===== CONDITION CONFIGURATION =====
export const CONDITION_CONFIG = {
    'Vergiftet': {
        color: 'bg-green-900/60',
        border: 'border-green-500',
        text: 'text-green-200',
        description: 'Am Ende jedes Zuges erleidet der Charakter Schaden basierend auf der Stufe der Vergiftung. Heilung durch Antidot oder Medizin.'
    },
    'Brennend': {
        color: 'bg-orange-900/60',
        border: 'border-orange-500',
        text: 'text-orange-200',
        description: 'Der Charakter erleidet jede Runde Feuerschaden. Kann durch Wasser oder spezielle Aktionen gelöscht werden.'
    },
    'Liegend': {
        color: 'bg-amber-900/40',
        border: 'border-amber-700',
        text: 'text-amber-200',
        description: 'Der Charakter liegt am Boden. Angriffe gegen ihn haben Vorteil. Aufstehen kostet eine Bewegung.'
    },
    'Ohnmächtig': {
        color: 'bg-purple-950/60',
        border: 'border-purple-800',
        text: 'text-purple-300',
        description: 'Der Charakter ist bewusstlos und kann keine Aktionen ausführen. Automatisch liegend und wehrlos.'
    },
    'Verwirrt': {
        color: 'bg-fuchsia-900/40',
        border: 'border-fuchsia-700',
        text: 'text-fuchsia-200',
        description: 'Der Charakter hat Schwierigkeiten, Freund von Feind zu unterscheiden. Würfelt bei Aktionen für Ziel oder Richtung.'
    },
    'Erschöpft': {
        color: 'bg-zinc-700/40',
        border: 'border-zinc-500',
        text: 'text-zinc-300',
        description: 'Alle Aktionen haben Nachteil. Geschwindigkeit halbiert. Benötigt Rast zur Erholung.'
    },
    'Sensibel': {
        color: 'bg-cyan-900/40',
        border: 'border-cyan-700',
        text: 'text-cyan-200',
        description: 'Erhöhte Empfindlichkeit gegenüber Schaden. Erlittener Schaden wird erhöht.'
    },
    'Verängstigt': {
        color: 'bg-yellow-200/10',
        border: 'border-yellow-500/40',
        text: 'text-yellow-200',
        description: 'Der Charakter hat Nachteil auf Angriffs- und Attributswürfe, solange die Quelle der Angst sichtbar ist.'
    },
    'Blutend': {
        color: 'bg-red-700/40',
        border: 'border-red-500',
        text: 'text-red-200',
        description: 'Der Charakter verliert jede Runde HP durch Blutverlust. Kann durch Heilung oder Verbände gestoppt werden.'
    },
    'Verwundbar': {
        color: 'bg-orange-700/40',
        border: 'border-orange-500',
        text: 'text-orange-200',
        description: 'Rüstungsklasse verringert. Angreifer haben es leichter, kritische Treffer zu landen.'
    },
    'Übergebend': {
        color: 'bg-lime-900/40',
        border: 'border-lime-700',
        text: 'text-lime-200',
        description: 'Der Charakter erbricht sich wiederholt. Kann keine Konzentration aufrechterhalten. Zähler zeigt verbleibende Runden.'
    },
    'Ergriffen': {
        color: 'bg-stone-800/60',
        border: 'border-stone-600',
        text: 'text-stone-300',
        description: 'Der Charakter wird festgehalten und kann sich nicht bewegen. Angriffe und Würfe haben Nachteil.'
    },
    'Betrunken': {
        color: 'bg-amber-600/30',
        border: 'border-amber-500',
        text: 'text-amber-100',
        description: 'Beeinträchtigt Geschicklichkeit und Wahrnehmung. Würfe haben Nachteil, aber Charisma kann erhöht sein.'
    },
    'Rasend': {
        color: 'bg-red-950/60',
        border: 'border-red-600',
        text: 'text-red-400',
        description: 'Der Charakter greift unkontrolliert an. Erhöhter Schaden, aber verringerte Verteidigung und keine taktischen Manöver.'
    },
    'Pokus fokussiert': {
        color: 'bg-teal-900/40',
        border: 'border-teal-600',
        text: 'text-teal-200',
        description: 'Magische Konzentration erhöht. Zauber sind effektiver, aber Unterbrechung kann schädlich sein.'
    },
    'Unsichtbar': {
        color: 'bg-sky-900/30',
        border: 'border-sky-400',
        text: 'text-sky-200',
        description: 'Der Charakter ist unsichtbar. Angriffe gegen ihn haben Nachteil, seine Angriffe haben Vorteil.'
    },
    'Verflucht': {
        color: 'bg-indigo-950/60',
        border: 'border-indigo-700',
        text: 'text-indigo-300',
        description: 'Übernatürliche negative Effekte. Würfe haben Nachteil. Benötigt spezielle Magie zur Aufhebung.'
    },
    'Gesegnet': {
        color: 'bg-yellow-600/30',
        border: 'border-yellow-500',
        text: 'text-yellow-200',
        description: 'Göttliche Gunst gewährt Vorteil auf Rettungswürfe und zusätzlichen Schutz gegen negative Effekte.'
    },
    'Taktisch': {
        color: 'bg-blue-900/40',
        border: 'border-blue-600',
        text: 'text-blue-200',
        description: 'Erhöhtes taktisches Bewusstsein. Vorteil auf Initiative und Wahrnehmung. Kann Verbündeten helfen.'
    },
    'Heldenmut': {
        color: 'bg-white/10',
        border: 'border-white/60',
        text: 'text-white',
        description: 'Temporäre Tapferkeit gibt Immunität gegen Furcht und Bonus auf Angriffswürfe. Endet nach bestimmten Runden.'
    }
};

export const CONDITIONS = Object.keys(CONDITION_CONFIG);
export const SAVE_KEYS = ['Handeln', 'Wissen', 'Soziales'];

// ===== ROLE-BASED ACCESS CONTROL =====
// IS_GOTT wird von Battle.cshtml gesetzt
// Falls nicht gesetzt (alte Implementierung), default auf true für Kompatibilität
export const IS_GOTT_ROLE = typeof IS_GOTT !== 'undefined' ? IS_GOTT : true;
