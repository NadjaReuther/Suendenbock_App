// =====================================================================
// Hesparis · Schulkalender: Jahreszeiten-Panorama mit Laternen
// (aus der hesparis.html übernommen, JS: MODS.kalender)
//
// Der Server liefert in #kpDaten das Schuljahr, den Spieltag und die Termine.
// Dieses Skript zeichnet nur; Zoom und Ausschnitt merkt sich der Browser.
// =====================================================================
(function () {
'use strict';
const datenEl = document.getElementById('kpDaten');
const w = document.getElementById('kpWrap');
if (!datenEl || !w) return;
const D = JSON.parse(datenEl.textContent);
const KAL_TYPES = D.types, KAL_MON = D.monate;
const KP_ZOOM = [['Jahr', 1], ['Halbjahr', 2], ['Quartal', 3.6], ['Monat', 9]];
const KP = { center: null, drag: null, wl: false };
const reduce = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;

function esc(t){ return String(t == null ? '' : t).replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c])); }
function kalParse(s){ const [y, m, d] = String(s).split('-').map(Number); return new Date(y, (m || 1) - 1, d || 1); }
function kalFmt(s){ const d = kalParse(s); return d.getDate() + '. ' + KAL_MON[d.getMonth()]; }
function kalIso(d){ return d.getFullYear() + '-' + String(d.getMonth() + 1).padStart(2, '0') + '-' + String(d.getDate()).padStart(2, '0'); }

// Zoomstufe pro Browser merken
function zoomLesen(){ try { return Math.max(0, Math.min(KP_ZOOM.length - 1, parseInt(localStorage.getItem('hes-kal-zoom') || '0', 10) || 0)); } catch (e) { return 0; } }
function zoomMerken(i){ try { localStorage.setItem('hes-kal-zoom', String(i)); } catch (e) { } }
let zoom = zoomLesen();

function render(){
  const heute = D.heute, y0 = D.year;
  const start = new Date(y0, 8, 1), end = new Date(y0 + 1, 6, 31), span = (end - start) / 864e5;
  const pos = iso => Math.max(0, Math.min(1, (kalParse(iso) - start) / 864e5 / span)) * 100;
  const dpos = d => Math.max(0, Math.min(1, (d - start) / 864e5 / span)) * 100;
  const tx = pos(heute);
  // Jahreszeiten
  const SEAS = [['Herbst', new Date(y0, 8, 1), new Date(y0, 11, 1), '#f08f5a', '78,32,44'], ['Winter', new Date(y0, 11, 1), new Date(y0 + 1, 2, 1), '#cfe4ff', '26,42,86'], ['Frühling', new Date(y0 + 1, 2, 1), new Date(y0 + 1, 5, 1), '#8fd67a', '24,70,62'], ['Sommer', new Date(y0 + 1, 5, 1), new Date(y0 + 1, 7, 1), '#e7c56a', '76,62,28']];
  const grad = SEAS.map(sz => 'rgb(' + sz[4] + ') ' + ((dpos(sz[1]) + dpos(sz[2])) / 2).toFixed(1) + '%').join(', ');
  // Termine: in Ebenen verteilen, damit sich die Schilder nicht überdecken
  const evs = D.events.slice().sort((a, b) => a.date.localeCompare(b.date)).filter(e => e.date >= kalIso(start) && e.date <= kalIso(end));
  const zi = zoom, Z = KP_ZOOM[zi][1];
  const kpGap = Math.max(8.5 / Z, 150 / (Math.max(400, window.innerWidth) * Z) * 100), lastAt = [-99, -99, -99];
  const lant = evs.map(e => {
    const x = Math.max(4 / Z, Math.min(100 - 4 / Z, pos(e.date))); let lv = [0, 1, 2].find(l => x - lastAt[l] > kpGap); if (lv === undefined) lv = [0, 1, 2].sort((p, q) => lastAt[p] - lastAt[q])[0];
    lastAt[lv] = x; const t = KAL_TYPES[e.type] || KAL_TYPES.sonstiges;
    return { e, x, lv, c: t[1], kind: t[0], past: e.date < heute, today: e.date === heute };
  });
  const groundY = x => 85 + Math.sin(x / 100 * Math.PI * 4) * 1.6;
  let gp = ''; for (let i = 0; i <= 100; i += 2) gp += (i ? ' L' : 'M') + (i * 10) + ' ' + groundY(i).toFixed(2);
  // Heute-Karte
  const todayEv = evs.filter(e => e.date === heute), nextEv = evs.find(e => e.date > heute), d = kalParse(heute);
  const daysTo = nextEv ? Math.round((kalParse(nextEv.date) - d) / 864e5) : 0;
  const sznNow = SEAS.find(sz => d >= sz[1] && d < sz[2]) || SEAS[0];
  let h = '<div class="kp-board"><section class="kp-sky" style="--kp-bg:linear-gradient(90deg, ' + grad + ')">';
  h += '<div class="kp-head"><div><div class="armory-eyebrow">Schulkalender · Schuljahr ' + y0 + '/' + String(y0 + 1).slice(2) + '</div><h1 class="armory-title">Von Herbstlaub bis Sommerglut</h1></div>' +
    '<div class="kp-zoom" role="group" aria-label="Zoom"><button type="button" class="st" data-kzstep="-1"' + (zi === 0 ? ' disabled' : '') + ' aria-label="Herauszoomen">−</button>' +
    KP_ZOOM.map((zz, i) => '<button type="button" class="' + (i === zi ? 'on' : '') + '" data-kzoom="' + i + '" aria-pressed="' + (i === zi) + '">' + zz[0] + '</button>').join('') +
    '<button type="button" class="st" data-kzstep="1"' + (zi === KP_ZOOM.length - 1 ? ' disabled' : '') + ' aria-label="Hineinzoomen">+</button>' +
    (zi ? '<button type="button" class="td" data-kztoday="1">heute</button>' : '') + '</div></div>';
  h += '<div class="kp-scene' + (Z > 1 ? ' zoomed' : '') + '" data-kpscene><div class="kp-track" style="width:' + (Z * 100) + '%">';
  SEAS.forEach(sz => { h += '<div class="kp-season" style="left:' + ((dpos(sz[1]) + dpos(sz[2])) / 2).toFixed(1) + '%;--sc:' + sz[3] + '">' + sz[0] + '</div>'; });
  // Schnee im Winter
  const w0 = dpos(SEAS[1][1]), w1 = dpos(SEAS[1][2]);
  for (let i = 0; i < 26; i++) h += '<i class="kp-flake" style="left:' + (w0 + ((i * 37) % 100) / 100 * (w1 - w0)).toFixed(1) + '%;top:' + (18 + ((i * 53) % 50)) + '%;animation-delay:' + ((i * 0.7) % 6).toFixed(1) + 's"></i>';
  h += '<svg class="kp-land" viewBox="0 0 1000 100" preserveAspectRatio="none" aria-hidden="true">' +
    '<path d="M0 70 L40 62 L80 67 L125 56 L170 65 L215 58 L260 64 L305 53 L350 62 L395 57 L440 64 L495 50 L545 60 L595 55 L645 63 L700 56 L750 64 L805 57 L860 63 L910 56 L955 62 L1000 59 L1000 100 L0 100 Z" fill="rgba(10,6,23,0.5)"/>' +
    '<path d="' + gp + ' L1000 100 L0 100 Z" fill="#0a0617"/>' +
    '<path d="' + gp + '" fill="none" stroke="rgba(231,197,106,0.6)" stroke-width="1.5" stroke-dasharray="3 9" vector-effect="non-scaling-stroke"/></svg>';
  h += '<div class="kp-past" style="width:' + tx.toFixed(2) + '%"></div>';
  lant.forEach(L => {
    h += '<div class="kp-lant lv' + L.lv + (L.past ? ' past' : '') + (L.today ? ' today' : '') + '" style="left:' + L.x.toFixed(2) + '%;top:' + groundY(L.x).toFixed(1) + '%;--lc:' + L.c + '" title="' + esc(L.kind + ': ' + L.e.title) + '">' +
      '<span class="kp-l-d">' + esc(kalFmt(L.e.date)) + '</span><span class="kp-l-t">' + esc(L.e.title) + '</span><i class="kp-lamp"></i><i class="kp-pole"></i></div>';
  });
  h += '<div class="kp-now" style="left:' + tx.toFixed(2) + '%"><i class="kp-beam"></i><i class="kp-orb"></i></div>';
  h += '<div class="kp-card' + (tx > 70 ? ' left' : '') + '" style="left:' + tx.toFixed(2) + '%"><div class="kp-card-k">Heute</div><div class="kp-card-d">' + ['Sonntag','Montag','Dienstag','Mittwoch','Donnerstag','Freitag','Samstag'][d.getDay()] + ', ' + esc(kalFmt(heute)) + '</div>' +
    '<div class="kp-card-s">' + (todayEv.length ? '<b>' + todayEv.map(e => esc(e.title)).join(' · ') + '</b>' : sznNow[0] + (nextEv ? ' · noch ' + daysTo + (daysTo === 1 ? ' Tag' : ' Tage') + ' bis ' + esc(nextEv.title) : '')) + '</div></div>';
  const MON = ['SEP','OKT','NOV','DEZ','JAN','FEB','MÄR','APR','MAI','JUN','JUL'];
  // ab der Quartalsansicht: Wochen am Boden, in der Monatsansicht jeder Tag
  if (Z >= 3) {
    let tk = '<div class="kp-ticks">';
    const dayPx = Math.max(360, window.innerWidth) * Z / span, days = dayPx >= 20, wk = Math.max(1, Math.ceil(46 / (dayPx * 7)));
    let wn = 0;
    for (let dd = new Date(start); dd <= end; dd.setDate(dd.getDate() + 1)) {
      const mo = dd.getDay() === 1; if (!mo && !days) continue;
      if (mo && !days && (wn++ % wk)) continue;
      tk += '<span class="' + (mo ? 'mo' : '') + (kalIso(dd) === heute ? ' td' : '') + '" style="left:' + dpos(dd).toFixed(3) + '%">' + (days ? dd.getDate() : dd.getDate() + '.' + (dd.getMonth() + 1) + '.') + '</span>';
    }
    h += tk + '</div>';
  }
  h += '<div class="kp-months">' + MON.map((m, i) => { const ms = new Date(y0 + (i >= 4 ? 1 : 0), (8 + i) % 12, 1), me = new Date(ms.getFullYear(), ms.getMonth() + 1, 1); const on = d >= ms && d < me; return '<span class="' + (on ? 'on' : '') + '" style="left:' + ((dpos(ms) + dpos(me)) / 2).toFixed(1) + '%">' + (Z >= 3 ? KAL_MON[ms.getMonth()].toUpperCase() : m) + '</span>'; }).join('') + '</div>';
  h += '</div></div>';
  h += '<div class="kp-legend">' + Object.values(KAL_TYPES).map(v => '<span><i style="--lc:' + v[1] + '"></i>' + v[0] + '</span>').join('') + '</div></section></div>';
  w.innerHTML = h;
  kpFit(); setTimeout(kpFit, 60);
  // Ausschnitt halten: beim Zoomen bleibt die Mitte, sonst steht „heute“ in der Mitte
  const sc = w.querySelector('[data-kpscene]');
  if (sc) { const f = typeof KP.center === 'number' ? KP.center : tx / 100; KP.center = null; requestAnimationFrame(() => { sc.scrollLeft = Math.max(0, f * sc.scrollWidth - sc.clientWidth / 2); }); }
}

function kpZoomTo(i, cx){
  const sc = w.querySelector('[data-kpscene]');
  const ziel = Math.max(0, Math.min(KP_ZOOM.length - 1, i));
  // Knöpfe zoomen auf „heute“, das Mausrad um die Stelle unter dem Zeiger
  if (sc && typeof cx === 'number') { KP.center = (sc.scrollLeft + cx) / sc.scrollWidth + (sc.clientWidth / 2 - cx) / (sc.scrollWidth * KP_ZOOM[ziel][1] / KP_ZOOM[zoom][1]); }
  else KP.center = null;
  zoom = ziel; zoomMerken(zoom); render();
}

// Höhe des Panoramas an das Fenster anpassen
function kpFit(){
  const b = w.querySelector('.kp-board'); if (!b) return;
  if (window.innerWidth <= 760) { b.style.height = ''; return; }
  const top = b.getBoundingClientRect().top + window.scrollY;
  let hh = Math.max(460, window.innerHeight - top - 46); b.style.height = hh + 'px';
}
window.addEventListener('resize', kpFit);

// Strg/Cmd + Mausrad zoomt, das Mausrad allein schiebt im Zoom seitlich
w.addEventListener('wheel', (e) => {
  const sc = e.target.closest && e.target.closest('[data-kpscene]'); if (!sc) return;
  if (e.ctrlKey || e.metaKey) { e.preventDefault(); const n = zoom + (e.deltaY < 0 ? 1 : -1); if (n >= 0 && n < KP_ZOOM.length && !KP.wl) { KP.wl = true; setTimeout(() => { KP.wl = false; }, 260); kpZoomTo(n, e.clientX - sc.getBoundingClientRect().left); } return; }
  if (zoom > 0 && Math.abs(e.deltaY) > Math.abs(e.deltaX)) { e.preventDefault(); sc.scrollLeft += e.deltaY; }
}, { passive: false });
// mit der Maus ziehen
w.addEventListener('pointerdown', (e) => {
  const sc = e.target.closest && e.target.closest('[data-kpscene]'); if (!sc || zoom === 0 || e.pointerType === 'touch' || e.button !== 0) return;
  KP.drag = { sc, x: e.clientX, l: sc.scrollLeft }; sc.classList.add('grab');
});
window.addEventListener('pointermove', (e) => { const g = KP.drag; if (!g) return; g.sc.scrollLeft = g.l - (e.clientX - g.x); });
window.addEventListener('pointerup', () => { if (KP.drag) { KP.drag.sc.classList.remove('grab'); KP.drag = null; } });

w.addEventListener('click', (e) => {
  let b;
  if ((b = e.target.closest('[data-kzoom]'))) { kpZoomTo(+b.dataset.kzoom); return; }
  if ((b = e.target.closest('[data-kzstep]'))) { kpZoomTo(zoom + +b.dataset.kzstep); return; }
  if (e.target.closest('[data-kztoday]')) {
    const sc = w.querySelector('[data-kpscene]'); if (!sc) return;
    const st0 = new Date(D.year, 8, 1), en = new Date(D.year + 1, 6, 31), f = (kalParse(D.heute) - st0) / (en - st0);
    sc.scrollTo({ left: Math.max(0, f * sc.scrollWidth - sc.clientWidth / 2), behavior: reduce ? 'auto' : 'smooth' });
  }
});

render();
})();
