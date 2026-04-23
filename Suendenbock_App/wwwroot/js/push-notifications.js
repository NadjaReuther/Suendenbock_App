// Push-Benachrichtigungs-Client
// Verwaltet die Subscription und Kommunikation mit dem Backend

class PushNotificationManager {
    constructor() {
        this.swRegistration = null;
        this.isSubscribed = false;
        this.publicKey = null;
    }

    /**
     * Initialisiert den Push-Service
     */
    async init() {
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
            // Fordere Berechtigung an
            const permission = await Notification.requestPermission();

            if (permission !== 'granted') {
                console.log('Benachrichtigungsberechtigung abgelehnt');
                return { success: false, message: 'Berechtigung wurde abgelehnt' };
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
            return { success: false, message: 'Fehler beim Aktivieren der Benachrichtigungen' };
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

            return await response.json();
        } catch (error) {
            console.error('Fehler beim Senden der Subscription:', error);
            throw error;
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
