// Service Worker für Push-Benachrichtigungen
// Dieser Worker läuft im Hintergrund und empfängt Push-Nachrichten

const CACHE_NAME = 'suendenbock-v1';

// Installation
self.addEventListener('install', (event) => {
    console.log('[Service Worker] Installing...');
    self.skipWaiting();
});

// Activation
self.addEventListener('activate', (event) => {
    console.log('[Service Worker] Activating...');
    event.waitUntil(clients.claim());
});

// Push-Benachrichtigungen empfangen
self.addEventListener('push', (event) => {
    console.log('[Service Worker] Push received:', event);

    let data = {
        title: 'Neue Benachrichtigung',
        body: 'Du hast eine neue Nachricht erhalten',
        icon: '/images/logo.png',
        badge: '/images/badge.png',
        url: '/'
    };

    if (event.data) {
        try {
            data = event.data.json();
        } catch (e) {
            console.error('[Service Worker] Error parsing push data:', e);
        }
    }

    const options = {
        body: data.body,
        icon: data.icon || '/images/logo.png',
        badge: data.badge || '/images/badge.png',
        data: {
            url: data.url || '/'
        },
        vibrate: [200, 100, 200],
        tag: 'suendenbock-notification',
        requireInteraction: false,
        actions: [
            {
                action: 'open',
                title: 'Öffnen'
            },
            {
                action: 'close',
                title: 'Schließen'
            }
        ]
    };

    event.waitUntil(
        self.registration.showNotification(data.title, options)
    );
});

// Klick auf Benachrichtigung
self.addEventListener('notificationclick', (event) => {
    console.log('[Service Worker] Notification clicked:', event);

    event.notification.close();

    if (event.action === 'close') {
        return;
    }

    // Öffne die URL aus der Benachrichtigung
    const urlToOpen = event.notification.data.url || '/';

    event.waitUntil(
        clients.matchAll({ type: 'window', includeUncontrolled: true })
            .then((clientList) => {
                // Prüfe ob bereits ein Fenster offen ist
                for (let i = 0; i < clientList.length; i++) {
                    const client = clientList[i];
                    if (client.url === urlToOpen && 'focus' in client) {
                        return client.focus();
                    }
                }
                // Öffne neues Fenster
                if (clients.openWindow) {
                    return clients.openWindow(urlToOpen);
                }
            })
    );
});

// Fetch-Events (für Offline-Funktionalität - optional)
self.addEventListener('fetch', (event) => {
    // Aktuell keine Caching-Strategie implementiert
    // Kann später erweitert werden für Offline-Support
});
