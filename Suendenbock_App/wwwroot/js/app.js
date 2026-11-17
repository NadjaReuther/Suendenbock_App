// ==== Sündenbock Handy-App (/App) ====
// Vorspann, "Seit deinem letzten Besuch", Wiki-Suche, Zusagen, Schreibmodus.
(function () {
    'use strict';

    var LAST_SEEN_KEY = 'sb-last-seen';      // localStorage: wann zuletzt "Alles klar" gedrückt
    var INTRO_KEY = 'sb-intro-done';         // sessionStorage: Vorspann in dieser Sitzung gesehen
    var FIRST_VISIT_DAYS = 7;                // beim allerersten Öffnen: Neues der letzten 7 Tage

    // ---------- kleine Hilfen ----------
    function $(sel, root) { return (root || document).querySelector(sel); }
    function $all(sel, root) { return Array.prototype.slice.call((root || document).querySelectorAll(sel)); }
    function esc(t) {
        return String(t == null ? '' : t).replace(/[&<>"']/g, function (c) {
            return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c];
        });
    }
    function store(kind) {
        try { return kind === 'session' ? window.sessionStorage : window.localStorage; } catch (e) { return null; }
    }
    function getItem(kind, key) { var s = store(kind); try { return s ? s.getItem(key) : null; } catch (e) { return null; } }
    function setItem(kind, key, val) { var s = store(kind); try { if (s) s.setItem(key, val); } catch (e) { } }

    function ago(ts) {
        var m = Math.round((Date.now() - ts) / 60000);
        if (m < 2) return 'gerade eben';
        if (m < 60) return 'vor ' + m + ' Minuten';
        var h = Math.round(m / 60);
        if (h < 24) return 'vor ' + h + ' Stunde' + (h > 1 ? 'n' : '');
        var d = Math.round(h / 24);
        return d === 1 ? 'gestern' : 'vor ' + d + ' Tagen';
    }
    window.sbAgo = ago;

    function toast(text) {
        var t = document.createElement('div');
        t.className = 'toast';
        t.setAttribute('role', 'status');
        t.textContent = text;
        document.body.appendChild(t);
        setTimeout(function () { t.remove(); }, 2800);
    }
    window.sbToast = toast;

    function lastSeen() {
        var v = parseInt(getItem('local', LAST_SEEN_KEY) || '', 10);
        return isNaN(v) ? Date.now() - FIRST_VISIT_DAYS * 864e5 : v;
    }

    // ---------- Zeitangaben ("vor 3 Tagen") ----------
    function renderTimes() {
        $all('[data-ago]').forEach(function (el) {
            var ts = parseInt(el.getAttribute('data-ago'), 10);
            if (!isNaN(ts)) el.textContent = ago(ts);
        });
        // Rote Punkte für alles, was seit dem letzten Besuch neu ist
        var seen = lastSeen();
        $all('[data-new-ts]').forEach(function (el) {
            var ts = parseInt(el.getAttribute('data-new-ts'), 10);
            if (!isNaN(ts) && ts > seen && !el.querySelector('.udot')) {
                var dot = document.createElement('span');
                dot.className = 'udot';
                dot.setAttribute('aria-label', 'neu');
                el.insertBefore(dot, el.firstChild);
            }
        });
    }

    // ---------- Vorspann ----------
    function startIntro() {
        var intro = $('#intro');
        if (!intro) return;
        if (getItem('session', INTRO_KEY)) return;   // schon gesehen in dieser Sitzung

        intro.classList.add('on');
        intro.removeAttribute('aria-hidden');
        var btn = $('#introBtn');
        if (!btn) return;

        btn.addEventListener('click', function () {
            if (intro.classList.contains('closing')) return;
            setItem('session', INTRO_KEY, '1');
            intro.classList.add('closing');
            setTimeout(function () { intro.classList.add('out'); }, 900);
            setTimeout(function () { intro.classList.remove('on'); intro.setAttribute('aria-hidden', 'true'); }, 1500);
            setTimeout(showWhatsNew, 1100);
        });
    }

    // ---------- Seit deinem letzten Besuch ----------
    function showWhatsNew() {
        fetch('/App/Neuigkeiten', { credentials: 'same-origin', headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(function (r) { return r.ok ? r.json() : []; })
            .then(function (feed) {
                var seen = lastSeen();
                var items = (feed || []).filter(function (f) { return f.timestamp > seen; });
                if (!items.length) { setItem('local', LAST_SEEN_KEY, String(Date.now())); return; }

                var hadVisit = !!getItem('local', LAST_SEEN_KEY);
                $('#wnSince').textContent = (hadVisit ? 'Zuletzt da: ' + ago(seen) : 'Die letzten ' + FIRST_VISIT_DAYS + ' Tage')
                    + ' · ' + items.length + ' Neuigkeit' + (items.length > 1 ? 'en' : '');

                var max = 6;
                $('#wnList').innerHTML = items.slice(0, max).map(function (f) {
                    return '<a class="wn-item k-' + esc(f.kind) + '" href="' + esc(f.url) + '">' +
                        '<div class="ic"><span class="ms">' + esc(f.icon) + '</span></div>' +
                        '<div class="body"><div class="k">' + esc(f.label) + ' · ' + ago(f.timestamp) + '</div>' +
                        '<h4>' + esc(f.title) + '</h4><p>' + esc(f.text) + '</p></div>' +
                        '<span class="ms chev">chevron_right</span></a>';
                }).join('') + (items.length > max ? '<p class="more">+ ' + (items.length - max) + ' weitere auf der Startseite</p>' : '');

                $('#whatsnew').classList.add('on');
                $all('#wnList a').forEach(function (a) { a.addEventListener('click', markSeen); });
                setTimeout(function () { var b = $('#wnClose'); if (b) b.focus({ preventScroll: true }); }, 300);
            })
            .catch(function () { /* still: kein Pop-up */ });
    }
    function markSeen() { setItem('local', LAST_SEEN_KEY, String(Date.now())); }
    function closeWhatsNew() { markSeen(); $('#whatsnew').classList.remove('on'); }

    // ---------- Wiki-Suche ----------
    var TYPES = {
        character: { label: 'Charaktere', icon: 'person' },
        guild: { label: 'Gilden', icon: 'shield' },
        infanterie: { label: 'Infanterien', icon: 'swords' },
        monster: { label: 'Monster', icon: 'pets' },
        magicclass: { label: 'Magie', icon: 'auto_awesome' },
        glossary: { label: 'Glossar', icon: 'menu_book' },
        forum: { label: 'Forum', icon: 'forum' }
    };
    var searchTimer = null, lastHits = [], lastQuery = '';

    function openSearch() {
        $('#search').classList.add('on');
        var q = $('#sq'); q.value = '';
        renderSearchHint();
        setTimeout(function () { q.focus(); }, 60);
    }
    function closeSearch() { $('#search').classList.remove('on'); }
    function renderSearchHint() {
        $('#sres').innerHTML = '<p class="empty"><span class="ms">search</span>Ein Wort eingeben – gesucht wird in Charakteren, Gilden, Infanterien, Monstern, Magie, Glossar und Forum.</p>';
    }
    function hl(text, q) {
        var i = text.toLowerCase().indexOf(q.toLowerCase());
        if (i < 0 || !q) return esc(text);
        return esc(text.slice(0, i)) + '<mark>' + esc(text.slice(i, i + q.length)) + '</mark>' + esc(text.slice(i + q.length));
    }
    function runSearch() {
        var q = $('#sq').value.trim();
        lastQuery = q;
        if (q.length < 2) { renderSearchHint(); return; }
        $('#sres').innerHTML = '<p class="empty">Suche …</p>';
        var hdr = { credentials: 'same-origin', headers: { 'X-Requested-With': 'XMLHttpRequest' } };
        Promise.all([
            fetch('/api/entity/search?query=' + encodeURIComponent(q), hdr).then(function (r) { return r.ok ? r.json() : { results: [] }; }).catch(function () { return { results: [] }; }),
            fetch('/App/ForumSuche?q=' + encodeURIComponent(q), hdr).then(function (r) { return r.ok ? r.json() : []; }).catch(function () { return []; })
        ]).then(function (res) {
            if (q !== lastQuery) return;   // inzwischen weitergetippt
            lastHits = (res[0].results || []).concat(res[1] || []);
            renderHits(q);
        });
    }
    function renderHits(q) {
        var box = $('#sres');
        if (!lastHits.length) { box.innerHTML = '<p class="empty">Nichts gefunden für „' + esc(q) + '“.</p>'; return; }
        var groups = {}, order = [];
        lastHits.forEach(function (h, i) {
            h._i = i;
            if (!groups[h.type]) { groups[h.type] = []; order.push(h.type); }
            groups[h.type].push(h);
        });
        box.innerHTML = order.map(function (type) {
            var t = TYPES[type] || { label: type, icon: 'article' };
            return '<div class="sgrp">' + esc(t.label) + ' · ' + groups[type].length + '</div>' +
                groups[type].map(function (h) {
                    return '<button type="button" class="sitem" data-hit="' + h._i + '">' +
                        '<div class="ic"><span class="ms">' + t.icon + '</span></div>' +
                        '<div style="flex:1;min-width:0"><h4>' + hl(h.name || '', q) + '</h4><p>' + esc(h.subtitle || t.label) + '</p></div>' +
                        '<span class="ms" style="color:var(--muted)">chevron_right</span></button>';
                }).join('');
        }).join('');
        $all('[data-hit]', box).forEach(function (b) {
            b.addEventListener('click', function () { openHit(lastHits[parseInt(b.getAttribute('data-hit'), 10)]); });
        });
    }
    function openHit(h) {
        if (!h) return;
        if (h.type === 'forum') { location.href = h.url; return; }
        var box = $('#sres');
        box.innerHTML = '<p class="empty">Lade …</p>';
        fetch('/App/Eintrag?type=' + encodeURIComponent(h.type) + '&id=' + encodeURIComponent(h.id),
            { credentials: 'same-origin', headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(function (r) { if (!r.ok) throw new Error(); return r.text(); })
            .then(function (html) {
                box.innerHTML = '<button type="button" class="back" id="sBack" style="background:none;border:0;font:inherit;cursor:pointer"><span class="ms">chevron_left</span>Ergebnisse</button>' + html;
                $('#sBack').addEventListener('click', function () { renderHits(lastQuery); });
                box.scrollTop = 0;
            })
            .catch(function () { box.innerHTML = '<p class="empty">Der Eintrag konnte nicht geladen werden.</p>'; });
    }

    // ---------- Zusagen zu Terminen ----------
    function initRsvp() {
        $all('[data-rsvp]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var card = btn.closest('[data-event-id]');
                var id = parseInt(card.getAttribute('data-event-id'), 10);
                var status = btn.getAttribute('data-rsvp');
                var all = $all('[data-rsvp]', card);
                all.forEach(function (b) { b.disabled = true; });

                fetch('/api/events/rsvp', {
                    method: 'POST', credentials: 'same-origin',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ eventId: id, status: status })
                }).then(function (r) {
                    if (!r.ok) throw new Error();
                    var before = card.getAttribute('data-my-rsvp');
                    var yes = card.querySelector('[data-yes-count]');
                    if (yes) {
                        var n = parseInt(yes.getAttribute('data-yes-count'), 10) || 0;
                        if (before === 'yes') n--;
                        if (status === 'yes') n++;
                        yes.setAttribute('data-yes-count', n);
                        yes.textContent = n + ' dabei';
                    }
                    card.setAttribute('data-my-rsvp', status);
                    all.forEach(function (b) {
                        b.classList.remove('sel-yes', 'sel-maybe', 'sel-no');
                        b.setAttribute('aria-pressed', 'false');
                        if (b.getAttribute('data-rsvp') === status) { b.classList.add('sel-' + status); b.setAttribute('aria-pressed', 'true'); }
                    });
                    toast(status === 'yes' ? 'Du bist dabei ✓' : status === 'maybe' ? 'Als „vielleicht“ gespeichert' : 'Abgesagt');
                }).catch(function () {
                    toast('Das hat nicht geklappt. Bitte nochmal versuchen.');
                }).then(function () {
                    all.forEach(function (b) { b.disabled = false; });
                });
            });
        });
    }

    // ---------- Antwortfeld (Forum) ----------
    function initComposer() {
        $all('.composer textarea').forEach(function (ta) {
            var btn = ta.closest('.composer').querySelector('button');
            function update() {
                ta.style.height = 'auto';
                ta.style.height = Math.min(ta.scrollHeight, 160) + 'px';
                if (btn) btn.disabled = !ta.value.trim();
            }
            ta.addEventListener('input', update);
            update();
        });
    }

    // ---------- Charakter: Änderungen merken, Schreibmodus ----------
    function initCharacter() {
        var form = $('#charForm');
        if (!form) return;
        var bar = $('#savebar');
        function dirty() { if (bar) bar.hidden = false; }
        form.addEventListener('input', dirty);
        form.addEventListener('change', dirty);

        var sw = $('#charSwitch');
        if (sw) sw.addEventListener('change', function () { location.href = '/App/Charakter/' + sw.value; });

        var file = $('#bildInput');
        if (file) file.addEventListener('change', function () {
            var f = file.files && file.files[0];
            if (!f) return;
            var img = $('#portraitImg');
            var reader = new FileReader();
            reader.onload = function () {
                if (!img) {
                    img = document.createElement('img'); img.id = 'portraitImg'; img.alt = '';
                    var p = $('#portrait'); var ini = $('#portraitIni'); if (ini) ini.remove();
                    p.insertBefore(img, p.firstChild);
                }
                img.src = reader.result;
            };
            reader.readAsDataURL(f);
            dirty();
        });

        // Schreibmodus für die Beschreibung
        var writer = $('#writer'), ed = $('#wEd'), hidden = $('#Description'), prev = $('#descPreview'), count = $('#wCount');
        if (!writer || !ed || !hidden) return;
        var draftKey = 'sb-draft-' + form.getAttribute('data-character-id');
        var draftTimer = null;

        function words(html) {
            var t = (html || '').replace(/<[^>]+>/g, ' ').replace(/&nbsp;/g, ' ').trim();
            return t ? t.split(/\s+/).length : 0;
        }
        function updateCount() {
            if (count) count.textContent = words(ed.innerHTML) + ' Wörter';
            var d = $('#wDraft'); if (d) d.textContent = 'Wird gesichert …';
            clearTimeout(draftTimer);
            draftTimer = setTimeout(function () {
                setItem('local', draftKey, ed.innerHTML);
                if (d) d.textContent = 'Entwurf auf diesem Handy gesichert ✓';
            }, 700);
        }
        $all('[data-writer-open]').forEach(function (b) {
            b.addEventListener('keydown', function (e) {
                if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); b.click(); }
            });
            b.addEventListener('click', function () {
                var draft = getItem('local', draftKey);
                ed.innerHTML = draft && draft !== hidden.value ? draft : hidden.value;
                if (draft && draft !== hidden.value) toast('Ungespeicherter Entwurf wiederhergestellt');
                updateCount();
                writer.classList.add('on');
                document.body.style.overflow = 'hidden';
                setTimeout(function () { ed.focus(); }, 50);
            });
        });
        ed.addEventListener('input', updateCount);
        $all('[data-cmd]', writer).forEach(function (b) {
            b.addEventListener('mousedown', function (e) { e.preventDefault(); });
            b.addEventListener('click', function () {
                document.execCommand(b.getAttribute('data-cmd'), false, b.getAttribute('data-val') || null);
                updateCount();
            });
        });
        $('#wCancel').addEventListener('click', function () {
            writer.classList.remove('on'); document.body.style.overflow = '';
        });
        $('#wDone').addEventListener('click', function () {
            hidden.value = ed.innerHTML;
            if (prev) prev.innerHTML = ed.innerHTML || '<i class="muted">Noch keine Beschreibung – tippen zum Schreiben</i>';
            var pc = $('#descCount'); if (pc) pc.textContent = words(ed.innerHTML) + ' Wörter';
            writer.classList.remove('on'); document.body.style.overflow = '';
            dirty();
        });
        form.addEventListener('submit', function () {
            // Nach dem Speichern brauchen wir den Entwurf nicht mehr
            try { localStorage.removeItem(draftKey); } catch (e) { }
        });
    }

    // ---------- Nachrichten: antworten und neu schreiben ----------
    // Nutzt dieselbe Schnittstelle wie die Website (/api/messages), daher automatisch synchron.
    function sendMessage(data) {
        return fetch('/api/messages', {
            method: 'POST', credentials: 'same-origin',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        }).then(function (r) {
            if (!r.ok) return r.text().then(function (t) { throw new Error(t || 'Fehler'); });
            return r.json();
        });
    }
    function receiverData(value) {
        // "char:12" oder "user:<id>:<typ>"
        if (!value) return null;
        if (value.indexOf('char:') === 0) return { receiverCharacterIds: [parseInt(value.slice(5), 10)] };
        var parts = value.split(':');
        return { receiverUserId: parts[1], receiverType: parseInt(parts[2], 10) };
    }
    function initMessages() {
        // Im Gesprächsverlauf direkt zur neuesten Nachricht springen
        if ($('.msgs') && $('#replyForm')) window.scrollTo(0, document.body.scrollHeight);
        var reply = $('#replyForm');
        if (reply) reply.addEventListener('submit', function (e) {
            e.preventDefault();
            var ta = reply.querySelector('textarea'), btn = reply.querySelector('button');
            var text = ta.value.trim();
            if (!text) return;
            var data = {
                subject: reply.getAttribute('data-subject'),
                content: text,
                parentMessageId: parseInt(reply.getAttribute('data-parent'), 10)
            };
            var ch = reply.getAttribute('data-char'), us = reply.getAttribute('data-user');
            if (ch) data.receiverCharacterIds = [parseInt(ch, 10)];
            else { data.receiverUserId = us; data.receiverType = parseInt(reply.getAttribute('data-type') || '1', 10); }
            btn.disabled = true;
            sendMessage(data).then(function () {
                location.reload();
            }).catch(function (err) {
                btn.disabled = false;
                toast('Senden hat nicht geklappt: ' + (err.message || 'Fehler'));
            });
        });

        var compose = $('#composeForm');
        if (compose) compose.addEventListener('submit', function (e) {
            e.preventDefault();
            var to = receiverData($('#cTo').value);
            var subject = $('#cSubject').value.trim(), text = $('#cText').value.trim();
            if (!to || !subject || !text) { toast('Bitte Empfänger, Betreff und Nachricht ausfüllen.'); return; }
            var btn = compose.querySelector('button[type=submit]');
            btn.disabled = true;
            var data = { subject: subject, content: text };
            for (var k in to) data[k] = to[k];
            sendMessage(data).then(function (res) {
                var id = res && res.ids && res.ids[0];
                location.href = id ? '/App/Nachricht/' + id : '/App/Post?ordner=gesendet';
            }).catch(function (err) {
                btn.disabled = false;
                toast('Senden hat nicht geklappt: ' + (err.message || 'Fehler'));
            });
        });
    }

    // ---------- Neues Thema ----------
    function initNewThread() {
        var open = $('#newThreadBtn'), box = $('#newThread');
        if (!open || !box) return;
        open.addEventListener('click', function () {
            box.hidden = false; open.hidden = true;
            var t = $('#ntTitle'); if (t) t.focus();
        });
        var cancel = $('#ntCancel');
        if (cancel) cancel.addEventListener('click', function () { box.hidden = true; open.hidden = false; });
    }

    // ---------- Start ----------
    document.addEventListener('DOMContentLoaded', function () {
        renderTimes();
        startIntro();
        initRsvp();
        initComposer();
        initCharacter();
        initNewThread();
        initMessages();

        var sb = $('#searchBtn'); if (sb) sb.addEventListener('click', openSearch);
        var sc = $('#searchClose'); if (sc) sc.addEventListener('click', closeSearch);
        var sq = $('#sq'); if (sq) sq.addEventListener('input', function () { clearTimeout(searchTimer); searchTimer = setTimeout(runSearch, 250); });
        var wc = $('#wnClose'); if (wc) wc.addEventListener('click', closeWhatsNew);
        var wn = $('#whatsnew'); if (wn) wn.addEventListener('click', function (e) { if (e.target === wn) closeWhatsNew(); });
        document.addEventListener('keydown', function (e) {
            if (e.key !== 'Escape') return;
            if ($('#search') && $('#search').classList.contains('on')) closeSearch();
            else if (wn && wn.classList.contains('on')) closeWhatsNew();
        });
    });
})();
