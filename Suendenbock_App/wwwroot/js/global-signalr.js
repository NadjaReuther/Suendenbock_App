// ===== GLOBAL SIGNALR CONNECTION =====
// Dieses Script läuft auf ALLEN Seiten und verbindet Spieler mit dem GameHub

let globalConnection = null;
let currentActId = null;
let currentUserName = null;

// ===== INITIALIZATION =====
document.addEventListener('DOMContentLoaded', async () => {
    // ActId aus dem DOM lesen (wird vom Server gesetzt)
    const actIdElement = document.getElementById('global-act-id');
    const userNameElement = document.getElementById('global-user-name');

    console.log('[global-signalr] DOMContentLoaded');
    console.log('[global-signalr] actIdElement:', actIdElement);
    console.log('[global-signalr] actIdElement.value:', actIdElement?.value);
    console.log('[global-signalr] userNameElement:', userNameElement);
    console.log('[global-signalr] userNameElement.value:', userNameElement?.value);

    if (actIdElement && actIdElement.value) {
        currentActId = parseInt(actIdElement.value);
        console.log('[global-signalr] currentActId set to:', currentActId);
    } else {
        console.warn('[global-signalr] No actIdElement or value found');
    }

    if (userNameElement && userNameElement.value) {
        currentUserName = userNameElement.value;
        console.log('[global-signalr] currentUserName set to:', currentUserName);
    }

    // Nur verbinden, wenn ActId vorhanden ist
    if (currentActId) {
        console.log('[global-signalr] Initializing SignalR...');
        await initializeGlobalSignalR();

        // Beim Laden prüfen, ob bereits ein Kampf läuft
        await checkForActiveCombat();
    } else {
        console.warn('[global-signalr] No currentActId, skipping SignalR initialization');
    }
});

// ===== SIGNALR SETUP =====
async function initializeGlobalSignalR() {
    try {
        // SignalR Connection erstellen
        globalConnection = new signalR.HubConnectionBuilder()
            .withUrl("/gamehub")
            .withAutomaticReconnect()
            .build();

        // Event: Combat Started
        globalConnection.on("CombatStarted", (data) => {
            handleCombatStarted(data);
        });

        // Event: Combat Ended
        globalConnection.on("CombatEnded", (data) => {
            handleCombatEnded(data);
        });

        // Event: Generische Nachricht
        globalConnection.on("ReceiveMessage", (data) => {
            // Hier könntest du weitere Message-Types handlen
        });

        // Event: Night Rest Requested (Gott empfängt)
        globalConnection.on("NightRestRequested", (data) => {
            // Dispatch Custom Event für Dashboard
            window.dispatchEvent(new CustomEvent('nightRestRequested', { detail: data }));
        });

        // Event: Night Rest Completed (Spieler empfangen)
        globalConnection.on("NightRestCompleted", (data) => {
            // Dispatch Custom Event für Dashboard
            window.dispatchEvent(new CustomEvent('nightRestCompleted', { detail: data }));
        });

        // Event: Night Rest Cancelled (beide Seiten empfangen)
        globalConnection.on("NightRestCancelled", (data) => {
            window.dispatchEvent(new CustomEvent('nightRestCancelled', { detail: data }));
        });

        // Reconnect Handler
        globalConnection.onreconnecting((error) => {
            // Reconnecting...
        });

        globalConnection.onreconnected((connectionId) => {
            // Act wieder beitreten nach Reconnect
            if (currentActId) {
                globalConnection.invoke("JoinAct", currentActId, currentUserName || "Unknown");
            }
        });

        globalConnection.onclose((error) => {
            // Connection closed
        });

        // Verbindung starten
        await globalConnection.start();
        console.log('[global-signalr] SignalR connected successfully, state:', globalConnection.state);

        // Act beitreten
        if (currentActId) {
            console.log('[global-signalr] Joining Act:', currentActId);
            await globalConnection.invoke("JoinAct", currentActId, currentUserName || "Unknown");
            console.log('[global-signalr] Successfully joined Act:', currentActId);
        }

    } catch (error) {
        console.error('[global-signalr] Error initializing SignalR:', error);
        // Retry nach 5 Sekunden
        setTimeout(initializeGlobalSignalR, 5000);
    }
}

// ===== COMBAT STARTED HANDLER =====
function handleCombatStarted(data) {
    // Modal/Banner anzeigen
    showCombatNotification(data);

    // Nach 3 Sekunden zur Battle-Seite weiterleiten (mit SessionId in URL)
    setTimeout(() => {
        window.location.href = `/Spielmodus/Battle?sessionId=${data.combatSessionId}`;
    }, 3000);
}

// ===== COMBAT ENDED HANDLER =====
function handleCombatEnded(data) {
    // Banner entfernen
    const banner = document.getElementById('active-combat-banner');
    if (banner) {
        banner.remove();
        document.body.style.paddingTop = '0';
    }

    // Combat Notification entfernen (falls angezeigt)
    const notification = document.getElementById('combat-notification');
    if (notification) {
        notification.remove();
    }
}

// ===== NOTIFICATION UI =====
function showCombatNotification(data) {
    // Erstelle ein großes Banner/Modal
    const notification = document.createElement('div');
    notification.id = 'combat-notification';
    notification.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(0, 0, 0, 0.9);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 10000;
        animation: fadeIn 0.3s ease-in;
    `;

    notification.innerHTML = `
        <div style="
            background: linear-gradient(135deg, #7f1d1d 0%, #991b1b 100%);
            border: 3px solid #fbbf24;
            border-radius: 1rem;
            padding: 3rem;
            text-align: center;
            box-shadow: 0 0 50px rgba(251, 191, 36, 0.5);
            max-width: 600px;
            animation: slideDown 0.5s ease-out;
        ">
            <div style="font-size: 4rem; margin-bottom: 1rem;">⚔️</div>
            <h1 style="
                font-family: 'Cinzel', serif;
                font-size: 2.5rem;
                color: #fbbf24;
                margin: 0 0 1rem 0;
                text-transform: uppercase;
                letter-spacing: 0.2em;
                text-shadow: 0 0 20px rgba(251, 191, 36, 0.5);
            ">
                KAMPF BEGINNT!
            </h1>
            <p style="
                font-size: 1.25rem;
                color: #fde68a;
                margin: 0 0 2rem 0;
                font-style: italic;
            ">
                Bereitet euch vor, Recken des Reiches...
            </p>
            <div style="
                background: rgba(0, 0, 0, 0.3);
                padding: 1rem;
                border-radius: 0.5rem;
                color: white;
                font-size: 0.875rem;
            ">
                Ihr werdet in <span id="countdown" style="color: #fbbf24; font-weight: bold;">3</span> Sekunden zum Schlachtfeld weitergeleitet.
            </div>
        </div>
    `;

    document.body.appendChild(notification);

    // Countdown
    let count = 3;
    const countdownElement = notification.querySelector('#countdown');
    const countdownInterval = setInterval(() => {
        count--;
        if (countdownElement) {
            countdownElement.textContent = count;
        }
        if (count <= 0) {
            clearInterval(countdownInterval);
        }
    }, 1000);

    // CSS Animations hinzufügen
    if (!document.getElementById('combat-notification-styles')) {
        const style = document.createElement('style');
        style.id = 'combat-notification-styles';
        style.textContent = `
            @keyframes fadeIn {
                from { opacity: 0; }
                to { opacity: 1; }
            }
            @keyframes slideDown {
                from {
                    opacity: 0;
                    transform: translateY(-50px);
                }
                to {
                    opacity: 1;
                    transform: translateY(0);
                }
            }
        `;
        document.head.appendChild(style);
    }
}

// ===== PRÜFEN OB KAMPF LÄUFT (beim Laden) =====
async function checkForActiveCombat() {
    if (!currentActId) return;

    try {
        // Prüfe ob ein aktiver Kampf läuft (verwendet ActNumber)
        const response = await fetch(`/api/battle/active/act/${currentActId}`);

        if (response.ok) {
            const data = await response.json();

            // Banner anzeigen
            showActiveCombatBanner(data.sessionId);
        }
    } catch (error) {
        // Kein aktiver Kampf oder Fehler - kein Problem
    }
}

// ===== BANNER FÜR AKTIVEN KAMPF =====
function showActiveCombatBanner(sessionId) {
    // Prüfen ob wir bereits auf der Battle-Seite sind
    if (window.location.pathname.includes('/Battle')) {
        return; // Banner nicht anzeigen wenn bereits im Kampf
    }

    // Banner erstellen
    const banner = document.createElement('div');
    banner.id = 'active-combat-banner';
    banner.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        background: linear-gradient(135deg, #7f1d1d 0%, #991b1b 100%);
        color: white;
        padding: 1rem;
        text-align: center;
        z-index: 9999;
        border-bottom: 3px solid #fbbf24;
        box-shadow: 0 4px 12px rgba(0,0,0,0.5);
        cursor: pointer;
        animation: slideDown 0.5s ease-out;
    `;

    banner.innerHTML = `
        <div style="display: flex; align-items: center; justify-content: center; gap: 1rem;">
            <span style="font-size: 1.5rem;">⚔️</span>
            <div>
                <strong style="font-size: 1.1rem;">AKTIVER KAMPF</strong>
                <div style="font-size: 0.9rem; opacity: 0.9;">Klicken um zum Gefecht beizutreten</div>
            </div>
            <span style="font-size: 1.5rem;">⚔️</span>
        </div>
    `;

    banner.onclick = () => {
        window.location.href = `/Spielmodus/Battle?sessionId=${sessionId}`;
    };

    document.body.appendChild(banner);

    // Body padding hinzufügen damit Inhalt nicht verdeckt wird
    document.body.style.paddingTop = '80px';
}

// ===== EXPORT FÜR ANDERE SCRIPTS =====
window.globalSignalR = {
    get connection() { return globalConnection; },
    getActId: () => currentActId,
    getUserName: () => currentUserName
};
