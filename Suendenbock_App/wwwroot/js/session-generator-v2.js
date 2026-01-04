// Session Generator JavaScript - Version 2 (lädt Wetterdaten von API)

class SessionGenerator {
    constructor() {
        this.currentView = 'initialModal';
        this.editingAct = null;
        this.actToLoad = null;
        this.selectedWeather = null;
        this.mapImageFile = null; // Speichere die Datei statt Base64
        this.companions = [];
        this.acts = [];
        this.weatherCache = {}; // Cache für Wetterdaten

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
                // Speichere die Datei für später
                this.mapImageFile = file;

                // Zeige Vorschau
                const reader = new FileReader();
                reader.onload = (event) => {
                    document.getElementById('mapPreview').src = event.target.result;
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

    async loadWeatherOptions(month) {
        // Check cache first
        if (this.weatherCache[month]) {
            return this.weatherCache[month];
        }

        try {
            const response = await fetch(`/api/game/weather?month=${encodeURIComponent(month)}`);
            if (response.ok) {
                const data = await response.json();
                this.weatherCache[month] = data;
                return data;
            } else {
                console.error('Failed to load weather options');
                return [];
            }
        } catch (error) {
            console.error('Error loading weather options:', error);
            return [];
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
                    // Zeige vorhandenes Bild als Vorschau (aber speichere nicht als File)
                    document.getElementById('mapPreview').src = act.mapImageUrl;
                    document.getElementById('mapPreview').style.display = 'block';
                    document.getElementById('uploadPlaceholder').style.display = 'none';
                    // this.mapImageFile bleibt null - nur wenn neues Bild hochgeladen wird, wird es gesetzt
                }

                // Load weather options if month selected
                if (act.month) {
                    await this.onMonthChange(act.month);
                    // Pre-select weather
                    setTimeout(() => {
                        const weatherRadios = document.querySelectorAll('input[name="weather"]');
                        weatherRadios.forEach(radio => {
                            if (radio.value === act.weather) {
                                const weatherOption = radio.closest('.weather-option');
                                this.selectWeather(act.weather, weatherOption);
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

        // FormData erstellen für Datei-Upload
        const formData = new FormData();
        formData.append('actNumber', actNumber);
        formData.append('name', `Akt ${actNumber}`);
        formData.append('description', '');
        formData.append('country', country);
        formData.append('companion1', companion1);
        if (companion2) {
            formData.append('companion2', companion2);
        }
        formData.append('month', month);
        formData.append('weather', weather);

        // Bild-Datei hinzufügen, falls vorhanden
        if (this.mapImageFile) {
            formData.append('mapImage', this.mapImageFile);
        }

        try {
            let response;
            if (this.editingAct) {
                // Update existing act
                response = await fetch(`/api/game/acts/${this.editingAct.id}`, {
                    method: 'PUT',
                    body: formData // Kein Content-Type Header nötig, Browser setzt automatisch multipart/form-data
                });
            } else {
                // Create new act
                response = await fetch('/api/game/acts', {
                    method: 'POST',
                    body: formData // Kein Content-Type Header nötig, Browser setzt automatisch multipart/form-data
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

    async startReselectWeather(actId) {
        const act = this.acts.find(a => a.id === actId);
        if (!act) return;

        this.actToLoad = act;

        // Display act info
        document.getElementById('actInfoDisplay').innerHTML = `
            <h3>Akt ${act.actNumber}: ${act.country}</h3>
            <p>Monat: ${act.month} (festgelegt)</p>
        `;

        // Render weather options for the month
        await this.renderWeatherOptions('weatherSelectionReselect', act.month);

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

    async onMonthChange(month) {
        if (!month) {
            document.getElementById('weatherSelection').style.display = 'none';
            return;
        }

        document.getElementById('selectedMonthLabel').textContent = month;
        await this.renderWeatherOptions('weatherOptions', month);
        document.getElementById('weatherSelection').style.display = 'block';
    }

    async renderWeatherOptions(containerId, month) {
        const container = document.getElementById(containerId);
        const weatherOptions = await this.loadWeatherOptions(month);

        if (weatherOptions.length === 0) {
            container.innerHTML = '<p>Keine Wetteroptionen verfügbar</p>';
            return;
        }

        container.innerHTML = weatherOptions.map((option, index) => `
            <div class="weather-option" onclick="sessionGen.selectWeather('${option.weatherName}', this, ${JSON.stringify(option.forecast).replace(/"/g, '&quot;')})">
                <input type="radio" name="weather" value="${option.weatherName}" id="weather-${containerId}-${index}">
                <label for="weather-${containerId}-${index}">${option.weatherName}</label>
            </div>
        `).join('');
    }

    selectWeather(weather, element, forecast) {
        this.selectedWeather = weather;

        // Update selected state
        document.querySelectorAll('.weather-option').forEach(opt => opt.classList.remove('selected'));
        element.classList.add('selected');

        // Update radio button
        element.querySelector('input[type="radio"]').checked = true;

        // Show forecast if provided
        if (forecast) {
            this.showWeatherForecast(forecast);
        }

        // Enable start game button if in reselect view
        const startGameBtn = document.getElementById('startGameBtn');
        if (startGameBtn) {
            startGameBtn.disabled = false;
        }
    }

    showWeatherForecast(forecast) {
        if (!forecast || forecast.length === 0) return;

        const forecastContainer = document.getElementById('forecastDays');
        forecastContainer.innerHTML = forecast.map(day => `
            <div class="forecast-day">
                <div class="forecast-day-name">${day.day}</div>
                <div class="forecast-icon">${day.icon}</div>
                <div class="forecast-temp">${day.temperature}</div>
            </div>
        `).join('');

        document.getElementById('weatherForecast').style.display = 'block';
    }

    resetForm() {
        document.getElementById('actForm').reset();
        document.getElementById('actId').value = '';
        this.editingAct = null;
        this.selectedWeather = null;
        this.mapImageFile = null;

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
