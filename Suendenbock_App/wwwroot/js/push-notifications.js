// Push-Benachrichtigungs-Client
// Verwaltet die Subscription und Kommunikation mit dem Backend

class PushNotificationManager {
    constructor() {
        this.swRegistration = null;
        this.isSubscribed = false;
        this.publicKey = null;
        this._initPromise = null;
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
            this.swRegistration = await navigator.serviceWorker.register('/sw.js');
            console.log('Service Worker registriert:', this.swRegistration);

            // Warte auf Aktivierung
            await navigator.serviceWorker.ready;

            // Lade VAPID Public Key
            await this.loadPublicKey();

            // Prüfe aktuellen Subscription-Status
            await this.checkSubscriptionStatus();

            return true;
        } catch (error) {
            console.error('Fehler bei Push-Init:', error);
            return false;
        }
    }

    /**
     * Lädt den VAPID Public Key vom Server
     */
    async loadPublicKey() {
        try {
            const response = await fetch('/api/push/vapid-public-key');
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
            const permission = await Notification.requestPermission();

            if (permission !== 'granted') {
                console.log('Benachrichtigungsberechtigung abgelehnt');
                return { success: false, message: this.getSupportStatus().message || 'Berechtigung wurde nicht erteilt.' };
            }

            // Falls die Vorbereitung im Hintergrund noch läuft (schnell getippt): darauf warten
            await this.init();
            if (!this.swRegistration) {
                return { success: false, message: 'Der Hintergrunddienst konnte nicht gestartet werden. Bitte die App einmal schließen und neu öffnen.' };
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
            const subscription = await this.swRegistration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: this.urlBase64ToUint8Array(this.publicKey)
            });

            console.log('Push-Subscription erstellt:', subscription);

            // Sende Subscription an Server
            const result = await this.sendSubscriptionToServer(subscription);

            if (result.success) {
                this.isSubscribed = true;
            }

            return result;
        } catch (error) {
            console.error('Fehler beim Abonnieren:', error);
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
window.pushManager = new PushNotificationManager();

// Auto-Initialisierung wenn Seite geladen ist
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.pushManager.init();
    });
} else {
    window.pushManager.init();
}
