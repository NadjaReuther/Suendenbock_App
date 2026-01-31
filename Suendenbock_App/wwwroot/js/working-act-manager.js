// Working Act Manager for Gott
// Allows Gott to work on any Act without activating it

let currentWorkingActId = null;
let allActs = [];

// Initialize on page load
document.addEventListener('DOMContentLoaded', async function() {
    // Load all acts
    await loadActs();

    // Get actId from URL if present
    const urlParams = new URLSearchParams(window.location.search);
    const actIdFromUrl = urlParams.get('actId');

    if (actIdFromUrl) {
        currentWorkingActId = parseInt(actIdFromUrl);
    } else {
        // No actId in URL -> use active act
        const activeAct = allActs.find(a => a.isActive);
        if (activeAct) {
            currentWorkingActId = activeAct.id;
        }
    }

    // Populate act selector
    populateActSelector();

    // Update status indicator
    updateStatusIndicator();
});

async function loadActs() {
    try {
        const response = await fetch('/api/game/acts');
        if (response.ok) {
            allActs = await response.json();
        }
    } catch (error) {
        // Fehler stillschweigend behandeln
    }
}

function populateActSelector() {
    const selector = document.getElementById('actSelector');
    if (!selector) return;

    selector.innerHTML = allActs.map(act => `
        <option value="${act.id}" ${act.id === currentWorkingActId ? 'selected' : ''}>
            Akt ${act.actNumber} - ${act.country || act.name} ${act.isActive ? '(AKTIV)' : ''}
        </option>
    `).join('');
}

function updateStatusIndicator() {
    const indicator = document.getElementById('actStatusIndicator');
    if (!indicator) return;

    const currentAct = allActs.find(a => a.id === currentWorkingActId);
    if (!currentAct) return;

    if (currentAct.isActive) {
        indicator.textContent = '✓ Aktiver Act';
        indicator.style.color = '#10b981';
    } else {
        indicator.textContent = '⚠ Vorbereitungsmodus';
        indicator.style.color = '#f59e0b';
    }
}

function switchWorkingAct(actId) {
    const newActId = parseInt(actId);

    // Redirect to Dashboard with new actId
    window.location.href = `/Spielmodus/Dashboard?actId=${newActId}`;
}

// Helper function to build URL with actId parameter
function buildUrlWithActId(baseUrl) {
    if (!currentWorkingActId) return baseUrl;

    const separator = baseUrl.includes('?') ? '&' : '?';
    return `${baseUrl}${separator}actId=${currentWorkingActId}`;
}

// Update navigation links to include actId
document.addEventListener('DOMContentLoaded', function() {
    // Find all Spielmodus navigation links
    const spielmodusLinks = document.querySelectorAll('a[href^="/Spielmodus/"]');

    spielmodusLinks.forEach(link => {
        const currentHref = link.getAttribute('href');

        // Add click handler to append actId
        link.addEventListener('click', function(e) {
            if (currentWorkingActId) {
                e.preventDefault();
                window.location.href = buildUrlWithActId(currentHref);
            }
        });
    });
});
