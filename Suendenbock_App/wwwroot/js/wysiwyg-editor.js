class WikiWYSIWYGEditor {
    constructor(textareaSelector, editorContainerSelector = '#wysiwyg-editor') {
        this.textarea = document.querySelector(textareaSelector);
        this.textareaSelector = textareaSelector;
        this.editorContainerSelector = editorContainerSelector;
        this.editor = null;

        if (!window.editors) {
            window.editors = {};
        }

        if (window.editors[textareaSelector]) {
            return;
        }

        if (this.textarea && this.textarea.hasAttribute('data-editor-initialized')) {
            return;
        }

        if (this.textarea) {
            this.textarea.setAttribute('data-editor-initialized', 'true');
            this.init();
        }
    }

    async init() {
        try {
            const editorContainer = document.querySelector(this.editorContainerSelector);

            if (!editorContainer) {
                this.textarea.classList.remove('d-none');
                return;
            }

            if (window.editors[this.editorContainerSelector]) {
                return;
            }

            this.editor = await ClassicEditor.create(editorContainer, {
                toolbar: [
                    'heading', '|',
                    'bold', 'italic', '|',
                    'link', 'bulletedList', 'numberedList', '|',
                    'undo', 'redo'
                ],
                placeholder: 'Beschreibung eingeben...',
                language: 'de'
            });

            window.editors[this.textareaSelector] = this.editor;
            window.editors[this.editorContainerSelector] = this.editor;
            window.currentEditor = this.editor;

            console.log('✅ Editor registriert:', this.textareaSelector, '→', this.editorContainerSelector);
            console.log('📦 Alle Editoren:', Object.keys(window.editors));

            this.editor.setData(this.textarea.value);

            this.editor.model.document.on('change:data', () => {
                this.textarea.value = this.editor.getData();
            });

        } catch (error) {
            this.textarea.classList.remove('d-none');
            this.textarea.removeAttribute('data-editor-initialized');
            const editorContainer = document.querySelector(this.editorContainerSelector);
            if (editorContainer) {
                editorContainer.style.display = 'none';
            }
        }
    }
}