class EntityMentions {
    constructor(textareaSelector) {
        this.textarea = document.querySelector(textareaSelector);
        this.dropdown = null;
        this.entities = [];
        this.currentMention = null;

        // Verschiedene Mention-Typen definieren
        this.mentionTypes = {
            '@': { type: 'character', name: 'Charaktere', color: '#d4af37' },
            '#': { type: 'guild', name: 'Gilden', color: '#198754' },
            '§': { type: 'infanterie', name: 'Infanterien', color: '#dc3545' },
            '&': { type: 'monster', name: 'Monster', color: '#6f42c1' },
            '%': { type: 'magicclass', name: 'Magieklassen', color: '#fd7e14' }
        };

        if (this.textarea) {
            this.init();
        }
    }

    init() {
        console.log('🔄 EntityMentions: Initialisierung gestartet');

        // Prüfen ob CKEditor verwendet wird
        if (this.checkForCKEditor()) {
            console.log('✅ CKEditor-Modus erkannt');
            this.initCKEditorMode();
        } else {
            console.log('✅ Textarea-Modus erkannt');
            // Alte Textarea-Methode
            this.textarea.addEventListener('input', (e) => this.handleInput(e));
            this.textarea.addEventListener('keydown', (e) => this.handleKeydown(e));
        }

        this.createDropdown();
        this.createHelpTooltip();
    }

    checkForCKEditor() {
        // Prüfe ob CKEditor-Element existiert
        return document.querySelector('.ck-editor') !== null;
    }

    initCKEditorMode() {
        // Warte auf CKEditor-Instanz
        let attempts = 0;
        const maxAttempts = 50; // 5 Sekunden

        const checkInterval = setInterval(() => {
            attempts++;

            if (window.currentEditor) {
                clearInterval(checkInterval);
                this.editor = window.currentEditor;
                console.log('✅ CKEditor-Instanz gefunden');
                this.setupCKEditorListeners();
            } else if (attempts >= maxAttempts) {
                clearInterval(checkInterval);
                console.error('❌ CKEditor-Instanz nicht gefunden nach 5 Sekunden');
            }
        }, 100);
    }

    setupCKEditorListeners() {
        console.log('✅ Richte CKEditor-Listeners ein');

        // Lausche auf Tastatureingaben im Editor
        this.editor.editing.view.document.on('keyup', (evt, data) => {
            this.handleCKEditorInput();
        });

        this.editor.editing.view.document.on('keydown', (evt, data) => {
            this.handleCKEditorKeydown(data);
        });
    }

    handleCKEditorInput() {
        // Hole den aktuellen Text aus dem Editor (ohne HTML-Tags)
        const htmlContent = this.editor.getData();
        const text = htmlContent.replace(/<[^>]*>/g, ''); // Entferne HTML-Tags

        // Hole die aktuelle Cursor-Position
        const selection = this.editor.model.document.selection;
        const position = selection.getFirstPosition();

        // Berechne ungefähre Cursor-Position im Text
        const cursorPos = text.length;
        const beforeCursor = text.substring(Math.max(0, cursorPos - 50), cursorPos);

        // Erweiterte Regex für alle Mention-Typen
        const mentionPattern = /([@#§&%])(\w*)$/;
        const match = beforeCursor.match(mentionPattern);

        if (match) {
            const symbol = match[1];
            const searchTerm = match[2];

            console.log(`🔍 Mention erkannt: ${symbol}${searchTerm}`);

            this.currentMention = {
                start: cursorPos - match[0].length,
                end: cursorPos,
                term: searchTerm,
                symbol: symbol,
                type: this.mentionTypes[symbol]?.type || 'all'
            };

            if (searchTerm.length >= 1) {
                this.searchEntities(searchTerm, this.currentMention.type);
            } else if (searchTerm.length === 0) {
                this.showTypeHelp(symbol);
            }
        } else {
            this.hideDropdown();
        }
    }

    handleCKEditorKeydown(data) {
        if (this.dropdown.style.display === 'none') return;

        if (data.keyCode === 27) { // Escape
            this.hideDropdown();
            data.preventDefault();
        }
    }

    insertEntityInCKEditor(entity) {
        console.log('✅ Füge Entity in CKEditor ein:', entity.name);

        // Erstelle HTML-Link
        const linkHtml = `<a href="${entity.url}" class="entity-link ${entity.type}-link" data-entity-type="${entity.type}" data-entity-id="${entity.id}">${entity.name}</a>&nbsp;`;

        // Lösche das getippte Symbol + Suchbegriff
        this.editor.model.change(writer => {
            const selection = this.editor.model.document.selection;
            const position = selection.getFirstPosition();

            // Füge Link ein
            const viewFragment = this.editor.data.processor.toView(linkHtml);
            const modelFragment = this.editor.data.toModel(viewFragment);

            this.editor.model.insertContent(modelFragment, position);
        });
    }

    createHelpTooltip() {
        // Prüfe ob bereits ein Hilfe-Tooltip existiert
        const existingHelp = this.textarea.parentNode.querySelector('.mention-help');
        if (existingHelp) {
            return; // Tooltip bereits vorhanden, keine Duplikate erstellen
        }

        // Hilfe-Tooltip neben dem Textarea erstellen
        const helpDiv = document.createElement('div');
        helpDiv.className = 'mention-help';
        helpDiv.innerHTML = `
        <small class="text-muted">
            💡 <strong>Verlinkungen:</strong> 
            <span style="color: #d4af37;">@Charakter</span> • 
            <span style="color: #198754;">#Gilde</span> • 
            <span style="color: #dc3545;">§Infanterie</span> • 
            <span style="color: #6f42c1;">&Monster</span> • 
            <span style="color: #fd7e14;">%Magie</span>
        </small>
    `;
        this.textarea.parentNode.appendChild(helpDiv);
    }

    createDropdown() {
        this.dropdown = document.createElement('div');
        this.dropdown.className = 'entity-mention-dropdown';
        this.dropdown.style.cssText = `
            position: absolute;
            background: white;
            border: 2px solid #d4c4a8;
            border-radius: 8px;
            max-height: 300px;
            min-width: 300px;
            overflow-y: auto;
            z-index: 1000;
            display: none;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
        `;
        document.body.appendChild(this.dropdown);
    }

    handleInput(e) {
        const text = this.textarea.value;
        const cursorPos = this.textarea.selectionStart;
        const beforeCursor = text.substring(0, cursorPos);

        // Erweiterte Regex für alle Mention-Typen
        const mentionPattern = /([@#§&%])(\w*)$/;
        const match = beforeCursor.match(mentionPattern);

        if (match) {
            const symbol = match[1];
            const searchTerm = match[2];

            this.currentMention = {
                start: cursorPos - match[0].length,
                end: cursorPos,
                term: searchTerm,
                symbol: symbol,
                type: this.mentionTypes[symbol]?.type || 'all'
            };

            if (searchTerm.length >= 1) {
                this.searchEntities(searchTerm, this.currentMention.type);
            } else if (searchTerm.length === 0) {
                // Zeige Hilfe für den Symbol-Typ
                this.showTypeHelp(symbol);
            }
        } else {
            this.hideDropdown();
        }
    }

    async searchEntities(searchTerm, type) {
        try {
            const response = await fetch(`/api/entity/search?query=${encodeURIComponent(searchTerm)}&type=${type}`);
            const data = await response.json();
            this.entities = data.results || [];
            this.showDropdown();
        } catch (error) {
            console.error('Fehler beim Laden der Entitäten:', error);
        }
    }

    showTypeHelp(symbol) {
        const typeInfo = this.mentionTypes[symbol];
        if (!typeInfo) return;

        this.entities = [{
            name: `Suche nach ${typeInfo.name}...`,
            type: 'help',
            icon: '💡',
            subtitle: `Tippe weiter um ${typeInfo.name} zu finden`
        }];
        this.showDropdown();
    }

    showDropdown() {
        if (this.entities.length === 0) {
            this.hideDropdown();
            return;
        }

        const rect = this.textarea.getBoundingClientRect();
        const style = window.getComputedStyle(this.textarea);
        const lineHeight = parseInt(style.lineHeight) || 20;

        this.dropdown.style.left = rect.left + 'px';
        this.dropdown.style.top = (rect.top + lineHeight + 5) + 'px';
        this.dropdown.style.display = 'block';

        // Erweiterte Dropdown-Items mit Icons und Typen
        this.dropdown.innerHTML = this.entities.map((entity, index) => `
            <div class="entity-mention-item ${entity.type === 'help' ? 'help-item' : ''}" 
                 data-index="${index}" 
                 style="
                    padding: 10px 12px;
                    cursor: ${entity.type === 'help' ? 'default' : 'pointer'};
                    border-bottom: 1px solid #eee;
                    display: flex;
                    align-items: center;
                    ${entity.type === 'help' ? 'background: #f8f9fa; font-style: italic;' : ''}
                ">
                <span style="margin-right: 10px; font-size: 1.2em;">${entity.icon}</span>
                <div style="flex: 1;">
                    <div style="font-weight: 600; color: ${this.getTypeColor(entity.type)};">
                        ${entity.name}
                    </div>
                    ${entity.subtitle ? `<div style="font-size: 0.85em; color: #666;">${entity.subtitle}</div>` : ''}
                </div>
                <span style="font-size: 0.75em; color: #999; text-transform: uppercase;">
                    ${this.getTypeLabel(entity.type)}
                </span>
            </div>
        `).join('');

        // Click-Events nur für echte Entitäten
        this.dropdown.querySelectorAll('.entity-mention-item:not(.help-item)').forEach(item => {
            item.addEventListener('click', (e) => {
                const index = parseInt(e.currentTarget.dataset.index);
                this.selectEntity(this.entities[index]);
            });

            item.addEventListener('mouseenter', () => {
                item.style.backgroundColor = '#f0e6d2';
            });
            item.addEventListener('mouseleave', () => {
                item.style.backgroundColor = 'white';
            });
        });
    }

    getTypeColor(type) {
        const colors = {
            'character': '#d4af37',
            'guild': '#198754',
            'infanterie': '#dc3545',
            'monster': '#6f42c1',
            'magicclass': '#fd7e14'
        };
        return colors[type] || '#6c757d';
    }

    getTypeLabel(type) {
        const labels = {
            'character': 'Charakter',
            'guild': 'Gilde',
            'infanterie': 'Infanterie',
            'monster': 'Monster',
            'magicclass': 'Magie'
        };
        return labels[type] || '';
    }

    selectEntity(entity) {
        if (!this.currentMention || entity.type === 'help') return;

        if (this.editor) {
            // CKEditor-Modus: HTML-Link einfügen
            console.log('🔄 Füge Entity im CKEditor-Modus ein');
            this.insertEntityInCKEditor(entity);
        } else {
            // Textarea-Modus: Text-Symbol einfügen
            console.log('🔄 Füge Entity im Textarea-Modus ein');
            const text = this.textarea.value;
            const beforeMention = text.substring(0, this.currentMention.start);
            const afterMention = text.substring(this.currentMention.end);
            const mention = `${this.currentMention.symbol}${entity.name}`;
            const newText = beforeMention + mention + ' ' + afterMention;
            this.textarea.value = newText;

            const newCursorPos = this.currentMention.start + mention.length + 1;
            this.textarea.setSelectionRange(newCursorPos, newCursorPos);
        }

        this.hideDropdown();
    }

    // Neue Methode für WYSIWYG-Integration
    insertEntityInWYSIWYG(entity) {
        const linkHtml = `<a href="/wiki/${entity.type}/${entity.id}" class="entity-link ${entity.type}-link">${entity.name}</a>`;

        // In CKEditor einfügen
        this.editor.model.change(writer => {
            const htmlDataProcessor = this.editor.data.processor;
            const viewFragment = htmlDataProcessor.toView(linkHtml);
            const modelFragment = this.editor.data.toModel(viewFragment);

            this.editor.model.insertContent(modelFragment);
        });
    }

    hideDropdown() {
        this.dropdown.style.display = 'none';
        this.currentMention = null;
    }

    handleKeydown(e) {
        if (this.dropdown.style.display === 'none') return;

        if (e.key === 'Escape') {
            this.hideDropdown();
        }
        // Weitere Keyboard-Navigation könnte hier implementiert werden
    }
}

document.addEventListener('DOMContentLoaded', function () {
    // Finde alle Textareas mit description-bezogenen Namen/IDs
    const descriptionTextareas = document.querySelectorAll([
        'textarea[id*="description"]',
        'textarea[name*="description"]',
        'textarea#description',
        'textarea#Description'
    ].join(', '));

    // Erstelle nur eine EntityMentions-Instanz pro Textarea
    descriptionTextareas.forEach(textarea => {
        if (!textarea.hasAttribute('data-mentions-initialized')) {
            new EntityMentions('#' + textarea.id || `[name="${textarea.name}"]`);
            textarea.setAttribute('data-mentions-initialized', 'true');
        }
    });
});