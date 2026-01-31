// Community Payments Management

// Track if changes were made
let paymentsChanged = false;

// Manage Payments Button
const managePaymentsBtn = document.getElementById('managePaymentsBtn');
if (managePaymentsBtn) {
    managePaymentsBtn.addEventListener('click', openPaymentsModal);
}

// Add Player Button
const addPlayerBtn = document.getElementById('addPlayerBtn');
if (addPlayerBtn) {
    addPlayerBtn.addEventListener('click', openAddPlayerModal);
}

// Add Player Form
const addPlayerForm = document.getElementById('addPlayerForm');
if (addPlayerForm) {
    addPlayerForm.addEventListener('submit', function(e) {
        e.preventDefault();
        addPlayer();
    });
}

// === MODAL FUNCTIONS ===

async function openPaymentsModal() {
    const modal = document.getElementById('paymentsModal');
    paymentsChanged = false; // Reset flag

    // Load payments
    await loadPayments();

    modal.style.display = 'flex';
}

function closePaymentsModal() {
    document.getElementById('paymentsModal').style.display = 'none';

    // Reload page if changes were made
    if (paymentsChanged) {
        window.location.reload();
    }
}

function openAddPlayerModal() {
    document.getElementById('addPlayerModal').style.display = 'flex';
}

function closeAddPlayerModal() {
    document.getElementById('addPlayerModal').style.display = 'none';
    document.getElementById('addPlayerForm').reset();
}

// === API FUNCTIONS ===

async function loadPayments() {
    const now = new Date();
    const year = now.getFullYear();
    const month = now.getMonth() + 1;

    try {
        const response = await fetch(`/api/payments?year=${year}&month=${month}`);
        if (response.ok) {
            const payments = await response.json();
            renderPayments(payments);
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: 'Fehler beim Laden der Zahlungen.',
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

function renderPayments(payments) {
    const paymentsList = document.getElementById('paymentsList');

    if (payments.length === 0) {
        paymentsList.innerHTML = `
            <div class="empty-payments">
                <span class="material-symbols-outlined">payments</span>
                <p>Noch keine Spieler für diesen Monat hinzugefügt.</p>
            </div>
        `;
        return;
    }

    paymentsList.innerHTML = payments.map(payment => `
        <div class="payment-management-item" data-payment-id="${payment.id}">
            <div class="payment-player">
                <div class="payment-status-indicator ${payment.status}"></div>
                <span class="payment-player-name">${payment.playerName}</span>
            </div>
            <div class="payment-controls">
                <select class="payment-method-select" data-payment-id="${payment.id}" ${payment.status !== 'paid' ? 'disabled' : ''}>
                    <option value="">-- Zahlungsart --</option>
                    <option value="PayPal" ${payment.paymentMethod === 'PayPal' ? 'selected' : ''}>PayPal</option>
                    <option value="Überweisung" ${payment.paymentMethod === 'Überweisung' ? 'selected' : ''}>Überweisung</option>
                    <option value="Bar" ${payment.paymentMethod === 'Bar' ? 'selected' : ''}>Bar</option>
                </select>
                <button class="payment-toggle-btn ${payment.status === 'paid' ? 'paid' : 'unpaid'}"
                        data-payment-id="${payment.id}"
                        data-status="${payment.status}"
                        onclick="togglePaymentStatus(${payment.id}, '${payment.status}')">
                    ${payment.status === 'paid' ? 'Bezahlt' : 'Ausstehend'}
                </button>
                <button class="payment-delete-btn" onclick="deletePayment(${payment.id})">
                    <span class="material-symbols-outlined">delete</span>
                </button>
            </div>
        </div>
    `).join('');

    // Add event listeners for payment method selects
    document.querySelectorAll('.payment-method-select').forEach(select => {
        select.addEventListener('change', function() {
            const paymentId = this.dataset.paymentId;
            const paymentMethod = this.value;
            updatePaymentMethod(paymentId, paymentMethod);
        });
    });
}

async function togglePaymentStatus(paymentId, currentStatus) {
    const newStatus = currentStatus === 'paid' ? 'unpaid' : 'paid';

    try {
        const response = await fetch(`/api/payments/${paymentId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                status: newStatus,
                paymentMethod: newStatus === 'paid' ? 'PayPal' : null
            })
        });

        if (response.ok) {
            paymentsChanged = true;
            await loadPayments();
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: 'Fehler beim Aktualisieren der Zahlung.',
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

async function updatePaymentMethod(paymentId, paymentMethod) {
    try {
        const response = await fetch(`/api/payments/${paymentId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                status: 'paid',
                paymentMethod: paymentMethod
            })
        });

        if (response.ok) {
            paymentsChanged = true;
            await loadPayments();
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: 'Fehler beim Aktualisieren der Zahlungsart.',
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

async function deletePayment(paymentId) {
    const result = await Swal.fire({
        title: 'Eintrag löschen?',
        text: 'Möchtest du diesen Eintrag wirklich löschen?',
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
        const response = await fetch(`/api/payments/${paymentId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            paymentsChanged = true;
            await loadPayments();
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: 'Fehler beim Löschen.',
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

async function addPlayer() {
    const playerName = document.getElementById('playerName').value.trim();

    if (!playerName) {
        await Swal.fire({
            icon: 'warning',
            title: 'Name fehlt',
            text: 'Bitte gib einen Spielernamen ein.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    const now = new Date();
    const year = now.getFullYear();
    const month = now.getMonth() + 1;

    const data = {
        playerName: playerName,
        year: year,
        month: month,
        status: 'unpaid'
    };

    try {
        const response = await fetch('/api/payments', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            paymentsChanged = true;
            closeAddPlayerModal();
            await loadPayments();
        } else {
            const error = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler beim Hinzufügen',
                text: error || 'Fehler beim Hinzufügen',
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

// Close modals when clicking outside
document.addEventListener('click', function(e) {
    const paymentsModal = document.getElementById('paymentsModal');
    const addPlayerModal = document.getElementById('addPlayerModal');

    if (e.target === paymentsModal) {
        closePaymentsModal();
    }
    if (e.target === addPlayerModal) {
        closeAddPlayerModal();
    }
});
