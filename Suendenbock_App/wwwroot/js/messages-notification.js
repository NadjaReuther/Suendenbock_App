// Messages Notification Badge - Update unread count
(function() {
    const messagesBadge = document.getElementById('messagesBadge');

    if (!messagesBadge) {
        return; // Not on a page with messages badge
    }

    async function updateUnreadCount() {
        try {
            const response = await fetch('/api/messages/unread-count');
            if (!response.ok) {
                return;
            }

            const data = await response.json();
            const count = data.count || 0;

            if (count > 0) {
                messagesBadge.textContent = count > 99 ? '99+' : count;
                messagesBadge.style.display = 'flex';
            } else {
                messagesBadge.style.display = 'none';
            }
        } catch (error) {
            console.error('Error fetching unread message count:', error);
        }
    }

    // Initial load
    updateUnreadCount();

    // Update every 30 seconds
    setInterval(updateUnreadCount, 30000);
})();
