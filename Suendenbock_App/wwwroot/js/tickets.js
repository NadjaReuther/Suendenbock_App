// Tickets Management JavaScript

// ==== TAB FILTERING ====
document.querySelectorAll('.ticket-tab').forEach(tab => {
    tab.addEventListener('click', function() {
        // Update active tab
        document.querySelectorAll('.ticket-tab').forEach(t => t.classList.remove('active'));
        this.classList.add('active');

        // Filter tickets
        const status = this.dataset.status;
        filterTickets(status);
    });
});

function filterTickets(status) {
    const tickets = document.querySelectorAll('.ticket-card');

    tickets.forEach(ticket => {
        if (status === 'all') {
            ticket.style.display = 'block';
        } else {
            const ticketStatus = ticket.dataset.status;
            ticket.style.display = ticketStatus === status ? 'block' : 'none';
        }
    });
}

// ==== CREATE TICKET ====
const createBtn = document.getElementById('createTicketBtn');
if (createBtn) {
    createBtn.addEventListener('click', openCreateTicketModal);
}

const createForm = document.getElementById('createTicketForm');
if (createForm) {
    createForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        await createTicket();
    });
}

function openCreateTicketModal() {
    document.getElementById('createTicketModal').style.display = 'flex';
}

function closeCreateTicketModal() {
    document.getElementById('createTicketModal').style.display = 'none';
    document.getElementById('createTicketForm').reset();
}

async function createTicket() {
    const title = document.getElementById('ticketTitle').value.trim();
    const description = document.getElementById('ticketDescription').value.trim();
    const category = document.getElementById('ticketCategory').value;

    if (!title || !description) {
        await Swal.fire({
            icon: 'warning',
            title: 'Pflichtfelder fehlen',
            text: 'Bitte fülle alle Pflichtfelder aus.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    try {
        const response = await fetch('/api/tickets', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                title: title,
                description: description,
                category: category
            })
        });

        if (response.ok) {
            const result = await response.json();
            await Swal.fire({
                icon: 'success',
                title: 'Erfolg',
                text: result.message || 'Ticket erfolgreich erstellt!',
                confirmButtonColor: '#d97706'
            });
            window.location.reload();
        } else {
            const error = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: error || 'Fehler beim Erstellen',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Ein Fehler ist aufgetreten.',
            confirmButtonColor: '#d97706'
        });
    }
}

// ==== EDIT TICKET ====
document.querySelectorAll('.edit-ticket-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const ticketId = this.dataset.ticketId;
        openEditTicketModal(ticketId);
    });
});

function openEditTicketModal(ticketId) {
    const ticketCard = document.querySelector(`.ticket-card[data-ticket-id="${ticketId}"]`);
    if (!ticketCard) return;

    const title = ticketCard.querySelector('.ticket-title').textContent;
    const description = ticketCard.querySelector('.ticket-description').textContent;
    const category = ticketCard.querySelector('.ticket-category').textContent;

    document.getElementById('editTicketId').value = ticketId;
    document.getElementById('editTicketTitle').value = title;
    document.getElementById('editTicketDescription').value = description;
    document.getElementById('editTicketCategory').value = category;

    document.getElementById('editTicketModal').style.display = 'flex';
}

function closeEditTicketModal() {
    document.getElementById('editTicketModal').style.display = 'none';
    document.getElementById('editTicketForm').reset();
}

const editForm = document.getElementById('editTicketForm');
if (editForm) {
    editForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        await updateTicket();
    });
}

async function updateTicket() {
    const ticketId = document.getElementById('editTicketId').value;
    const title = document.getElementById('editTicketTitle').value.trim();
    const description = document.getElementById('editTicketDescription').value.trim();
    const category = document.getElementById('editTicketCategory').value;

    if (!title || !description) {
        await Swal.fire({
            icon: 'warning',
            title: 'Pflichtfelder fehlen',
            text: 'Bitte fülle alle Pflichtfelder aus.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    try {
        const response = await fetch(`/api/tickets/${ticketId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                title: title,
                description: description,
                category: category
            })
        });

        if (response.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Erfolg',
                text: 'Ticket erfolgreich aktualisiert!',
                confirmButtonColor: '#d97706'
            });
            window.location.reload();
        } else {
            const error = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: error || 'Fehler beim Aktualisieren',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Ein Fehler ist aufgetreten.',
            confirmButtonColor: '#d97706'
        });
    }
}

// ==== RESOLVE TICKET ====
document.querySelectorAll('.resolve-ticket-btn').forEach(btn => {
    btn.addEventListener('click', async function() {
        const ticketId = this.dataset.ticketId;

        const result = await Swal.fire({
            title: 'Ticket als gelöst markieren?',
            text: 'Möchtest du dieses Ticket als gelöst markieren?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#d97706',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Als gelöst markieren',
            cancelButtonText: 'Abbrechen'
        });

        if (result.isConfirmed) {
            await resolveTicket(ticketId);
        }
    });
});

async function resolveTicket(ticketId) {
    try {
        const response = await fetch(`/api/tickets/${ticketId}/resolve`, {
            method: 'PUT'
        });

        if (response.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Erfolg',
                text: 'Ticket als gelöst markiert!',
                confirmButtonColor: '#d97706'
            });
            window.location.reload();
        } else {
            const error = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: error || 'Fehler beim Markieren als gelöst',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Ein Fehler ist aufgetreten.',
            confirmButtonColor: '#d97706'
        });
    }
}

// ==== REOPEN TICKET ====
document.querySelectorAll('.reopen-ticket-btn').forEach(btn => {
    btn.addEventListener('click', async function() {
        const ticketId = this.dataset.ticketId;

        const result = await Swal.fire({
            title: 'Ticket wieder öffnen?',
            text: 'Möchtest du dieses Ticket wieder öffnen?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#d97706',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Wieder öffnen',
            cancelButtonText: 'Abbrechen'
        });

        if (result.isConfirmed) {
            await reopenTicket(ticketId);
        }
    });
});

async function reopenTicket(ticketId) {
    try {
        const response = await fetch(`/api/tickets/${ticketId}/reopen`, {
            method: 'PUT'
        });

        if (response.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Erfolg',
                text: 'Ticket wieder geöffnet!',
                confirmButtonColor: '#d97706'
            });
            window.location.reload();
        } else {
            const error = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: error || 'Fehler beim Wiedereröffnen',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Ein Fehler ist aufgetreten.',
            confirmButtonColor: '#d97706'
        });
    }
}

// ==== DELETE TICKET ====
document.querySelectorAll('.delete-ticket-btn').forEach(btn => {
    btn.addEventListener('click', async function() {
        const ticketId = this.dataset.ticketId;

        const result = await Swal.fire({
            title: 'Ticket löschen?',
            text: 'Möchtest du dieses Ticket wirklich löschen? Diese Aktion kann nicht rückgängig gemacht werden.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d97706',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Löschen',
            cancelButtonText: 'Abbrechen'
        });

        if (result.isConfirmed) {
            await deleteTicket(ticketId);
        }
    });
});

async function deleteTicket(ticketId) {
    try {
        const response = await fetch(`/api/tickets/${ticketId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            await Swal.fire({
                icon: 'success',
                title: 'Erfolg',
                text: 'Ticket gelöscht!',
                confirmButtonColor: '#d97706'
            });
            window.location.reload();
        } else {
            const error = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: error || 'Fehler beim Löschen',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Ein Fehler ist aufgetreten.',
            confirmButtonColor: '#d97706'
        });
    }
}

// ==== CLOSE MODALS ON OUTSIDE CLICK ====
document.addEventListener('click', function(e) {
    if (e.target.classList.contains('modal-overlay')) {
        if (e.target.id === 'createTicketModal') {
            closeCreateTicketModal();
        } else if (e.target.id === 'editTicketModal') {
            closeEditTicketModal();
        }
    }
});

// ==== ESCAPE KEY TO CLOSE MODALS ====
document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape') {
        closeCreateTicketModal();
        closeEditTicketModal();
    }
});
