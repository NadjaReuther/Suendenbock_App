// wwwroot/js/quests.js

let selectedCharacters = [];
let currentTab = 'all';

// ===== INITIALIZATION =====

document.addEventListener('DOMContentLoaded', () => {
    initQuests();

    // Focus auf initialen Quest (wenn von Karte)
    if (typeof initialFocusQuestId !== 'undefined' && initialFocusQuestId) {
        currentTab = 'all';
        const activeTab = document.querySelector('.quest-tab.active');
        if (activeTab) activeTab.classList.remove('active');
        document.querySelector('[data-status="all"]').classList.add('active');

        setTimeout(() => {
            const questCard = document.querySelector(`[data-quest-id="${initialFocusQuestId}"]`);
            if (questCard) {
                questCard.classList.add('expanded');
                questCard.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        }, 100);
    }
});

function initQuests() {
    // Tab Buttons
    document.querySelectorAll('.quest-tab').forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.quest-tab').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            currentTab = btn.dataset.status;
            filterQuests();
        });
    });

    // Character Filter Buttons
    document.querySelectorAll('.character-filter-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            const character = btn.dataset.character;
            if (selectedCharacters.includes(character)) {
                selectedCharacters = selectedCharacters.filter(c => c !== character);
                btn.classList.remove('active');
            } else {
                selectedCharacters.push(character);
                btn.classList.add('active');
            }
            filterQuests();
        });
    });

    // Quest Card Click (Expand/Collapse)
    document.querySelectorAll('.quest-card-header').forEach(header => {
        header.addEventListener('click', () => {
            const card = header.closest('.quest-card');
            card.classList.toggle('expanded');

            // Chevron Icon wechseln
            const chevron = header.querySelector('.quest-chevron');
            if (card.classList.contains('expanded')) {
                chevron.textContent = 'expand_more';
                chevron.classList.add('rotated');
            } else {
                chevron.textContent = 'chevron_right';
                chevron.classList.remove('rotated');
            }
        });
    });

    // Status Change Buttons
    document.querySelectorAll('.status-btn').forEach(btn => {
        btn.addEventListener('click', async (e) => {
            e.stopPropagation(); // Verhindert Expand/Collapse
            const questId = btn.dataset.questId;
            const newStatus = btn.dataset.newStatus;

            // Check if this is an edit or delete button (no data-new-status)
            if (!newStatus) {
                const buttonText = btn.textContent.trim();
                if (buttonText.includes('bearbeiten')) {
                    handleEditQuest(questId);
                    return;
                }
                if (buttonText.includes('löschen')) {
                    handleDeleteQuest(questId);
                    return;
                }
                return; // Unknown button type, do nothing
            }

            // Otherwise, it's a status change
            await updateQuestStatus(questId, newStatus);
        });
    });

    // Modal Buttons
    document.getElementById('addQuestBtn').addEventListener('click', openModal);
    document.getElementById('closeModalBtn').addEventListener('click', closeModal);
    document.querySelector('.quest-modal').addEventListener('click', (e) => {
        if (e.target.classList.contains('quest-modal')) {
            closeModal();
        }
    });

    // Quest Form
    document.getElementById('questForm').addEventListener('submit', handleQuestSubmit);

    // Type-Änderung: Character-Assignment ein/ausblenden
    document.getElementById('questType').addEventListener('change', (e) => {
        const characterGroup = document.getElementById('characterAssignmentGroup');
        if (e.target.value === 'group') {
            characterGroup.style.display = 'none';
            document.getElementById('questCharacter').required = false;
        } else {
            characterGroup.style.display = 'block';
            document.getElementById('questCharacter').required = true;
        }
    });

    // Vorgänger-Quest: Requirement-Gruppe ein/ausblenden
    document.getElementById('questPreviousQuest').addEventListener('change', (e) => {
        const requirementGroup = document.getElementById('questRequirementGroup');
        if (e.target.value) {
            // Vorgänger-Quest ausgewählt → zeige Requirement-Dropdown
            requirementGroup.style.display = 'block';
        } else {
            // Keine Vorgänger-Quest → verstecke Requirement
            requirementGroup.style.display = 'none';
        }
    });

    // Marker-Checkbox: Button-Gruppe ein/ausblenden
    document.getElementById('questCreateMarker').addEventListener('change', (e) => {
        const buttonGroup = document.getElementById('markerSetButtonGroup');
        if (e.target.checked) {
            buttonGroup.style.display = 'block';
        } else {
            buttonGroup.style.display = 'none';
            // Koordinaten zurücksetzen
            document.getElementById('questMarkerX').value = '';
            document.getElementById('questMarkerY').value = '';
        }
    });

    // "Zur Karte" Button: Quest-Daten speichern und zur Map weiterleiten
    document.getElementById('setMarkerOnMapBtn').addEventListener('click', async () => {
        // Sammle aktuelle Formular-Daten
        const form = document.getElementById('questForm');
        const isEditMode = form.dataset.editMode === 'true';
        const editQuestId = form.dataset.editQuestId;

        const questData = {
            title: document.getElementById('questTitle').value,
            description: document.getElementById('questDescription').value,
            type: document.getElementById('questType').value,
            status: document.getElementById('questStatus').value,
            characterId: document.getElementById('questCharacter').value,
            previousQuestId: document.getElementById('questPreviousQuest').value,
            previousQuestRequirement: document.getElementById('questPreviousQuestRequirement').value,
            isEditMode: isEditMode,
            editQuestId: editQuestId
        };

        // Validierung
        if (!questData.title) {
            await Swal.fire({
                icon: 'warning',
                title: 'Titel fehlt',
                text: 'Bitte gib einen Quest-Titel ein!',
                confirmButtonColor: '#d97706'
            });
            return;
        }
        if (questData.type === 'individual' && !questData.characterId) {
            await Swal.fire({
                icon: 'warning',
                title: 'Charakter fehlt',
                text: 'Bitte wähle einen Charakter aus!',
                confirmButtonColor: '#d97706'
            });
            return;
        }

        // In SessionStorage speichern
        sessionStorage.setItem('pendingQuestData', JSON.stringify(questData));
        sessionStorage.setItem('questMarkerMode', 'true');

        // Zur Map weiterleiten
        window.location.href = '/Spielmodus/Map';
    });

    // Beim Laden: Prüfe ob Marker-Koordinaten von Map zurückkommen
    checkForMarkerCoordinates();
}

// ===== MARKER COORDINATES FROM MAP =====

function checkForMarkerCoordinates() {
    // Prüfe ob wir von der Map zurückkommen mit Marker-Koordinaten
    const markerCoords = sessionStorage.getItem('questMarkerCoordinates');
    const questData = sessionStorage.getItem('pendingQuestData');

    if (markerCoords && questData) {
        const coords = JSON.parse(markerCoords);
        const data = JSON.parse(questData);

        // Modal öffnen
        openModal();

        // Formular mit gespeicherten Daten füllen
        document.getElementById('questTitle').value = data.title;
        document.getElementById('questDescription').value = data.description;
        document.getElementById('questType').value = data.type;
        document.getElementById('questStatus').value = data.status;

        if (data.characterId) {
            document.getElementById('questCharacter').value = data.characterId;
        }
        if (data.previousQuestId) {
            document.getElementById('questPreviousQuest').value = data.previousQuestId;
            // Requirement-Gruppe anzeigen wenn Vorgänger-Quest gesetzt
            document.getElementById('questRequirementGroup').style.display = 'block';
            if (data.previousQuestRequirement) {
                document.getElementById('questPreviousQuestRequirement').value = data.previousQuestRequirement;
            }
        }

        // Marker-Checkbox aktivieren
        document.getElementById('questCreateMarker').checked = true;
        document.getElementById('markerSetButtonGroup').style.display = 'block';

        // Marker-Koordinaten setzen
        document.getElementById('questMarkerX').value = coords.x;
        document.getElementById('questMarkerY').value = coords.y;

        // Button-Text aktualisieren
        document.getElementById('markerBtnText').textContent = `✓ Marker gesetzt (${coords.x.toFixed(1)}%, ${coords.y.toFixed(1)}%)`;

        // Character-Gruppe anzeigen falls individual
        const characterGroup = document.getElementById('characterAssignmentGroup');
        if (data.type === 'individual') {
            characterGroup.style.display = 'block';
            document.getElementById('questCharacter').required = true;
        } else {
            characterGroup.style.display = 'none';
            document.getElementById('questCharacter').required = false;
        }

        // Edit-Modus setzen falls vorhanden
        if (data.isEditMode && data.editQuestId) {
            const form = document.getElementById('questForm');
            form.dataset.editMode = 'true';
            form.dataset.editQuestId = data.editQuestId;
            document.querySelector('.modal-header-quest h3').textContent = 'Quest bearbeiten';
            document.querySelector('.submit-btn').textContent = 'Änderungen speichern';
        }

        // SessionStorage aufräumen
        sessionStorage.removeItem('questMarkerCoordinates');
        sessionStorage.removeItem('pendingQuestData');
        sessionStorage.removeItem('questMarkerMode');
    }
}

// ===== FILTER LOGIC =====

function filterQuests() {
    const cards = document.querySelectorAll('.quest-card');
    let visibleCount = 0;

    cards.forEach(card => {
        const status = card.dataset.status;
        const type = card.dataset.type;
        const character = card.dataset.character;

        // Status-Filter (Tab)
        let statusMatch = currentTab === 'all' || status === currentTab;

        // Character-Filter
        let characterMatch = true;
        if (selectedCharacters.length > 0) {
            if (type === 'group') {
                characterMatch = true; // Gruppenquests immer anzeigen
            } else {
                characterMatch = selectedCharacters.includes(character);
            }
        }

        if (statusMatch && characterMatch) {
            card.style.display = 'block';
            visibleCount++;
        } else {
            card.style.display = 'none';
        }
    });

    // Empty State
    const emptyState = document.getElementById('emptyState');
    if (visibleCount === 0) {
        emptyState.style.display = 'flex';
    } else {
        emptyState.style.display = 'none';
    }
}

// ===== API CALLS =====

async function updateQuestStatus(questId, newStatus) {
    try {
        const response = await fetch(`/api/game/quests/${questId}/status`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newStatus)
        });

        if (response.ok) {
            // Seite neu laden (einfachste Variante)
            location.reload();
        } else {
            const error = await response.json();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: error.error || 'Status konnte nicht geändert werden',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Fehler beim Ändern des Status!',
            confirmButtonColor: '#d97706'
        });
    }
}

async function handleQuestSubmit(e) {
    e.preventDefault();

    const form = e.target;
    const isEditMode = form.dataset.editMode === 'true';
    const editQuestId = form.dataset.editQuestId;

    const formData = {
        title: document.getElementById('questTitle').value,
        description: document.getElementById('questDescription').value,
        type: document.getElementById('questType').value,
        status: document.getElementById('questStatus').value,
        characterId: null,
        previousQuestId: null,
        previousQuestRequirement: null,
        createMarker: false,
        markerXPercent: null,
        markerYPercent: null
    };

    // CharacterId nur bei individual-Quests
    if (formData.type === 'individual') {
        const characterId = document.getElementById('questCharacter').value;
        if (!characterId) {
            await Swal.fire({
                icon: 'warning',
                title: 'Charakter fehlt',
                text: 'Bitte wähle einen Charakter aus!',
                confirmButtonColor: '#d97706'
            });
            return;
        }
        formData.characterId = parseInt(characterId);
    }

    // PreviousQuestId (optional)
    const previousQuestId = document.getElementById('questPreviousQuest').value;
    if (previousQuestId) {
        formData.previousQuestId = parseInt(previousQuestId);
        // PreviousQuestRequirement (nur wenn PreviousQuestId gesetzt)
        const previousQuestRequirement = document.getElementById('questPreviousQuestRequirement').value;
        formData.previousQuestRequirement = previousQuestRequirement || 'both';
    }

    // Questmarker-Daten (optional)
    const createMarker = document.getElementById('questCreateMarker').checked;
    if (createMarker) {
        const markerX = document.getElementById('questMarkerX').value;
        const markerY = document.getElementById('questMarkerY').value;

        if (!markerX || !markerY) {
            await Swal.fire({
                icon: 'warning',
                title: 'Koordinaten fehlen',
                text: 'Bitte gib X- und Y-Koordinaten für den Marker an!',
                confirmButtonColor: '#d97706'
            });
            return;
        }

        formData.createMarker = true;
        formData.markerXPercent = parseFloat(markerX);
        formData.markerYPercent = parseFloat(markerY);
    }

    try {
        let response;
        if (isEditMode) {
            // Edit mode: POST to EditQuest
            response = await fetch(`/Spielmodus/EditQuest?id=${editQuestId}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData)
            });
        } else {
            // Create mode: POST to API
            response = await fetch('/api/game/quests', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData)
            });
        }

        if (response.ok) {
            closeModal();
            location.reload(); // Seite neu laden
        } else {
            const errorText = await response.text();
            let errorMsg;
            try {
                const errorJson = JSON.parse(errorText);
                errorMsg = errorJson.error || errorText;
            } catch {
                errorMsg = errorText;
            }
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: errorMsg || (isEditMode ? 'Quest konnte nicht aktualisiert werden' : 'Quest konnte nicht erstellt werden'),
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: `Fehler beim ${isEditMode ? 'Aktualisieren' : 'Erstellen'} der Quest!`,
            confirmButtonColor: '#d97706'
        });
    }
}

// ===== MODAL =====

function openModal() {
    document.getElementById('questModal').style.display = 'flex';
    document.getElementById('questTitle').focus();
}

function closeModal() {
    document.getElementById('questModal').style.display = 'none';
    document.getElementById('questForm').reset();

    // Reset marker button group
    document.getElementById('markerSetButtonGroup').style.display = 'none';
    document.getElementById('markerBtnText').textContent = 'Zur Karte → Marker setzen';
    document.getElementById('questMarkerX').value = '';
    document.getElementById('questMarkerY').value = '';

    // Remove edit mode
    delete document.getElementById('questForm').dataset.editMode;
    delete document.getElementById('questForm').dataset.editQuestId;
    document.querySelector('.modal-header-quest h3').textContent = 'Neue Quest anlegen';
    document.querySelector('.submit-btn').textContent = 'In die Chronik aufnehmen';
}

// ===== EDIT & DELETE =====

function handleEditQuest(questId) {
    // Find the quest card
    const questCard = document.querySelector(`[data-quest-id="${questId}"]`);
    if (!questCard) return;

    // Extract current data from the card
    const title = questCard.querySelector('.quest-title').textContent.trim();
    const description = questCard.querySelector('.quest-description').textContent.trim();
    const typeBadge = questCard.querySelector('.quest-type-badge').textContent.trim();
    const type = typeBadge.includes('Gruppenquest') ? 'group' : 'individual';
    const status = questCard.dataset.status;

    // Get character if individual quest
    const assignedCharacter = questCard.querySelector('.assigned-character:not([style*="italic"])');
    let characterName = assignedCharacter ? assignedCharacter.textContent.trim() : null;

    // Populate modal
    document.getElementById('questTitle').value = title;
    document.getElementById('questDescription').value = description === 'Keine Beschreibung hinterlegt.' ? '' : description;
    document.getElementById('questType').value = type;
    document.getElementById('questStatus').value = status;

    // Handle character assignment
    const characterGroup = document.getElementById('characterAssignmentGroup');
    const characterSelect = document.getElementById('questCharacter');
    if (type === 'group') {
        characterGroup.style.display = 'none';
        characterSelect.required = false;
        characterSelect.value = '';
    } else {
        characterGroup.style.display = 'block';
        characterSelect.required = true;
        // Find matching character in dropdown
        if (characterName) {
            const options = characterSelect.querySelectorAll('option');
            for (let option of options) {
                if (option.textContent.trim() === characterName) {
                    characterSelect.value = option.value;
                    break;
                }
            }
        }
    }

    // Reset previous quest (kann nicht automatisch gesetzt werden, da nicht im dataset)
    document.getElementById('questPreviousQuest').value = '';

    // Reset marker fields
    document.getElementById('questCreateMarker').checked = false;
    document.getElementById('markerSetButtonGroup').style.display = 'none';
    document.getElementById('markerBtnText').textContent = 'Zur Karte → Marker setzen';
    document.getElementById('questMarkerX').value = '';
    document.getElementById('questMarkerY').value = '';

    // Mark as edit mode
    const form = document.getElementById('questForm');
    form.dataset.editMode = 'true';
    form.dataset.editQuestId = questId;

    // Update modal title
    document.querySelector('.modal-header-quest h3').textContent = 'Quest bearbeiten';
    document.querySelector('.submit-btn').textContent = 'Änderungen speichern';

    // Open modal
    openModal();
}

async function handleDeleteQuest(questId) {
    const questCard = document.querySelector(`[data-quest-id="${questId}"]`);
    const questTitle = questCard ? questCard.querySelector('.quest-title').textContent.trim() : 'diese Quest';

    const result = await Swal.fire({
        title: 'Quest löschen?',
        text: `Quest "${questTitle}" wirklich löschen? Diese Aktion kann nicht rückgängig gemacht werden.`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#d97706',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Löschen',
        cancelButtonText: 'Abbrechen'
    });

    if (!result.isConfirmed) {
        return;
    }

    try {
        const response = await fetch(`/Spielmodus/DeleteQuest?id=${questId}`, {
            method: 'POST'
        });

        if (response.ok) {
            location.reload();
        } else {
            const text = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler beim Löschen',
                text: text || 'Unbekannter Fehler',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Fehler beim Löschen der Quest!',
            confirmButtonColor: '#d97706'
        });
    }
}
