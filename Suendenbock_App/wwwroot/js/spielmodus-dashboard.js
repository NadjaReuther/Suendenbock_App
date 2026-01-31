let characters = [];
let restFoods = [];
let isCombatPrimed = false;
let isCampConfirming = false;
let selectedFoodId = null;
let IS_GOTT_ROLE = false; // Wird vom inline script überschrieben
let isGodNightRestActive = false; // Tracks ob Gott das Night-Rest-Modal gerade offen hat

function initDashboard(initialCharacters, initialRestFoods, isGod) {
    characters = initialCharacters;
    restFoods = initialRestFoods;
    IS_GOTT_ROLE = isGod;

    updateStatusDisplay();
    loadPokusFromLocalStorage();
    updateRestButtonVisibility();

    // SignalR Event-Listener registrieren
    setupNightRestListeners();

    // Prüfe ob bereits eine Nachtlager-Anfrage offen ist (nur für Gott)
    if (IS_GOTT_ROLE) {
        checkForPendingNightRestRequest();
    }
}

// ===== PRÜFE AUF OFFENE NACHTLAGER-ANFRAGE =====

async function checkForPendingNightRestRequest() {
    const actId = window.globalSignalR?.getActId();
    if (!actId) return;

    try {
        const response = await fetch(`/api/night-rest/pending/${actId}`);

        if (response.ok) {
            const data = await response.json();
            console.log('[checkForPendingNightRestRequest] Found pending request:', data);

            // Zeige Modal mit kleiner Verzögerung
            setTimeout(() => {
                showGodNightRestModal(data);
            }, 1000);
        }
    } catch (error) {
        // Keine Anfrage vorhanden - kein Problem
    }
}

// ===== SIGNALR EVENT LISTENERS =====

function setupNightRestListeners() {
    // Event: Night Rest Requested (nur für Gott)
    window.addEventListener('nightRestRequested', async (event) => {
        if (!IS_GOTT_ROLE) {
            console.log('[nightRestRequested] User is not God, ignoring');
            return;
        }

        const data = event.detail;
        await showGodNightRestModal(data);
    });

    // Event: Night Rest Completed (für alle Spieler)
    window.addEventListener('nightRestCompleted', async (event) => {
        const data = event.detail;

        // Schließe Warte-Dialog
        Swal.close();

        // Baue Pro-Charakter Pokus-Übersicht
        let charactersHtml = '';
        if (data.characters && data.characters.length > 0) {
            charactersHtml = '<ul style="list-style: none; padding: 0; text-align: left;">';
            data.characters.forEach(char => {
                const extraPart = char.extraPokus > 0 ? ` + ${char.extraPokus} Extra` : '';
                charactersHtml += `
                    <li style="margin-bottom: 0.4rem; padding: 0.35rem 0.5rem; background: rgba(255,255,255,0.05); border-radius: 4px; font-size: 0.9rem;">
                        <strong>${char.name}:</strong> ${char.spellCount} Zauber${extraPart} = <strong>${char.totalPokus} Pokuspunkte</strong>
                    </li>
                `;
            });
            charactersHtml += '</ul>';
        }

        // Zeige Ergebnis
        await Swal.fire({
            icon: 'success',
            title: 'Nachtlager abgeschlossen',
            html: `
                <div style="text-align: left; margin: 1rem 0;">
                    <p><strong>Nahrung:</strong> ${data.foodName} (+${data.healthBonus} LP)</p>
                    <hr style="border-color: rgba(255,255,255,0.15); margin: 0.6rem 0;">
                    <p style="margin-bottom: 0.4rem;"><strong>Pokuspunkte:</strong></p>
                    ${charactersHtml}
                </div>
            `,
            confirmButtonColor: '#d97706',
            confirmButtonText: 'Verstanden'
        });

        // Seite neu laden
        setTimeout(() => window.location.reload(), 1000);
    });

    // Event: Night Rest Cancelled (für beide Seiten)
    window.addEventListener('nightRestCancelled', async (event) => {
        if (IS_GOTT_ROLE) {
            // Gott: Modal schließen wenn offen (ohne erneut CancelNightRest aufzurufen)
            if (isGodNightRestActive) {
                isGodNightRestActive = false;
                document.getElementById('campModal').style.display = 'none';
            }
        } else {
            // Spieler: Warte-Dialog schließen und Benachrichtigung zeigen
            Swal.close();
            await Swal.fire({
                icon: 'info',
                title: 'Nachtlager abgebrochen',
                text: 'Die Nachtlager-Anfrage wurde abgebrochen.',
                confirmButtonColor: '#d97706',
                confirmButtonText: 'OK'
            });
        }
    });
}

// ===== GOTT: NACHTLAGER MODAL =====

async function showGodNightRestModal(data) {
    // Fülle Pokus-Grid mit Extra-Pokus Eingabe pro Charakter
    const pokusGrid = document.getElementById('pokusGrid');
    pokusGrid.innerHTML = '';

    data.characters.forEach(char => {
        if (char.isCompanion) return;

        const wrapper = document.createElement('div');
        wrapper.innerHTML = `
            <div class="pokus-item">
                <span class="name">${char.name}</span>
                <div class="value">
                    <span class="number">${char.currentPokus}</span>
                    <span class="material-symbols-outlined icon">auto_fix_high</span>
                </div>
            </div>
            <div style="display: flex; align-items: center; gap: 0.4rem; margin-top: 0.3rem; justify-content: flex-end;">
                <span style="font-size: 0.75rem; opacity: 0.6;">+ Extra:</span>
                <input type="number" class="extra-pokus-input" data-char-id="${char.id}" value="0" min="0"
                       style="width: 48px; padding: 0.15rem 0.25rem; background: rgba(255,255,255,0.08); border: 1px solid rgba(255,255,255,0.2); border-radius: 4px; color: inherit; text-align: center; font-size: 0.85rem; -moz-appearance: textfield;"
                       oninput="this.value = this.value.replace(/[^0-9]/g, '')">
            </div>
        `;
        pokusGrid.appendChild(wrapper);
    });

    // Zeige Food-Section und fülle Nahrungsoptionen als Combobox
    const foodSection = document.querySelector('.food-section');
    if (foodSection) foodSection.style.display = 'block';

    const foodOptions = document.getElementById('foodOptions');
    let selectHtml = '<select id="god-food-select" style="width: 100%; padding: 0.5rem 0.6rem; background: rgba(255,255,255,0.08); border: 1px solid rgba(255,255,255,0.2); border-radius: 6px; color: inherit; font-size: 0.9rem; cursor: pointer;">';
    restFoods.forEach(food => {
        selectHtml += `<option value="${food.id}">${food.name} (+${food.healthBonus} LP)</option>`;
    });
    selectHtml += '</select>';
    foodOptions.innerHTML = selectHtml;

    // Setze Buttons
    const modalButtons = document.querySelector('.modal-buttons');
    modalButtons.innerHTML = `
        <button class="modal-button-primary" onclick="confirmGodNightRest()">
            Nachtlager gewähren
        </button>
        <button class="modal-button-secondary" onclick="closeCampModal()">
            Abbrechen
        </button>
    `;

    isGodNightRestActive = true;
    document.getElementById('campModal').style.display = 'flex';
}

async function confirmGodNightRest() {
    const foodSelect = document.getElementById('god-food-select');
    if (!foodSelect) return;
    const foodId = parseInt(foodSelect.value);
    if (!foodId) return;

    // Sammle Extra-Pokus pro Charakter
    const extraPokusPerCharacter = {};
    document.querySelectorAll('.extra-pokus-input').forEach(input => {
        extraPokusPerCharacter[input.dataset.charId] = parseInt(input.value) || 0;
    });

    isGodNightRestActive = false;
    closeCampModal();
    await applyNightRest(foodId, extraPokusPerCharacter);
}

function updateStatusDisplay() {
    if (characters.length === 0) return;

    // Berechne Durchschnittswerte
    const totalHealth = characters.reduce((sum, c) => sum + c.healthPercent, 0);
    const avgHealth = Math.round(totalHealth / characters.length);

    // Gesamt-Pokus
    const totalPokus = characters.reduce((sum, c) => sum + getPokusForCharacter(c.id), 0);

    // Update UI (nur wenn Elemente existieren)
    const healthElement = document.getElementById('healthPercent');
    const pokusElement = document.getElementById('generatedPokus');

    if (healthElement) {
        healthElement.textContent = avgHealth + '%';
    }
    if (pokusElement) {
        pokusElement.textContent = totalPokus;
    }
}

// ===== REST BUTTONS SICHTBARKEIT =====

function updateRestButtonVisibility() {
    // Gott sieht diese Buttons gar nicht (wird bereits server-seitig gefiltert)
    if (IS_GOTT_ROLE) return;

    const restButtons = document.getElementById('restButtons');
    if (!restButtons) return;

    // Zeige Buttons nur wenn mindestens ein Spieler beschädigt ist oder Zauber gewirkt hat
    const needsRest = characters.some(c => c.currentHealth < c.baseMaxHealth || c.currentPokus > 0);
    restButtons.style.display = needsRest ? 'flex' : 'none';
}

// ===== COMBAT BUTTON =====

function goToCombatSetup() {
    window.location.href = '/Spielmodus/CombatSetup';
}
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

    setTimeout(async () => {
        hideLoading();

        await Swal.fire({
            icon: 'info',
            title: '✨ Magie-Slots',
            html: '<div style="font-size: 1.2rem; margin: 1rem 0;">Setzt bitte jetzt eure Slots um die Hälfte zurück.</div>',
            confirmButtonColor: '#d97706',
            confirmButtonText: 'Verstanden'
        });

        window.location.reload();
    }, 1500);
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

async function openCampModal() {
    resetCampButton();

    if (IS_GOTT_ROLE) {
        // Gott öffnet NICHT das Modal direkt, sondern empfängt Requests via SignalR
        await Swal.fire({
            icon: 'info',
            title: 'Nachtlager',
            text: 'Als Gott empfängst du Nachtlager-Anfragen von Spielern. Warte auf eine Anfrage.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    // SPIELER: Zeige Pokus-Übersicht und sende Request
    const pokusGrid = document.getElementById('pokusGrid');
    pokusGrid.innerHTML = '';
    
    characters.forEach(char => {
        const pokus = char.currentPokus || 0; // Aus DB-Daten
        
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

    // Verstecke Food-Section für Spieler
    const foodSection = document.querySelector('.food-section');
    if (foodSection) {
        foodSection.style.display = 'none';
    }

    // Ändere Button-Text
    const modalButtons = document.querySelector('.modal-buttons');
    modalButtons.innerHTML = `
        <button class="modal-button-primary" onclick="requestNightRest()">
            Nachtlager anfordern
        </button>
        <button class="modal-button-secondary" onclick="closeCampModal()">
            Abbrechen
        </button>
    `;
    document.getElementById('campModal').style.display = 'flex';
}

function closeCampModal() {
    // Wenn Gott das Night-Rest-Modal schließt → Anfrage abbrechen
    if (isGodNightRestActive) {
        isGodNightRestActive = false;
        cancelNightRest();
    }

    document.getElementById('campModal').style.display = 'none';
    isCampConfirming = false;
    resetCampButton();
}

// Nachtlager-Anfrage abbrechen (von Gott oder Spieler)
async function cancelNightRest() {
    const connection = window.globalSignalR?.connection;
    const actId = window.globalSignalR?.getActId();

    if (connection && actId && connection.state === 'Connected') {
        try {
            await connection.invoke("CancelNightRest", actId);
        } catch (error) {
            console.error('[cancelNightRest] Error:', error);
        }
    }
}

// SPIELER: Nachtlager anfordern (via SignalR)
async function requestNightRest() {
    closeCampModal();

    const connection = window.globalSignalR?.connection;
    const actId = window.globalSignalR?.getActId();
    const userName = window.globalSignalR?.getUserName();

    if (!connection) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'SignalR-Verbindung nicht gefunden! Bitte Seite neu laden.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    if (!actId) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Keine Act-ID gefunden! Bitte Seite neu laden.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    if (connection.state !== 'Connected') {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'SignalR ist nicht verbunden. Bitte warten oder Seite neu laden.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    try {
        // Sende Request an Gott via SignalR
        await connection.invoke("RequestNightRest", actId, userName || "Spieler");
    
        // Zeige Warte-Nachricht mit Abbrechen-Option
        const { isConfirmed } = await Swal.fire({
            icon: 'info',
            title: 'Nachtlager angefordert',
            html: '⏳ Warte auf die Wahl des Gottes...',
            showConfirmButton: true,
            confirmButtonText: 'Abbrechen',
            confirmButtonColor: '#6b7280',
            allowOutsideClick: false
        });

        // Wenn Spieler aktiv auf Abbrechen klickt
        if (isConfirmed) {
            await cancelNightRest();
        }

    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Fehler beim Senden der Anfrage!',
            confirmButtonColor: '#d97706'
        });
    }
}

// GOTT: Nachtlager anwenden (via SignalR)
async function applyNightRest(foodId, extraPokusPerCharacter) {
    const connection = window.globalSignalR?.connection;
    const actId = window.globalSignalR?.getActId();

    if (!connection || !actId) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Keine Verbindung zum Server!',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    try {
        await connection.invoke("ApplyNightRest", actId, foodId, extraPokusPerCharacter);

        await Swal.fire({
            icon: 'success',
            title: 'Nachtlager durchgeführt',
            text: 'Die Gruppe wurde geheilt!',
            confirmButtonColor: '#d97706'
        });

        setTimeout(() => window.location.reload(), 1500);

    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Fehler beim Anwenden des Nachtlagers!',
            confirmButtonColor: '#d97706'
        });
    }
}

// ===== POKUS LOCAL STORAGE =====

function getPokusForCharacter(characterId) {
    const pokus = JSON.parse(localStorage.getItem('characterPokus') || '{}');
    return pokus[characterId] || 0;
}

function loadPokusFromLocalStorage() {
    const pokus = JSON.parse(localStorage.getItem('characterPokus') || '{}');
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