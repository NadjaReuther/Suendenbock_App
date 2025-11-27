class EntityMentions {
    constructor(textareaSelector) {
        this.textareaSelector = textareaSelector;
        this.textarea = document.querySelector(textareaSelector);
        this.dropdown = null;
        this.entities = [];
        this.currentMention = null;

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
        // Prüfen ob CKEditor verwendet wird
        if (this.checkForCKEditor()) {           
            this.initCKEditorMode();
        } else {
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
        let attempts = 0;
        const maxAttempts = 50;

        const checkInterval = setInterval(() => {
            attempts++;
            const editorInstance = window.editors?.[this.textareaSelector] || window.currentEditor;

            if (editorInstance) {
                clearInterval(checkInterval);
                this.editor = editorInstance;
                this.setupCKEditorListeners();
            } else if (attempts >= maxAttempts) {
                clearInterval(checkInterval);
            }
        }, 100);
    }

    setupCKEditorListeners() {
        // Lausche auf Tastatureingaben im Editor
        this.editor.editing.view.document.on('keyup', (evt, data) => {
            this.handleCKEditorInput();
        });

        this.editor.editing.view.document.on('keydown', (evt, data) => {
            this.handleCKEditorKeydown(data);
        });
    }

    handleCKEditorInput() {
        const selection = this.editor.model.document.selection;
        const position = selection.getFirstPosition();

        // Hole den Text VOR der Cursor-Position
        const root = this.editor.model.document.getRoot();
        const range = this.editor.model.createRange(
            this.editor.model.createPositionAt(root, 0),
            position
        );

        // Extrahiere den Text aus dem Range
        let textBeforeCursor = '';
        for (const item of range.getItems()) {
            if (item.is('$textProxy')) {
                textBeforeCursor += item.data;
            } else if (item.is('$text')) {
                textBeforeCursor += item.data;
            }
        }
        // Suche nach Mention-Pattern in den letzten 50 Zeichen
        const searchText = textBeforeCursor.substring(Math.max(0, textBeforeCursor.length - 50));
        const mentionPattern = /([@#§&%])(\w*)$/;
        const match = searchText.match(mentionPattern);

        if (match) {
            const symbol = match[1];
            const searchTerm = match[2];

            this.currentMention = {
                start: textBeforeCursor.length - match[0].length,
                end: textBeforeCursor.length,
                term: searchTerm,
                symbol: symbol,
                type: this.mentionTypes[symbol]?.type || 'all',
                position: position  // ✅ Speichere die echte Position für später
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
        // Emoji-Mapping
        const emojiMap = {
            'character': '👤',
            'guild': '🏰',
            'infanterie': '⚔️',
            'monster': '👹',
            'magicclass': '🔮'
        };

        const emoji = emojiMap[entity.type] || '🔗';

        // ✅ Einfacher Link mit nur Emoji, keine CSS-Klassen
        const linkHtml = `<a href="${entity.url}" data-entity-type="${entity.type}" data-entity-id="${entity.id}">${emoji}${entity.name}</a>`;
        const textToReplace = this.currentMention.symbol + this.currentMention.term;

        // FIX: & wird zu &amp; encodiert - versuche beide Varianten
        const textToReplaceEncoded = textToReplace.replace(/&/g, '&amp;');

        const currentContent = this.editor.getData();
        let lastIndex = currentContent.lastIndexOf(textToReplace);

        // Wenn nicht gefunden, versuche die HTML-encodierte Version
        if (lastIndex === -1) {
            lastIndex = currentContent.lastIndexOf(textToReplaceEncoded);
        }

        if (lastIndex !== -1) {
            // Verwende die korrekte Länge (encodiert oder nicht)
            const actualTextLength = currentContent.substring(lastIndex).startsWith(textToReplaceEncoded)
                ? textToReplaceEncoded.length
                : textToReplace.length;

            const newContent =
                currentContent.substring(0, lastIndex) +
                linkHtml +
                ' ' +
                currentContent.substring(lastIndex + actualTextLength);

            this.editor.setData(newContent);

            setTimeout(() => {
                this.editor.editing.view.focus();
                const model = this.editor.model;
                const root = model.document.getRoot();
                const endPosition = model.createPositionAt(root, 'end');

                model.change(writer => {
                    writer.setSelection(endPosition);
                });
            }, 50);
        }
        this.editor.editing.view.focus();
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
            position: fixed;
            background: white;
            border: 2px solid #d4c4a8;
            border-radius: 8px;
            max-height: 300px;
            min-width: 300px;
            overflow-y: auto;
            z-index: 10000;
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

        let rect, top, left;

        if (this.editor) {
            const editorElement = this.editor.ui.view.editable.element;

            if (editorElement) {
                rect = editorElement.getBoundingClientRect();

                const selection = this.editor.editing.view.document.selection;
                const range = selection.getFirstRange();

                if (range) {
                    const domRange = this.editor.editing.view.domConverter.viewRangeToDom(range);

                    if (domRange) {
                        const cursorRect = domRange.getBoundingClientRect();
                        left = cursorRect.left;
                        top = cursorRect.bottom + 5;
                    } else {
                        left = rect.left;
                        top = rect.top + 50;
                    }
                } else {
                    left = rect.left;
                    top = rect.top + 50;
                }
            } else {
                rect = this.textarea.getBoundingClientRect();
                left = rect.left;
                top = rect.top + 30;
            }
        } else {
            rect = this.textarea.getBoundingClientRect();
            const style = window.getComputedStyle(this.textarea);
            const lineHeight = parseInt(style.lineHeight) || 20;

            left = rect.left;
            top = rect.top + lineHeight + 5;
        }

        // ✅ Setze Position
        this.dropdown.style.left = left + 'px';
        this.dropdown.style.top = top + 'px';
        this.dropdown.style.display = 'block';

        // Prüfe ob Dropdown außerhalb des Viewports ist
        const dropdownRect = this.dropdown.getBoundingClientRect();
        const viewportHeight = window.innerHeight;

        // Wenn Dropdown nach unten aus dem Viewport ragt, zeige es oberhalb
        if (dropdownRect.bottom > viewportHeight) {
            const newTop = top - dropdownRect.height - 30; // 30px Abstand
            if (newTop > 0) {
                this.dropdown.style.top = newTop + 'px';
            }
        }

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
            this.insertEntityInCKEditor(entity);
        } else {
            // Textarea-Modus: Text-Symbol einfügen
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

//Helper Function für Entity-mention außerhalb vom ckEditor verwenden
function initEntityMentionsForTextarea(selector) {
    const textarea = document.querySelector(selector);
    if (textarea && !textarea.hasAttribute('data-mentions-initialized')) {
        new EntityMentions(selector);
        textarea.setAttribute('data-mentions-initialized', 'true');
    }
}