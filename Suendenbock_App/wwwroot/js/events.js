// ==== Initialization ====
// Prevent multiple initializations
if (!window.eventsJsInitialized) {
    window.eventsJsInitialized = true;

    document.addEventListener('DOMContentLoaded', () => {
        initEventListeners();
    });
}

function initEventListeners() {
    // RSVP Buttons
    document.querySelectorAll('.rsvp-btn').forEach(btn => {
        btn.addEventListener('click', handleRsvp);
    });

    // Toggle Chores
    document.querySelectorAll('.toggle-chores').forEach(btn => {
        btn.addEventListener('click', toggleChoreMatrix);
    });

    // Toggle Teilnehmer-Badges
    document.querySelectorAll('.toggle-participants-btn').forEach(btn => {
        btn.addEventListener('click', toggleParticipantBadges);
    });

    // Create Event Button (nur für Admins)
    const createBtn = document.getElementById('createEventBtn');
    if (createBtn) {
        createBtn.addEventListener('click', () => openEventModal());
    }

    // Edit Event Buttons
    document.querySelectorAll('.edit-event-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const eventId = e.currentTarget.dataset.eventId;
            openEventModal(eventId);
        });
    });

    // Delete Event Buttons
    document.querySelectorAll('.delete-event-btn').forEach(btn => {
        btn.addEventListener('click', handleDeleteEvent);
    });
}

// ==== RSVP Handling ====
async function handleRsvp(e) {
    const btn = e.currentTarget;
    const eventId = btn.dataset.eventId;
    const status = btn.dataset.status;

    try {
        const response = await fetch('/api/events/rsvp', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ eventId: parseInt(eventId), status: status })
        });

        if (response.ok) {
            // Sofort Feedback: Button aktiv schalten
            const eventCard = btn.closest('.event-card');
            eventCard.querySelectorAll('.rsvp-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');

            // Seite neu laden damit Teilnehmer-Liste und Button-Zustand synchron bleiben
            setTimeout(() => location.reload(), 400);
        }
        else {
            const error = await response.json();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: error.error || 'RSVP konnte nicht gespeichert werden',
                confirmButtonColor: '#d97706'
            });
        }
    }
    catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Fehler beim Speichern des RSVP!',
            confirmButtonColor: '#d97706'
        });
    }
}

// ====  CHORE MATRIX TOGGLE ====
function toggleChoreMatrix(e) {
    const btn = e.currentTarget;
    const eventId = btn.dataset.eventId;
    const choreMatrix = document.querySelector(`.chore-matrix[data-event-id="${eventId}"]`);
    const icon = btn.querySelector('.material-symbols-outlined');

    if (choreMatrix.style.display === 'none' || !choreMatrix.style.display) {
        // Show
        choreMatrix.style.display = 'block';
        icon.textContent = 'visibility_off';
        btn.innerHTML = '<span class="material-symbols-outlined">visibility_off</span> Dienstplan ausblenden';
    }
    else {
        // Hide
        choreMatrix.style.display = 'none';
        icon.textContent = 'visibility';
        btn.innerHTML = '<span class="material-symbols-outlined">visibility</span> Dienstplan einsehen';
    }
}

// ==== TEILNEHMER TOGGLE ====
function toggleParticipantBadges(e) {
    const btn = e.currentTarget;
    const eventId = btn.dataset.eventId;
    const badgesContainer = document.querySelector(`.participant-badges[data-event-id="${eventId}"]`);

    if (badgesContainer.style.display === 'none' || !badgesContainer.style.display) {
        badgesContainer.style.display = 'flex';
        btn.classList.add('active');
    } else {
        badgesContainer.style.display = 'none';
        btn.classList.remove('active');
    }
}

// ==== CREATE/EDIT EVENT MODAL ===

function openEventModal(eventId = null) {
    const modal = document.getElementById('eventModal');
    const form = document.getElementById('eventForm');
    const modalTitle = document.getElementById('modalTitle');

    // Reset form
    form.reset();
    document.getElementById('eventId').value = '';
    document.getElementById('choreSection').style.display = 'none';

    // Reset save flag
    window.isSavingEvent = false;

    // Entferne alten Listener und füge GENAU EINEN neuen hinzu
    form.removeEventListener('submit', saveEvent);
    form.addEventListener('submit', saveEvent, { once: false });

    if (eventId) {
        // Edit Mode - Lade Event-Daten
        modalTitle.textContent = 'Termin bearbeiten';
        loadEventForEdit(eventId);
    } else {
        // Create Mode
        modalTitle.textContent = 'Termin verkünden';
    }

    // Show Modal
    modal.style.display = 'flex';
}

function closeEventModal() {
    const modal = document.getElementById('eventModal');
    const form = document.getElementById('eventForm');

    // Entferne Listener beim Schließen
    form.removeEventListener('submit', saveEvent);
    modal.style.display = 'none';

    // Reset flag
    window.isSavingEvent = false;
}

function toggleChoreSection() {
    const eventType = document.getElementById('eventType').value;
    const choreSection = document.getElementById('choreSection');

    if (eventType === 'Spieltag') {
        choreSection.style.display = 'block';
    } else {
        choreSection.style.display = 'none';
    }
}

async function loadEventForEdit(eventId) {
    try {
        // Finde Event in der DOM
        const eventCard = document.querySelector(`.event-card[data-event-id="${eventId}"]`);
        if (!eventCard) {
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: 'Event nicht gefunden!',
                confirmButtonColor: '#d97706'
            });
            return;
        }

        // Set Event ID
        document.getElementById('eventId').value = eventId;

        // Parse values from card
        const title = eventCard.querySelector('.event-title')?.textContent || '';
        const description = eventCard.querySelector('.event-description')?.textContent || '';
        const type = eventCard.querySelector('.event-type')?.textContent || '';

        // Parse times from event-time div
        const timeText = eventCard.querySelector('.event-time')?.textContent || '';
        const timeMatch = timeText.match(/(\d{2}:\d{2})\s*-\s*(\d{2}:\d{2})/);
        const startTime = timeMatch ? timeMatch[1] : '';
        const endTime = timeMatch ? timeMatch[2] : '';

        // Parse date from badge
        const day = eventCard.querySelector('.badge-day')?.textContent || '';
        const monthText = eventCard.querySelector('.badge-month')?.textContent || '';

        // Convert month name to number
        const months = { 'Jan': '01', 'Feb': '02', 'Mär': '03', 'Apr': '04', 'Mai': '05', 'Jun': '06',
                        'Jul': '07', 'Aug': '08', 'Sep': '09', 'Okt': '10', 'Nov': '11', 'Dez': '12' };
        const month = months[monthText] || '01';
        const year = new Date().getFullYear(); // Assume current year
        const dateString = `${year}-${month}-${day.padStart(2, '0')}`;

        // Set form values
        document.getElementById('eventTitle').value = title;
        document.getElementById('eventDescription').value = description;
        document.getElementById('eventType').value = type;
        document.getElementById('eventDate').value = dateString;
        document.getElementById('eventStartTime').value = startTime;
        document.getElementById('eventEndTime').value = endTime;

        // Trigger chore section
        toggleChoreSection();

        // Load chores if Spieltag
        if (type === 'Spieltag') {
            const choreMatrix = eventCard.querySelector('.chore-matrix');
            if (choreMatrix) {
                const choreItems = choreMatrix.querySelectorAll('.chore-item');
                choreItems.forEach(item => {
                    const choreName = item.querySelector('.chore-name')?.textContent.trim().replace('priority_high', '').replace('Aufgabe', '').trim();
                    const assignedTo = item.querySelector('.chore-assigned')?.textContent.trim();

                    if (choreName) {
                        const input = document.getElementById(`chore_${choreName.replace(/ /g, '_')}`);
                        if (input && assignedTo && assignedTo !== 'Offen') {
                            input.value = assignedTo;
                        }
                    }
                });
            }
        }

    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Fehler beim Laden der Event-Daten!',
            confirmButtonColor: '#d97706'
        });
    }
}

async function saveEvent(e) {
    e.preventDefault();
    e.stopPropagation();
    e.stopImmediatePropagation();

    // Check if already saving
    if (window.isSavingEvent) {
        return false;
    }
    window.isSavingEvent = true;

    const eventId = document.getElementById('eventId').value;
    const isEdit = eventId !== '';

    // Sammle Form-Daten
    const dateValue = document.getElementById('eventDate').value;
    if (!dateValue) {
        await Swal.fire({
            icon: 'warning',
            title: 'Datum fehlt',
            text: 'Bitte wähle ein Datum aus!',
            confirmButtonColor: '#d97706'
        });
        window.isSavingEvent = false;
        return false;
    }

    const formData = {
        title: document.getElementById('eventTitle').value.trim(),
        description: document.getElementById('eventDescription').value.trim(),
        date: dateValue, // Format: YYYY-MM-DD (ISO 8601)
        startTime: document.getElementById('eventStartTime').value,
        endTime: document.getElementById('eventEndTime').value,
        type: document.getElementById('eventType').value,
        chores: {}
    };

    // Validierung
    if (!formData.title || !formData.type || !formData.startTime || !formData.endTime) {
        await Swal.fire({
            icon: 'warning',
            title: 'Pflichtfelder fehlen',
            text: 'Bitte fülle alle Pflichtfelder aus!',
            confirmButtonColor: '#d97706'
        });
        window.isSavingEvent = false;
        return false;
    }

    // Sammle Chore-Daten (nur bei Spieltag)
    if (formData.type === 'Spieltag') {
        const choreInputs = document.querySelectorAll('.chore-input');
        choreInputs.forEach(input => {
            const choreName = input.name.replace('chore_', '').replace(/_/g, ' ');
            const assignedTo = input.value.trim();
            // ALLE Chores speichern, auch leere als "Offen"
            formData.chores[choreName] = assignedTo || 'Offen';
        });
    }

    try {
        // Disable submit button to prevent double-submit
        const submitBtn = e.target.querySelector('button[type="submit"]');
        const originalBtnHtml = submitBtn.innerHTML;
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<span class="material-symbols-outlined">hourglass_empty</span> Wird gespeichert...';

        const url = isEdit ? `/api/events/${eventId}` : '/api/events';
        const method = isEdit ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(formData)
        });

        if (response.ok) {
            const result = await response.json();

            // Unterschiedliche Meldung für Create vs. Edit
            const successMessage = isEdit
                ? `Termin erfolgreich aktualisiert! ID: ${eventId}`
                : `Termin erfolgreich verkündet! ID: ${result.eventId}`;

            await Swal.fire({
                icon: 'success',
                title: 'Erfolg',
                text: successMessage,
                confirmButtonColor: '#d97706'
            });

            // Erfolg - Modal schließen und Seite neu laden
            closeEventModal();
            setTimeout(() => location.reload(), 500);
        } else {
            const error = await response.json();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: error.error || 'Event konnte nicht gespeichert werden',
                confirmButtonColor: '#d97706'
            });
            // Re-enable button on error
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalBtnHtml;
            window.isSavingEvent = false;
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Fehler beim Speichern des Events!',
            confirmButtonColor: '#d97706'
        });

        // Re-enable button on error
        const submitBtn = e.target.querySelector('button[type="submit"]');
        submitBtn.disabled = false;
        submitBtn.innerHTML = '<span class="material-symbols-outlined">save</span> Verkünden';
        window.isSavingEvent = false;
    }

    return false;
}

// Modal schließen bei Klick außerhalb
document.addEventListener('click', (e) => {
    const modal = document.getElementById('eventModal');
    if (e.target === modal) {
        closeEventModal();
    }
});

    // ==== DELETE EVENT ====
 async function handleDeleteEvent(e) {
    const btn = e.currentTarget;
    const eventId = btn.dataset.eventId;

    const result = await Swal.fire({
        title: 'Termin löschen?',
        text: 'Soll dieser Termin wirklich aus den Annalen gelöscht werden?',
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
         const response = await fetch(`/api/events/${eventId}`, {
             method: 'DELETE',
             headers: { 'Content-Type': 'application/json' }
         });

         if (response.ok) {
             // Remove event card from DOM
             const eventCard = btn.closest('.event-card');
             eventCard.style.opacity = '0';
             setTimeout(() => {
                 eventCard.remove();

                 // Check if list is empty
                 const eventsList = document.querySelector('.events-list');
                 if (!eventsList.querySelectorAll('.event-card').length) {
                     location.reload(); // Reload to show empty state
                 }
             }, 300);
         }
         else {
             const error = await response.json();
             await Swal.fire({
                 icon: 'error',
                 title: 'Fehler beim Löschen',
                 text: error.error || 'Event konnte nicht gelöscht werden',
                 confirmButtonColor: '#d97706'
             });
         }
     }
    catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Fehler beim Löschen des Events!',
            confirmButtonColor: '#d97706'
        });
    }
 }