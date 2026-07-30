// Message Detail - Löschen Funktionalität
document.addEventListener('DOMContentLoaded', function() {
    const deleteBtn = document.querySelector('.delete-btn');

    if (deleteBtn) {
        deleteBtn.addEventListener('click', async function() {
            const messageId = this.dataset.messageId;

            const result = await Swal.fire({
                title: 'Nachricht löschen?',
                text: 'Möchtest du diese Nachricht wirklich löschen?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#ef4444',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Ja, löschen',
                cancelButtonText: 'Abbrechen'
            });

            if (!result.isConfirmed) {
                return;
            }

            try {
                const response = await fetch(`/api/messages/${messageId}`, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok) {
                    throw new Error('Fehler beim Löschen');
                }

                await Swal.fire({
                    title: 'Gelöscht!',
                    text: 'Die Nachricht wurde gelöscht.',
                    icon: 'success',
                    timer: 1500,
                    showConfirmButton: false
                });

                // Zurück zur Inbox oder Sent
                const urlParams = new URLSearchParams(window.location.search);
                const fromSent = urlParams.get('from') === 'sent';
                window.location.href = fromSent ? '/Messages/Sent' : '/Messages/Inbox';

            } catch (error) {
                console.error('Error deleting message:', error);
                await Swal.fire({
                    title: 'Fehler',
                    text: 'Die Nachricht konnte nicht gelöscht werden.',
                    icon: 'error'
                });
            }
        });
    }
});
