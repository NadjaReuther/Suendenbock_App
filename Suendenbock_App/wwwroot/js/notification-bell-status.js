// Notification Bell Status Updater
// Aktualisiert das Bell-Icon basierend auf dem Subscription-Status

document.addEventListener('DOMContentLoaded', async function() {
    const bellLink = document.getElementById('notificationBellIcon');
    const bellIcon = document.getElementById('bellIcon');
    const badge = document.getElementById('notificationBadge');

    if (!bellLink || !bellIcon || !badge) {
        return; // Elements nicht gefunden
    }

    // Warte bis pushManager initialisiert ist
    if (window.pushManager) {
        try {
            await window.pushManager.init();
            updateBellIcon();

            // Update alle 30 Sekunden (falls Status sich ändert)
            setInterval(updateBellIcon, 30000);
        } catch (error) {
            console.error('Fehler beim Initialisieren des Push Managers:', error);
        }
    }

    async function updateBellIcon() {
        try {
            const isSubscribed = window.pushManager && window.pushManager.isSubscribed;

            if (isSubscribed) {
                // Aktiv - goldene Glocke
                bellIcon.textContent = 'notifications_active';
                bellLink.classList.add('active');
                bellLink.classList.remove('inactive');
                bellLink.title = 'Benachrichtigungen aktiv (klick für Einstellungen)';
                badge.style.display = 'none';
            } else {
                // Inaktiv - graue Glocke mit Badge
                bellIcon.textContent = 'notifications_off';
                bellLink.classList.add('inactive');
                bellLink.classList.remove('active');
                bellLink.title = 'Benachrichtigungen deaktiviert (klick zum Aktivieren)';
                badge.style.display = 'flex';
            }
        } catch (error) {
            console.error('Fehler beim Aktualisieren des Bell Icons:', error);
        }
    }
});
