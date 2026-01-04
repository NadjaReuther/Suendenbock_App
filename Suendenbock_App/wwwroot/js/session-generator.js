// Session Generator JavaScript
// Basierend auf dem Angular-Beispiel, umgesetzt in Vanilla JS

const weatherData = {
    'Januar': ['Kalter, klarer Himmel', 'Anhaltender Schneefall', 'Beißender Wind und Frost'],
    'Februar': ['Grauer Himmel, Schneeregen', 'Überraschender Wärmeeinbruch', 'Eisige Nächte, sonnige Tage'],
    'März': ['Stürmische Winde, erster Regen', 'Langsame Schneeschmelze', 'Wechselhaft mit Sonne und Schauern'],
    'April': ['Heftige Regenschauer', 'Milder Frühlingsbeginn', 'Später Frosteinbruch'],
    'Mai': ['Warme, sonnige Tage', 'Gewitter am Nachmittag', 'Kühle, feuchte Morgen'],
    'Juni': ['Heiß und trocken', 'Schwüle mit häufigen Gewittern', 'Angenehm warm mit leichter Brise'],
    'Juli': ['Drückende Hitze, Dürre', 'Kurze, heftige Sommergewitter', 'Warme Nächte unter klarem Himmel'],
    'August': ['Erntewetter, goldenes Licht', 'Hitzewelle mit Waldbrandgefahr', 'Feuchte, neblige Morgen'],
    'September': ['Kühle, klare Herbsttage', 'Anhaltender Nieselregen', 'Erster Nachtfrost'],
    'Oktober': ['Bunter Blätterfall, milde Sonne', 'Stürmisch und regnerisch', 'Dichter Morgennebel'],
    'November': ['Kalt und grau, erster Schnee', 'Dauerregen und Schlamm', 'Trockene, kalte Winde'],
    'Dezember': ['Tiefer Schnee, Stille', 'Eisregen und gefrierende Nässe', 'Klirrende Kälte bei Sternenhimmel']
};

const weatherForecasts = {
    'Kalter, klarer Himmel': [
        { day: 'Mo', icon: '☀️', temp: '2°/-5°' },
        { day: 'Di', icon: '☀️', temp: '1°/-6°' },
        { day: 'Mi', icon: '☀️', temp: '2°/-5°' },
        { day: 'Do', icon: '☁️', temp: '0°/-4°' },
        { day: 'Fr', icon: '☀️', temp: '3°/-3°' }
    ],
    'Anhaltender Schneefall': [
        { day: 'Mo', icon: '🌨️', temp: '-1°/-4°' },
        { day: 'Di', icon: '🌨️', temp: '-2°/-5°' },
        { day: 'Mi', icon: '❄️', temp: '-3°/-7°' },
        { day: 'Do', icon: '🌨️', temp: '-2°/-5°' },
        { day: 'Fr', icon: '☁️', temp: '0°/-3°' }
    ],
    'Beißender Wind und Frost': [
        { day: 'Mo', icon: '💨', temp: '-5°/-10°' },
        { day: 'Di', icon: '💨', temp: '-6°/-12°' },
        { day: 'Mi', icon: '❄️', temp: '-7°/-14°' },
        { day: 'Do', icon: '💨', temp: '-6°/-11°' },
        { day: 'Fr', icon: '☀️', temp: '-4°/-8°' }
    ],
    'Grauer Himmel, Schneeregen': [
        { day: 'Mo', icon: '🌧️', temp: '1°/-2°' },
        { day: 'Di', icon: '🌧️', temp: '0°/-3°' },
        { day: 'Mi', icon: '🌨️', temp: '-1°/-4°' },
        { day: 'Do', icon: '🌧️', temp: '2°/0°' },
        { day: 'Fr', icon: '🌧️', temp: '1°/-1°' }
    ],
    'Überraschender Wärmeeinbruch': [
        { day: 'Mo', icon: '☀️', temp: '10°/2°' },
        { day: 'Di', icon: '☀️', temp: '12°/3°' },
        { day: 'Mi', icon: '🌧️', temp: '8°/1°' },
        { day: 'Do', icon: '☁️', temp: '7°/0°' },
        { day: 'Fr', icon: '☀️', temp: '11°/2°' }
    ],
    'Eisige Nächte, sonnige Tage': [
        { day: 'Mo', icon: '☀️', temp: '4°/-4°' },
        { day: 'Di', icon: '☀️', temp: '5°/-5°' },
        { day: 'Mi', icon: '☀️', temp: '6°/-4°' },
        { day: 'Do', icon: '☀️', temp: '5°/-3°' },
        { day: 'Fr', icon: '☁️', temp: '3°/-2°' }
    ],
    'Stürmische Winde, erster Regen': [
        { day: 'Mo', icon: '💨', temp: '8°/3°' },
        { day: 'Di', icon: '🌧️', temp: '7°/4°' },
        { day: 'Mi', icon: '🌧️', temp: '6°/2°' },
        { day: 'Do', icon: '💨', temp: '9°/3°' },
        { day: 'Fr', icon: '☁️', temp: '7°/1°' }
    ],
    'Langsame Schneeschmelze': [
        { day: 'Mo', icon: '☁️', temp: '3°/0°' },
        { day: 'Di', icon: '🌧️', temp: '4°/1°' },
        { day: 'Mi', icon: '🌫️', temp: '2°/0°' },
        { day: 'Do', icon: '☀️', temp: '5°/1°' },
        { day: 'Fr', icon: '☁️', temp: '4°/0°' }
    ],
    'Wechselhaft mit Sonne und Schauern': [
        { day: 'Mo', icon: '⛅', temp: '10°/4°' },
        { day: 'Di', icon: '🌧️', temp: '8°/3°' },
        { day: 'Mi', icon: '☀️', temp: '12°/5°' },
        { day: 'Do', icon: '🌧️', temp: '9°/4°' },
        { day: 'Fr', icon: '⛅', temp: '11°/5°' }
    ],
    'Heftige Regenschauer': [
        { day: 'Mo', icon: '🌧️', temp: '9°/5°' },
        { day: 'Di', icon: '🌧️', temp: '8°/4°' },
        { day: 'Mi', icon: '⛈️', temp: '10°/6°' },
        { day: 'Do', icon: '🌧️', temp: '7°/3°' },
        { day: 'Fr', icon: '☁️', temp: '8°/4°' }
    ],
    'Milder Frühlingsbeginn': [
        { day: 'Mo', icon: '☀️', temp: '14°/6°' },
        { day: 'Di', icon: '⛅', temp: '15°/7°' },
        { day: 'Mi', icon: '☀️', temp: '16°/8°' },
        { day: 'Do', icon: '☁️', temp: '13°/6°' },
        { day: 'Fr', icon: '🌧️', temp: '12°/5°' }
    ],
    'Später Frosteinbruch': [
        { day: 'Mo', icon: '❄️', temp: '5°/-2°' },
        { day: 'Di', icon: '❄️', temp: '4°/-3°' },
        { day: 'Mi', icon: '☀️', temp: '6°/-1°' },
        { day: 'Do', icon: '☁️', temp: '5°/0°' },
        { day: 'Fr', icon: '🌧️', temp: '3°/-1°' }
    ],
    'Warme, sonnige Tage': [
        { day: 'Mo', icon: '☀️', temp: '22°/12°' },
        { day: 'Di', icon: '☀️', temp: '24°/14°' },
        { day: 'Mi', icon: '☀️', temp: '25°/15°' },
        { day: 'Do', icon: '☀️', temp: '23°/13°' },
        { day: 'Fr', icon: '⛅', temp: '21°/12°' }
    ],
    'Gewitter am Nachmittag': [
        { day: 'Mo', icon: '⛅', temp: '23°/14°' },
        { day: 'Di', icon: '⛈️', temp: '20°/15°' },
        { day: 'Mi', icon: '⛅', temp: '24°/16°' },
        { day: 'Do', icon: '⛈️', temp: '21°/15°' },
        { day: 'Fr', icon: '☀️', temp: '25°/17°' }
    ],
    'Kühle, feuchte Morgen': [
        { day: 'Mo', icon: '🌫️', temp: '18°/10°' },
        { day: 'Di', icon: '🌫️', temp: '19°/11°' },
        { day: 'Mi', icon: '☀️', temp: '21°/12°' },
        { day: 'Do', icon: '🌫️', temp: '17°/9°' },
        { day: 'Fr', icon: '☁️', temp: '18°/10°' }
    ],
    'Heiß und trocken': [
        { day: 'Mo', icon: '☀️', temp: '28°/18°' },
        { day: 'Di', icon: '☀️', temp: '30°/20°' },
        { day: 'Mi', icon: '☀️', temp: '31°/21°' },
        { day: 'Do', icon: '☀️', temp: '29°/19°' },
        { day: 'Fr', icon: '☀️', temp: '30°/20°' }
    ],
    'Schwüle mit häufigen Gewittern': [
        { day: 'Mo', icon: '⛈️', temp: '26°/19°' },
        { day: 'Di', icon: '☁️', temp: '27°/20°' },
        { day: 'Mi', icon: '⛈️', temp: '25°/18°' },
        { day: 'Do', icon: '⛈️', temp: '26°/19°' },
        { day: 'Fr', icon: '🌧️', temp: '24°/18°' }
    ],
    'Angenehm warm mit leichter Brise': [
        { day: 'Mo', icon: '⛅', temp: '24°/16°' },
        { day: 'Di', icon: '💨', temp: '23°/15°' },
        { day: 'Mi', icon: '☀️', temp: '25°/17°' },
        { day: 'Do', icon: '💨', temp: '22°/14°' },
        { day: 'Fr', icon: '⛅', temp: '24°/16°' }
    ],
    'Drückende Hitze, Dürre': [
        { day: 'Mo', icon: '☀️', temp: '33°/22°' },
        { day: 'Di', icon: '☀️', temp: '35°/24°' },
        { day: 'Mi', icon: '☀️', temp: '36°/25°' },
        { day: 'Do', icon: '☀️', temp: '34°/23°' },
        { day: 'Fr', icon: '🔥', temp: '37°/26°' }
    ],
    'Kurze, heftige Sommergewitter': [
        { day: 'Mo', icon: '☀️', temp: '30°/20°' },
        { day: 'Di', icon: '⛈️', temp: '28°/19°' },
        { day: 'Mi', icon: '☀️', temp: '31°/21°' },
        { day: 'Do', icon: '⛈️', temp: '29°/20°' },
        { day: 'Fr', icon: '☀️', temp: '32°/22°' }
    ],
    'Warme Nächte unter klarem Himmel': [
        { day: 'Mo', icon: '🌙', temp: '28°/19°' },
        { day: 'Di', icon: '🌙', temp: '29°/20°' },
        { day: 'Mi', icon: '🌙', temp: '30°/21°' },
        { day: 'Do', icon: '🌙', temp: '28°/19°' },
        { day: 'Fr', icon: '🌙', temp: '29°/20°' }
    ],
    'Erntewetter, goldenes Licht': [
        { day: 'Mo', icon: '☀️', temp: '25°/15°' },
        { day: 'Di', icon: '☀️', temp: '26°/16°' },
        { day: 'Mi', icon: '☀️', temp: '27°/17°' },
        { day: 'Do', icon: '⛅', temp: '24°/14°' },
        { day: 'Fr', icon: '☀️', temp: '25°/15°' }
    ],
    'Hitzewelle mit Waldbrandgefahr': [
        { day: 'Mo', icon: '🔥', temp: '32°/21°' },
        { day: 'Di', icon: '🔥', temp: '34°/23°' },
        { day: 'Mi', icon: '🔥', temp: '35°/24°' },
        { day: 'Do', icon: '☀️', temp: '33°/22°' },
        { day: 'Fr', icon: '🔥', temp: '36°/25°' }
    ],
    'Feuchte, neblige Morgen': [
        { day: 'Mo', icon: '🌫️', temp: '20°/12°' },
        { day: 'Di', icon: '🌫️', temp: '21°/13°' },
        { day: 'Mi', icon: '☀️', temp: '23°/14°' },
        { day: 'Do', icon: '🌫️', temp: '19°/11°' },
        { day: 'Fr', icon: '☁️', temp: '20°/12°' }
    ],
    'Kühle, klare Herbsttage': [
        { day: 'Mo', icon: '☀️', temp: '15°/5°' },
        { day: 'Di', icon: '☀️', temp: '16°/6°' },
        { day: 'Mi', icon: '⛅', temp: '14°/4°' },
        { day: 'Do', icon: '☀️', temp: '17°/7°' },
        { day: 'Fr', icon: '☀️', temp: '15°/5°' }
    ],
    'Anhaltender Nieselregen': [
        { day: 'Mo', icon: '🌧️', temp: '12°/8°' },
        { day: 'Di', icon: '🌧️', temp: '11°/7°' },
        { day: 'Mi', icon: '🌧️', temp: '10°/6°' },
        { day: 'Do', icon: '☁️', temp: '11°/7°' },
        { day: 'Fr', icon: '🌧️', temp: '12°/8°' }
    ],
    'Erster Nachtfrost': [
        { day: 'Mo', icon: '❄️', temp: '8°/-1°' },
        { day: 'Di', icon: '☀️', temp: '9°/0°' },
        { day: 'Mi', icon: '❄️', temp: '7°/-2°' },
        { day: 'Do', icon: '☀️', temp: '10°/1°' },
        { day: 'Fr', icon: '❄️', temp: '6°/-3°' }
    ],
    'Bunter Blätterfall, milde Sonne': [
        { day: 'Mo', icon: '☀️', temp: '14°/6°' },
        { day: 'Di', icon: '⛅', temp: '13°/5°' },
        { day: 'Mi', icon: '💨', temp: '12°/4°' },
        { day: 'Do', icon: '☀️', temp: '15°/7°' },
        { day: 'Fr', icon: '☁️', temp: '11°/3°' }
    ],
    'Stürmisch und regnerisch': [
        { day: 'Mo', icon: '💨', temp: '11°/7°' },
        { day: 'Di', icon: '🌧️', temp: '10°/6°' },
        { day: 'Mi', icon: '⛈️', temp: '12°/8°' },
        { day: 'Do', icon: '💨', temp: '9°/5°' },
        { day: 'Fr', icon: '🌧️', temp: '10°/6°' }
    ],
    'Dichter Morgennebel': [
        { day: 'Mo', icon: '🌫️', temp: '9°/3°' },
        { day: 'Di', icon: '🌫️', temp: '8°/2°' },
        { day: 'Mi', icon: '⛅', temp: '11°/4°' },
        { day: 'Do', icon: '🌫️', temp: '7°/1°' },
        { day: 'Fr', icon: '☁️', temp: '8°/2°' }
    ],
    'Kalt und grau, erster Schnee': [
        { day: 'Mo', icon: '☁️', temp: '3°/-1°' },
        { day: 'Di', icon: '🌨️', temp: '1°/-2°' },
        { day: 'Mi', icon: '🌨️', temp: '0°/-3°' },
        { day: 'Do', icon: '🌧️', temp: '2°/0°' },
        { day: 'Fr', icon: '☁️', temp: '1°/-1°' }
    ],
    'Dauerregen und Schlamm': [
        { day: 'Mo', icon: '🌧️', temp: '5°/2°' },
        { day: 'Di', icon: '🌧️', temp: '4°/1°' },
        { day: 'Mi', icon: '🌧️', temp: '6°/3°' },
        { day: 'Do', icon: '☁️', temp: '5°/2°' },
        { day: 'Fr', icon: '🌧️', temp: '4°/1°' }
    ],
    'Trockene, kalte Winde': [
        { day: 'Mo', icon: '💨', temp: '2°/-4°' },
        { day: 'Di', icon: '💨', temp: '1°/-5°' },
        { day: 'Mi', icon: '☀️', temp: '3°/-3°' },
        { day: 'Do', icon: '💨', temp: '0°/-6°' },
        { day: 'Fr', icon: '☁️', temp: '1°/-4°' }
    ],
    'Tiefer Schnee, Stille': [
        { day: 'Mo', icon: '❄️', temp: '-2°/-8°' },
        { day: 'Di', icon: '🌨️', temp: '-3°/-9°' },
        { day: 'Mi', icon: '❄️', temp: '-4°/-10°' },
        { day: 'Do', icon: '🌨️', temp: '-3°/-8°' },
        { day: 'Fr', icon: '❄️', temp: '-5°/-11°' }
    ],
    'Eisregen und gefrierende Nässe': [
        { day: 'Mo', icon: '🌧️', temp: '0°/-3°' },
        { day: 'Di', icon: '❄️', temp: '-1°/-4°' },
        { day: 'Mi', icon: '🌧️', temp: '0°/-2°' },
        { day: 'Do', icon: '🌧️', temp: '1°/-1°' },
        { day: 'Fr', icon: '🌧️', temp: '-1°/-5°' }
    ],
    'Klirrende Kälte bei Sternenhimmel': [
        { day: 'Mo', icon: '🌙', temp: '-8°/-15°' },
        { day: 'Di', icon: '🌙', temp: '-9°/-16°' },
        { day: 'Mi', icon: '🌙', temp: '-10°/-17°' },
        { day: 'Do', icon: '💨', temp: '-7°/-14°' },
        { day: 'Fr', icon: '🌙', temp: '-9°/-16°' }
    ]
};

class SessionGenerator {
    constructor() {
        this.currentView = 'initialModal';
        this.editingAct = null;
        this.actToLoad = null;
        this.selectedWeather = null;
        this.mapImageBase64 = null;
        this.companions = [];
        this.acts = [];

        this.init();
    }

    init() {
        this.setupEventListeners();
        this.loadCompanions();
        this.showView('initialModal');
    }

    setupEventListeners() {
        // Map Upload
        const mapUploadArea = document.getElementById('mapUploadArea');
        const mapFile = document.getElementById('mapFile');

        mapUploadArea.addEventListener('click', () => mapFile.click());

        mapFile.addEventListener('change', (e) => {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = (event) => {
                    this.mapImageBase64 = event.target.result;
                    document.getElementById('mapPreview').src = this.mapImageBase64;
                    document.getElementById('mapPreview').style.display = 'block';
                    document.getElementById('uploadPlaceholder').style.display = 'none';
                };
                reader.readAsDataURL(file);
            }
        });

        // Month Selection
        document.getElementById('month').addEventListener('change', (e) => {
            this.onMonthChange(e.target.value);
        });

        // Companion1 Selection
        document.getElementById('companion1').addEventListener('change', (e) => {
            this.updateCompanion2Options(e.target.value);
        });

        // Form Submit
        document.getElementById('actForm').addEventListener('submit', (e) => {
            e.preventDefault();
            this.saveAct();
        });
    }

    showView(viewId) {
        // Hide all modals
        document.querySelectorAll('.generator-modal').forEach(modal => {
            modal.classList.remove('active');
        });

        // Show selected modal
        document.getElementById(viewId).classList.add('active');
        this.currentView = viewId;
    }

    backToInitial() {
        this.resetForm();
        this.showView('initialModal');
    }

    backToLoadView() {
        this.showLoadActView();
    }

    showNewActForm() {
        this.resetForm();
        this.editingAct = null;
        document.getElementById('formTitle').textContent = 'Neuen Akt anlegen';
        document.getElementById('saveBtnText').textContent = 'Akt erstellen';
        this.showView('actFormModal');
    }

    showLoadActView() {
        this.loadActs().then(() => {
            this.renderActsList('actsList', 'load');
            this.showView('loadActModal');
        });
    }

    showManageActsView() {
        this.loadActs().then(() => {
            this.renderActsList('manageActsList', 'manage');
            this.showView('manageActsModal');
        });
    }

    async loadCompanions() {
        // Companions werden vom Server via ViewBag bereitgestellt
        // Hier laden wir sie aus dem Select
        const companion1Select = document.getElementById('companion1');
        this.companions = Array.from(companion1Select.options)
            .filter(opt => opt.value !== '')
            .map(opt => opt.value);
    }

    async loadActs() {
        try {
            const response = await fetch('/api/game/acts');
            if (response.ok) {
                this.acts = await response.json();
            } else {
                console.error('Failed to load acts');
                this.acts = [];
            }
        } catch (error) {
            console.error('Error loading acts:', error);
            this.acts = [];
        }
    }

    renderActsList(containerId, mode) {
        const container = document.getElementById(containerId);

        if (this.acts.length === 0) {
            container.innerHTML = `
                <div class="empty-state">
                    <i class="bi bi-inbox"></i>
                    <p>Es wurden noch keine Akte gespeichert.</p>
                </div>
            `;
            return;
        }

        container.innerHTML = this.acts.map(act => {
            const mapImage = act.mapImageUrl || '/images/placeholder-map.png';

            if (mode === 'load') {
                return `
                    <div class="act-card">
                        <div class="act-card-content">
                            <img src="${mapImage}" alt="Karte" class="act-card-image">
                            <div class="act-card-info">
                                <h3>Akt ${act.actNumber}</h3>
                                <p>${act.country || ''} - ${act.month || ''}</p>
                                <p style="font-size:0.8rem; color:#999;">
                                    Begleiter: ${act.companion1 || ''}${act.companion2 ? ' & ' + act.companion2 : ''}
                                </p>
                            </div>
                        </div>
                        <button class="btn-icon load" onclick="sessionGen.startReselectWeather(${act.id})">
                            <i class="bi bi-play-fill"></i> Laden
                        </button>
                    </div>
                `;
            } else if (mode === 'manage') {
                return `
                    <div class="act-card">
                        <div class="act-card-content">
                            <img src="${mapImage}" alt="Karte" class="act-card-image">
                            <div class="act-card-info">
                                <h3>Akt ${act.actNumber}</h3>
                                <p>${act.country || ''} - ${act.month || ''}</p>
                            </div>
                        </div>
                        <div class="act-card-actions">
                            <button class="btn-icon edit" onclick="sessionGen.editAct(${act.id})">
                                <i class="bi bi-pencil"></i>
                            </button>
                            <button class="btn-icon delete" onclick="sessionGen.deleteAct(${act.id})">
                                <i class="bi bi-trash"></i>
                            </button>
                        </div>
                    </div>
                `;
            }
        }).join('');
    }

    async editAct(actId) {
        try {
            const response = await fetch(`/api/game/acts/${actId}`);
            if (response.ok) {
                const act = await response.json();
                this.editingAct = act;

                // Fill form
                document.getElementById('actId').value = act.id;
                document.getElementById('actNumber').value = act.actNumber;
                document.getElementById('country').value = act.country || 'Böhmen';
                document.getElementById('companion1').value = act.companion1 || '';
                this.updateCompanion2Options(act.companion1);
                document.getElementById('companion2').value = act.companion2 || '';
                document.getElementById('month').value = act.month || '';

                if (act.mapImageUrl) {
                    this.mapImageBase64 = act.mapImageUrl;
                    document.getElementById('mapPreview').src = act.mapImageUrl;
                    document.getElementById('mapPreview').style.display = 'block';
                    document.getElementById('uploadPlaceholder').style.display = 'none';
                }

                // Load weather options if month selected
                if (act.month) {
                    this.onMonthChange(act.month);
                    // Pre-select weather
                    setTimeout(() => {
                        const weatherRadios = document.querySelectorAll('input[name="weather"]');
                        weatherRadios.forEach(radio => {
                            if (radio.value === act.weather) {
                                radio.checked = true;
                                this.onWeatherChange(act.weather);
                            }
                        });
                    }, 100);
                }

                document.getElementById('formTitle').textContent = 'Akt bearbeiten';
                document.getElementById('saveBtnText').textContent = 'Änderungen speichern';
                this.showView('actFormModal');
            }
        } catch (error) {
            console.error('Error loading act:', error);
            alert('Fehler beim Laden des Akts');
        }
    }

    async deleteAct(actId) {
        if (!confirm('Diesen Akt wirklich endgültig löschen?')) {
            return;
        }

        try {
            const response = await fetch(`/api/game/acts/${actId}`, {
                method: 'DELETE'
            });

            if (response.ok) {
                alert('Akt erfolgreich gelöscht!');
                this.showManageActsView();
            } else {
                const error = await response.json();
                alert('Fehler: ' + (error.error || 'Unbekannter Fehler'));
            }
        } catch (error) {
            console.error('Error deleting act:', error);
            alert('Fehler beim Löschen des Akts');
        }
    }

    async saveAct() {
        const actNumber = parseInt(document.getElementById('actNumber').value);
        const country = document.getElementById('country').value;
        const companion1 = document.getElementById('companion1').value;
        const companion2 = document.getElementById('companion2').value;
        const month = document.getElementById('month').value;
        const weather = this.selectedWeather;

        if (!companion1) {
            alert('Bitte wähle mindestens einen Begleiter aus!');
            return;
        }

        if (!month) {
            alert('Bitte wähle einen Monat aus!');
            return;
        }

        if (!weather) {
            alert('Bitte wähle ein Wetter aus!');
            return;
        }

        const actData = {
            actNumber: actNumber,
            name: `Akt ${actNumber}`,
            description: '',
            country: country,
            companion1: companion1,
            companion2: companion2 || null,
            month: month,
            weather: weather,
            mapImageUrl: this.mapImageBase64
        };

        try {
            let response;
            if (this.editingAct) {
                // Update existing act
                response = await fetch(`/api/game/acts/${this.editingAct.id}`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(actData)
                });
            } else {
                // Create new act
                response = await fetch('/api/game/acts', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(actData)
                });
            }

            if (response.ok) {
                const result = await response.json();
                alert(result.message);
                this.resetForm();
                this.showLoadActView();
            } else {
                const error = await response.json();
                alert('Fehler: ' + (error.error || 'Unbekannter Fehler'));
            }
        } catch (error) {
            console.error('Error saving act:', error);
            alert('Fehler beim Speichern des Akts');
        }
    }

    startReselectWeather(actId) {
        const act = this.acts.find(a => a.id === actId);
        if (!act) return;

        this.actToLoad = act;

        // Display act info
        document.getElementById('actInfoDisplay').innerHTML = `
            <h3>Akt ${act.actNumber}: ${act.country}</h3>
            <p>Monat: ${act.month} (festgelegt)</p>
        `;

        // Render weather options for the month
        this.renderWeatherOptions('weatherSelectionReselect', act.month);

        this.showView('reselectWeatherModal');
    }

    async startGame() {
        if (!this.selectedWeather || !this.actToLoad) {
            alert('Bitte wähle ein Wetter aus!');
            return;
        }

        try {
            // Update act with selected weather
            const response = await fetch(`/api/game/acts/${this.actToLoad.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ weather: this.selectedWeather })
            });

            if (response.ok) {
                // Activate the act
                await fetch(`/api/game/acts/${this.actToLoad.id}/activate`, {
                    method: 'POST'
                });

                // Redirect to Spielmodus
                window.location.href = '/Spielmodus/Dashboard';
            } else {
                alert('Fehler beim Starten der Session');
            }
        } catch (error) {
            console.error('Error starting game:', error);
            alert('Fehler beim Starten der Session');
        }
    }

    updateCompanion2Options(companion1Value) {
        const companion2Select = document.getElementById('companion2');
        companion2Select.innerHTML = '<option value="">-- Zweiter Begleiter (optional) --</option>';

        this.companions
            .filter(c => c !== companion1Value)
            .forEach(companion => {
                const option = document.createElement('option');
                option.value = companion;
                option.textContent = companion;
                companion2Select.appendChild(option);
            });
    }

    onMonthChange(month) {
        if (!month) {
            document.getElementById('weatherSelection').style.display = 'none';
            return;
        }

        document.getElementById('selectedMonthLabel').textContent = month;
        this.renderWeatherOptions('weatherOptions', month);
        document.getElementById('weatherSelection').style.display = 'block';
    }

    renderWeatherOptions(containerId, month) {
        const container = document.getElementById(containerId);
        const options = weatherData[month] || [];

        container.innerHTML = options.map((weather, index) => `
            <div class="weather-option" onclick="sessionGen.selectWeather('${weather}', this)">
                <input type="radio" name="weather" value="${weather}" id="weather-${index}">
                <label for="weather-${index}">${weather}</label>
            </div>
        `).join('');
    }

    selectWeather(weather, element) {
        this.selectedWeather = weather;

        // Update selected state
        document.querySelectorAll('.weather-option').forEach(opt => opt.classList.remove('selected'));
        element.classList.add('selected');

        // Update radio button
        element.querySelector('input[type="radio"]').checked = true;

        // Show forecast
        this.showWeatherForecast(weather);

        // Enable start game button if in reselect view
        const startGameBtn = document.getElementById('startGameBtn');
        if (startGameBtn) {
            startGameBtn.disabled = false;
        }
    }

    showWeatherForecast(weather) {
        const forecast = weatherForecasts[weather];
        if (!forecast) return;

        const forecastContainer = document.getElementById('forecastDays');
        forecastContainer.innerHTML = forecast.map(day => `
            <div class="forecast-day">
                <div class="forecast-day-name">${day.day}</div>
                <div class="forecast-icon">${day.icon}</div>
                <div class="forecast-temp">${day.temp}</div>
            </div>
        `).join('');

        document.getElementById('weatherForecast').style.display = 'block';
    }

    onWeatherChange(weather) {
        this.selectedWeather = weather;
        this.showWeatherForecast(weather);
    }

    resetForm() {
        document.getElementById('actForm').reset();
        document.getElementById('actId').value = '';
        this.editingAct = null;
        this.selectedWeather = null;
        this.mapImageBase64 = null;

        document.getElementById('mapPreview').style.display = 'none';
        document.getElementById('mapPreview').src = '';
        document.getElementById('uploadPlaceholder').style.display = 'block';
        document.getElementById('weatherSelection').style.display = 'none';
        document.getElementById('weatherForecast').style.display = 'none';
    }
}

// Initialize when DOM is ready
let sessionGen;
document.addEventListener('DOMContentLoaded', () => {
    sessionGen = new SessionGenerator();
});
