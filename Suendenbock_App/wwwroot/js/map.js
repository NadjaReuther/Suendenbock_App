// wwwroot/js/map.js

let selectedMarkerType = 'quest';
let pendingPosition = null;

// ===== INITIALIZATION =====

document.addEventListener('DOMContentLoaded', () => {
    initMap();
});

function initMap() {
    const mapCanvas = document.getElementById('mapCanvas');
    const isGod = MAP_DATA.isGod;

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
    });

    // Popup schließen
    document.getElementById('closePopup')?.addEventListener('click', closePopup);
    document.getElementById('markerPopup')?.addEventListener('click', (e) => {
        if (e.target.id === 'markerPopup') {
            closePopup();
        }
    });
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
        description: description
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

    title.textContent = marker.label;

    let detailsHtml = `
        <div class="marker-details">
            <div class="marker-type-badge marker-${marker.type}">
                <span class="material-symbols-outlined">${getMarkerIcon(marker.type)}</span>
                <span>${getMarkerTypeLabel(marker.type)}</span>
            </div>
    `;

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