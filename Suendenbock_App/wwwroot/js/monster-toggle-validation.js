/**
 * Monster Toggle Validation
 * Stellt sicher, dass die Toggle-Hierarchie eingehalten wird:
 * - Encounter kann nur aktiviert werden, wenn Meet aktiviert ist
 * - Perfected kann nur aktiviert werden, wenn Meet UND Encounter aktiviert sind
 */

class MonsterToggleValidator {
    constructor(meetSelector, encounterSelector, perfectedSelector) {
        this.meetToggle = $(meetSelector);
        this.encounterToggle = $(encounterSelector);
        this.perfectedToggle = $(perfectedSelector);

        this.init();
    }

    init() {
        // Event-Listener hinzufügen
        this.meetToggle.on('change', () => this.updateToggleStates());
        this.encounterToggle.on('change', () => this.updateToggleStates());

        // Initiale Zustände setzen
        this.updateToggleStates();
    }

    updateToggleStates() {
        const isMeetChecked = this.meetToggle.is(':checked');
        const isEncounterChecked = this.encounterToggle.is(':checked');

        // Encounter kann nur aktiviert werden, wenn Meet aktiviert ist
        if (!isMeetChecked) {
            this.encounterToggle.prop('checked', false).prop('disabled', true);
            this.encounterToggle.parent().css('opacity', '0.5');
        } else {
            this.encounterToggle.prop('disabled', false);
            this.encounterToggle.parent().css('opacity', '1');
        }

        // Perfected kann nur aktiviert werden, wenn Meet UND Encounter aktiviert sind
        if (!isMeetChecked || !isEncounterChecked) {
            this.perfectedToggle.prop('checked', false).prop('disabled', true);
            this.perfectedToggle.parent().css('opacity', '0.5');
        } else {
            this.perfectedToggle.prop('disabled', false);
            this.perfectedToggle.parent().css('opacity', '1');
        }
    }

    // Validierung vor dem Absenden (für Formulare)
    validateForSubmit() {
        const isMeetChecked = this.meetToggle.is(':checked');
        const isEncounterChecked = this.encounterToggle.is(':checked');
        const isPerfectedChecked = this.perfectedToggle.is(':checked');

        // Validierung: Encounter ohne Meet
        if (isEncounterChecked && !isMeetChecked) {
            Swal.fire({
                title: 'Ungültige Auswahl',
                text: 'Encounter kann nur aktiviert werden, wenn Begegnung aktiviert ist.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return false;
        }

        // Validierung: Perfected ohne Meet oder Encounter
        if (isPerfectedChecked && (!isMeetChecked || !isEncounterChecked)) {
            Swal.fire({
                title: 'Ungültige Auswahl',
                text: 'Perfected kann nur aktiviert werden, wenn sowohl Begegnung als auch Encounter aktiviert sind.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return false;
        }

        return true;
    }

    // Validierung für Admin-Toggle (AJAX)
    validateToggle(toggleType, newValue) {
        const isMeetChecked = this.meetToggle.is(':checked');
        const isEncounterChecked = this.encounterToggle.is(':checked');

        // Wenn Encounter aktiviert werden soll, muss Meet aktiviert sein
        if (toggleType === 'encounter' && newValue && !isMeetChecked) {
            Swal.fire({
                title: 'Ungültige Auswahl',
                text: 'Encounter kann nur aktiviert werden, wenn Begegnung aktiviert ist.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return false;
        }

        // Wenn Perfected aktiviert werden soll, müssen Meet UND Encounter aktiviert sein
        if (toggleType === 'perfected' && newValue && (!isMeetChecked || !isEncounterChecked)) {
            Swal.fire({
                title: 'Ungültige Auswahl',
                text: 'Perfected kann nur aktiviert werden, wenn sowohl Begegnung als auch Encounter aktiviert sind.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return false;
        }

        return true;
    }
}

// Hilfsfunktion für Admin-Tabelle: Validierung für einzelne Monster-Zeile
function validateMonsterToggle(monsterId, toggleType, isChecked) {
    console.log('validateMonsterToggle called:', { monsterId, toggleType, isChecked });

    // Suche nach Toggles mit data-monster-id
    const meetToggle = $(`.monster-toggle[data-monster-id="${monsterId}"][data-toggle-type="meet"]`);
    const encounterToggle = $(`.monster-toggle[data-monster-id="${monsterId}"][data-toggle-type="encounter"]`);
    const perfectedToggle = $(`.monster-toggle[data-monster-id="${monsterId}"][data-toggle-type="perfected"]`);

    console.log('Found toggles:', {
        meet: meetToggle.length,
        encounter: encounterToggle.length,
        perfected: perfectedToggle.length
    });

    const isMeetChecked = meetToggle.is(':checked');
    const isEncounterChecked = encounterToggle.is(':checked');

    console.log('Current states:', { isMeetChecked, isEncounterChecked });

    // AKTIVIERUNGS-Validierungen (wenn isChecked = true)
    if (isChecked) {
        // Wenn Encounter aktiviert werden soll, muss Meet aktiviert sein
        if (toggleType === 'encounter' && !isMeetChecked) {
            console.log('Validation failed: Encounter ohne Meet');
            Swal.fire({
                title: 'Ungültige Auswahl',
                text: 'Encounter kann nur aktiviert werden, wenn Begegnung aktiviert ist.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return false;
        }

        // Wenn Perfected aktiviert werden soll, müssen Meet UND Encounter aktiviert sein
        if (toggleType === 'perfected' && (!isMeetChecked || !isEncounterChecked)) {
            console.log('Validation failed: Perfected ohne Meet/Encounter');
            Swal.fire({
                title: 'Ungültige Auswahl',
                text: 'Perfected kann nur aktiviert werden, wenn sowohl Begegnung als auch Encounter aktiviert sind.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return false;
        }
    }

    console.log('Validation passed');
    return true;
}
