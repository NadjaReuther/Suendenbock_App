// Push-Benachrichtigungs-Client
// Verwaltet die Subscription und Kommunikation mit dem Backend

class PushNotificationManager {
    constructor() {
        this.swRegistration = null;
        this.isSubscribed = false;
        this.publicKey = null;
        this._initPromise = null;
        this.lastError = null;
    }

    /**
     * Wartet höchstens ms Millisekunden auf promise. Auf dem iPhone bleibt
     * navigator.serviceWorker.ready manchmal für immer hängen, dann soll der Knopf nicht still stehen.
     */
    withTimeout(promise, ms, text) {
        return Promise.race([
            promise,
            new Promise((_, reject) => setTimeout(() => reject(new Error(text)), ms))
        ]);
    }

    /**
     * Wartet, bis der Hintergrunddienst aktiv ist (ohne navigator.serviceWorker.ready).
     */
    waitForActive(registration, ms) {
        const worker = registration.active || registration.waiting || registration.installing;
        if (registration.active) return Promise.resolve(registration);
        return this.withTimeout(new Promise((resolve) => {
            if (!worker) { navigator.serviceWorker.ready.then(resolve); return; }
            worker.addEventListener('statechange', () => {
                if (worker.state === 'activated') resolve(registration);
            });
        }), ms, 'Der Hintergrunddienst startet nicht (Zeitüberschreitung).');
    }

    /**
     * Kurzer Gerätebericht für die Einstellungsseite (zum Abfotografieren, wenn es klemmt).
     */
    async getDiagnose() {
        const ua = navigator.userAgent || '';
        const ios = ua.match(/OS (\d+)[_.](\d+)/);
        const isIOS = /iPad|iPhone|iPod/.test(ua) || (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1);
        const standalone = window.navigator.standalone === true ||
            (window.matchMedia && window.matchMedia('(display-mode: standalone)').matches);
        let sw = 'nicht verfügbar';
        if ('serviceWorker' in navigator) {
            try {
                const reg = await this.withTimeout(navigator.serviceWorker.getRegistration('/'), 3000, 'Zeitüberschreitung');
                sw = !reg ? 'nicht registriert' : reg.active ? 'aktiv' : reg.waiting ? 'wartet' : reg.installing ? 'installiert gerade' : 'unbekannt';
            } catch (e) { sw = 'Fehler: ' + e.message; }
        }
        return [
            isIOS ? ('iOS ' + (ios ? ios[1] + '.' + ios[2] : '?')) : 'kein iPhone',
            'Home-Bildschirm: ' + (standalone ? 'ja' : 'nein'),
            'Erlaubnis: ' + ('Notification' in window ? Notification.permission : 'fehlt'),
            'Push: ' + ('PushManager' in window ? 'ja' : 'nein'),
            'Dienst: ' + sw,
            'Abo: ' + (this.isSubscribed ? 'ja' : 'nein'),
            this.lastError ? 'Letzter Fehler: ' + this.lastError : null
        ].filter(Boolean).join(' · ');
    }

    /**
     * Kann dieses Gerät Push überhaupt?
     * iPhone/iPad: nur in der App vom Home-Bildschirm (ab iOS 16.4), nicht im Safari-Tab.
     * @returns {{supported: boolean, reason: string|null, message: string|null}}
     */
    getSupportStatus() {
        const ua = navigator.userAgent || '';
        const isIOS = /iPad|iPhone|iPod/.test(ua) || (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1);
        const isStandalone = window.navigator.standalone === true ||
            (window.matchMedia && window.matchMedia('(display-mode: standalone)').matches);

        if (isIOS && !isStandalone) {
            return {
                supported: false, reason: 'ios-browser',
                message: 'Auf dem iPhone funktionieren Benachrichtigungen nur in der App: Seite über „Teilen → Zum Home-Bildschirm“ hinzufügen und die App dann über das Icon öffnen.'
            };
        }
        if (!('serviceWorker' in navigator) || !('PushManager' in window) || !('Notification' in window)) {
            return {
                supported: false, reason: 'unsupported',
                message: isIOS
                    ? 'Dein iPhone braucht mindestens iOS 16.4 für Benachrichtigungen. Bitte unter Einstellungen → Allgemein → Softwareupdate aktualisieren.'
                    : 'Dieser Browser unterstützt keine Push-Benachrichtigungen.'
            };
        }
        if (Notification.permission === 'denied') {
            return {
                supported: false, reason: 'denied',
                message: isIOS
                    ? 'Benachrichtigungen wurden abgelehnt. Erlauben kannst du sie in den iPhone-Einstellungen → Mitteilungen → Sündenbock.'
                    : 'Benachrichtigungen wurden abgelehnt. Erlaube sie in den Website-Einstellungen deines Browsers (Schloss-Symbol neben der Adresse).'
            };
        }
        return { supported: true, reason: null, message: null };
    }

    /**
     * Initialisiert den Push-Service (nur einmal, weitere Aufrufe warten auf denselben Vorgang)
     */
    init() {
        if (!this._initPromise) {
            this._initPromise = this._init();
        }
        return this._initPromise;
    }

    async _init() {
        try {
            // Prüfe ob Service Worker unterstützt wird
            if (!('serviceWorker' in navigator)) {
                console.warn('Service Worker nicht unterstützt');
                return false;
            }

            // Prüfe ob Push unterstützt wird
            if (!('PushManager' in window)) {
                console.warn('Push-Benachrichtigungen nicht unterstützt');
                return false;
            }

            // Registriere Service Worker
            this.swRegistration = await this.withTimeout(
                navigator.serviceWorker.register('/sw.js', { scope: '/' }),
                10000, 'Der Hintergrunddienst lässt sich nicht registrieren (Zeitüberschreitung).');
            console.log('Service Worker registriert:', this.swRegistration);

            // Warte auf Aktivierung (höchstens 10 Sekunden statt für immer)
            await this.waitForActive(this.swRegistration, 10000);

            // Lade VAPID Public Key
            await this.withTimeout(this.loadPublicKey(), 10000, 'Der Server antwortet nicht (Schlüssel).');

            // Prüfe aktuellen Subscription-Status
            await this.checkSubscriptionStatus();

            return true;
        } catch (error) {
            console.error('Fehler bei Push-Init:', error);
            this.lastError = error && error.message ? error.message : String(error);
            // Beim nächsten Versuch neu starten statt das hängende Ergebnis zu behalten
            this._initPromise = null;
            return false;
        }
    }

    /**
     * Lädt den VAPID Public Key vom Server
     */
    async loadPublicKey() {
        try {
            const response = await fetch('/api/push/vapid-public-key', { credentials: 'same-origin' });
            const data = await response.json();
            this.publicKey = data.publicKey;
            console.log('VAPID Public Key geladen');
        } catch (error) {
            console.error('Fehler beim Laden des Public Keys:', error);
            throw error;
        }
    }

    /**
     * Prüft ob bereits eine Subscription existiert
     */
    async checkSubscriptionStatus() {
        try {
            if (!this.swRegistration) { this.isSubscribed = false; return false; }
            const subscription = await this.swRegistration.pushManager.getSubscription();
            this.isSubscribed = subscription !== null;
            console.log('Subscription-Status:', this.isSubscribed);
            return this.isSubscribed;
        } catch (error) {
            console.error('Fehler beim Prüfen des Subscription-Status:', error);
            return false;
        }
    }

    /**
     * Abonniert Push-Benachrichtigungen
     */
    async subscribe() {
        try {
            const support = this.getSupportStatus();
            if (!support.supported) {
                return { success: false, message: support.message };
            }

            // WICHTIG fürs iPhone: Die Erlaubnis-Abfrage muss direkt aus dem Tipp kommen,
            // also VOR jedem anderen await. Sonst zeigt Safari keine Abfrage an.
            const permission = await this.withTimeout(Notification.requestPermission(), 30000,
                'Das iPhone hat die Erlaubnis-Abfrage nicht angezeigt (Zeitüberschreitung).');

            if (permission !== 'granted') {
                console.log('Benachrichtigungsberechtigung abgelehnt');
                return { success: false, message: this.getSupportStatus().message || 'Berechtigung wurde nicht erteilt.' };
            }

            // Falls die Vorbereitung im Hintergrund noch läuft (schnell getippt): darauf warten
            const bereit = await this.init();
            if (!bereit || !this.swRegistration || !this.swRegistration.active) {
                return { success: false, message: 'Der Hintergrunddienst konnte nicht gestartet werden. Bitte die App ganz schließen (vom unteren Rand hochwischen) und über das Icon neu öffnen.' + (this.lastError ? ' (' + this.lastError + ')' : '') };
            }

            // Prüfe ob Public Key geladen wurde
            if (!this.publicKey || this.publicKey === 'GENERATE_ME') {
                console.error('VAPID Public Key nicht konfiguriert');
                return {
                    success: false,
                    message: 'Server-Konfigurationsfehler: VAPID-Keys fehlen. Bitte informiere den Administrator.'
                };
            }

            // Erstelle Subscription
            // Ein altes Abo mit anderem Schlüssel blockiert auf dem iPhone ein neues: erst entfernen
            const alt = await this.swRegistration.pushManager.getSubscription();
            if (alt) {
                try { await alt.unsubscribe(); } catch (e) { console.warn('Altes Abo ließ sich nicht entfernen:', e); }
            }
            const subscription = await this.withTimeout(this.swRegistration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: this.urlBase64ToUint8Array(this.publicKey)
            }), 15000, 'Apple antwortet nicht (Zeitüberschreitung beim Anmelden).');

            console.log('Push-Subscription erstellt:', subscription);

            // Sende Subscription an Server
            const result = await this.sendSubscriptionToServer(subscription);

            if (result.success) {
                this.isSubscribed = true;
            }

            return result;
        } catch (error) {
            console.error('Fehler beim Abonnieren:', error);
            this.lastError = (error && (error.name ? error.name + ': ' : '') + (error.message || '')) || String(error);
            return {
                success: false,
                message: 'Fehler beim Aktivieren: ' + (error.message || 'Unbekannter Fehler')
            };
        }
    }

    /**
     * Deabonniert Push-Benachrichtigungen
     */
    async unsubscribe() {
        try {
            const subscription = await this.swRegistration.pushManager.getSubscription();

            if (!subscription) {
                return { success: true, message: 'Keine aktive Subscription' };
            }

            // Sende Deabonnierung an Server
            await this.sendUnsubscriptionToServer(subscription);

            // Deabonniere lokal
            await subscription.unsubscribe();

            this.isSubscribed = false;
            console.log('Push-Benachrichtigungen deaktiviert');

            return { success: true, message: 'Benachrichtigungen deaktiviert' };
        } catch (error) {
            console.error('Fehler beim Deabonnieren:', error);
            return { success: false, message: 'Fehler beim Deaktivieren der Benachrichtigungen' };
        }
    }

    /**
     * Sendet Subscription an Backend
     */
    async sendSubscriptionToServer(subscription) {
        try {
            const response = await fetch('/api/push/subscribe', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    endpoint: subscription.endpoint,
                    keys: {
                        p256dh: this.arrayBufferToBase64(subscription.getKey('p256dh')),
                        auth: this.arrayBufferToBase64(subscription.getKey('auth'))
                    }
                })
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Server-Fehler:', response.status, errorText);
                return {
                    success: false,
                    message: `Server-Fehler (${response.status}): ${errorText}`
                };
            }

            return await response.json();
        } catch (error) {
            console.error('Fehler beim Senden der Subscription:', error);
            return {
                success: false,
                message: 'Netzwerkfehler: ' + error.message
            };
        }
    }

    /**
     * Sendet Deabonnierung an Backend
     */
    async sendUnsubscriptionToServer(subscription) {
        try {
            const response = await fetch('/api/push/unsubscribe', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    endpoint: subscription.endpoint
                })
            });

            return await response.json();
        } catch (error) {
            console.error('Fehler beim Senden der Deabonnierung:', error);
            throw error;
        }
    }

    /**
     * Sendet Test-Benachrichtigung
     */
    async sendTestNotification() {
        try {
            const response = await fetch('/api/push/test', {
                method: 'POST'
            });

            return await response.json();
        } catch (error) {
            console.error('Fehler beim Senden der Test-Benachrichtigung:', error);
            throw error;
        }
    }

    /**
     * Lädt Benachrichtigungs-Einstellungen
     */
    async getPreferences() {
        try {
            const response = await fetch('/api/push/preferences');
            return await response.json();
        } catch (error) {
            console.error('Fehler beim Laden der Einstellungen:', error);
            throw error;
        }
    }

    /**
     * Speichert Benachrichtigungs-Einstellungen
     */
    async updatePreferences(preferences) {
        try {
            const response = await fetch('/api/push/preferences', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(preferences)
            });

            return await response.json();
        } catch (error) {
            console.error('Fehler beim Speichern der Einstellungen:', error);
            throw error;
        }
    }

    // Hilfsfunktionen

    urlBase64ToUint8Array(base64String) {
        const padding = '='.repeat((4 - base64String.length % 4) % 4);
        const base64 = (base64String + padding)
            .replace(/\-/g, '+')
            .replace(/_/g, '/');

        const rawData = window.atob(base64);
        const outputArray = new Uint8Array(rawData.length);

        for (let i = 0; i < rawData.length; ++i) {
            outputArray[i] = rawData.charCodeAt(i);
        }
        return outputArray;
    }

    arrayBufferToBase64(buffer) {
        const bytes = new Uint8Array(buffer);
        let binary = '';
        for (let i = 0; i < bytes.byteLength; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
}

// Globale Instanz
// Nicht window.pushManager: Safari (iOS 18.4+, Declarative Web Push) hat dort ein eigenes,
// schreibgeschütztes PushManager-Objekt. Die Zuweisung wurde auf dem iPhone still ignoriert.
window.sbPush = new PushNotificationManager();

// Auto-Initialisierung wenn Seite geladen ist
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.sbPush.init();
    });
} else {
    window.sbPush.init();
}
