// DOM Elemente
const textInput = document.getElementById('text-input');
const sizeInput = document.getElementById('size-input');
const sizeValue = document.getElementById('size-value');
const errorLevel = document.getElementById('error-level');
const fgColor = document.getElementById('fg-color');
const fgColorText = document.getElementById('fg-color-text');
const bgColor = document.getElementById('bg-color');
const bgColorText = document.getElementById('bg-color-text');
const logoInput = document.getElementById('logo-input');
const removeLogo = document.getElementById('remove-logo');
const logoSizeGroup = document.getElementById('logo-size-group');
const logoSize = document.getElementById('logo-size');
const logoSizeValue = document.getElementById('logo-size-value');
const generateBtn = document.getElementById('generate-btn');
const downloadBtn = document.getElementById('download-btn');
const qrCodeContainer = document.getElementById('qr-code');
const placeholderText = document.querySelector('.placeholder-text');

let currentQRCode = null;
let uploadedLogo = null;

// Event Listeners
sizeInput.addEventListener('input', () => {
    sizeValue.textContent = sizeInput.value;
});

logoSize.addEventListener('input', () => {
    logoSizeValue.textContent = logoSize.value;
});

fgColor.addEventListener('input', () => {
    fgColorText.value = fgColor.value.toUpperCase();
});

fgColorText.addEventListener('input', () => {
    if (/^#[0-9A-F]{6}$/i.test(fgColorText.value)) {
        fgColor.value = fgColorText.value;
    }
});

bgColor.addEventListener('input', () => {
    bgColorText.value = bgColor.value.toUpperCase();
});

bgColorText.addEventListener('input', () => {
    if (/^#[0-9A-F]{6}$/i.test(bgColorText.value)) {
        bgColor.value = bgColorText.value;
    }
});

logoInput.addEventListener('change', (e) => {
    const file = e.target.files[0];
    if (file && file.type.startsWith('image/')) {
        const reader = new FileReader();
        reader.onload = (event) => {
            const img = new Image();
            img.onload = () => {
                uploadedLogo = img;
                removeLogo.style.display = 'block';
                logoSizeGroup.style.display = 'flex';
            };
            img.src = event.target.result;
        };
        reader.readAsDataURL(file);
    }
});

removeLogo.addEventListener('click', () => {
    uploadedLogo = null;
    logoInput.value = '';
    removeLogo.style.display = 'none';
    logoSizeGroup.style.display = 'none';
});

generateBtn.addEventListener('click', generateQRCode);

downloadBtn.addEventListener('click', downloadQRCode);

// Enter-Taste im Textfeld soll auch generieren
textInput.addEventListener('keypress', (e) => {
    if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        generateQRCode();
    }
});

async function generateQRCode() {
    const text = textInput.value.trim();

    if (!text) {
        await Swal.fire({
            icon: 'warning',
            title: 'Eingabe fehlt',
            text: 'Bitte geben Sie Text oder eine URL ein!',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    // Alten QR-Code entfernen
    qrCodeContainer.innerHTML = '';
    placeholderText.style.display = 'none';

    const size = parseInt(sizeInput.value);

    // QR-Code generieren
    currentQRCode = new QRCode(qrCodeContainer, {
        text: text,
        width: size,
        height: size,
        colorDark: fgColor.value,
        colorLight: bgColor.value,
        correctLevel: QRCode.CorrectLevel[errorLevel.value]
    });

    // Wenn ein Logo vorhanden ist, nach kurzer Verzögerung hinzufügen
    if (uploadedLogo) {
        setTimeout(() => {
            addLogoToQRCode();
        }, 100);
    } else {
        downloadBtn.style.display = 'block';
    }
}

function addLogoToQRCode() {
    const qrImage = qrCodeContainer.querySelector('img');
    if (!qrImage) return;

    // Warten bis das QR-Code Bild geladen ist
    if (!qrImage.complete) {
        qrImage.onload = () => addLogoToQRCode();
        return;
    }

    const size = parseInt(sizeInput.value);
    const logoSizePercent = parseInt(logoSize.value) / 100;
    const logoPixelSize = size * logoSizePercent;

    // Canvas erstellen
    const canvas = document.createElement('canvas');
    canvas.width = size;
    canvas.height = size;
    const ctx = canvas.getContext('2d');

    // QR-Code auf Canvas zeichnen
    ctx.drawImage(qrImage, 0, 0, size, size);

    // Weißen Hintergrund für Logo erstellen
    const logoBgSize = logoPixelSize * 1.2; // 20% größer als Logo
    const logoBgX = (size - logoBgSize) / 2;
    const logoBgY = (size - logoBgSize) / 2;

    ctx.fillStyle = bgColor.value;
    ctx.fillRect(logoBgX, logoBgY, logoBgSize, logoBgSize);

    // Logo auf Canvas zeichnen
    const logoX = (size - logoPixelSize) / 2;
    const logoY = (size - logoPixelSize) / 2;

    ctx.drawImage(uploadedLogo, logoX, logoY, logoPixelSize, logoPixelSize);

    // Canvas anstelle des img-Tags anzeigen
    qrCodeContainer.innerHTML = '';
    qrCodeContainer.appendChild(canvas);

    downloadBtn.style.display = 'block';
}

async function downloadQRCode() {
    const canvas = qrCodeContainer.querySelector('canvas');
    const img = qrCodeContainer.querySelector('img');

    let downloadCanvas;

    if (canvas) {
        downloadCanvas = canvas;
    } else if (img) {
        // Wenn nur ein Bild vorhanden ist, konvertieren wir es zu Canvas
        downloadCanvas = document.createElement('canvas');
        const size = parseInt(sizeInput.value);
        downloadCanvas.width = size;
        downloadCanvas.height = size;
        const ctx = downloadCanvas.getContext('2d');
        ctx.drawImage(img, 0, 0, size, size);
    } else {
        await Swal.fire({
            icon: 'warning',
            title: 'QR-Code fehlt',
            text: 'Bitte generieren Sie zuerst einen QR-Code!',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    // Canvas als PNG herunterladen
    downloadCanvas.toBlob((blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'qr-code.png';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }, 'image/png');
}

// Initiale QR-Code-Generierung mit Beispieltext
window.addEventListener('load', () => {
    textInput.value = 'https://www.example.com';
    generateQRCode();
});
