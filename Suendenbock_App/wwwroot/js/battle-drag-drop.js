// ===== BATTLE SYSTEM - DRAG AND DROP INITIATIVE REORDERING =====

import { battleState } from './battle-state.js';
import { IS_GOTT_ROLE } from './battle-config.js';
import { renderBattleGrid, updateTurnIndicator } from './battle-rendering.js';
import { syncBattleState } from './battle-signalr.js';

// Make functions available globally for onclick handlers
if (typeof window !== 'undefined') {
    window.handleDragStart = handleDragStart;
    window.handleDragOver = handleDragOver;
    window.handleDragLeave = handleDragLeave;
    window.handleDrop = handleDrop;
    window.handleDragEnd = handleDragEnd;
    window.moveParticipantUp = moveParticipantUp;
    window.moveParticipantDown = moveParticipantDown;
}

// ===== DRAG & DROP FOR INITIATIVE =====
let draggedIndex = null;
let autoScrollInterval = null;

export function handleDragStart(event, index) {
    if (!IS_GOTT_ROLE) return;
    draggedIndex = index;
    event.currentTarget.style.opacity = '0.5';
    event.dataTransfer.effectAllowed = 'move';

    // Auto-Scroll aktivieren
    enableAutoScroll();
}

function enableAutoScroll() {
    const container = document.getElementById('participantsContainer');
    if (!container) return;

    // Intervall für Auto-Scroll während Drag
    if (autoScrollInterval) clearInterval(autoScrollInterval);

    autoScrollInterval = setInterval(() => {
        if (draggedIndex === null) {
            clearInterval(autoScrollInterval);
            return;
        }

        // Prüfe Mausposition relativ zum Container
        const rect = container.getBoundingClientRect();
        const mouseY = event.clientY;

        const scrollThreshold = 100; // Pixel vom Rand
        const scrollSpeed = 10;

        // Scroll nach oben wenn Maus nahe oberer Rand
        if (mouseY - rect.top < scrollThreshold) {
            container.scrollTop -= scrollSpeed;
        }
        // Scroll nach unten wenn Maus nahe unterer Rand
        else if (rect.bottom - mouseY < scrollThreshold) {
            container.scrollTop += scrollSpeed;
        }
    }, 50);
}

export function handleDragOver(event) {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';

    // Visual Feedback: Border highlighten
    const dropZone = event.currentTarget;
    dropZone.style.borderColor = 'rgb(245, 158, 11)';
    dropZone.style.borderWidth = '3px';

    // Auto-Scroll basierend auf Mausposition
    const container = document.getElementById('participantsContainer');
    if (container) {
        const rect = container.getBoundingClientRect();
        const mouseY = event.clientY;

        const scrollThreshold = 100;
        const scrollSpeed = 10;

        if (mouseY - rect.top < scrollThreshold) {
            container.scrollTop -= scrollSpeed;
        } else if (rect.bottom - mouseY < scrollThreshold) {
            container.scrollTop += scrollSpeed;
        }
    }

    return false;
}

export function handleDragLeave(event) {
    // Border zurücksetzen
    const dropZone = event.currentTarget;
    dropZone.style.borderColor = '';
    dropZone.style.borderWidth = '';
}

export function handleDrop(event, dropIndex) {
    if (!IS_GOTT_ROLE) return;
    event.stopPropagation();
    event.preventDefault();

    // Border zurücksetzen
    event.currentTarget.style.borderColor = '';
    event.currentTarget.style.borderWidth = '';

    if (draggedIndex === null || draggedIndex === dropIndex) return;

    // WICHTIG: Verschiebe den Teilnehmer direkt im Array UND passe Initiative an!
    const draggedParticipant = battleState.participants[draggedIndex];
    const targetParticipant = battleState.participants[dropIndex];
    const oldCurrentTurnIndex = battleState.currentTurnIndex;

    // Entferne den gezogenen Teilnehmer aus dem Array
    battleState.participants.splice(draggedIndex, 1);

    // Berechne die neue Einfügeposition
    let newInsertIndex = dropIndex;
    if (draggedIndex < dropIndex) {
        // Nach unten gezogen: Index ist schon korrekt
        newInsertIndex = dropIndex;
    }

    // Füge den Teilnehmer an der neuen Position ein
    battleState.participants.splice(newInsertIndex, 0, draggedParticipant);

    // WICHTIG: Initiative anpassen basierend auf der neuen Position
    // So bleibt die Position während der Runde stabil!
    if (draggedIndex < newInsertIndex) {
        // Nach unten gezogen: Initiative zwischen Ziel und nächstem
        const nextParticipant = battleState.participants[newInsertIndex + 1];
        if (nextParticipant) {
            draggedParticipant.initiative = Math.floor((targetParticipant.initiative + nextParticipant.initiative) / 2);
        } else {
            // Ans Ende: Initiative höher als Ziel
            draggedParticipant.initiative = targetParticipant.initiative + 1;
        }
    } else {
        // Nach oben gezogen: Initiative zwischen vorherigem und Ziel
        const prevParticipant = battleState.participants[newInsertIndex - 1];
        if (prevParticipant) {
            draggedParticipant.initiative = Math.floor((prevParticipant.initiative + targetParticipant.initiative) / 2);
        } else {
            // An den Anfang: Initiative niedriger als Ziel
            draggedParticipant.initiative = targetParticipant.initiative - 1;
        }
    }

    // CurrentTurnIndex anpassen basierend auf der Verschiebung
    const currentPlayerId = battleState.participants[oldCurrentTurnIndex].id;

    if (oldCurrentTurnIndex === draggedIndex) {
        // WICHTIG: Der aktuelle Spieler wurde verschoben!
        // Der Zug geht IMMER an den Spieler, der vorher NACH ihm kam
        // Das ist jetzt der Spieler an der alten Position (draggedIndex)
        battleState.currentTurnIndex = draggedIndex;
    } else {
        // Jemand ANDERES wurde verschoben
        // Der aktuelle Zug soll auf dem GLEICHEN SPIELER bleiben (nicht Index!)
        // Finde den aktuellen Spieler nach der Verschiebung
        battleState.currentTurnIndex = battleState.participants.findIndex(p => p.id === currentPlayerId);
    }

    renderBattleGrid();
    updateTurnIndicator();
    syncBattleState();
}

export function handleDragEnd(event) {
    event.currentTarget.style.opacity = '1';
    draggedIndex = null;

    // Auto-Scroll deaktivieren
    if (autoScrollInterval) {
        clearInterval(autoScrollInterval);
        autoScrollInterval = null;
    }
}

// ===== MOVE UP/DOWN BUTTONS FOR INITIATIVE =====
export function moveParticipantUp(index) {
    if (!IS_GOTT_ROLE || index === 0) return;

    // WICHTIG: Verschiebe im Array UND tausche Initiative!
    const participant = battleState.participants[index];
    const prevParticipant = battleState.participants[index - 1];
    const oldCurrentTurnIndex = battleState.currentTurnIndex;

    // Tausche Initiativen (damit Position während Runde stabil bleibt!)
    const tempInitiative = participant.initiative;
    participant.initiative = prevParticipant.initiative;
    prevParticipant.initiative = tempInitiative;

    // Entferne Teilnehmer aus aktueller Position
    battleState.participants.splice(index, 1);

    // Füge ihn eine Position früher ein
    battleState.participants.splice(index - 1, 0, participant);

    // CurrentTurnIndex anpassen
    const currentPlayerId = battleState.participants[oldCurrentTurnIndex].id;

    if (oldCurrentTurnIndex === index) {
        // WICHTIG: Der aktuelle Spieler wurde nach oben verschoben
        // Der Zug geht an den Spieler, der vorher nach ihm kam (jetzt an alter Position)
        battleState.currentTurnIndex = index;
    } else {
        // Jemand ANDERES wurde verschoben
        // Der aktuelle Zug soll auf dem GLEICHEN SPIELER bleiben (nicht Index!)
        battleState.currentTurnIndex = battleState.participants.findIndex(p => p.id === currentPlayerId);
    }

    renderBattleGrid();
    updateTurnIndicator();
    syncBattleState();

    // Scroll zur verschobenen Karte
    scrollToParticipant(index - 1);
}

export function moveParticipantDown(index) {
    if (!IS_GOTT_ROLE || index === battleState.participants.length - 1) return;

    // WICHTIG: Verschiebe im Array UND tausche Initiative!
    const participant = battleState.participants[index];
    const nextParticipant = battleState.participants[index + 1];
    const oldCurrentTurnIndex = battleState.currentTurnIndex;

    // Tausche Initiativen (damit Position während Runde stabil bleibt!)
    const tempInitiative = participant.initiative;
    participant.initiative = nextParticipant.initiative;
    nextParticipant.initiative = tempInitiative;

    // Entferne Teilnehmer aus aktueller Position
    battleState.participants.splice(index, 1);

    // Füge ihn eine Position später ein
    battleState.participants.splice(index + 1, 0, participant);

    // CurrentTurnIndex anpassen
    const currentPlayerId = battleState.participants[oldCurrentTurnIndex].id;

    if (oldCurrentTurnIndex === index) {
        // WICHTIG: Der aktuelle Spieler wurde nach unten verschoben
        // Der Zug geht an den Spieler, der vorher nach ihm kam (jetzt an alter Position)
        battleState.currentTurnIndex = index;
    } else {
        // Jemand ANDERES wurde verschoben
        // Der aktuelle Zug soll auf dem GLEICHEN SPIELER bleiben (nicht Index!)
        battleState.currentTurnIndex = battleState.participants.findIndex(p => p.id === currentPlayerId);
    }

    renderBattleGrid();
    updateTurnIndicator();
    syncBattleState();

    // Scroll zur verschobenen Karte
    scrollToParticipant(index + 1);
}

export function scrollToParticipant(index) {
    const container = document.getElementById('participantsContainer');
    if (!container) return;

    setTimeout(() => {
        const cards = container.querySelectorAll('[data-participant-index]');
        if (cards[index]) {
            cards[index].scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        }
    }, 100);
}
