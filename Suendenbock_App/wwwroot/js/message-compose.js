// Message Compose - Formular-Funktionalität
document.addEventListener('DOMContentLoaded', function() {
    const composeForm = document.getElementById('composeForm');

    if (composeForm) {
        composeForm.addEventListener('submit', async function(e) {
            e.preventDefault();

            const receiverSelect = document.getElementById('receiverId');
            const canSendGroupMessage = document.getElementById('canSendGroupMessage').value === 'true';
            const subject = document.getElementById('subject').value.trim();
            const content = document.getElementById('content').value.trim();

            // Empfänger sammeln (kann single oder multiple sein)
            // Format: "char:123" oder "user:abc-def:Gott"
            let selectedValues = [];

            // Bei Antwort: receiverId ist hidden input
            if (receiverSelect.type === 'hidden') {
                selectedValues = [receiverSelect.value];
            } else if (canSendGroupMessage && receiverSelect.multiple) {
                // Multiple select
                const selectedOptions = Array.from(receiverSelect.selectedOptions);
                selectedValues = selectedOptions.map(opt => opt.value);
            } else {
                // Single select
                const receiverId = receiverSelect.value;
                if (receiverId) {
                    selectedValues = [receiverId];
                }
            }

            // Validierung
            if (selectedValues.length === 0) {
                await Swal.fire({
                    title: 'Fehler',
                    text: 'Bitte wähle mindestens einen Empfänger aus.',
                    icon: 'error'
                });
                return;
            }

            // Parse Empfänger
            const characterIds = [];
            let userReceiver = null;

            selectedValues.forEach(val => {
                if (val.startsWith('char:')) {
                    // Character
                    const charId = parseInt(val.substring(5));
                    characterIds.push(charId);
                } else if (val.startsWith('user:')) {
                    // User (Gott/Moderator) - nur einer bei single select
                    const parts = val.substring(5).split(':');
                    if (parts.length >= 2) {
                        userReceiver = {
                            userId: parts[0],
                            type: parts[1]
                        };
                    }
                }
            });

            if (!subject) {
                await Swal.fire({
                    title: 'Fehler',
                    text: 'Bitte gib einen Betreff ein.',
                    icon: 'error'
                });
                return;
            }

            if (!content) {
                await Swal.fire({
                    title: 'Fehler',
                    text: 'Bitte schreibe eine Nachricht.',
                    icon: 'error'
                });
                return;
            }

            // Baue Request-Daten
            const data = {
                subject: subject,
                content: content
            };

            // Falls User-Empfänger (Gott/Moderator) und kein Charakter
            if (userReceiver && characterIds.length === 0) {
                data.receiverUserId = userReceiver.userId;
                data.receiverType = userReceiver.type === 'Gott' ? 0 : (userReceiver.type === 'Moderator' ? 2 : 0); // MessageSenderType enum
            } else {
                // Character-Empfänger
                data.receiverCharacterIds = characterIds;
            }

            // Thread-System: ParentMessageId aus URL holen (wenn es eine Antwort ist)
            const urlParams = new URLSearchParams(window.location.search);
            const replyTo = urlParams.get('replyTo');
            if (replyTo) {
                data.parentMessageId = parseInt(replyTo);
            }

            try {
                const response = await fetch('/api/messages', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(data)
                });

                if (!response.ok) {
                    const errorText = await response.text();
                    throw new Error(errorText || 'Fehler beim Senden');
                }

                await Swal.fire({
                    title: 'Gesendet!',
                    text: 'Deine Nachricht wurde versendet.',
                    icon: 'success',
                    timer: 1500,
                    showConfirmButton: false
                });

                // Redirect to sent messages
                window.location.href = '/Messages/Sent';

            } catch (error) {
                console.error('Error sending message:', error);
                await Swal.fire({
                    title: 'Fehler',
                    text: 'Die Nachricht konnte nicht gesendet werden. ' + error.message,
                    icon: 'error'
                });
            }
        });
    }
});
