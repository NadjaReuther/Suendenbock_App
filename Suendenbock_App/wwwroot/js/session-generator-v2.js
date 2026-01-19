// Session Generator JavaScript - Version 2 (lädt Wetterdaten von API)

class SessionGenerator {
    constructor(skipInitialModal = false) {
        this.currentView = 'initialModal';
        this.editingAct = null;
        this.actToLoad = null;
        this.selectedWeather = null;
        this.mapImageFile = null;
        this.companions = [];
        this.acts = [];
        this.allActs = []; // For act activation modal
        this.weatherCache = {};
        this.skipInitialModal = skipInitialModal;
        this.loadActMode = null; // 'work' or 'session'

        this.init();
    }

    init() {
        this.setupEventListeners();
        this.loadCompanions();

        // Initial-Modal nur bei "Session vorbereiten" zeigen, nicht bei "Session starten"
        if (!this.skipInitialModal) {
            this.showView('initialModal');
        }
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

    showLoadActViewForWork() {
        this.loadActMode = 'work';
        this.loadActs().then(() => {
            this.renderActsList('actsList', 'work');
            this.showView('loadActModal');
        });
    }

    showLoadActViewForSession() {
        this.loadActMode = 'session';
        this.loadActs().then(() => {
            this.renderActsList('actsList', 'session');
            this.showView('loadActModal');
        });
    }

    // Backward compatibility
    showLoadActView() {
        this.showLoadActViewForWork();
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
                this.allActs = this.acts; // Für Act-Aktivierung Modal
            } else {
                console.error('Failed to load acts');
                this.acts = [];
                this.allActs = [];
            }
        } catch (error) {
            console.error('Error loading acts:', error);
            this.acts = [];
            this.allActs = [];
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
            const isActive = act.isActive;

            if (mode === 'work') {
                // "An Act arbeiten" - direkt zum Dashboard
                return `
                    <div class="act-card ${isActive ? 'active-act' : ''}">
                        <div class="act-card-content">
                            <img src="${mapImage}" alt="Karte" class="act-card-image">
                            <div class="act-card-info">
                                <h3>Akt ${act.actNumber} ${isActive ? '<span style="color: #10b981; font-size: 0.9rem;">✓ Aktiv</span>' : ''}</h3>
                                <p>${act.country || ''} - ${act.month || ''}</p>
                                <p style="font-size:0.8rem; color:#999;">
                                    Begleiter: ${act.companion1 || ''}${act.companion2 ? ' & ' + act.companion2 : ''}
                                </p>
                            </div>
                        </div>
                        <button class="btn-icon load" onclick="sessionGen.openActForWork(${act.id})">
                            <i class="bi bi-folder-open"></i> Öffnen
                        </button>
                    </div>
                `;
            } else if (mode === 'session') {
                // "Session starten" - Wetter wählen dann aktivieren
                return `
                    <div class="act-card ${isActive ? 'active-act' : ''}">
                        <div class="act-card-content">
                            <img src="${mapImage}" alt="Karte" class="act-card-image">
                            <div class="act-card-info">
                                <h3>Akt ${act.actNumber} ${isActive ? '<span style="color: #10b981; font-size: 0.9rem;">✓ Aktiv</span>' : ''}</h3>
                                <p>${act.country || ''} - ${act.month || ''}</p>
                                <p style="font-size:0.8rem; color:#999;">
                                    Begleiter: ${act.companion1 || ''}${act.companion2 ? ' & ' + act.companion2 : ''}
                                </p>
                            </div>
                        </div>
                        <button class="btn-icon load" onclick="sessionGen.startReselectWeather(${act.id})">
                            <i class="bi bi-play-fill"></i> Session starten
                        </button>
                    </div>
                `;
            } else if (mode === 'manage') {
                const isActive = act.isActive;
                return `
                    <div class="act-card ${isActive ? 'active-act' : ''}">
                        <div class="act-card-content">
                            <img src="${mapImage}" alt="Karte" class="act-card-image">
                            <div class="act-card-info">
                                <h3>Akt ${act.actNumber} ${isActive ? '<span style="color: #10b981;">✓ Aktiv</span>' : ''}</h3>
                                <p>${act.country || ''} - ${act.month || ''}</p>
                            </div>
                        </div>
                        <div class="act-card-actions">
                            ${!isActive ? `
                                <button class="btn-icon activate" onclick="sessionGen.activateAct(${act.id})" title="Act aktivieren">
                                    <i class="bi bi-check-circle"></i>
                                </button>
                            ` : ''}
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

    async activateAct(actId) {
        if (!confirm('Diesen Akt aktivieren? Alle anderen Acts werden deaktiviert.')) {
            return;
        }

        try {
            const response = await fetch(`/api/game/acts/${actId}/activate`, {
                method: 'POST'
            });

            if (response.ok) {
                const result = await response.json();
                alert(result.message || 'Act erfolgreich aktiviert!');
                this.showManageActsView(); // Liste neu laden
            } else {
                const error = await response.json();
                alert('Fehler: ' + (error.error || 'Unbekannter Fehler'));
            }
        } catch (error) {
            console.error('Error activating act:', error);
            alert('Fehler beim Aktivieren des Akts');
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

    async showActActivationModal() {
        if (!this.selectedWeather || !this.actToLoad) {
            alert('Bitte wähle ein Wetter aus!');
            return;
        }

        // Ensure acts are loaded
        if (!this.allActs || this.allActs.length === 0) {
            await this.loadActs();
        }

        // Populate act dropdown
        const actSelect = document.getElementById('actToActivate');
        actSelect.innerHTML = this.allActs.map(act => `
            <option value="${act.id}" ${act.id === this.actToLoad.id ? 'selected' : ''}>
                Akt ${act.actNumber} - ${act.country || act.name} ${act.isActive ? '(AKTUELL AKTIV)' : ''}
            </option>
        `).join('');

        // Show preview of selected act
        this.updateActActivationPreview(this.actToLoad.id);

        // Add change listener to update preview (only once)
        const existingListener = actSelect.getAttribute('data-listener-added');
        if (!existingListener) {
            actSelect.addEventListener('change', () => {
                this.updateActActivationPreview(parseInt(actSelect.value));
            });
            actSelect.setAttribute('data-listener-added', 'true');
        }

        // Show modal
        this.showView('actActivationModal');
    }

    updateActActivationPreview(actId) {
        const act = this.allActs.find(a => a.id === actId);
        if (!act) return;

        const preview = document.getElementById('actActivationPreview');
        const weatherDisplay = act.id === this.actToLoad.id
            ? `Wird gesetzt: ${this.selectedWeather}`
            : (act.weather || 'Nicht festgelegt');

        preview.innerHTML = `
            <div class="act-preview-card">
                <h4>Akt ${act.actNumber}: ${act.country || act.name}</h4>
                <p><strong>Monat:</strong> ${act.month || 'Nicht festgelegt'}</p>
                <p><strong>Wetter:</strong> ${weatherDisplay}</p>
                <p><strong>Status:</strong> ${act.isActive ? '<span style="color: #10b981;">Aktiv</span>' : '<span style="color: #6b7280;">Inaktiv</span>'}</p>
            </div>
        `;
    }

    async confirmActActivation() {
        const selectedActId = parseInt(document.getElementById('actToActivate').value);

        try {
            // First, save the weather for the act we loaded
            const weatherResponse = await fetch(`/api/game/acts/${this.actToLoad.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ weather: this.selectedWeather })
            });

            if (!weatherResponse.ok) {
                alert('Fehler beim Speichern des Wetters');
                return;
            }

            // Then activate the selected act
            const activateResponse = await fetch(`/api/game/acts/${selectedActId}/activate`, {
                method: 'POST'
            });

            if (activateResponse.ok) {
                // Redirect to Spielmodus with the activated act
                window.location.href = `/Spielmodus/Dashboard?actId=${selectedActId}`;
            } else {
                alert('Fehler beim Aktivieren des Acts');
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Fehler beim Starten der Session');
        }
    }

    backToWeatherSelection() {
        this.showView('reselectWeatherModal');
    }

    openActForWork(actId) {
        // Direkt zum Dashboard ohne Aktivierung, ohne Wetter-Auswahl
        window.location.href = `/Spielmodus/Dashboard?actId=${actId}`;
    }

    async startSession() {
        // Session starten: Wetter speichern + Act aktivieren + zum Dashboard
        if (!this.selectedWeather || !this.actToLoad) {
            alert('Bitte wähle ein Wetter aus!');
            return;
        }

        try {
            // 1. Wetter für den Act speichern
            const weatherResponse = await fetch(`/api/game/acts/${this.actToLoad.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ weather: this.selectedWeather })
            });

            if (!weatherResponse.ok) {
                alert('Fehler beim Speichern des Wetters');
                return;
            }

            // 2. Act aktivieren
            const activateResponse = await fetch(`/api/game/acts/${this.actToLoad.id}/activate`, {
                method: 'POST'
            });

            if (!activateResponse.ok) {
                alert('Fehler beim Aktivieren des Acts');
                return;
            }

            // 3. Zum Dashboard mit dem aktivierten Act
            window.location.href = `/Spielmodus/Dashboard?actId=${this.actToLoad.id}`;
        } catch (error) {
            console.error('Error starting session:', error);
            alert('Fehler beim Starten der Session');
        }
    }

    async startGame() {
        // Backward compatibility - redirects to new function
        await this.startSession();
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
document.addEventListener('DOMContentLoaded', () => {
    const skipInitialModal = window.autoLoadActiveAct === true;
    window.sessionGen = new SessionGenerator(skipInitialModal);

    // Signalisiere dass sessionGen bereit ist (für StartSession Auto-Load)
    window.dispatchEvent(new CustomEvent('sessionGenReady'));
});
