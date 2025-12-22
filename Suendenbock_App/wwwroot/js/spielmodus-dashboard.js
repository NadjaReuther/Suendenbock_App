let characters = [];
let restFoods = [];
let isCombatPrimed = false;
let isCampConfirming = false;
let selectedFoodId = null;

function initDashboard(initialCharacters, initialRestFoods) {
    characters = initialCharacters;
    restFoods = initialRestFoods;
    console.log('Dashboard initialisiert:', { characters, restFoods });
    updateStatusDisplay();
    loadPokusFromLocalStorage();
}

function updateStatusDisplay() {
    if (characters.length === 0) return;

    // Berechne Durchschnittswerte
    const totalHealth = characters.reduce((sum, c) => sum + c.healthPercent, 0);
    const avgHealth = Math.round(totalHealth / characters.length);

    // Gesamt-Pokus
    const totalPokus = characters.reduce((sum, c) => sum + getPokusForCharacter(c.id), 0);

    // Update UI
    document.getElementById('healthPercent').textContent = avgHealth + '%';
    document.getElementById('generatedPokus').textContent = totalPokus;
}

function navigateTo(page) {
    const routes = {
        'map': '/Spielmodus/Map',
        'quests': '/Spielmodus/Quests',
        'trophies': '/Spielmodus/Trophies'
    };
    if (routes[page]) window.location.href = routes[page];
}

// ===== COMBAT BUTTON =====

function toggleCombatReady() {
    const btn = document.getElementById('combatBtn');
    if (!isCombatPrimed) {
        isCombatPrimed = true;
        btn.classList.add('primed');
        document.getElementById('combatText').textContent = 'Bereit zum Kampf';
        setTimeout(() => { if (isCombatPrimed) resetCombatButton(); }, 5000);
    } else {
        window.location.href = '/Spielmodus/Battle';
    }
}

function resetCombatButton() {
    isCombatPrimed = false;
    const btn = document.getElementById('combatBtn');
    btn.classList.remove('primed');
    document.getElementById('combatText').textContent = 'Kampfbereit Machen';
}

// ===== SHORT REST =====

async function shortRest() {
    showLoading();
    try {
        const response = await fetch('/api/game/short-rest', { method: 'POST' });
        if (response.ok) setTimeout(() => window.location.reload(), 1500);
        else { hideLoading(); alert('Fehler bei der Rast!'); }
    } catch (error) {
        console.error('Fehler:', error);
        hideLoading();
        alert('Fehler bei der Rast!');
    }
}

// ===== CAMP =====

function toggleCamp() {
    const btn = document.getElementById('campBtn');
    const icon = document.getElementById('campIcon');
    const text = document.getElementById('campText');
    const confirmText = document.getElementById('confirmText');

    if (!isCampConfirming) {
        isCampConfirming = true;
        btn.classList.add('confirming');
        icon.textContent = 'bedtime';
        text.textContent = 'Gute Nacht';
        if (confirmText) confirmText.style.display = 'block';
        setTimeout(() => { if (isCampConfirming) resetCampButton(); }, 10000);
    } else {
        openCampModal();
    }
}

function resetCampButton() {
    isCampConfirming = false;
    const btn = document.getElementById('campBtn');
    btn.classList.remove('confirming');
    document.getElementById('campIcon').textContent = 'fireplace';
    document.getElementById('campText').textContent = 'Nachtlager';
    const confirmText = document.getElementById('confirmText');
    if (confirmText) confirmText.style.display = 'none';
}

function openCampModal() {
    resetCampButton();
    const pokusGrid = document.getElementById('pokusGrid');
    pokusGrid.innerHTML = '';
    let totalPokus = 0;

    characters.forEach(char => {
        const pokus = getPokusForCharacter(char.id);
        totalPokus += pokus;

        const item = document.createElement('div');
        item.className = 'pokus-item';
        item.innerHTML = `
            <span class="name">${char.name}</span>
            <div class="value">
                <span class="number">${pokus}</span>
                <span class="material-symbols-outlined icon">auto_fix_high</span>
            </div>
        `;
        pokusGrid.appendChild(item);
    });

    document.getElementById('totalPokus').textContent = totalPokus;

    const foodOptions = document.getElementById('foodOptions');
    foodOptions.innerHTML = '';

    restFoods.forEach(food => {
        const option = document.createElement('div');
        option.className = 'food-option';
        option.innerHTML = `
            <div class="food-option-content">
                <div class="food-info">
                    <div class="name">${food.name}</div>
                    <div class="description">${food.description}</div>
                    <div class="bonus">+${food.healthBonus} Gesundheit</div>
                </div>
                <input type="radio" name="food" value="${food.id}">
            </div>
        `;

        option.addEventListener('click', () => {
            document.querySelectorAll('.food-option').forEach(opt => {
                opt.classList.remove('selected');
            });
            option.classList.add('selected');
            option.querySelector('input').checked = true;
            selectedFoodId = food.id;
        });

        foodOptions.appendChild(option);
    });

    document.getElementById('campModal').style.display = 'flex';
}

function closeCampModal() {
    document.getElementById('campModal').style.display = 'none';
    isCampConfirming = false;
    resetCampButton();
}

async function startRest() {
    if (!selectedFoodId) { alert('Bitte wähle zuerst ein Mahl aus!'); return; }
    closeCampModal();
    showLoading();
    try {
        const response = await fetch('/api/game/night-rest', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ characterIds: characters.map(c => c.id), foodId: selectedFoodId })
        });
        if (response.ok) {
            clearPokusInLocalStorage();
            setTimeout(() => window.location.reload(), 1500);
        } else { hideLoading(); alert('Fehler beim Nachtlager!'); }
    } catch (error) {
        console.error('Fehler:', error);
        hideLoading();
        alert('Fehler beim Nachtlager!');
    }
}

// ===== POKUS LOCAL STORAGE =====

function getPokusForCharacter(characterId) {
    const pokus = JSON.parse(localStorage.getItem('characterPokus') || '{}');
    return pokus[characterId] || 0;
}

function loadPokusFromLocalStorage() {
    const pokus = JSON.parse(localStorage.getItem('characterPokus') || '{}');
    console.log('Geladene Pokus-Werte:', pokus);
}

function clearPokusInLocalStorage() {
    localStorage.setItem('characterPokus', JSON.stringify({}));
}

// ===== LOADING =====

function showLoading() {
    document.getElementById('loadingOverlay').style.display = 'flex';
}

function hideLoading() {
    document.getElementById('loadingOverlay').style.display = 'none';
}

// ===== EXPORT =====

window.SpielmodusDashboard = {
    incrementPokus: (characterId) => {
        const pokus = JSON.parse(localStorage.getItem('characterPokus') || '{}');
        pokus[characterId] = (pokus[characterId] || 0) + 1;
        localStorage.setItem('characterPokus', JSON.stringify(pokus));
    },
    getPokusForCharacter,
    characters
};