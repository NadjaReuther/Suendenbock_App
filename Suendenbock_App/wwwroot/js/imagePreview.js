function previewImage(input, previewSelector) {
    if (input.files && input.files[0]) {
        const file = input.files[0];
        const maxSize = 5 * 1024 * 1024;
        const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];

        if (file.size > maxSize) {
            Swal.fire({
                title: 'Datei zu groß!',
                text: 'Die Datei darf maximal 5MB groß sein.',
                icon: 'error'
            });
            input.value = '';
            return;
        }

        if (!allowedTypes.includes(file.type)) {
            Swal.fire({
                title: 'Ungültiger Dateityp!',
                text: 'Nur JPG und PNG Dateien sind erlaubt!',
                icon: 'error'
            });
            input.value = '';
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            const preview = document.querySelector(previewSelector);
            preview.innerHTML = `<img src="${e.target.result}" alt="Vorschau" style="width: 100%; height: 100%; object-fit: cover; border-radius: 12px;">`;
        };
        reader.readAsDataURL(file);
    }
}

function enableImageDragDrop(dropZoneSelector, inputId) {
    const dropZone = document.querySelector(dropZoneSelector);
    const fileInput = document.getElementById(inputId);

    dropZone.addEventListener('dragover', function (e) {
        e.preventDefault();
        dropZone.classList.add('drag-over');
    });

    dropZone.addEventListener('dragleave', function (e) {
        e.preventDefault();
        dropZone.classList.remove('drag-over');
    });

    dropZone.addEventListener('drop', function (e) {
        e.preventDefault();
        dropZone.classList.remove('drag-over');

        const files = e.dataTransfer.files;
        if (files.length > 0) {
            fileInput.files = files;
            previewImage(fileInput, dropZoneSelector);
        }
    });

    enableImageDragDrop(dropZoneSelector, inputId);
}

function enableImageDragOut(dromZoneSelector, inputId) {
    const dropZone = document.querySelector(dropZoneSelector);
    const fileInput = document.getElementById(inputId);

    dropZone.addEventListener('dragstart', function (e) {
        const img = dropZone.querySelector('img');
        if (img && fileInput.files.length > 0) {
            e.dataTransfer.setData('text/plain', img.src);
            e.dataTransfer.effectAllowed = 'copy';

            dropZone.classList.add('dragging-out');

            const canvas = document.createElement('canvas');
            const ctx = canvas.getContext('2d');

            canvas.width = 100;
            canvas.height = 100;

            const dragImg = new Image();
            dragImg.onload = function () {
                ctx.drawImage(dragImg, 0, 0, 100, 100);
                const dragImageUrl = canvas.toDataURL();
                const dragImage = new Image();
                dragImage.src = dragImageUrl();
                e.dataTransfer.setDragImage(dragImage, 50, 50);
            };
            dragImg.src = img.src;
        }
    });

    dropZone.addEventListener('dragend', function (e) {
        dropZone.classList.remove('dragging-out');
    });

    const observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            const img = dropZone.querySelector('img');
            if (img) {
                img.draggable = true;
                dropZone.draggable = true;

                img.addEventListener('contextmenu', function (e) {
                    e.preventDefault();
                    showImageContextMenu(e, dropZoneSelector, inputId);
                });
            }
            else {
                dropZone.draggable = false;
            }
        });
    });

    observer.observe(dropZone, { childList: true, subtree: true });
}

function showImageContextMenu(e, dropZoneSelector, inputId) {
    const existingMenu = document.querySelector('.image-context-menu');
    if (existingMenu) {
        existingMenu.remove();
    }

    const menu = document.createElement('div');
    menu.className = 'image-context-menu';
    menu.style.cssText = `
        position: fixed;
        top: ${e.clientY}px;
        left: ${e.clientX}px;
        background: white;
        border: 2px solid #6e4b1f;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.3);
        z-index: 1000;
        padding: 8px 0;
        min-width: 180px;
        `;

    const options = [
        { text: '📁 Bild speichern unter...', action: () => downloadImage(dropZoneSelector) },
        { text: '📋 Bild kopieren', action: () => copyImageToClipboard(dropZoneSelector) },
        { text: '🗑️ Bild entfernen', action: () => removeImage(dropZoneSelector, inputId) },
        { text: '🔄 Bild ersetzen', action: () => document.getElementById(inputId).click() }
    ];

    options.forEach(option => {
        const item = document.createElement('div');
        item.textContent = option.text;
        item.style.cssText = `
            padding: 8px 16px;
            cursor: pointer;
            font-size: 14px;
            border-bottom: 1px solid #eee;
            `;

        item.addEventListener('mouseenter', () => item.style.backgroundColor = '#f5f5f5');
        item.addEventListener('mouseleave', () => item.style.backgroundColor = 'white');
        item.addEventListener('click', () => {
            option.action();
            menu.remove();
        });
        menu.appendChild(item);
    });
    document.body.appendChild(menu);

    setTimeout(() => {
        document.addEventListener('click', function closeMenu() {
            menu.remove();
            document.removeEventListener('click', closeMenu);
        });
    }, 100);
}

function downloadImage(dropZoneSelector) {
    const img = document.querySelector(dropZoneSelector + ' img');
    if (img) {
        const link = document.createElement('a');
        link.download = 'bild.jpg';
        link.href = img.src;
        link.click();
    }
}

function copyImageToClipboard(dropZoneSelector) {
    const img = document.querySelector(dropZoneSelector + ' img');
    if (img) {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');
        canvas.width = img.naturalWidth;
        canvas.height = img.naturalHeight;
        ctx.drawImage(img, 0, 0);

        canvas.toBlob(blob => {
            navigator.clipboard.write([
                new ClipboardItem({ 'image/png': blob })
            ]).then(() => {
                Swal.fire({
                    title: 'Erfolg!',
                    text: 'Bild wurde in die Zwischenablage kopiert.',
                    icon: 'success',
                    timer: 2000
                });
            });
        });
    }
}

function removeImage(dropZoneSelector, inputId) {
    Swal.fire({
        title: 'Bild entfernen?',
        text: 'Möchtest du das aktuelle Bild wirklich entfernen?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Ja, entfernen',
        cancelButtonText: 'Abbrechen'
    }).then((result) => {
        if (result.isConfirmed) {
            const dropZone = document.querySelector(dropZoneSelector);
            const fileInput = document.getElementById(inputId);

            fileInput.value = '';

            dropZone.innerHTML = `
                <div class="image-placeholder">
                    <i>🏰</i>
                    <div>Gildenlogo<br><small>Klicken zum Hochladen</small></div>
                </div>
                <div class="drop-zone-hint">📁 Datei hier ablegen</div>
            `;
            dropZone.draggable = false;
        }
    });
}