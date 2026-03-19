// ==== GLOBALE SWEETALERT2-LÖSCHBESTÄTIGUNG ====
// Greift auf alle Buttons mit data-delete-form="true" und
// alle Formulare mit class "delete-confirm-form" auf der ganzen Webseite.

document.addEventListener('DOMContentLoaded', function () {

    // Alle Buttons die ein Lösch-Formular absenden wollen
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-delete-form]');
        if (!btn) return;

        e.preventDefault();
        e.stopPropagation();

        const message = btn.dataset.deleteMessage
            || 'Soll dieser Eintrag wirklich unwiderruflich entfernt werden?';

        Swal.fire({
            title: 'Seid Ihr sicher?',
            text: message,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#B89E68',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Ja, entfernen',
            cancelButtonText: 'Abbrechen',
            background: '#FFFCF7',
            color: '#2D241E',
            customClass: {
                title: 'swal-medieval-title',
                confirmButton: 'swal-medieval-confirm'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                // Das nächste <form>-Element relativ zum Button absenden
                const form = btn.closest('form') || btn.form;
                if (form) form.submit();
            }
        });
    });
});
