/**
 * Responsive Tables für Mobile - Konvertiert DataTables in Karten-Ansicht
 */

(function() {
    'use strict';

    // Konfiguration für verschiedene Tabellen
    const tableConfigs = {
        'characterTable': {
            title: row => {
                const img = row.find('td:eq(0) img').attr('src');
                const name = row.find('td:eq(0) strong').text();
                return { img, text: name };
            },
            fields: [
                { label: 'Rufname', selector: 'td:eq(0) strong' },
                { label: 'Vorname', selector: 'td:eq(1) strong' },
                { label: 'Nachname', selector: 'td:eq(2) strong' },
                { label: 'Haus', selector: 'td:eq(3)' },
                { label: 'Herkunftsland', selector: 'td:eq(4)' },
                { label: 'Blutgruppe', selector: 'td:eq(5)' },
                { label: 'Magie', selector: 'td:eq(6)', html: true },
                { label: 'Begleiter', selector: 'td:eq(7)', html: true }
            ],
            actions: row => row.find('td:last .medieval-actions').html()
        },
        'guildTable': {
            title: row => {
                const img = row.find('td:eq(0) img').attr('src');
                const name = row.find('td:eq(0) strong').text();
                return { img, text: name };
            },
            fields: [
                { label: 'Name', selector: 'td:eq(0) strong' },
                { label: 'Gildenmeister', selector: 'td:eq(1)' },
                { label: 'Anmeldung', selector: 'td:eq(2)' },
                { label: 'Rang', selector: 'td:eq(3)' }
            ],
            actions: row => row.find('td:last .medieval-actions').html()
        },
        'infanterieTable': {
            title: row => {
                const img = row.find('td:eq(1) img').attr('src');
                const name = row.find('td:eq(1) strong').text();
                return { img, text: name };
            },
            fields: [
                { label: 'Name', selector: 'td:eq(1) strong' },
                { label: 'General', selector: 'td:eq(2)' },
                { label: 'Sitz', selector: 'td:eq(3)' },
                { label: 'Regimente', selector: 'td:eq(4)', html: true }
            ],
            actions: row => row.find('td:last .medieval-actions').html(),
            expandable: true
        },
        'monsterTable': {
            title: row => {
                const img = row.find('td:eq(0) img').attr('src');
                const name = row.find('td:eq(0) strong').text();
                return { img, text: name };
            },
            fields: [
                { label: 'Name', selector: 'td:eq(0) strong' },
                { label: 'Monstertyp', selector: 'td:eq(1)' },
                { label: 'Begegnung', selector: 'td:eq(2)', html: true },
                { label: 'Encounter', selector: 'td:eq(3)', html: true },
                { label: 'Perfected', selector: 'td:eq(4)', html: true },
                { label: 'Kaufbare Trophäe', selector: 'td:eq(5)', html: true },
                { label: 'Erlegte Trophäe', selector: 'td:eq(6)', html: true }
            ],
            actions: row => row.find('td:last .medieval-actions').html()
        },
        'monstertypTable': {
            title: row => {
                const name = row.find('td:eq(0) strong').text();
                return { text: name };
            },
            fields: [
                { label: 'Name', selector: 'td:eq(0) strong' },
                { label: 'Würfel', selector: 'td:eq(1)' },
                { label: 'Intelligenz', selector: 'td:eq(2)' },
                { label: 'Gruppe', selector: 'td:eq(3)' }
            ],
            actions: row => row.find('td:last .medieval-actions').html()
        }
    };

    /**
     * Konvertiert eine Tabelle in Mobile Cards
     */
    function convertTableToMobileCards(tableId) {
        const config = tableConfigs[tableId];
        if (!config) return;

        const $table = $('#' + tableId);
        const $wrapper = $table.closest('.pergament-table-card');

        // Prüfe ob Mobile Cards Container bereits existiert
        let $mobileContainer = $wrapper.find('.mobile-table-cards');
        if ($mobileContainer.length === 0) {
            $mobileContainer = $('<div class="mobile-table-cards"></div>');
            $table.after($mobileContainer);
        }

        // Lösche bestehende Karten
        $mobileContainer.empty();

        // Hole alle sichtbaren Zeilen (berücksichtigt DataTables Filterung/Pagination)
        const dataTable = $table.DataTable();
        const rows = dataTable.rows({ search: 'applied', page: 'current' }).nodes();

        if (rows.length === 0) {
            $mobileContainer.html('<div class="mobile-empty-message">📋 Keine Einträge vorhanden</div>');
            return;
        }

        // Erstelle eine Karte für jede Zeile
        $(rows).each(function() {
            const $row = $(this);
            const card = createMobileCard($row, config);
            $mobileContainer.append(card);
        });

        // Accordion-Funktionalität hinzufügen
        setupAccordion();
    }

    /**
     * Erstellt eine Mobile Card aus einer Tabellenzeile
     */
    function createMobileCard($row, config) {
        const titleData = config.title($row);
        const $card = $('<div class="mobile-card"></div>');

        // Header
        const $header = $('<div class="mobile-card-header"></div>');
        const $title = $('<div class="mobile-card-title"></div>');

        if (titleData.img) {
            $title.append(`<img src="${titleData.img}" alt="${titleData.text}">`);
        }
        $title.append(`<span>${titleData.text}</span>`);

        const $toggle = $('<span class="mobile-card-toggle">+</span>');

        $header.append($title, $toggle);
        $card.append($header);

        // Body (zusammenklappbar)
        const $body = $('<div class="mobile-card-body"></div>');
        const $content = $('<div class="mobile-card-content"></div>');

        // Felder hinzufügen
        config.fields.forEach(field => {
            const $field = $('<div class="mobile-field"></div>');
            const $label = $(`<div class="mobile-field-label">${field.label}:</div>`);
            const $value = $('<div class="mobile-field-value"></div>');

            if (field.html) {
                $value.html($row.find(field.selector).html() || '-');
            } else {
                $value.text($row.find(field.selector).text() || '-');
            }

            $field.append($label, $value);
            $content.append($field);
        });

        $body.append($content);

        // Aktionen hinzufügen
        if (config.actions) {
            const actionsHtml = config.actions($row);
            if (actionsHtml) {
                const $actions = $('<div class="mobile-card-actions"></div>');

                // Parse Actions und konvertiere in Mobile Buttons
                const $tempDiv = $('<div></div>').html(actionsHtml);

                const $editBtn = $tempDiv.find('.edit-btn');
                if ($editBtn.length > 0) {
                    const href = $editBtn.attr('href');
                    const title = $editBtn.attr('title') || 'Bearbeiten';
                    $actions.append(`<a href="${href}" class="mobile-action-btn mobile-action-edit" title="${title}">✏️ Bearbeiten</a>`);
                }

                const $deleteBtn = $tempDiv.find('.delete-btn');
                if ($deleteBtn.length > 0) {
                    const onclick = $deleteBtn.attr('onclick');
                    const title = $deleteBtn.attr('title') || 'Löschen';
                    $actions.append(`<button class="mobile-action-btn mobile-action-delete" onclick="${onclick}" title="${title}">🗡️ Löschen</button>`);
                }

                $body.append($actions);
            }
        }

        $card.append($body);

        return $card;
    }

    /**
     * Richtet Accordion-Funktionalität ein
     */
    function setupAccordion() {
        $('.mobile-card-header').off('click').on('click', function() {
            const $header = $(this);
            const $body = $header.siblings('.mobile-card-body');
            const $toggle = $header.find('.mobile-card-toggle');

            // Toggle Body
            $body.toggleClass('expanded');
            $toggle.toggleClass('expanded');

            // Update Toggle-Text
            if ($body.hasClass('expanded')) {
                $toggle.text('−');
            } else {
                $toggle.text('+');
            }
        });
    }

    /**
     * Aktualisiert Mobile Cards wenn DataTable sich ändert
     */
    function setupDataTableListeners() {
        Object.keys(tableConfigs).forEach(tableId => {
            const $table = $('#' + tableId);
            if ($table.length > 0) {
                const dataTable = $table.DataTable();

                // Bei Draw-Event (Filterung, Pagination, etc.)
                dataTable.on('draw', function() {
                    if (window.innerWidth <= 768) {
                        convertTableToMobileCards(tableId);
                    }
                });
            }
        });
    }

    /**
     * Prüft Bildschirmgröße und konvertiert Tabellen
     */
    function checkScreenSize() {
        if (window.innerWidth <= 768) {
            Object.keys(tableConfigs).forEach(tableId => {
                if ($('#' + tableId).length > 0) {
                    convertTableToMobileCards(tableId);
                }
            });
        }
    }

    /**
     * Initialisierung
     */
    $(document).ready(function() {
        // Warte bis DataTables initialisiert sind
        setTimeout(function() {
            checkScreenSize();
            setupDataTableListeners();
        }, 500);

        // Bei Fenstergrößen-Änderung
        let resizeTimer;
        $(window).on('resize', function() {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(function() {
                checkScreenSize();
            }, 250);
        });
    });
})();
