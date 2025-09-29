class WikiWYSIWYGEditor {
    constructor(textareaSelector) {
        this.textarea = document.querySelector(textareaSelector);
        this.editor = null;

        if (this.textarea) {
            console.log('✅ WikiWYSIWYGEditor: Textarea gefunden');
            this.init();
        } else {
            console.error('❌ WikiWYSIWYGEditor: Textarea nicht gefunden:', textareaSelector);
        }
    }

    async init() {
        try {
            console.log('🔄 Starte CKEditor Initialisierung...');

            // CKEditor erstellen
            this.editor = await ClassicEditor.create(
                document.querySelector('#wysiwyg-editor'),
                {
                    toolbar: [
                        'heading', '|',
                        'bold', 'italic', 'underline', '|',
                        'link', 'bulletedList', 'numberedList', '|',
                        'undo', 'redo'
                    ],
                    placeholder: 'Beschreibung eingeben...',
                    language: 'de'
                }
            );

            console.log('✅ CKEditor erfolgreich initialisiert');

            // Editor global verfügbar machen für EntityMentions
            window.currentEditor = this.editor;
            console.log('✅ Editor ist jetzt global verfügbar (window.currentEditor)');

            // Inhalt vom versteckten Textarea laden
            this.editor.setData(this.textarea.value);

            // Bei Änderungen im Editor -> Textarea aktualisieren
            this.editor.model.document.on('change:data', () => {
                this.textarea.value = this.editor.getData();
            });

            console.log('✅ WikiWYSIWYGEditor vollständig initialisiert');

        } catch (error) {
            console.error('❌ Fehler beim Laden des Editors:', error);
            // Fallback: Zeige normales Textarea
            this.textarea.classList.remove('d-none');
            document.querySelector('#wysiwyg-editor').style.display = 'none';
        }
    }
}