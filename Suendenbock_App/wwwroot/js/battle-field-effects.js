// ===== BATTLE SYSTEM - FIELD EFFECTS AND BIOMES =====

import { battleState } from './battle-state.js';
import { IS_GOTT_ROLE } from './battle-config.js';
import { syncBattleState } from './battle-signalr.js';

// Make functions available globally for onclick handlers
if (typeof window !== 'undefined') {
    window.openFieldEffectModal = openFieldEffectModal;
    window.closeFieldEffectModal = closeFieldEffectModal;
    window.selectStrength = selectStrength;
    window.addFieldEffect = addFieldEffect;
    window.removeFieldEffect = removeFieldEffect;
    window.showFieldEffectDescription = showFieldEffectDescription;
    window.closeFieldEffectPanel = closeFieldEffectPanel;
    window.setBiom = setBiom;
    window.removeBiom = removeBiom;
    window.openBiomDescriptionPanel = openBiomDescriptionPanel;
    window.closeBiomPanel = closeBiomPanel;
    window.openEffectModal = openEffectModal;
    window.closeEffectModal = closeEffectModal;
    window.switchEffectTab = switchEffectTab;
}

// ===== FIELD EFFECTS SYSTEM =====

// Global state for modal
let selectedStrength = null;

// Render active field effects as gradient banner
export function renderFieldEffects() {
    const banner = document.getElementById('fieldEffectsBanner');
    const container = document.getElementById('activeFieldEffectsContainer');

    if (!banner || !container) return;

    // Für Gott: Banner IMMER anzeigen (auch ohne aktive Effekte, damit der Button sichtbar ist)
    // Für Spieler: Nur anzeigen wenn Effekte aktiv sind
    if (battleState.activeFieldEffects.length === 0) {
        if (!IS_GOTT_ROLE) {
            banner.classList.add('hidden');
            return;
        }
        // Gott: Banner anzeigen, aber mit neutralem Hintergrund
        banner.classList.remove('hidden');
        banner.style = 'background: linear-gradient(135deg, #374151 0%, #1f2937 100%)';
        container.innerHTML = '<p class="text-gray-400 italic text-sm">Keine Feldeffekte aktiv</p>';
        return;
    }

    banner.classList.remove('hidden');

    // Create gradient from all active field effect colors
    const colors = battleState.activeFieldEffects.map(fe => fe.colorCode);
    let gradientStyle = '';

    if (colors.length === 1) {
        gradientStyle = `background: ${colors[0]}`;
    } else {
        // Create linear gradient for multiple colors
        const stops = colors.map((color, index) => {
            const percent = (index / (colors.length - 1)) * 100;
            return `${color} ${percent}%`;
        }).join(', ');
        gradientStyle = `background: linear-gradient(to right, ${stops})`;
    }

    banner.style = gradientStyle;

    // Render individual effect badges
    container.innerHTML = battleState.activeFieldEffects.map(fe => {
        const strengthColors = {
            'einfach': 'bg-green-900/60 border-green-500 text-green-200',
            'mittel': 'bg-yellow-900/60 border-yellow-500 text-yellow-200',
            'schwer': 'bg-red-900/60 border-red-500 text-red-200'
        };

        const colorClass = strengthColors[fe.schwere] || 'bg-gray-700 border-gray-500 text-gray-200';

        // Beschreibung für onclick vorbereiten (escapen)
        const escapedName = (fe.name || '').replace(/'/g, "\\'").replace(/"/g, '&quot;');
        const escapedDescription = (fe.beschreibung || 'Keine Beschreibung verfügbar').replace(/'/g, "\\'").replace(/"/g, '&quot;');
        const escapedSchwere = (fe.schwere || '').replace(/'/g, "\\'");
        const escapedColorCode = (fe.colorCode || '').replace(/'/g, "\\'");

        return `
            <div class="flex items-center gap-2 ${colorClass} border-2 rounded-lg px-3 py-2 relative group">
                <button onclick="showFieldEffectDescription('${escapedName}', '${escapedDescription}', '${escapedSchwere}', '${escapedColorCode}')"
                        class="flex items-center gap-2 hover:opacity-80 transition-opacity cursor-pointer"
                        title="Klicken für Details">
                    <span class="font-bold">${fe.name}</span>
                    <span class="text-sm opacity-80">(${fe.schwere})</span>
                    <span class="text-xs opacity-60">ℹ️</span>
                </button>
                ${IS_GOTT_ROLE ? `
                    <button onclick="removeFieldEffect(${fe.feldEffektId})"
                            class="ml-2 hover:text-white transition-colors text-lg leading-none"
                            title="Entfernen">
                        ×
                    </button>
                ` : ''}
            </div>
        `;
    }).join('');
}

// Open field effect modal
export function openFieldEffectModal() {
    if (!IS_GOTT_ROLE) return;

    const modal = document.getElementById('fieldEffectModal');
    if (!modal) return;

    // Reset modal state
    document.getElementById('fieldEffectSelect').value = '';
    document.getElementById('fieldEffectDescription').classList.add('hidden');
    selectedStrength = null;
    document.querySelectorAll('.strength-btn').forEach(btn => {
        btn.classList.remove('ring-2', 'ring-white');
    });

    modal.classList.remove('hidden');
}

// Close field effect modal
export function closeFieldEffectModal() {
    const modal = document.getElementById('fieldEffectModal');
    if (modal) {
        modal.classList.add('hidden');
    }
}

// Select strength
export function selectStrength(strength) {
    selectedStrength = strength;

    // Update button styling
    document.querySelectorAll('.strength-btn').forEach(btn => {
        if (btn.getAttribute('data-strength') === strength) {
            btn.classList.add('ring-2', 'ring-white');
        } else {
            btn.classList.remove('ring-2', 'ring-white');
        }
    });
}

// Add field effect
export async function addFieldEffect() {
    if (!IS_GOTT_ROLE) return;

    const selectEl = document.getElementById('fieldEffectSelect');
    const selectedOption = selectEl.options[selectEl.selectedIndex];

    if (!selectedOption.value) {
        await Swal.fire({
            icon: 'warning',
            title: 'Keine Auswahl',
            text: 'Bitte wähle einen Feldeffekt aus.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    const feldEffektId = parseInt(selectedOption.value);
    const name = selectedOption.getAttribute('data-name');
    const schwere = selectedOption.getAttribute('data-schwere'); // Schwere aus Datenbank
    const colorCode = selectedOption.getAttribute('data-color');
    const beschreibung = selectedOption.getAttribute('data-description') || '';

    // Check if already active
    if (battleState.activeFieldEffects.some(fe => fe.feldEffektId === feldEffektId)) {
        await Swal.fire({
            icon: 'info',
            title: 'Bereits aktiv',
            text: 'Dieser Feldeffekt ist bereits aktiv.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    // Add to state
    battleState.activeFieldEffects.push({
        feldEffektId,
        name,
        colorCode,
        schwere: schwere,
        beschreibung: beschreibung
    });

    // Re-render
    renderFieldEffects();

    // Sync via SignalR
    await syncBattleState();

    // Close modal
    closeEffectModal();
}

// Remove field effect
export async function removeFieldEffect(feldEffektId) {
    if (!IS_GOTT_ROLE) return;

    battleState.activeFieldEffects = battleState.activeFieldEffects.filter(
        fe => fe.feldEffektId !== feldEffektId
    );

    // Re-render
    renderFieldEffects();

    // Sync via SignalR
    await syncBattleState();
}

// Show field effect description in slide-in panel (für alle sichtbar)
export function showFieldEffectDescription(name, beschreibung, strength, colorCode) {
    const panel = document.getElementById('fieldEffectPanel');
    const overlay = document.getElementById('fieldEffectPanelOverlay');
    const panelTitle = document.getElementById('panelTitle');
    const panelDescription = document.getElementById('panelDescription');
    const panelStrength = document.getElementById('panelStrength');
    const panelHeader = document.getElementById('panelHeader');

    if (!panel || !overlay) return;

    // Setze Inhalt
    panelTitle.textContent = name;
    // HTML-Tags in Beschreibung rendern (innerHTML statt textContent)
    panelDescription.innerHTML = beschreibung || '<em class="text-gray-400">Keine Beschreibung verfügbar</em>';

    // Stärke Badge
    const strengthConfig = {
        'einfach': { text: 'Einfach', classes: 'bg-green-900/60 border border-green-500 text-green-200' },
        'mittel': { text: 'Mittel', classes: 'bg-yellow-900/60 border border-yellow-500 text-yellow-200' },
        'schwer': { text: 'Schwer', classes: 'bg-red-900/60 border border-red-500 text-red-200' }
    };

    const strengthInfo = strengthConfig[strength] || { text: strength, classes: 'bg-gray-700 border border-gray-500 text-gray-200' };
    panelStrength.textContent = strengthInfo.text;
    panelStrength.className = `inline-block px-3 py-1 rounded-full text-sm font-semibold ${strengthInfo.classes}`;

    // Header Border mit Feldeffekt-Farbe
    if (colorCode) {
        panelHeader.style.borderBottomColor = colorCode;
        panelHeader.style.borderBottomWidth = '3px';
        panel.style.borderLeftColor = colorCode;
    }

    // Overlay anzeigen
    overlay.classList.remove('hidden');
    setTimeout(() => {
        overlay.classList.add('overlay-active');
    }, 10);

    // Panel einsliden
    panel.classList.add('panel-open');
}

// Close field effect panel
export function closeFieldEffectPanel() {
    const panel = document.getElementById('fieldEffectPanel');
    const overlay = document.getElementById('fieldEffectPanelOverlay');

    if (!panel || !overlay) return;

    // Panel aussliden
    panel.classList.remove('panel-open');

    // Overlay ausblenden
    overlay.classList.remove('overlay-active');
    setTimeout(() => {
        overlay.classList.add('hidden');
    }, 300);
}

// ===== BIOM SYSTEM =====

// Set active biom
export function setBiom() {
    const select = document.getElementById('biomSelect');
    const selectedOption = select.options[select.selectedIndex];

    if (!selectedOption || !selectedOption.value) {
        alert('Bitte wähle ein Biom aus!');
        return;
    }

    const biomId = parseInt(selectedOption.value);
    const name = selectedOption.getAttribute('data-name');
    const colorCode = selectedOption.getAttribute('data-color');
    const description = selectedOption.getAttribute('data-description');

    battleState.activeBiom = {
        biomId: biomId,
        name: name,
        colorCode: colorCode,
        description: description || ''
    };

    renderBiomDisplay();
    applyBiomBackground();
    closeEffectModal();
    syncBattleState();
}

export async function removeBiom() {
    const confirmed = await Swal.fire({
        title: 'Biom entfernen?',
        text: 'Möchtest du das aktive Biom wirklich entfernen?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Ja, entfernen',
        cancelButtonText: 'Abbrechen',
        confirmButtonColor: '#ef4444'
    });

    if (!confirmed.isConfirmed) {
        return;
    }

    // Remove biom from battle state
    battleState.activeBiom = null;

    // Hide biom banner
    const banner = document.getElementById('biomBanner');
    if (banner) {
        banner.classList.add('hidden');
    }

    // Reset background
    const container = document.getElementById('battleContainer');
    if (container) {
        container.style.background = '';
    }

    // Sync via SignalR
    await syncBattleState();

    await Swal.fire({
        title: 'Entfernt',
        text: 'Das Biom wurde entfernt.',
        icon: 'success',
        timer: 2000,
        showConfirmButton: false
    });
}

// Render biom display
export function renderBiomDisplay() {
    const banner = document.getElementById('biomBanner');
    const nameElement = document.getElementById('activeBiomName');

    if (battleState.activeBiom) {
        nameElement.textContent = battleState.activeBiom.name;
        banner.classList.remove('hidden');

        // Setze Hintergrund-Gradient mit Biom-Farbe
        banner.style.background = `linear-gradient(135deg, ${battleState.activeBiom.colorCode}40 0%, ${battleState.activeBiom.colorCode}20 100%)`;
        banner.style.borderColor = `${battleState.activeBiom.colorCode}40`;
    } else {
        banner.classList.add('hidden');
    }
}

// Apply biom background color to battle container
export function applyBiomBackground() {
    const container = document.getElementById('battleContainer');

    if (battleState.activeBiom && battleState.activeBiom.colorCode) {
        const color = battleState.activeBiom.colorCode;
        // Erstelle einen subtilen Radial-Gradient mit der Biom-Farbe
        container.style.background = `radial-gradient(circle at center, ${color}15 0%, #0a0a0a 60%)`;
    } else {
        // Zurück zum Standard
        container.style.background = '';
    }
}

// Open biom description panel
export function openBiomDescriptionPanel() {
    if (!battleState.activeBiom) return;

    const panel = document.getElementById('biomPanel');
    const overlay = document.getElementById('biomPanelOverlay');
    const header = document.getElementById('biomPanelHeader');
    const title = document.getElementById('biomPanelTitle');
    const description = document.getElementById('biomPanelDescription');

    // Set content
    title.textContent = battleState.activeBiom.name;
    description.innerHTML = battleState.activeBiom.description || 'Keine Beschreibung verfügbar.';

    // Set header color
    if (battleState.activeBiom.colorCode) {
        header.style.background = `linear-gradient(135deg, ${battleState.activeBiom.colorCode}80 0%, ${battleState.activeBiom.colorCode}60 100%)`;
        header.style.borderBottomColor = battleState.activeBiom.colorCode;
        header.style.borderBottomWidth = '3px';
        panel.style.borderLeftColor = battleState.activeBiom.colorCode;
    }

    // Show panel
    overlay.classList.remove('hidden');
    setTimeout(() => {
        overlay.classList.add('overlay-active');
    }, 10);
    panel.classList.add('panel-open');
}

// Close biom panel
export function closeBiomPanel() {
    const panel = document.getElementById('biomPanel');
    const overlay = document.getElementById('biomPanelOverlay');

    if (!panel || !overlay) return;

    panel.classList.remove('panel-open');
    overlay.classList.remove('overlay-active');
    setTimeout(() => {
        overlay.classList.add('hidden');
    }, 300);
}

// ===== EFFECT MODAL (FELDEFFEKTE & BIOME) =====

// Open effect modal
export function openEffectModal() {
    const modal = document.getElementById('effectModal');
    if (modal) {
        modal.classList.remove('hidden');
        // Default tab: Feldeffekte
        switchEffectTab('fieldeffect');
    }
}

// Close effect modal
export function closeEffectModal() {
    const modal = document.getElementById('effectModal');
    if (modal) {
        modal.classList.add('hidden');
    }
}

// Switch between tabs
export function switchEffectTab(tab) {
    const fieldEffectTab = document.getElementById('fieldEffectTab');
    const biomTab = document.getElementById('biomTab');
    const fieldEffectBtn = document.getElementById('tabFieldEffect');
    const biomBtn = document.getElementById('tabBiom');

    if (tab === 'fieldeffect') {
        fieldEffectTab.classList.remove('hidden');
        biomTab.classList.add('hidden');
        fieldEffectBtn.classList.add('border-amber-500', 'text-amber-300');
        fieldEffectBtn.classList.remove('border-transparent', 'text-gray-400');
        biomBtn.classList.add('border-transparent', 'text-gray-400');
        biomBtn.classList.remove('border-green-500', 'text-green-300');
    } else if (tab === 'biom') {
        fieldEffectTab.classList.add('hidden');
        biomTab.classList.remove('hidden');
        fieldEffectBtn.classList.add('border-transparent', 'text-gray-400');
        fieldEffectBtn.classList.remove('border-amber-500', 'text-amber-300');
        biomBtn.classList.add('border-green-500', 'text-green-300');
        biomBtn.classList.remove('border-transparent', 'text-gray-400');
    }
}
