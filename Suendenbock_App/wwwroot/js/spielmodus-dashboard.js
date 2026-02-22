let characters = [];
let restFoods = [];
let isCombatPrimed = false;
let isCampConfirming = false;
let IS_GOTT_ROLE = false; // Wird vom inline script überschrieben
let isNightRestPending = false; // Spieler: Tracks ob eine Nachtlager-Anfrage gerade offen ist

function initDashboard(initialCharacters, initialRestFoods, isGod) {
    characters = initialCharacters;
    restFoods = initialRestFoods;
    IS_GOTT_ROLE = isGod;

    updateStatusDisplay();
    loadPokusFromLocalStorage();
    updateRestButtonVisibility();

    // SignalR Event-Listener registrieren
    setupNightRestListeners();

    // Spieler: beim Verlassen der Seite aktive Anfrage stornieren
    if (!IS_GOTT_ROLE) {
        window.addEventListener('beforeunload', () => {
            if (isNightRestPending) {
                const actId = window.globalSignalR?.getActId();
                if (actId) {
                    navigator.sendBeacon(`/api/night-rest/cancel/${actId}`);
                }
            }
        });
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
        openGodNightRestModal(data);
    });

    // Event: Night Rest Completed (für alle Spieler)
    window.addEventListener('nightRestCompleted', async (event) => {
        const data = event.detail;
        isNightRestPending = false;

        // Schließe Warte-Dialog
        Swal.close();

        // Baue Pro-Charakter Übersicht mit HP + Pokus + Mahlzeit
        let charactersHtml = '';
        if (data.characters && data.characters.length > 0) {
            charactersHtml = '<ul style="list-style: none; padding: 0; text-align: left;">';
            data.characters.forEach(char => {
                const extraPart = char.extraPokus > 0 ? ` + ${char.extraPokus} Extra` : '';
                charactersHtml += `
                    <li style="margin-bottom: 0.5rem; padding: 0.4rem 0.6rem; background: rgba(255,255,255,0.05); border-radius: 4px; font-size: 0.85rem;">
                        <strong>${char.name}:</strong><br>
                        <span style="opacity: 0.85;">❤️ ${char.currentHealth} / ${char.maxHealth} LP</span>
                        <span style="opacity: 0.55; margin: 0 0.3rem;">|</span>
                        <span style="opacity: 0.7;">🍽️ ${char.foodName} (+${char.healthBonus} LP)</span><br>
                        <span style="opacity: 0.85;">✨ ${char.spellCount} Zauber${extraPart} = <strong>${char.totalPokus} Pokuspunkte</strong></span>
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
            // Gott: Schließe Nachtlager-Modal falls offen
            const godModal = document.getElementById('godNightRestModal');
            if (godModal && godModal.style.display !== 'none') {
                closeGodNightRestModal();
            }
        } else {
            // Spieler: Warte-Dialog schließen und Benachrichtigung zeigen
            isNightRestPending = false;
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

// Alte showGodNightRestModal entfernt - wird durch openGodNightRestModal ersetzt (siehe unten)

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
    if (!btn) return; // Gott hat keinen campBtn

    btn.classList.remove('confirming');
    const campIcon = document.getElementById('campIcon');
    const campText = document.getElementById('campText');
    const confirmText = document.getElementById('confirmText');

    if (campIcon) campIcon.textContent = 'fireplace';
    if (campText) campText.textContent = 'Nachtlager';
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
        isNightRestPending = true;

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
            isNightRestPending = false;
            await cancelNightRest();
        }

    } catch (error) {
        isNightRestPending = false;
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Fehler beim Senden der Anfrage!',
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

// ===== GOTT NIGHT REST MODAL =====

let currentNightRestData = null;

function openGodNightRestModal(data) {
    currentNightRestData = data;

    // Player Info
    document.getElementById('godRequestPlayerName').textContent = data.playerName || 'Unbekannt';
    document.getElementById('godRequestTimestamp').textContent = new Date(data.timestamp).toLocaleString('de-DE');

    // Character Table
    const grid = document.getElementById('godCharacterGrid');

    // Build table
    let tableHtml = `
        <table style="width: 100%; border-collapse: collapse; font-family: 'Cinzel', serif; font-size: 0.9rem;">
            <thead>
                <tr style="background: rgba(139, 115, 85, 0.3); border-bottom: 2px solid rgba(139, 115, 85, 0.5);">
                    <th style="padding: 0.5rem; text-align: left; color: var(--ink);">Charakter</th>
                    <th style="padding: 0.5rem; text-align: center; color: var(--ink); width: 100px;">HP</th>
                    <th style="padding: 0.5rem; text-align: center; color: var(--ink); width: 80px;">Zauber</th>
                    <th style="padding: 0.5rem; text-align: left; color: var(--ink); width: 200px;">Essen</th>
                    <th style="padding: 0.5rem; text-align: center; color: var(--ink); width: 100px;">+ Pokus</th>
                </tr>
            </thead>
            <tbody>
    `;

    data.characters.forEach((char, idx) => {
        const bgColor = idx % 2 === 0 ? 'rgba(212, 175, 55, 0.05)' : 'rgba(212, 175, 55, 0.1)';
        const hpPercent = char.maxHealth > 0 ? (char.currentHealth / char.maxHealth * 100) : 100;
        const hpColor = hpPercent > 50 ? '#10b981' : hpPercent > 25 ? '#f59e0b' : '#ef4444';

        tableHtml += `
            <tr style="background: ${bgColor}; border-bottom: 1px solid rgba(139, 115, 85, 0.2);">
                <td style="padding: 0.5rem; color: var(--ink); font-weight: bold;">${char.name}</td>
                <td style="padding: 0.5rem; text-align: center; color: ${hpColor}; font-weight: bold;">${char.currentHealth || 0} / ${char.maxHealth || 0}</td>
                <td style="padding: 0.5rem; text-align: center; color: var(--ink);">${char.currentPokus || 0}</td>
                <td style="padding: 0.5rem;">
                    <select id="food-${char.id}" class="form-control" style="font-family: 'Cinzel', serif; font-size: 0.85rem; padding: 0.25rem;">
                        <option value="">-- Keine --</option>
                        ${restFoods.map(food => `
                            <option value="${food.id}">
                                ${food.name} (${food.healthBonus > 0 ? '+' : ''}${food.healthBonus} HP)
                            </option>
                        `).join('')}
                    </select>
                </td>
                <td style="padding: 0.5rem; text-align: center;">
                    <input type="number" id="pokus-${char.id}" class="form-control" value="0" min="0" max="100"
                           style="font-family: 'Cinzel', serif; font-size: 0.85rem; padding: 0.25rem; text-align: center; width: 80px;">
                </td>
            </tr>
        `;
    });

    tableHtml += `
            </tbody>
        </table>
    `;

    grid.innerHTML = tableHtml;
    document.getElementById('godNightRestModal').style.display = 'flex';
}

function closeGodNightRestModal() {
    document.getElementById('godNightRestModal').style.display = 'none';
    currentNightRestData = null;
}

async function applyGodNightRest() {
    if (!currentNightRestData) return;

    // Collect food and extra pokus for each character
    const foodPerCharacter = {};
    const extraPokusPerCharacter = {};

    currentNightRestData.characters.forEach(char => {
        const foodSelect = document.getElementById(`food-${char.id}`);
        const pokusInput = document.getElementById(`pokus-${char.id}`);

        if (foodSelect && foodSelect.value) {
            foodPerCharacter[char.id] = parseInt(foodSelect.value);
        }

        if (pokusInput && pokusInput.value) {
            const extraPokus = parseInt(pokusInput.value) || 0;
            if (extraPokus > 0) {
                extraPokusPerCharacter[char.id] = extraPokus;
            }
        }
    });

    const connection = window.globalSignalR?.connection;
    const actId = currentNightRestData.actId;

    if (!connection || !actId) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'SignalR-Verbindung nicht verfügbar.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    try {
        showLoading();
        await connection.invoke("ApplyNightRest", actId, foodPerCharacter, extraPokusPerCharacter);

        await Swal.fire({
            icon: 'success',
            title: 'Nachtlager angewendet',
            text: 'Die Charaktere haben sich ausgeruht.',
            confirmButtonColor: '#d97706',
            timer: 2000
        });

        closeGodNightRestModal();
    } catch (error) {
        console.error('Fehler beim Anwenden des Nachtlagers:', error);
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Das Nachtlager konnte nicht angewendet werden.',
            confirmButtonColor: '#d97706'
        });
    } finally {
        hideLoading();
    }
}

async function cancelGodNightRest() {
    if (!currentNightRestData) return;

    const connection = window.globalSignalR?.connection;
    const actId = currentNightRestData.actId;

    if (connection && actId) {
        try {
            await connection.invoke("CancelNightRest", actId);
        } catch (error) {
            console.error('Fehler beim Abbrechen:', error);
        }
    }

    closeGodNightRestModal();
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