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
        alert('Bitte fülle alle Pflichtfelder aus.');
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
            alert(result.message || 'Ticket erfolgreich erstellt!');
            window.location.reload();
        } else {
            const error = await response.text();
            alert('Fehler: ' + error);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Ein Fehler ist aufgetreten.');
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
        alert('Bitte fülle alle Pflichtfelder aus.');
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
            alert('Ticket erfolgreich aktualisiert!');
            window.location.reload();
        } else {
            const error = await response.text();
            alert('Fehler: ' + error);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Ein Fehler ist aufgetreten.');
    }
}

// ==== RESOLVE TICKET ====
document.querySelectorAll('.resolve-ticket-btn').forEach(btn => {
    btn.addEventListener('click', async function() {
        const ticketId = this.dataset.ticketId;

        if (confirm('Möchtest du dieses Ticket als gelöst markieren?')) {
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
            alert('Ticket als gelöst markiert!');
            window.location.reload();
        } else {
            const error = await response.text();
            alert('Fehler: ' + error);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Ein Fehler ist aufgetreten.');
    }
}

// ==== REOPEN TICKET ====
document.querySelectorAll('.reopen-ticket-btn').forEach(btn => {
    btn.addEventListener('click', async function() {
        const ticketId = this.dataset.ticketId;

        if (confirm('Möchtest du dieses Ticket wieder öffnen?')) {
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
            alert('Ticket wieder geöffnet!');
            window.location.reload();
        } else {
            const error = await response.text();
            alert('Fehler: ' + error);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Ein Fehler ist aufgetreten.');
    }
}

// ==== DELETE TICKET ====
document.querySelectorAll('.delete-ticket-btn').forEach(btn => {
    btn.addEventListener('click', async function() {
        const ticketId = this.dataset.ticketId;

        if (confirm('Möchtest du dieses Ticket wirklich löschen? Diese Aktion kann nicht rückgängig gemacht werden.')) {
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
            alert('Ticket gelöscht!');
            window.location.reload();
        } else {
            const error = await response.text();
            alert('Fehler: ' + error);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Ein Fehler ist aufgetreten.');
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
