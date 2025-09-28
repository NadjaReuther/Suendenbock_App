class WikiWYSIWYGEditor {
    constructor(textareaSelector) {
        this.textarea = document.querySelector(textareaSelector);
        this.editor = null;
        this.entityMentions = null;

        if (this.textarea) {
            this.init();
        }
    }

    async init() {
        try {
            // CKEditor initialisieren
            this.editor = await ClassicEditor.create(
                document.querySelector('#wysiwyg-editor'),
                {
                    toolbar: [
                        'heading', '|',
                        'bold', 'italic', 'underline', '|',
                        'link', 'bulletedList', 'numberedList', '|',
                        'outdent', 'indent', '|',
                        'blockQuote', 'insertTable', '|',
                        'undo', 'redo'
                    ],
                    placeholder: 'Beschreibung eingeben...'
                }
            );

            // Inhalt vom versteckten Textarea laden
            this.editor.setData(this.textarea.value);

            // Editor-Inhalt mit Textarea synchronisieren
            this.editor.model.document.on('change:data', () => {
                this.textarea.value = this.editor.getData();
            });

            // Entity-Mentions-System integrieren
            this.setupEntityMentions();

        } catch (error) {
            console.error('Editor konnte nicht geladen werden:', error);
            // Fallback: Normales Textarea anzeigen
            this.textarea.classList.remove('d-none');
        }
    }

    setupEntityMentions() {
        // Bestehende EntityMentions-Funktionalität erweitern
        this.editor.editing.view.document.on('keydown', (evt, data) => {
            if (data.keyCode === 64) { // @ Symbol
                this.showMentionsDropdown('character');
            } else if (data.keyCode === 35) { // # Symbol
                this.showMentionsDropdown('guild');
            }
            // Weitere Symbole...
        });
    }
}