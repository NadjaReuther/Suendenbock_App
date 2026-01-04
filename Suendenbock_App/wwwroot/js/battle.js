// wwwroot/js/battle.js - PART 1: SETUP & SELECTION

// ===== GLOBAL STATE =====
let selectedMonster = null;
let selectedCharacters = [];
let combatState = {
    monster: null,
    party: [],
    currentTurn: 'player', // 'player' oder 'monster'
    roundNumber: 1,
    isAnimating: false
};

// ===== INITIALIZATION =====
document.addEventListener('DOMContentLoaded', () => {
    console.log('Battle initialized');
});

// ===== SCREEN NAVIGATION =====

function showScreen(screenId) {
    document.querySelectorAll('.battle-screen').forEach(screen => {
        screen.classList.remove('active');
    });
    document.getElementById(screenId).classList.add('active');
}

// ===== MONSTER AUSWAHL =====

function selectMonster(element) {
    // Alte Auswahl entfernen
    document.querySelectorAll('.monster-card').forEach(card => {
        card.classList.remove('selected');
    });

    // Neue Auswahl
    element.classList.add('selected');

    selectedMonster = {
        id: parseInt(element.dataset.monsterId),
        name: element.dataset.monsterName,
        health: parseInt(element.dataset.monsterHealth),
        maxHealth: parseInt(element.dataset.monsterHealth),
        attack: parseInt(element.dataset.monsterAttack),
        defense: parseInt(element.dataset.monsterDefense),
        imageUrl: element.dataset.monsterImage
    };

    console.log('Monster ausgewählt:', selectedMonster);

    // Gehe zur Character-Auswahl
    setTimeout(() => {
        updateMonsterPreview();
        showScreen('partySelectScreen');
    }, 300);
}

function updateMonsterPreview() {
    if (!selectedMonster) return;

    document.getElementById('previewMonsterImage').src = selectedMonster.imageUrl;
    document.getElementById('previewMonsterName').textContent = selectedMonster.name;
    document.getElementById('previewMonsterStats').innerHTML = `
        <span>${selectedMonster.health} HP</span> •
        <span>${selectedMonster.attack} ATK</span> •
        <span>${selectedMonster.defense} DEF</span>
    `;
}

function backToMonsterSelect() {
    selectedMonster = null;
    selectedCharacters = [];
    showScreen('monsterSelectScreen');
}

// ===== CHARACTER AUSWAHL =====

function toggleCharacter(element) {
    // Tote Characters können nicht ausgewählt werden
    if (element.classList.contains('dead')) {
        return;
    }

    const characterId = parseInt(element.dataset.characterId);
    const isSelected = element.classList.contains('selected');

    if (isSelected) {
        // Abwählen
        element.classList.remove('selected');
        selectedCharacters = selectedCharacters.filter(c => c.id !== characterId);
    } else {
        // Auswählen (max 4)
        if (selectedCharacters.length >= 4) {
            alert('Du kannst maximal 4 Helden auswählen!');
            return;
        }

        element.classList.add('selected');
        selectedCharacters.push({
            id: characterId,
            name: element.dataset.characterName,
            health: parseInt(element.dataset.characterHealth),
            maxHealth: parseInt(element.dataset.characterMaxHealth)
        });
    }

    console.log('Ausgewählte Characters:', selectedCharacters);

    // Start-Button aktivieren/deaktivieren
    const startBtn = document.getElementById('startBattleBtn');
    startBtn.disabled = selectedCharacters.length === 0;
}

// ===== KAMPF STARTEN =====

function startBattle() {
    if (selectedCharacters.length === 0) {
        alert('Wähle mindestens einen Helden!');
        return;
    }

    // Combat State initialisieren
    combatState.monster = { ...selectedMonster };
    combatState.party = selectedCharacters.map(c => ({ ...c }));
    combatState.currentTurn = 'player';
    combatState.roundNumber = 1;

    console.log('Kampf beginnt!', combatState);

    // Combat Screen aufbauen
    buildCombatScreen();
    showScreen('combatScreen');
}
// wwwroot/js/battle.js - PART 2: COMBAT

// ===== COMBAT SCREEN AUFBAUEN =====

function buildCombatScreen() {
    const screen = document.getElementById('combatScreen');

    screen.innerHTML = `
        <div class="combat-layout">
            <!-- Monster Seite -->
            <div class="combat-enemy-side">
                <div class="enemy-container">
                    <img src="${combatState.monster.imageUrl}" 
                         alt="${combatState.monster.name}" 
                         class="enemy-sprite" 
                         id="enemySprite" />
                    
                    <div class="enemy-info">
                        <h2 class="enemy-name">${combatState.monster.name}</h2>
                        <div class="hp-bar-container">
                            <div class="hp-bar">
                                <div class="hp-bar-fill enemy-hp" 
                                     id="enemyHpBar"
                                     style="width: 100%"></div>
                            </div>
                            <span class="hp-text" id="enemyHpText">
                                ${combatState.monster.health} / ${combatState.monster.maxHealth} HP
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Party Seite -->
            <div class="combat-party-side">
                <div class="party-container">
                    ${combatState.party.map((char, index) => `
                        <div class="party-member" id="character-${char.id}" data-index="${index}">
                            <h3 class="party-member-name">${char.name}</h3>
                            <div class="hp-bar-container">
                                <div class="hp-bar">
                                    <div class="hp-bar-fill party-hp" 
                                         id="charHp-${char.id}"
                                         style="width: ${(char.health / char.maxHealth * 100).toFixed(2)}%"></div>
                                </div>
                                <span class="hp-text" id="charHpText-${char.id}">
                                    ${char.health} / ${char.maxHealth} HP
                                </span>
                            </div>
                        </div>
                    `).join('')}
                </div>
            </div>
            
            <!-- Combat Log -->
            <div class="combat-log" id="combatLog">
                <div class="log-entry">⚔️ Runde ${combatState.roundNumber} beginnt!</div>
            </div>
            
            <!-- Action Bar -->
            <div class="action-bar" id="actionBar">
                <div class="action-buttons">
                    ${combatState.party.map((char, index) => `
                        <button class="action-btn" 
                                data-character-index="${index}"
                                onclick="playerAttack(${index})">
                            <span class="material-symbols-outlined">swords</span>
                            <span>${char.name} greift an!</span>
                        </button>
                    `).join('')}
                </div>
                
                <button class="action-btn action-flee" onclick="fleeBattle()">
                    <span class="material-symbols-outlined">directions_run</span>
                    <span>Fliehen</span>
                </button>
            </div>
        </div>
    `;
}

// ===== SPIELER ANGRIFF =====

async function playerAttack(characterIndex) {
    if (combatState.isAnimating) return;
    if (combatState.currentTurn !== 'player') return;

    const character = combatState.party[characterIndex];
    if (character.health <= 0) {
        addLogEntry(`${character.name} ist bewusstlos!`);
        return;
    }

    combatState.isAnimating = true;
    disableActions();

    // Schaden berechnen (einfache Formel)
    const baseDamage = Math.floor(Math.random() * 10) + 5; // 5-15
    const defense = combatState.monster.defense || 0;
    const damage = Math.max(1, baseDamage - Math.floor(defense / 2));

    // Log
    addLogEntry(`⚔️ ${character.name} greift an!`);

    // Animation
    await animateAttack('party');

    // Schaden anwenden
    combatState.monster.health = Math.max(0, combatState.monster.health - damage);
    updateEnemyHP();

    addLogEntry(`💥 ${combatState.monster.name} erleidet ${damage} Schaden!`);

    // Sieg-Check
    if (combatState.monster.health <= 0) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        victory();
        return;
    }

    // Monster ist dran
    await new Promise(resolve => setTimeout(resolve, 1000));
    combatState.currentTurn = 'monster';
    monsterTurn();
}

// ===== MONSTER ZUG =====

async function monsterTurn() {
    if (combatState.isAnimating) return;

    combatState.isAnimating = true;

    // Zufälliges lebendes Ziel
    const aliveCharacters = combatState.party.filter(c => c.health > 0);
    if (aliveCharacters.length === 0) {
        defeat();
        return;
    }

    const target = aliveCharacters[Math.floor(Math.random() * aliveCharacters.length)];
    const targetIndex = combatState.party.findIndex(c => c.id === target.id);

    // Schaden berechnen
    const baseDamage = combatState.monster.attack || 10;
    const damage = Math.floor(baseDamage * (0.8 + Math.random() * 0.4)); // ±20%

    // Log
    addLogEntry(`👹 ${combatState.monster.name} greift ${target.name} an!`);

    // Animation
    await animateAttack('enemy');

    // Schaden anwenden
    target.health = Math.max(0, target.health - damage);
    combatState.party[targetIndex] = target;
    updateCharacterHP(target.id);

    addLogEntry(`💔 ${target.name} erleidet ${damage} Schaden!`);

    // Niederlage-Check
    const anyAlive = combatState.party.some(c => c.health > 0);
    if (!anyAlive) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        defeat();
        return;
    }

    // Nächste Runde
    await new Promise(resolve => setTimeout(resolve, 1500));
    nextRound();
}

// ===== HP UPDATES =====

function updateEnemyHP() {
    const percent = (combatState.monster.health / combatState.monster.maxHealth * 100);
    document.getElementById('enemyHpBar').style.width = percent + '%';
    document.getElementById('enemyHpText').textContent =
        `${combatState.monster.health} / ${combatState.monster.maxHealth} HP`;
}

function updateCharacterHP(characterId) {
    const char = combatState.party.find(c => c.id === characterId);
    if (!char) return;

    const percent = (char.health / char.maxHealth * 100);
    const hpBar = document.getElementById(`charHp-${characterId}`);
    const hpText = document.getElementById(`charHpText-${characterId}`);

    if (hpBar) hpBar.style.width = percent + '%';
    if (hpText) hpText.textContent = `${char.health} / ${char.maxHealth} HP`;

    // Tote markieren
    const memberDiv = document.getElementById(`character-${characterId}`);
    if (char.health <= 0) {
        memberDiv?.classList.add('dead');
    }
}

// ===== ANIMATIONEN =====

async function animateAttack(attacker) {
    const sprite = attacker === 'party'
        ? document.querySelector('.party-container')
        : document.getElementById('enemySprite');

    if (!sprite) return;

    sprite.classList.add('attacking');
    await new Promise(resolve => setTimeout(resolve, 300));
    sprite.classList.remove('attacking');
}

// ===== COMBAT LOG =====

function addLogEntry(message) {
    const log = document.getElementById('combatLog');
    if (!log) return;

    const entry = document.createElement('div');
    entry.className = 'log-entry new';
    entry.textContent = message;
    log.appendChild(entry);

    // Auto-scroll
    log.scrollTop = log.scrollHeight;

    // Animation entfernen
    setTimeout(() => entry.classList.remove('new'), 100);
}

// ===== NÄCHSTE RUNDE =====

function nextRound() {
    combatState.roundNumber++;
    combatState.currentTurn = 'player';
    combatState.isAnimating = false;

    addLogEntry(`⚔️ Runde ${combatState.roundNumber}!`);
    enableActions();
}

// ===== UI HELPERS =====

function disableActions() {
    document.querySelectorAll('.action-btn').forEach(btn => {
        btn.disabled = true;
    });
}

function enableActions() {
    document.querySelectorAll('.action-btn').forEach(btn => {
        btn.disabled = false;
    });
}

// ===== FLIEHEN =====

function fleeBattle() {
    if (!confirm('Willst du wirklich fliehen?')) return;

    showResult('fled', 'Flucht!', 'Die Gruppe zieht sich zurück...');
}

// ===== SIEG / NIEDERLAGE =====

function victory() {
    showResult('victory', 'Sieg!', `${combatState.monster.name} wurde besiegt!`);

    // Monster als "encounter" markieren (API-Call)
    markMonsterEncountered(combatState.monster.id);
}

function defeat() {
    showResult('defeat', 'Niederlage!', 'Die Gruppe wurde besiegt...');
}

function showResult(type, title, message) {
    const screen = document.getElementById('resultScreen');
    const icon = document.getElementById('resultIcon');
    const titleEl = document.getElementById('resultTitle');
    const messageEl = document.getElementById('resultMessage');

    // Icon
    const icons = {
        victory: '<span class="material-symbols-outlined">emoji_events</span>',
        defeat: '<span class="material-symbols-outlined">heart_broken</span>',
        fled: '<span class="material-symbols-outlined">directions_run</span>'
    };
    icon.innerHTML = icons[type] || '';
    icon.className = 'result-icon ' + type;

    // Text
    titleEl.textContent = title;
    messageEl.textContent = message;

    // Screen zeigen
    setTimeout(() => {
        showScreen('resultScreen');
    }, 1000);
}

// ===== API CALLS =====

async function markMonsterEncountered(monsterId) {
    try {
        // Hier könntest du einen API-Call machen um Monster.encounter = true zu setzen
        console.log('Monster encountered:', monsterId);
    } catch (error) {
        console.error('Fehler:', error);
    }
}
