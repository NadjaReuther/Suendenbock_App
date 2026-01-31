// Polls.js - Interactivity for Polls Page

let currentPollData = null;

// Create Poll Button
const createPollBtn = document.getElementById('createPollBtn');
if (createPollBtn) {
    createPollBtn.addEventListener('click', openPollModal);
}

// Vote Buttons
document.querySelectorAll('.vote-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const pollId = this.dataset.pollId;
        openVoteModal(pollId);
    });
});

// Edit Poll Buttons
document.querySelectorAll('.edit-poll-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const pollId = this.dataset.pollId;
        openEditPollModal(pollId);
    });
});

// Delete Poll Buttons
document.querySelectorAll('.delete-poll-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const pollId = this.dataset.pollId;
        deletePoll(pollId);
    });
});

// Close Poll Buttons
document.querySelectorAll('.close-poll-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const pollId = this.dataset.pollId;
        closePoll(pollId);
    });
});

// Poll Form Submit
const pollForm = document.getElementById('pollForm');
if (pollForm) {
    pollForm.addEventListener('submit', function(e) {
        e.preventDefault();
        createPoll();
    });
}

// Add Option Button
const addOptionBtn = document.getElementById('addOptionBtn');
if (addOptionBtn) {
    addOptionBtn.addEventListener('click', addPollOption);
}

// Cast Vote Button
const castVoteBtn = document.getElementById('castVoteBtn');
if (castVoteBtn) {
    castVoteBtn.addEventListener('click', castVote);
}

// Toggle Voters Buttons
document.querySelectorAll('.toggle-voters-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const pollId = this.dataset.pollId;
        const votersList = document.querySelector(`.poll-voters-list[data-poll-id="${pollId}"]`);

        if (votersList.style.display === 'none') {
            votersList.style.display = 'flex';
            this.classList.add('active');
        } else {
            votersList.style.display = 'none';
            this.classList.remove('active');
        }
    });
});

// === VOTE MODAL FUNCTIONS ===

function openVoteModal(pollId) {
    // Find poll card
    const pollCard = document.querySelector(`.poll-card[data-poll-id="${pollId}"]`);
    if (!pollCard) return;

    // Extract poll data
    const question = pollCard.querySelector('.poll-question').textContent;
    const allowMultiple = pollCard.querySelector('.poll-type').textContent.includes('Mehrfachauswahl');

    // Get options
    const options = [];
    const optionItems = pollCard.querySelectorAll('.poll-option-item');
    optionItems.forEach((item) => {
        const text = item.querySelector('.option-label span:last-child').textContent.trim();
        const isUserChoice = item.querySelector('.user-choice-icon') !== null;
        const optionId = parseInt(item.dataset.optionId);

        options.push({
            id: optionId,
            text: text,
            isSelected: isUserChoice
        });
    });

    currentPollData = {
        pollId: pollId,
        question: question,
        allowMultiple: allowMultiple,
        options: options
    };

    // Update modal content
    document.getElementById('voteModalTitle').textContent = question;
    document.getElementById('voteModalHint').textContent = allowMultiple
        ? 'Du kannst mehrere Optionen wählen.'
        : 'Du kannst nur eine Option wählen.';

    // Build options list
    const optionsList = document.getElementById('voteOptionsList');
    optionsList.innerHTML = '';

    options.forEach(option => {
        const optionDiv = document.createElement('div');
        optionDiv.className = 'vote-option-item';
        optionDiv.dataset.optionId = option.id;

        if (option.isSelected) {
            optionDiv.classList.add('selected');
        }

        const inputType = allowMultiple ? 'checkbox' : 'radio';
        optionDiv.innerHTML = `
            <input type="${inputType}"
                   name="voteOption"
                   value="${option.id}"
                   ${option.isSelected ? 'checked' : ''}
                   id="voteOption${option.id}">
            <label for="voteOption${option.id}">
                <span class="material-symbols-outlined vote-option-icon">
                    ${allowMultiple ? 'check_box_outline_blank' : 'radio_button_unchecked'}
                </span>
                <span class="vote-option-text">${option.text}</span>
            </label>
        `;

        // Add click handler
        optionDiv.addEventListener('click', function(e) {
            if (e.target.tagName !== 'INPUT') {
                const input = this.querySelector('input');

                if (allowMultiple) {
                    input.checked = !input.checked;
                } else {
                    // Uncheck all others
                    optionsList.querySelectorAll('input').forEach(i => i.checked = false);
                    input.checked = true;
                }

                updateVoteOptionStyles();
            }
        });

        // Add input change handler for icon updates
        const input = optionDiv.querySelector('input');
        input.addEventListener('change', updateVoteOptionStyles);

        optionsList.appendChild(optionDiv);
    });

    // Show modal
    document.getElementById('voteModal').style.display = 'flex';
}

function updateVoteOptionStyles() {
    document.querySelectorAll('.vote-option-item').forEach(item => {
        const input = item.querySelector('input');
        const icon = item.querySelector('.vote-option-icon');

        if (input.checked) {
            item.classList.add('selected');
            if (input.type === 'checkbox') {
                icon.textContent = 'check_box';
            } else {
                icon.textContent = 'radio_button_checked';
            }
        } else {
            item.classList.remove('selected');
            if (input.type === 'checkbox') {
                icon.textContent = 'check_box_outline_blank';
            } else {
                icon.textContent = 'radio_button_unchecked';
            }
        }
    });
}

function closeVoteModal() {
    document.getElementById('voteModal').style.display = 'none';
    currentPollData = null;
}

async function castVote() {
    if (!currentPollData) return;

    // Get selected options
    const selectedInputs = document.querySelectorAll('#voteOptionsList input:checked');
    const selectedOptionIds = Array.from(selectedInputs).map(input => parseInt(input.value));

    if (selectedOptionIds.length === 0) {
        await Swal.fire({
            icon: 'warning',
            title: 'Keine Option gewählt',
            text: 'Bitte wähle mindestens eine Option.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    const data = {
        pollId: currentPollData.pollId,
        optionIds: selectedOptionIds
    };

    try {
        const response = await fetch('/api/polls/vote', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            closeVoteModal();
            window.location.reload();
        } else {
            const error = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler beim Abstimmen',
                text: error || 'Fehler beim Abstimmen',
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

// === POLL CREATION MODAL FUNCTIONS ===

function openPollModal() {
    const modal = document.getElementById('pollModal');
    const modalTitle = document.getElementById('pollModalTitle');

    modalTitle.textContent = 'Volksbefragung starten';
    document.getElementById('pollForm').reset();

    // Reset options list to 2 default options
    const optionsList = document.getElementById('pollOptionsList');
    optionsList.innerHTML = `
        <div class="poll-option-input">
            <input type="text" class="form-control option-input" placeholder="Option 1" required>
            <button type="button" class="remove-option-btn" onclick="removePollOption(this)" style="display: none;">
                <span class="material-symbols-outlined">close</span>
            </button>
        </div>
        <div class="poll-option-input">
            <input type="text" class="form-control option-input" placeholder="Option 2" required>
            <button type="button" class="remove-option-btn" onclick="removePollOption(this)" style="display: none;">
                <span class="material-symbols-outlined">close</span>
            </button>
        </div>
    `;

    modal.style.display = 'flex';
}

function closePollModal() {
    document.getElementById('pollModal').style.display = 'none';
}

async function addPollOption() {
    const optionsList = document.getElementById('pollOptionsList');
    const currentCount = optionsList.querySelectorAll('.poll-option-input').length;

    if (currentCount >= 10) {
        await Swal.fire({
            icon: 'warning',
            title: 'Limit erreicht',
            text: 'Maximal 10 Optionen erlaubt.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    const optionDiv = document.createElement('div');
    optionDiv.className = 'poll-option-input';
    optionDiv.innerHTML = `
        <input type="text" class="form-control option-input" placeholder="Option ${currentCount + 1}" required>
        <button type="button" class="remove-option-btn" onclick="removePollOption(this)">
            <span class="material-symbols-outlined">close</span>
        </button>
    `;

    optionsList.appendChild(optionDiv);

    // Show remove buttons if more than 2 options
    updateRemoveButtons();
}

async function removePollOption(button) {
    const optionDiv = button.closest('.poll-option-input');
    const optionsList = document.getElementById('pollOptionsList');

    // Don't allow removing if only 2 options left
    if (optionsList.querySelectorAll('.poll-option-input').length <= 2) {
        await Swal.fire({
            icon: 'warning',
            title: 'Mindestanzahl erreicht',
            text: 'Mindestens 2 Optionen erforderlich.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    optionDiv.remove();

    // Update placeholders
    const inputs = optionsList.querySelectorAll('.option-input');
    inputs.forEach((input, index) => {
        input.placeholder = `Option ${index + 1}`;
    });

    // Update remove buttons visibility
    updateRemoveButtons();
}

function updateRemoveButtons() {
    const optionsList = document.getElementById('pollOptionsList');
    const removeButtons = optionsList.querySelectorAll('.remove-option-btn');
    const count = removeButtons.length;

    removeButtons.forEach(btn => {
        btn.style.display = count > 2 ? 'flex' : 'none';
    });
}

async function createPoll() {
    const pollId = document.getElementById('pollId').value;
    const question = document.getElementById('pollQuestion').value.trim();
    const allowMultiple = document.getElementById('allowMultiple').checked;

    // Get all options
    const optionInputs = document.querySelectorAll('.option-input');
    const options = [];

    for (let input of optionInputs) {
        const text = input.value.trim();
        if (text) {
            options.push(text);
        }
    }

    if (options.length < 2) {
        await Swal.fire({
            icon: 'warning',
            title: 'Zu wenige Optionen',
            text: 'Mindestens 2 Optionen erforderlich.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    const data = {
        question: question,
        allowMultipleChoices: allowMultiple,
        options: options
    };

    try {
        let url, method;
        if (pollId) {
            // Update existing poll
            url = `/api/polls/${pollId}`;
            method = 'PUT';
        } else {
            // Create new poll
            url = '/api/polls';
            method = 'POST';
        }

        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            closePollModal();
            window.location.reload();
        } else {
            const error = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler beim Speichern',
                text: error || 'Fehler beim Speichern der Umfrage',
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

async function openEditPollModal(pollId) {
    const modal = document.getElementById('pollModal');
    const modalTitle = document.getElementById('pollModalTitle');
    const pollIdField = document.getElementById('pollId');

    modalTitle.textContent = 'Volksbefragung anpassen';
    pollIdField.value = pollId;

    // Load poll data from the card
    const pollCard = document.querySelector(`.poll-card[data-poll-id="${pollId}"]`);
    if (!pollCard) return;

    const question = pollCard.querySelector('.poll-question').textContent;
    const allowMultiple = pollCard.querySelector('.poll-type').textContent.includes('Mehrfachauswahl');

    // Set form values
    document.getElementById('pollQuestion').value = question;
    document.getElementById('allowMultiple').checked = allowMultiple;

    // Get options from card
    const optionItems = pollCard.querySelectorAll('.poll-option-item');
    const optionsList = document.getElementById('pollOptionsList');
    optionsList.innerHTML = '';

    optionItems.forEach((item, index) => {
        const text = item.querySelector('.option-label span:last-child').textContent.trim();

        const optionDiv = document.createElement('div');
        optionDiv.className = 'poll-option-input';
        optionDiv.innerHTML = `
            <input type="text" class="form-control option-input" placeholder="Option ${index + 1}" value="${text}" required>
            <button type="button" class="remove-option-btn" onclick="removePollOption(this)" style="display: ${optionItems.length > 2 ? 'flex' : 'none'};">
                <span class="material-symbols-outlined">close</span>
            </button>
        `;
        optionsList.appendChild(optionDiv);
    });

    modal.style.display = 'flex';
}

async function deletePoll(pollId) {
    const result = await Swal.fire({
        title: 'Volksbefragung löschen?',
        text: 'Soll diese Volksbefragung wirklich für immer aus den Annalen gelöscht werden?',
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
        const response = await fetch(`/api/polls/${pollId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            window.location.reload();
        } else {
            const error = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler beim Löschen',
                text: error || 'Fehler beim Löschen der Umfrage',
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

async function closePoll(pollId) {
    const result = await Swal.fire({
        title: 'Volksbefragung beenden?',
        text: 'Soll diese Volksbefragung beendet werden? Danach können keine weiteren Stimmen abgegeben werden.',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#d97706',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Beenden',
        cancelButtonText: 'Abbrechen'
    });

    if (!result.isConfirmed) {
        return;
    }

    try {
        const response = await fetch(`/api/polls/${pollId}/close`, {
            method: 'PUT'
        });

        if (response.ok) {
            window.location.reload();
        } else {
            const error = await response.text();
            await Swal.fire({
                icon: 'error',
                title: 'Fehler beim Beenden',
                text: error || 'Fehler beim Beenden der Umfrage',
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
    const voteModal = document.getElementById('voteModal');
    const pollModal = document.getElementById('pollModal');

    if (e.target === voteModal) {
        closeVoteModal();
    }
    if (e.target === pollModal) {
        closePollModal();
    }
});
