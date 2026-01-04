// wwwroot/js/trophies.js

// ===== INITIALIZATION =====

document.addEventListener('DOMContentLoaded', () => {
    initTrophies();
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

    // Equip Buttons (An die Wand hängen)
    document.querySelectorAll('.equip-btn').forEach(btn => {
        btn.addEventListener('click', async () => {
            const trophyId = btn.dataset.trophyId;
            await equipTrophy(trophyId);
        });
    });

    // Equipped Trophy Click (Entfernen)
    document.querySelectorAll('.trophy-shield').forEach(shield => {
        shield.addEventListener('click', async () => {
            const trophyId = shield.dataset.trophyId;
            if (confirm('Diese Trophäe von der Wand nehmen?')) {
                await unequipTrophy(trophyId);
            }
        });
    });
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
 * Trophäe ausrüsten (an die Wand hängen)
 */
async function equipTrophy(trophyId) {
    try {
        const response = await fetch(`/api/game/trophies/${trophyId}/toggle-equip`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
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
