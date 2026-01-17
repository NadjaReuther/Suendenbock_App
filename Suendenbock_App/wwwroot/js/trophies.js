// wwwroot/js/trophies.js

// ===== INITIALIZATION =====

// Touch-Support Variablen
let selectedTrophyId = null;
let isTouchDevice = false;

document.addEventListener('DOMContentLoaded', () => {
    // Detect Touch Device
    isTouchDevice = ('ontouchstart' in window) || (navigator.maxTouchPoints > 0);

    // Detect if it's a pure touch device (no mouse)
    const isPureTouchDevice = isTouchDevice && !window.matchMedia('(pointer: fine)').matches;

    initTrophies();

    // Show touch hint only on pure touch devices
    if (isPureTouchDevice) {
        showTouchHint();
    }
});

function initTrophies() {
    // Status Toggle Buttons (bought ↔ slain)
    document.querySelectorAll('.status-toggle-btn').forEach(btn => {
        btn.addEventListener('click', async (e) => {
            e.stopPropagation();
            const trophyId = btn.dataset.trophyId;
            await toggleTrophyStatus(trophyId);
        });
    });

    // Equipped Trophy Click (Entfernen)
    document.querySelectorAll('.trophy-shield').forEach(shield => {
        shield.addEventListener('click', async () => {
            const trophyId = shield.dataset.trophyId;
            await unequipTrophy(trophyId);
        });
    });

    // Initialize description previews
    initDescriptionPreviews();

    // Initialize Drag & Drop (immer für Maus-Nutzer)
    initDragAndDrop();

    // Initialize Touch Support zusätzlich wenn Touch verfügbar
    if (isTouchDevice) {
        initTouchSupport();
    }
}

// ===== API CALLS =====

/**
 * Status wechseln: bought ↔ slain
 */
async function toggleTrophyStatus(trophyId) {
    try {
        const response = await fetch(`/api/game/trophies/${trophyId}/toggle-status`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });

        if (response.ok) {
            // Seite neu laden (einfachste Variante)
            location.reload();
        } else {
            const error = await response.json();
            alert(`Fehler: ${error.error || 'Status konnte nicht gewechselt werden'}`);
        }
    } catch (error) {
        console.error('Fehler:', error);
        alert('Fehler beim Wechseln des Status!');
    }
}

/**
 * Trophäe an bestimmter Position ausrüsten (Drag & Drop)
 */
async function equipTrophyAtPosition(trophyId, position) {
    try {
        const response = await fetch(`/api/game/trophies/${trophyId}/equip`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ position: position })
        });

        if (response.ok) {
            location.reload();
        } else {
            const error = await response.json();
            alert(`Fehler: ${error.error || 'Trophäe konnte nicht ausgerüstet werden'}`);
        }
    } catch (error) {
        console.error('Fehler:', error);
        alert('Fehler beim Ausrüsten!');
    }
}

/**
 * Trophäe entfernen (von der Wand nehmen)
 */
async function unequipTrophy(trophyId) {
    try {
        const response = await fetch(`/api/game/trophies/${trophyId}/toggle-equip`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });

        if (response.ok) {
            location.reload();
        } else {
            const error = await response.json();
            alert(`Fehler: ${error.error || 'Trophäe konnte nicht entfernt werden'}`);
        }
    } catch (error) {
        console.error('Fehler:', error);
        alert('Fehler beim Entfernen!');
    }
}

/**
 * Toggle Trophy Type (für ausgerüstete Trophäen)
 * Wechselt zwischen bought und slain, wenn beide Varianten verfügbar sind
 */
async function toggleTrophyType(trophyId, currentStatus) {
    try {
        const response = await fetch(`/api/game/trophies/${trophyId}/toggle-status`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });

        if (response.ok) {
            // Seite neu laden um die neue Anzeige zu reflektieren
            location.reload();
        } else {
            const error = await response.json();
            alert(`Fehler: ${error.error || 'Status konnte nicht gewechselt werden'}`);
        }
    } catch (error) {
        console.error('Fehler:', error);
        alert('Fehler beim Wechseln des Trophäentyps!');
    }
}

// ===== DRAG & DROP =====

let draggedTrophyId = null;

/**
 * Initialisiert Drag & Drop für Trophäen
 */
function initDragAndDrop() {
    // Make inventory cards draggable
    document.querySelectorAll('.inventory-card.draggable').forEach(card => {
        card.addEventListener('dragstart', handleDragStart);
        card.addEventListener('dragend', handleDragEnd);
    });

    // Make trophy slots drop zones
    document.querySelectorAll('.trophy-slot.drop-zone').forEach(slot => {
        slot.addEventListener('dragover', handleDragOver);
        slot.addEventListener('drop', handleDrop);
        slot.addEventListener('dragleave', handleDragLeave);
        slot.addEventListener('dragenter', handleDragEnter);
    });
}

/**
 * Drag Start Handler
 */
function handleDragStart(e) {
    draggedTrophyId = this.dataset.trophyId;
    this.style.opacity = '0.5';
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/html', this.innerHTML);

    // Clear touch selection if drag starts
    selectedTrophyId = null;
    document.querySelectorAll('.inventory-card.draggable').forEach(c => {
        c.classList.remove('touch-selected');
    });
}

/**
 * Drag End Handler
 */
function handleDragEnd(e) {
    this.style.opacity = '1';
    // Remove all drag-over classes
    document.querySelectorAll('.trophy-slot').forEach(slot => {
        slot.classList.remove('drag-over');
    });
}

/**
 * Drag Over Handler
 */
function handleDragOver(e) {
    if (e.preventDefault) {
        e.preventDefault(); // Allows drop
    }
    e.dataTransfer.dropEffect = 'move';
    return false;
}

/**
 * Drag Enter Handler
 */
function handleDragEnter(e) {
    this.classList.add('drag-over');
}

/**
 * Drag Leave Handler
 */
function handleDragLeave(e) {
    this.classList.remove('drag-over');
}

/**
 * Drop Handler
 */
function handleDrop(e) {
    if (e.stopPropagation) {
        e.stopPropagation(); // Stops browser from redirecting
    }

    this.classList.remove('drag-over');

    if (draggedTrophyId) {
        const position = parseInt(this.dataset.position);
        equipTrophyAtPosition(draggedTrophyId, position);
    }

    return false;
}

// ===== TOUCH SUPPORT =====

/**
 * Zeigt Touch-Hint für reine Touch-Geräte
 */
function showTouchHint() {
    const inventorySection = document.querySelector('.inventory-section');
    if (inventorySection && !document.querySelector('.touch-hint')) {
        const hint = document.createElement('div');
        hint.className = 'touch-hint';
        hint.innerHTML = '<span class="material-symbols-outlined">touch_app</span> Tippe auf eine Trophäe, dann auf einen freien Platz an der Wand';
        inventorySection.insertBefore(hint, inventorySection.firstChild);
    }
}

/**
 * Initialisiert Touch-Support für mobile Geräte
 * Click-to-Select System:
 * 1. Click auf Inventory-Trophäe → markiert sie
 * 2. Click auf Trophy-Slot → platziert die markierte Trophäe dort
 */
function initTouchSupport() {
    // Inventory Cards: Click to Select
    document.querySelectorAll('.inventory-card.draggable').forEach(card => {
        card.addEventListener('click', (e) => {
            // Prevent default if clicking on description toggle
            if (e.target.classList.contains('description-toggle')) {
                return;
            }

            const trophyId = card.dataset.trophyId;

            // Clear drag state if touch selection starts
            draggedTrophyId = null;

            // Deselect all
            document.querySelectorAll('.inventory-card.draggable').forEach(c => {
                c.classList.remove('touch-selected');
            });

            // Select this one
            if (selectedTrophyId === trophyId) {
                // Toggle off if clicking the same trophy
                selectedTrophyId = null;
            } else {
                selectedTrophyId = trophyId;
                card.classList.add('touch-selected');
            }
        });
    });

    // Trophy Slots: Click to Place
    document.querySelectorAll('.trophy-slot.drop-zone').forEach(slot => {
        // Add a visual indicator that it's clickable
        slot.style.cursor = 'pointer';

        slot.addEventListener('click', async (e) => {
            // If clicking on the trophy shield itself, ignore (that's for removing)
            if (e.target.closest('.trophy-shield')) {
                return;
            }

            if (selectedTrophyId) {
                const position = parseInt(slot.dataset.position);

                // Show loading state
                slot.classList.add('placing-trophy');

                await equipTrophyAtPosition(selectedTrophyId, position);

                // Reset selection
                selectedTrophyId = null;
                document.querySelectorAll('.inventory-card.draggable').forEach(c => {
                    c.classList.remove('touch-selected');
                });
            }
        });
    });
}

// ===== DESCRIPTION PREVIEW/EXPAND =====

/**
 * Initialisiert die Preview-Funktion für Monster-Beschreibungen
 * Zeigt nur die ersten ~10 Wörter an und fügt "..." Button hinzu
 */
function initDescriptionPreviews() {
    const MAX_WORDS = 10;

    document.querySelectorAll('.description-section').forEach(section => {
        const textElement = section.querySelector('.description-text');
        const toggleElement = section.querySelector('.description-toggle');

        if (!textElement || !toggleElement) return;

        // Speichere den vollen HTML-Inhalt
        const fullHtml = textElement.innerHTML;

        // Extrahiere Text ohne HTML-Tags für Wort-Zählung
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = fullHtml;
        const fullText = tempDiv.textContent || tempDiv.innerText || '';

        // Teile Text in Wörter
        const words = fullText.trim().split(/\s+/);

        if (words.length > MAX_WORDS) {
            // Text ist lang genug → Kürzen + Toggle Button zeigen

            // Speichere vollen HTML als data-Attribut
            textElement.dataset.fullHtml = fullHtml;

            // Erstelle Preview (nur Text, keine HTML-Tags für Preview)
            const previewText = words.slice(0, MAX_WORDS).join(' ');
            textElement.textContent = previewText + ' ';

            // Zeige Toggle Button
            toggleElement.textContent = '...';
            toggleElement.style.display = 'inline';
            toggleElement.style.cursor = 'pointer';

            // Markiere als collapsed
            textElement.dataset.expanded = 'false';
        } else {
            // Text ist kurz → Volltext direkt anzeigen, kein Toggle
            toggleElement.style.display = 'none';
        }
    });
}

/**
 * Toggle zwischen Preview und vollständigem Text
 * @param {number} trophyId - Die Trophy ID
 * @param {string} section - 'basics', 'encounter', oder 'perfected'
 */
function toggleDescription(trophyId, section) {
    const sectionElement = document.querySelector(
        `.description-section[data-trophy-id="${trophyId}"][data-section="${section}"]`
    );

    if (!sectionElement) return;

    const textElement = sectionElement.querySelector('.description-text');
    const toggleElement = sectionElement.querySelector('.description-toggle');

    if (!textElement || !toggleElement) return;

    const isExpanded = textElement.dataset.expanded === 'true';
    const fullHtml = textElement.dataset.fullHtml;

    if (!fullHtml) return; // Kein vollständiger Text gespeichert

    if (isExpanded) {
        // Collapse: Zurück zur Preview
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = fullHtml;
        const fullText = tempDiv.textContent || tempDiv.innerText || '';
        const words = fullText.trim().split(/\s+/);
        const previewText = words.slice(0, 10).join(' ');

        textElement.textContent = previewText + ' ';
        toggleElement.textContent = '...';
        textElement.dataset.expanded = 'false';
    } else {
        // Expand: Zeige vollen Text mit HTML
        textElement.innerHTML = fullHtml;
        toggleElement.textContent = ' ▲';
        textElement.dataset.expanded = 'true';
    }
}
