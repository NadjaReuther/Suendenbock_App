// ========================================
// ADVENT CALENDAR - Komplette JavaScript Datei
// ========================================

// Warten bis die Seite vollständig geladen ist
document.addEventListener('DOMContentLoaded', function () {
    initializeAdventCalendar();
});

// ========================================
// INITIALISIERUNG
// ========================================
function initializeAdventCalendar() {
    // Alle Türchen finden
    const doors = document.querySelectorAll('.advent-door');

    // Für jedes Türchen einen Click-Handler hinzufügen
    doors.forEach(door => {
        door.addEventListener('click', function () {
            handleDoorClick(this);
        });
    });
}

// ========================================
// TÜRCHEN-KLICK BEHANDELN
// ========================================
function handleDoorClick(doorElement) {
    const dayNumber = parseInt(doorElement.getAttribute('data-day'));

    // SCHRITT 1: Ist der Tag schon erreicht?
    if (!isDayAvailable(dayNumber)) {
        showLockedMessage(doorElement, dayNumber);
        return;  // Türchen bleibt zu!
    }

    // SCHRITT 2: Ist das Türchen schon offen?
    if (!doorElement.classList.contains('opened')) {
        // Türchen öffnen (CSS Animation startet)
        doorElement.classList.add('opened');

        // Inhalt vom Server laden
        loadDoorContent(dayNumber, doorElement);
    }
}

// ========================================
// DATUMS-PRÜFUNG
// ========================================
function isDayAvailable(dayNumber) {
    const today = new Date();
    const currentDay = today.getDate();
    const currentMonth = today.getMonth() + 1;  // Monate starten bei 0

    // Nur im Dezember erlauben
    if (currentMonth !== 12) {
        return false;
    }

    // Nur wenn der Tag erreicht oder überschritten ist
    return currentDay >= dayNumber;
}

// ========================================
// VERSCHLOSSENES TÜRCHEN - MODAL ANZEIGEN
// ========================================
function showLockedMessage(doorElement, dayNumber) {
    const lockIcon = doorElement.querySelector('.door-lock');
    const modalElement = document.getElementById('adventModal');

    // Schloss anzeigen und wackeln starten
    lockIcon.style.display = 'block';
    lockIcon.classList.add('shaking');

    // Modal öffnen
    const modal = new bootstrap.Modal(modalElement);
    modal.show();

    // Inhalt setzen
    document.getElementById('adventModalTitle').innerHTML = '🔒 Türchen verschlossen';
    document.getElementById('adventModalBody').innerHTML = `
        <p>Dieses Türchen öffnet sich erst am <strong>${dayNumber}. Dezember</strong>!</p>
        <p style="font-size: 3rem; margin: 1rem 0;">🗓️</p>
        <p class="text-muted">Hab noch etwas Geduld...</p>
    `;

    // Beim Schließen: Schloss verstecken und Wackeln stoppen
    modalElement.addEventListener('hidden.bs.modal', function cleanup() {
        lockIcon.style.display = 'none';
        lockIcon.classList.remove('shaking');
        modalElement.removeEventListener('hidden.bs.modal', cleanup);
    }, { once: true });
}

// ========================================
// TÜRCHEN-INHALT VOM SERVER LADEN
// ========================================
function loadDoorContent(day, doorElement) {
    const contentDiv = doorElement.querySelector('.door-content');

    // Loading-Anzeige
    contentDiv.innerHTML = '<div class="spinner-border text-warning" role="status"></div>';

    // Vom Server holen
    fetch(`/AdventCalendar/GetContent?day=${day}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Fehler beim Laden');
            }
            return response.json();
        })
        .then(data => {
            // Hat der User schon gewählt?
            if (data.alreadyChosen) {
                // Zeige nur das Ergebnis (keine Änderung mehr möglich!)
                showChoiceResult(contentDiv, data);
            } else {
                // Zeige die Auswahlmöglichkeiten
                showChoiceOptions(contentDiv, data, day);
            }
        })
        .catch(error => {
            console.error('Fehler:', error);
            contentDiv.innerHTML = '<p class="text-danger">Fehler beim Laden :(</p>';
        });
}

// ========================================
// AUSWAHLMÖGLICHKEITEN ANZEIGEN
// ========================================
function showChoiceOptions(contentDiv, data, day) {
    // HTML für Frage und Optionen erstellen
    let html = `
        <div class="advent-question">
            <h5 style="font-family: 'Cinzel', serif; color: #5d4e37; margin-bottom: 1rem;">
                ${data.question}
            </h5>
        </div>
    `;

    // Optional: Bild anzeigen
    if (data.imagePath) {
        html += `
            <div class="advent-image" style="margin-bottom: 1rem;">
                <img src="${data.imagePath}" style="max-width: 100%; border-radius: 8px;" />
            </div>
        `;
    }

    html += '<div class="advent-choices">';

    // Für jede Auswahl-Option einen Button erstellen
    data.choices.forEach((choice, index) => {
        html += `
            <button class="choice-btn btn btn-outline-warning w-100 mb-2" 
                    onclick="makeChoice(${day}, ${index})"
                    style="font-family: 'MedievalSharp', cursive; padding: 0.75rem;">
                ${choice.text}
            </button>
        `;
    });

    html += '</div>';
    contentDiv.innerHTML = html;
}

// ========================================
// BEREITS GETROFFENE AUSWAHL ANZEIGEN
// ========================================
function showChoiceResult(contentDiv, data) {
    const selectedChoice = data.choices[data.chosenIndex];

    contentDiv.innerHTML = `
        <div class="choice-result text-center">
            <h5 style="font-family: 'Cinzel', serif; color: #5d4e37;">
                ${data.question}
            </h5>
            <div style="margin: 1.5rem 0; padding: 1rem; background: rgba(212, 175, 55, 0.2); border-radius: 8px;">
                <p style="font-size: 1.1rem; margin-bottom: 0.5rem;">
                    <strong>Deine Wahl:</strong>
                </p>
                <p style="font-size: 1.2rem; color: #d4af37; font-weight: bold;">
                    ${selectedChoice.text}
                </p>
            </div>
            <p style="font-size: 1rem; color: #8b7355;">
                ⭐ ${data.pointsEarned} Punkte erhalten
            </p>
            <p style="font-size: 0.9rem; color: #999; margin-top: 1rem;">
                <em>Diese Entscheidung ist endgültig</em>
            </p>
        </div>
    `;
}

// ========================================
// AUSWAHL SPEICHERN
// ========================================
function makeChoice(day, choiceIndex) {
    // Button deaktivieren um Doppel-Klicks zu verhindern
    const buttons = document.querySelectorAll(`[data-day="${day}"] .choice-btn`);
    buttons.forEach(btn => btn.disabled = true);

    // An Server senden
    fetch('/AdventCalendar/SaveChoice', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
        body: JSON.stringify({
            day: day,
            choiceIndex: choiceIndex
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Fehler beim Speichern');
            }
            return response.json();
        })
        .then(result => {
            // Erfolgreich gespeichert!
            showSuccessModal(result, day);
        })
        .catch(error => {
            console.error('Fehler:', error);
            showAdventMessage(
                'Fehler',
                '<p>Deine Auswahl konnte nicht gespeichert werden. Bitte versuche es erneut.</p>',
                '❌'
            );
            // Buttons wieder aktivieren
            buttons.forEach(btn => btn.disabled = false);
        });
}

// ========================================
// ERFOLGS-MODAL NACH AUSWAHL
// ========================================
function showSuccessModal(result, day) {
    showAdventMessage(
        'Auswahl gespeichert!',
        `
            <p style="font-size: 2.5rem; margin: 1rem 0;">✨</p>
            <p style="font-size: 1.2rem;">
                Du hast <strong style="color: #d4af37;">${result.pointsEarned} Punkte</strong> erhalten!
            </p>
            <div style="margin-top: 1.5rem; padding: 1rem; background: linear-gradient(145deg, #d4af37, #b8941f); border-radius: 8px;">
                <p style="font-size: 1.5rem; color: white; font-weight: bold; margin: 0;">
                    Gesamt: ${result.totalPoints} Punkte
                </p>
            </div>
        `,
        '🎁'
    );

    // Nach Modal-Schließen: Türchen-Inhalt aktualisieren
    const modalElement = document.getElementById('adventModal');
    modalElement.addEventListener('hidden.bs.modal', function updateContent() {
        const doorElement = document.querySelector(`[data-day="${day}"]`);
        loadDoorContent(day, doorElement);
        modalElement.removeEventListener('hidden.bs.modal', updateContent);
    }, { once: true });
}

// ========================================
// UNIVERSELLE MODAL-FUNKTION
// ========================================
function showAdventMessage(title, content, icon = '🎄') {
    document.getElementById('adventModalTitle').innerHTML = `${icon} ${title}`;
    document.getElementById('adventModalBody').innerHTML = content;

    const modal = new bootstrap.Modal(document.getElementById('adventModal'));
    modal.show();
}