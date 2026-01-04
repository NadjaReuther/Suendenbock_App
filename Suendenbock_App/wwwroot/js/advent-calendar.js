// ========================================
// ADVENT CALENDAR - Komplette JavaScript Datei
// ========================================

// Globale Variablen
let isGodUser = false;
let openedDoors = [];

// Warten bis die Seite vollständig geladen ist
document.addEventListener('DOMContentLoaded', function () {
    Promise.all([
        checkGodStatus(),
        loadOpenedDoors()
    ]).then(() => {
        initializeAdventCalendar();
    });
});

// ========================================
// GOD-STATUS PRÜFEN
// ========================================
async function checkGodStatus() {
    try {
        const response = await fetch('/AdventCalendar/IsGod');
        if (response.ok) {
            const data = await response.json();
            isGodUser = data.isGod;
        }
    } catch (error) {
        console.error('Fehler beim Prüfen des God-Status:', error);
    }
}

// ========================================
// GEÖFFNETE TÜRCHEN LADEN
// ========================================
async function loadOpenedDoors() {
    try {
        const response = await fetch('/AdventCalendar/GetOpenedDoors');
        if (response.ok) {
            const data = await response.json();
            openedDoors = data.openedDoors || [];
        }
    } catch (error) {
        console.error('Fehler beim Laden der geöffneten Türchen:', error);
    }
}

// ========================================
// INITIALISIERUNG
// ========================================
function initializeAdventCalendar() {
    // Alle Türchen finden
    const doors = document.querySelectorAll('.advent-door');

    // Für jedes Türchen einen Click-Handler hinzufügen und geöffnete markieren
    doors.forEach(door => {
        const dayNumber = parseInt(door.getAttribute('data-day'));

        // Geöffnete Türchen markieren
        if (openedDoors.includes(dayNumber)) {
            door.classList.add('already-opened');
        }

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

    // SCHRITT 2: Zur Detail-Seite navigieren
    window.location.href = `/Player/WeihnachtstuerDetail?day=${dayNumber}`;
}

// ========================================
// DATUMS-PRÜFUNG
// ========================================
function isDayAvailable(dayNumber) {
   
    // God-User können alle Türchen öffnen (für Testing)
    if (isGodUser) {
        return true;
    }

    const today = new Date();
    const currentDay = today.getDate();
    const currentMonth = today.getMonth() + 1;  // Monate starten bei 0

    // Nur im Dezember erlauben
    /*if (currentMonth !== 12) {
        return false;
    }*/

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
// UNIVERSELLE MODAL-FUNKTION
// ========================================
function showAdventMessage(title, content, icon = '🎄') {
    document.getElementById('adventModalTitle').innerHTML = `${icon} ${title}`;
    document.getElementById('adventModalBody').innerHTML = content;

    const modal = new bootstrap.Modal(document.getElementById('adventModal'));
    modal.show();
}