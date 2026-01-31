// News.js - Interactivity for News Page

// Toggle Comments Section
document.querySelectorAll('.toggle-comments-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const newsId = this.dataset.newsId;
        const commentsSection = document.querySelector(`.comments-section[data-news-id="${newsId}"]`);
        const icon = this.querySelector('.material-symbols-outlined');
        const textSpan = this.querySelector('span:last-child');

        if (commentsSection.style.display === 'none') {
            commentsSection.style.display = 'block';
            this.classList.add('active');
            textSpan.textContent = 'Diskussion verbergen';
            icon.textContent = 'expand_less';
        } else {
            commentsSection.style.display = 'none';
            this.classList.remove('active');
            textSpan.textContent = 'Diskussion öffnen';
            icon.textContent = 'forum';
        }
    });
});

// Create News Button
const createNewsBtn = document.getElementById('createNewsBtn');
if (createNewsBtn) {
    createNewsBtn.addEventListener('click', openCreateModal);
}

// Edit News Buttons
document.querySelectorAll('.edit-news-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const newsId = this.dataset.newsId;
        openEditModal(newsId);
    });
});

// Delete News Buttons
document.querySelectorAll('.delete-news-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const newsId = this.dataset.newsId;
        deleteNews(newsId);
    });
});

// Delete Comment Buttons
document.querySelectorAll('.delete-comment-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const newsId = this.dataset.newsId;
        const commentId = this.dataset.commentId;
        deleteComment(newsId, commentId);
    });
});

// Send Comment Buttons
document.querySelectorAll('.send-comment-btn').forEach(btn => {
    btn.addEventListener('click', function() {
        const newsId = this.dataset.newsId;
        const textarea = document.querySelector(`.comment-input[data-news-id="${newsId}"]`);
        const text = textarea.value.trim();

        if (text) {
            addComment(newsId, text);
        }
    });
});

// News Form Submit
const newsForm = document.getElementById('newsForm');
if (newsForm) {
    newsForm.addEventListener('submit', function(e) {
        e.preventDefault();
        saveNews();
    });
}

// === MODAL FUNCTIONS ===

function openCreateModal() {
    const modal = document.getElementById('newsModal');
    const modalTitle = document.getElementById('modalTitle');
    const newsId = document.getElementById('newsId');

    modalTitle.textContent = 'Botschaft verfassen';
    newsId.value = '';
    document.getElementById('newsForm').reset();

    modal.style.display = 'flex';
}

async function openEditModal(newsId) {
    const modal = document.getElementById('newsModal');
    const modalTitle = document.getElementById('modalTitle');
    const newsIdField = document.getElementById('newsId');

    modalTitle.textContent = 'Botschaft anpassen';
    newsIdField.value = newsId;

    // Load news data via AJAX
    try {
        const response = await fetch(`/api/news/${newsId}`);
        if (response.ok) {
            const data = await response.json();
            document.getElementById('newsTitle').value = data.title;
            document.getElementById('newsContent').value = data.content;
            document.getElementById('newsCategory').value = data.category;
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: 'Fehler beim Laden der Neuigkeit.',
                confirmButtonColor: '#d97706'
            });
            return;
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Ein Fehler ist aufgetreten.',
            confirmButtonColor: '#d97706'
        });
        return;
    }

    modal.style.display = 'flex';
}

function closeNewsModal() {
    const modal = document.getElementById('newsModal');
    modal.style.display = 'none';
    document.getElementById('newsForm').reset();
}

// === API FUNCTIONS ===

async function saveNews() {
    const newsId = document.getElementById('newsId').value;
    const title = document.getElementById('newsTitle').value;
    const content = document.getElementById('newsContent').value;
    const category = document.getElementById('newsCategory').value;

    const data = {
        title: title,
        content: content,
        category: category
    };

    try {
        let url, method;
        if (newsId) {
            // Update existing news
            url = `/api/news/${newsId}`;
            method = 'PUT';
            data.id = newsId;
        } else {
            // Create new news
            url = '/api/news';
            method = 'POST';
        }

        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            closeNewsModal();
            window.location.reload();
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: 'Fehler beim Speichern der Neuigkeit.',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Ein Fehler ist aufgetreten.',
            confirmButtonColor: '#d97706'
        });
    }
}

async function deleteNews(newsId) {
    const result = await Swal.fire({
        title: 'Botschaft löschen?',
        text: 'Möchtest du diese Botschaft wirklich für immer aus der Chronik tilgen?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#d97706',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Löschen',
        cancelButtonText: 'Abbrechen'
    });

    if (!result.isConfirmed) {
        return;
    }

    try {
        const response = await fetch(`/api/news/${newsId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            window.location.reload();
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: 'Fehler beim Löschen der Neuigkeit.',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Ein Fehler ist aufgetreten.',
            confirmButtonColor: '#d97706'
        });
    }
}

async function addComment(newsId, text) {
    const data = {
        newsItemId: newsId,
        text: text
    };

    try {
        const response = await fetch('/api/news/comment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            window.location.reload();
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: 'Fehler beim Hinzufügen des Kommentars.',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Ein Fehler ist aufgetreten.',
            confirmButtonColor: '#d97706'
        });
    }
}

async function deleteComment(newsId, commentId) {
    const result = await Swal.fire({
        title: 'Kommentar löschen?',
        text: 'Soll dieser Kommentar aus der Versammlung entfernt werden?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#d97706',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Löschen',
        cancelButtonText: 'Abbrechen'
    });

    if (!result.isConfirmed) {
        return;
    }

    try {
        const response = await fetch(`/api/news/comment/${commentId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            window.location.reload();
        } else {
            await Swal.fire({
                icon: 'error',
                title: 'Fehler',
                text: 'Fehler beim Löschen des Kommentars.',
                confirmButtonColor: '#d97706'
            });
        }
    } catch (error) {
        await Swal.fire({
            icon: 'error',
            title: 'Fehler',
            text: 'Ein Fehler ist aufgetreten.',
            confirmButtonColor: '#d97706'
        });
    }
}

// Close modal when clicking outside
document.addEventListener('click', function(e) {
    const modal = document.getElementById('newsModal');
    if (e.target === modal) {
        closeNewsModal();
    }
});
