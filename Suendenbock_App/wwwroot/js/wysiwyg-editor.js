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

            // Prüfe ob das Editor-Element existiert
            const editorContainer = document.querySelector('#wysiwyg-editor');

            if (!editorContainer) {
                console.warn('⚠️ #wysiwyg-editor Element nicht gefunden - Editor wird nicht initialisiert');
                this.textarea.classList.remove('d-none');
                return;
            }

            // CKEditor erstellen
            this.editor = await ClassicEditor.create(editorContainer, {
                toolbar: [
                    'heading', '|',
                    'bold', 'italic', 'underline', '|',
                    'link', 'bulletedList', 'numberedList', '|',
                    'undo', 'redo'
                ],
                placeholder: 'Beschreibung eingeben...',
                language: 'de'
            });

            console.log('✅ CKEditor erfolgreich initialisiert');

            // Editor global verfügbar machen
            window.currentEditor = this.editor;
            console.log('✅ Editor ist jetzt global verfügbar');

            // Inhalt vom Textarea laden
            this.editor.setData(this.textarea.value);

            // Synchronisierung
            this.editor.model.document.on('change:data', () => {
                this.textarea.value = this.editor.getData();
            });

            console.log('✅ WikiWYSIWYGEditor vollständig initialisiert');

        } catch (error) {
            console.error('❌ Fehler beim Laden des Editors:', error);
            // Fallback: Zeige normales Textarea
            this.textarea.classList.remove('d-none');
            const editorContainer = document.querySelector('#wysiwyg-editor');
            if (editorContainer) {
                editorContainer.style.display = 'none';
            }
        }
    }
}