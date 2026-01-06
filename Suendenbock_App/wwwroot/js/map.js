// wwwroot/js/map.js

let selectedMarkerType = 'quest';
let pendingPosition = null;

// ===== INITIALIZATION =====

document.addEventListener('DOMContentLoaded', () => {
    initMap();

    // Fokussiere Marker wenn focusMarkerId gesetzt ist
    if (MAP_DATA.focusMarkerId) {
        setTimeout(() => focusOnMarker(MAP_DATA.focusMarkerId), 500);
    }
});

function initMap() {
    const mapCanvas = document.getElementById('mapCanvas');
    const isGod = MAP_DATA.isGod;

    // Prüfe ob wir im "Quest-Marker-Modus" sind
    const questMarkerMode = sessionStorage.getItem('questMarkerMode');
    if (questMarkerMode === 'true') {
        showQuestMarkerModeInfo();
        // Karte klicken → Questmarker-Koordinaten speichern und zurück
        mapCanvas.addEventListener('click', handleQuestMarkerClick);
        return; // Normale Map-Funktionen deaktivieren
    }

    // Marker-Typ Buttons (nur Gott)
    if (isGod) {
        document.querySelectorAll('.marker-type-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                document.querySelectorAll('.marker-type-btn').forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
                selectedMarkerType = btn.dataset.type;
            });
        });

        // Karte klicken → Marker erstellen
        mapCanvas.addEventListener('click', handleMapClick);

        // Act aktivieren Button
        const activateBtn = document.querySelector('.activate-act-btn');
        if (activateBtn && !activateBtn.disabled) {
            activateBtn.addEventListener('click', handleActivateAct);
        }
    }

    // Marker anklicken → Details (für alle)
    document.querySelectorAll('.map-marker').forEach(marker => {
        marker.addEventListener('click', (e) => {
            e.stopPropagation();
            const markerId = marker.dataset.markerId;
            showMarkerDetails(markerId);
        });

        // Drag & Drop für Marker (nur Gott)
        if (isGod) {
            makeMarkerDraggable(marker);
        }
    });

    // Popup schließen
    document.getElementById('closePopup')?.addEventListener('click', closePopup);
    document.getElementById('markerPopup')?.addEventListener('click', (e) => {
        if (e.target.id === 'markerPopup') {
            closePopup();
        }
    });
}

// ===== QUEST MARKER MODE =====

function showQuestMarkerModeInfo() {
    // Info-Banner anzeigen
    const infoBanner = document.createElement('div');
    infoBanner.id = 'questMarkerModeInfo';
    infoBanner.style.cssText = `
        position: fixed;
        top: 20px;
        left: 50%;
        transform: translateX(-50%);
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        padding: 1rem 2rem;
        border-radius: 8px;
        box-shadow: 0 4px 20px rgba(0,0,0,0.3);
        z-index: 10000;
        font-weight: 500;
        display: flex;
        align-items: center;
        gap: 1rem;
        animation: slideDown 0.3s ease-out;
    `;
    infoBanner.innerHTML = `
        <span class="material-symbols-outlined" style="font-size: 2rem;">location_on</span>
        <div>
            <div style="font-size: 1.1rem; margin-bottom: 0.25rem;">Questmarker setzen</div>
            <div style="font-size: 0.9rem; opacity: 0.9;">Klicke auf die Karte, um den Marker für deine Quest zu platzieren</div>
        </div>
        <button onclick="cancelQuestMarkerMode()" style="
            background: rgba(255,255,255,0.2);
            border: none;
            color: white;
            padding: 0.5rem 1rem;
            border-radius: 4px;
            cursor: pointer;
            margin-left: 1rem;
        ">Abbrechen</button>
    `;
    document.body.appendChild(infoBanner);

    // Animation hinzufügen
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideDown {
            from {
                transform: translateX(-50%) translateY(-20px);
                opacity: 0;
            }
            to {
                transform: translateX(-50%) translateY(0);
                opacity: 1;
            }
        }
    `;
    document.head.appendChild(style);
}

function cancelQuestMarkerMode() {
    // SessionStorage aufräumen
    sessionStorage.removeItem('questMarkerMode');
    sessionStorage.removeItem('pendingQuestData');
    // Zurück zu Quests
    window.location.href = '/Spielmodus/Quests';
}

function handleQuestMarkerClick(e) {
    // Wenn auf Marker geklickt oder auf Bestätigungs-Dialog, ignorieren
    if (e.target.closest('.map-marker') || e.target.closest('#questMarkerPreview')) return;

    const mapCanvas = document.getElementById('mapCanvas');
    if (!mapCanvas) return;

    const rect = mapCanvas.getBoundingClientRect();

    // Klick-Position relativ zum Layer (0-100%)
    const x = ((e.clientX - rect.left) / rect.width) * 100;
    const y = ((e.clientY - rect.top) / rect.height) * 100;

    // Validierung: Position muss im Bereich 0-100 sein
    if (x < 0 || x > 100 || y < 0 || y > 100) {
        console.log('Position außerhalb:', { x, y });
        return;
    }

    // Zeige Vorschau-Marker und Bestätigungs-Dialog
    showMarkerPreview(x, y);
}

function showMarkerPreview(x, y) {
    // Entferne vorherige Vorschau falls vorhanden
    const existingPreview = document.getElementById('questMarkerPreview');
    if (existingPreview) {
        existingPreview.remove();
    }

    const mapCanvas = document.getElementById('mapCanvas');

    // Erstelle Vorschau-Marker
    const previewMarker = document.createElement('div');
    previewMarker.id = 'questMarkerPreview';
    previewMarker.style.cssText = `
        position: absolute;
        left: ${x}%;
        top: ${y}%;
        transform: translate(-50%, -50%);
        z-index: 1000;
        animation: markerPulse 1.5s infinite;
    `;
    previewMarker.innerHTML = `
        <div style="position: relative;">
            <!-- Pulsierender Ring -->
            <div style="
                position: absolute;
                width: 60px;
                height: 60px;
                border-radius: 50%;
                background: rgba(102, 126, 234, 0.2);
                border: 3px solid rgba(102, 126, 234, 0.6);
                top: 50%;
                left: 50%;
                transform: translate(-50%, -50%);
                animation: pulse 1.5s infinite;
            "></div>

            <!-- Marker Icon -->
            <div class="map-marker quest" style="
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                width: 40px;
                height: 40px;
                border-radius: 50% 50% 50% 0;
                transform: rotate(-45deg);
                box-shadow: 0 4px 12px rgba(0,0,0,0.3);
                display: flex;
                align-items: center;
                justify-content: center;
                position: relative;
            ">
                <span class="material-symbols-outlined" style="
                    transform: rotate(45deg);
                    color: white;
                    font-size: 1.5rem;
                ">priority_high</span>
            </div>
        </div>
    `;

    mapCanvas.appendChild(previewMarker);

    // Erstelle Bestätigungs-Dialog
    const confirmDialog = document.createElement('div');
    confirmDialog.id = 'markerConfirmDialog';
    confirmDialog.style.cssText = `
        position: fixed;
        bottom: 30px;
        left: 50%;
        transform: translateX(-50%);
        background: white;
        padding: 1.5rem;
        border-radius: 12px;
        box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        z-index: 10001;
        display: flex;
        flex-direction: column;
        gap: 1rem;
        min-width: 300px;
        animation: slideUp 0.3s ease-out;
    `;
    confirmDialog.innerHTML = `
        <div style="text-align: center;">
            <div style="font-weight: 600; font-size: 1.1rem; margin-bottom: 0.5rem;">
                Marker hier setzen?
            </div>
            <div style="color: #666; font-size: 0.9rem;">
                Position: ${x.toFixed(1)}% / ${y.toFixed(1)}%
            </div>
        </div>
        <div style="display: flex; gap: 0.75rem;">
            <button onclick="confirmMarkerPosition(${x}, ${y})" style="
                flex: 1;
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: white;
                border: none;
                padding: 0.75rem 1.5rem;
                border-radius: 8px;
                font-weight: 600;
                cursor: pointer;
                transition: transform 0.2s;
            " onmouseover="this.style.transform='scale(1.05)'" onmouseout="this.style.transform='scale(1)'">
                ✓ Bestätigen
            </button>
            <button onclick="removeMarkerPreview()" style="
                flex: 1;
                background: #f3f4f6;
                color: #374151;
                border: none;
                padding: 0.75rem 1.5rem;
                border-radius: 8px;
                font-weight: 600;
                cursor: pointer;
                transition: transform 0.2s;
            " onmouseover="this.style.transform='scale(1.05)'" onmouseout="this.style.transform='scale(1)'">
                ↻ Neu platzieren
            </button>
        </div>
    `;
    document.body.appendChild(confirmDialog);

    // Animationen hinzufügen
    const style = document.createElement('style');
    style.id = 'markerPreviewStyles';
    style.textContent = `
        @keyframes pulse {
            0%, 100% {
                transform: translate(-50%, -50%) scale(1);
                opacity: 0.6;
            }
            50% {
                transform: translate(-50%, -50%) scale(1.3);
                opacity: 0.2;
            }
        }
        @keyframes slideUp {
            from {
                transform: translateX(-50%) translateY(20px);
                opacity: 0;
            }
            to {
                transform: translateX(-50%) translateY(0);
                opacity: 1;
            }
        }
    `;
    if (!document.getElementById('markerPreviewStyles')) {
        document.head.appendChild(style);
    }
}

function confirmMarkerPosition(x, y) {
    // Koordinaten in SessionStorage speichern
    sessionStorage.setItem('questMarkerCoordinates', JSON.stringify({ x, y }));

    // Zurück zur Quest-Seite
    window.location.href = '/Spielmodus/Quests';
}

function removeMarkerPreview() {
    // Entferne Vorschau und Dialog
    const preview = document.getElementById('questMarkerPreview');
    const dialog = document.getElementById('markerConfirmDialog');

    if (preview) preview.remove();
    if (dialog) dialog.remove();
}

// ===== MAP CLICK (Marker erstellen - nur Gott) =====

function handleMapClick(e) {
    // Wenn auf Marker geklickt, ignorieren
    if (e.target.closest('.map-marker')) return;

    const mapCanvas = document.getElementById('mapCanvas');
    if (!mapCanvas) return;

    const rect = mapCanvas.getBoundingClientRect();

    // Klick-Position relativ zum Layer (0-100%)
    const x = ((e.clientX - rect.left) / rect.width) * 100;
    const y = ((e.clientY - rect.top) / rect.height) * 100;

    // Debug
    console.log('Klick auf Layer:', {
        x: x.toFixed(2),
        y: y.toFixed(2),
        clientX: e.clientX,
        clientY: e.clientY,
        rectLeft: rect.left,
        rectTop: rect.top,
        rectWidth: rect.width,
        rectHeight: rect.height
    });

    // Validierung: Position muss im Bereich 0-100 sein
    if (x < 0 || x > 100 || y < 0 || y > 100) {
        console.log('Position außerhalb:', { x, y });
        return;
    }

    pendingPosition = { x, y };
    showCreateMarkerForm();
}

// ===== MARKER ERSTELLEN FORMULAR =====

function showCreateMarkerForm() {
    const popup = document.getElementById('markerPopup');
    const title = document.getElementById('popupTitle');
    const body = document.getElementById('popupBody');

    title.textContent = selectedMarkerType === 'quest' ? 'Quest zuweisen' : 'Punkt benennen';

    let formHtml = '<form id="createMarkerForm">';

    if (selectedMarkerType === 'quest') {
        // Quest-Dropdown
        if (MAP_DATA.activeQuests.length === 0) {
            formHtml += '<p class="error-message">Keine aktiven Quests verfügbar!</p>';
        } else {
            formHtml += `
                <div class="form-group">
                    <label>Quest auswählen</label>
                    <select name="questId" required>
                        <option value="">Bitte wählen...</option>
                        ${MAP_DATA.activeQuests.map(q => `<option value="${q.id}">${q.title}</option>`).join('')}
                    </select>
                </div>
            `;
        }
    } else {
        // Label + Description
        formHtml += `
            <div class="form-group">
                <label>${selectedMarkerType === 'settlement' ? 'Ortsname' : selectedMarkerType === 'danger' ? 'Gefahrenquelle' : 'Titel'}</label>
                <input type="text" name="label" required autofocus />
            </div>
        `;

        if (selectedMarkerType === 'info') {
            formHtml += `
                <div class="form-group">
                    <label>Beschreibung</label>
                    <textarea name="description" rows="3" required></textarea>
                </div>
            `;
        }
    }

    formHtml += `
        <div class="form-actions">
            <button type="submit" class="btn-primary" ${MAP_DATA.activeQuests.length === 0 && selectedMarkerType === 'quest' ? 'disabled' : ''}>
                Punkt setzen
            </button>
            <button type="button" class="btn-secondary" onclick="closePopup()">Abbrechen</button>
        </div>
    </form>
    `;

    body.innerHTML = formHtml;
    popup.style.display = 'flex';

    // Form Submit
    document.getElementById('createMarkerForm')?.addEventListener('submit', handleCreateMarker);
}

// ===== MARKER ERSTELLEN =====

async function handleCreateMarker(e) {
    e.preventDefault();

    const formData = new FormData(e.target);
    let label = formData.get('label') || '';
    const description = formData.get('description') || null;
    const questId = formData.get('questId');

    // Bei Quest: Label = Quest-Titel
    if (selectedMarkerType === 'quest' && questId) {
        const quest = MAP_DATA.activeQuests.find(q => q.id == questId);
        if (quest) {
            label = quest.title;
        }
    }

    const requestData = {
        mapId: MAP_DATA.mapId,
        label: label,
        type: selectedMarkerType,
        xPercent: pendingPosition.x,
        yPercent: pendingPosition.y,
        description: description,
        questId: questId ? parseInt(questId) : null
    };

    try {
        const response = await fetch('/api/game/markers', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestData)
        });

        if (response.ok) {
            closePopup();
            location.reload(); // Seite neu laden
        } else {
            const error = await response.json();
            alert(`Fehler: ${error.error || 'Marker konnte nicht erstellt werden'}`);
        }
    } catch (error) {
        console.error('Fehler:', error);
        alert('Fehler beim Erstellen des Markers!');
    }
}

// ===== MARKER DETAILS ANZEIGEN =====

function showMarkerDetails(markerId) {
    const marker = MAP_DATA.markers.find(m => m.id == markerId);
    if (!marker) return;

    const popup = document.getElementById('markerPopup');
    const title = document.getElementById('popupTitle');
    const body = document.getElementById('popupBody');

    // Für Quest-Marker: Zeige Quest-Titel als Überschrift
    title.textContent = marker.type === 'quest' && marker.questTitle
        ? marker.questTitle
        : marker.label;

    let detailsHtml = `
        <div class="marker-details">
            <div class="marker-type-badge marker-${marker.type}">
                <span class="material-symbols-outlined">${getMarkerIcon(marker.type)}</span>
                <span>${getMarkerTypeLabel(marker.type)}</span>
            </div>
    `;

    // Quest-spezifische Informationen
    if (marker.type === 'quest' && marker.questTitle) {
        detailsHtml += `
            <div class="quest-info">
                <p class="quest-info-label">Quest für:</p>
                <p class="quest-info-value">${
                    marker.questType === 'group'
                        ? '<strong>Alle Gefährten der Gruppe</strong>'
                        : marker.questCharacterName || 'Unbekannt'
                }</p>
            </div>
        `;

        // Link zur Quest-Seite
        if (marker.questId) {
            detailsHtml += `
                <div class="quest-link">
                    <a href="/Spielmodus/Quests?focusQuestId=${marker.questId}" class="btn-quest-link">
                        <span class="material-symbols-outlined">menu_book</span>
                        <span>Quest in Chronik öffnen</span>
                    </a>
                </div>
            `;
        }
    }

    if (marker.description) {
        detailsHtml += `
            <div class="marker-description">
                <p class="description-label">Information:</p>
                <p>${marker.description}</p>
            </div>
        `;
    }

    // Gott: Löschen-Button
    if (MAP_DATA.isGod) {
        detailsHtml += `
            <div class="marker-actions">
                <button class="btn-danger" onclick="deleteMarker(${markerId})">
                    <span class="material-symbols-outlined">delete</span>
                    <span>Marker löschen</span>
                </button>
            </div>
        `;
    }

    detailsHtml += '</div>';
    body.innerHTML = detailsHtml;
    popup.style.display = 'flex';
}

// ===== MARKER LÖSCHEN =====

async function deleteMarker(markerId) {
    if (!confirm('Diesen Marker wirklich löschen?')) return;

    try {
        const response = await fetch(`/api/game/markers/${markerId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            closePopup();
            location.reload();
        } else {
            const error = await response.json();
            alert(`Fehler: ${error.error || 'Marker konnte nicht gelöscht werden'}`);
        }
    } catch (error) {
        console.error('Fehler:', error);
        alert('Fehler beim Löschen!');
    }
}

// ===== ACT AKTIVIEREN =====

async function handleActivateAct(e) {
    const btn = e.currentTarget;
    const actId = btn.dataset.actId;

    if (!confirm('Diesen Act aktivieren? (Der aktuelle Act wird deaktiviert)')) return;

    try {
        const response = await fetch(`/api/game/acts/${actId}/activate`, {
            method: 'POST'
        });

        if (response.ok) {
            location.reload();
        } else {
            const error = await response.json();
            alert(`Fehler: ${error.error || 'Act konnte nicht aktiviert werden'}`);
        }
    } catch (error) {
        console.error('Fehler:', error);
        alert('Fehler beim Aktivieren!');
    }
}

// ===== POPUP =====

function closePopup() {
    const popup = document.getElementById('markerPopup');
    popup.style.display = 'none';
    pendingPosition = null;
}

// ===== HELPERS =====

function getMarkerIcon(type) {
    const icons = {
        quest: 'priority_high',
        info: 'info',
        danger: 'skull',
        settlement: 'fort'
    };
    return icons[type] || 'location_on';
}

function getMarkerTypeLabel(type) {
    const labels = {
        quest: 'Quest',
        info: 'Information',
        danger: 'Gefahr',
        settlement: 'Ort'
    };
    return labels[type] || type;
}

// ===== MARKER FOKUS =====

function focusOnMarker(markerId) {
    const marker = document.querySelector(`[data-marker-id="${markerId}"]`);
    if (!marker) return;

    // Scroll zum Marker
    marker.scrollIntoView({ behavior: 'smooth', block: 'center' });

    // Highlight-Effekt
    marker.classList.add('marker-focused');

    // Pulse-Animation
    const style = document.createElement('style');
    style.id = 'markerFocusStyles';
    style.textContent = `
        .marker-focused {
            animation: markerFocusPulse 2s ease-in-out 3;
        }
        @keyframes markerFocusPulse {
            0%, 100% {
                transform: translate(-50%, -100%) scale(1);
            }
            50% {
                transform: translate(-50%, -100%) scale(1.3);
            }
        }
        .marker-focused::before {
            content: '';
            position: absolute;
            width: 80px;
            height: 80px;
            border-radius: 50%;
            background: rgba(102, 126, 234, 0.3);
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            animation: focusRingPulse 2s ease-in-out 3;
            z-index: -1;
        }
        @keyframes focusRingPulse {
            0%, 100% {
                transform: translate(-50%, -50%) scale(1);
                opacity: 0.6;
            }
            50% {
                transform: translate(-50%, -50%) scale(1.5);
                opacity: 0;
            }
        }
    `;
    if (!document.getElementById('markerFocusStyles')) {
        document.head.appendChild(style);
    }

    // Nach 6 Sekunden entfernen
    setTimeout(() => {
        marker.classList.remove('marker-focused');
    }, 6000);
}

// ===== MARKER DRAG & DROP =====

let draggedMarker = null;
let dragOffset = { x: 0, y: 0 };
let originalPosition = { x: 0, y: 0 };

function makeMarkerDraggable(marker) {
    marker.style.cursor = 'move';
    marker.draggable = false; // Verhindere native Drag-Funktionalität

    let isDragging = false;
    let clickTimeout = null;

    marker.addEventListener('mousedown', (e) => {
        // Nur linke Maustaste
        if (e.button !== 0) return;

        // Verzögerung für Drag-Start (um Klicks zu ermöglichen)
        clickTimeout = setTimeout(() => {
            isDragging = true;
            draggedMarker = marker;

            const mapCanvas = document.getElementById('mapCanvas');
            const rect = mapCanvas.getBoundingClientRect();

            // Aktuelle Position speichern
            const currentLeft = parseFloat(marker.style.left);
            const currentTop = parseFloat(marker.style.top);
            originalPosition = { x: currentLeft, y: currentTop };

            // Offset berechnen
            const markerX = (currentLeft / 100) * rect.width;
            const markerY = (currentTop / 100) * rect.height;
            dragOffset.x = e.clientX - rect.left - markerX;
            dragOffset.y = e.clientY - rect.top - markerY;

            // Visuelles Feedback
            marker.style.opacity = '0.7';
            marker.style.zIndex = '9999';

            e.preventDefault();
        }, 150); // 150ms Verzögerung
    });

    marker.addEventListener('mouseup', () => {
        clearTimeout(clickTimeout);
        if (!isDragging) return;

        endDrag();
    });
}

// Globale Mouse-Events
document.addEventListener('mousemove', (e) => {
    if (!draggedMarker) return;

    const mapCanvas = document.getElementById('mapCanvas');
    const rect = mapCanvas.getBoundingClientRect();

    // Neue Position berechnen (in Prozent)
    const newX = ((e.clientX - rect.left - dragOffset.x) / rect.width) * 100;
    const newY = ((e.clientY - rect.top - dragOffset.y) / rect.height) * 100;

    // Begrenzung auf Karte (0-100%)
    const clampedX = Math.max(0, Math.min(100, newX));
    const clampedY = Math.max(0, Math.min(100, newY));

    // Marker verschieben
    draggedMarker.style.left = `${clampedX}%`;
    draggedMarker.style.top = `${clampedY}%`;
});

document.addEventListener('mouseup', () => {
    if (!draggedMarker) return;
    endDrag();
});

async function endDrag() {
    if (!draggedMarker) return;

    const marker = draggedMarker;
    const markerId = marker.dataset.markerId;
    const newX = parseFloat(marker.style.left);
    const newY = parseFloat(marker.style.top);

    // Visuelles Feedback zurücksetzen
    marker.style.opacity = '1';
    marker.style.zIndex = '';

    // Prüfe ob Position geändert wurde
    if (Math.abs(newX - originalPosition.x) < 0.1 && Math.abs(newY - originalPosition.y) < 0.1) {
        // Keine signifikante Änderung - behandle als Klick
        draggedMarker = null;
        return;
    }

    // Position an Server senden
    try {
        const response = await fetch(`/api/game/markers/${markerId}/position`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                xPercent: newX,
                yPercent: newY
            })
        });

        if (!response.ok) {
            // Bei Fehler: Zurück zur ursprünglichen Position
            marker.style.left = `${originalPosition.x}%`;
            marker.style.top = `${originalPosition.y}%`;
            alert('Fehler beim Aktualisieren der Marker-Position!');
        } else {
            // Erfolg-Feedback
            showSuccessToast('Marker-Position aktualisiert');
        }
    } catch (error) {
        console.error('Fehler:', error);
        // Bei Fehler: Zurück zur ursprünglichen Position
        marker.style.left = `${originalPosition.x}%`;
        marker.style.top = `${originalPosition.y}%`;
        alert('Fehler beim Aktualisieren!');
    }

    draggedMarker = null;
}

function showSuccessToast(message) {
    const toast = document.createElement('div');
    toast.style.cssText = `
        position: fixed;
        bottom: 30px;
        right: 30px;
        background: linear-gradient(135deg, #10b981 0%, #059669 100%);
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.2);
        z-index: 10000;
        display: flex;
        align-items: center;
        gap: 0.75rem;
        animation: slideInRight 0.3s ease-out;
    `;
    toast.innerHTML = `
        <span class="material-symbols-outlined">check_circle</span>
        <span>${message}</span>
    `;
    document.body.appendChild(toast);

    // Animation
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideInRight {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
    `;
    document.head.appendChild(style);

    // Nach 3 Sekunden entfernen
    setTimeout(() => {
        toast.style.animation = 'slideInRight 0.3s ease-out reverse';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}