// =====================================================================
// Zirkel der Priore: das gläserne Brett (aus der hesparis.html übernommen)
//
// Der Server liefert in #zkDaten, was auf dem Brett steht (Figuren, Farben, wer tot ist).
// Dieses Skript zeichnet nur und dreht das Brett. Gespeichert und geprüft wird auf dem Server.
// =====================================================================
(function () {
'use strict';
const datenEl = document.getElementById('zkDaten');
const svg = document.getElementById('zkSvg');
if (!datenEl || !svg) return;
const D = JSON.parse(datenEl.textContent);
const ZK_EL = D.magien;
// Schlüssel der Figur → Form des Symbols
const ZK_FIG = { koenig:['König','','king'], dame:['Dame','','queen'], turm:['Turm','','rook'], laeufer:['Läufer','','bishop'], springer:['Springer','','knight'], bauer:['Bauer','','pawn'] };
function esc(t){ return String(t == null ? '' : t).replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c])); }
const reduce = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;

const ZK_BASE = ' M7 44 h26 c1 0 2 1 2 2 v2 h-30 v-2 c0 -1 1 -2 2 -2 z';
const ZK_PATH = {
  pawn:'M20 7 a6 6 0 1 1 0 12 a6 6 0 1 1 0 -12 z M14 21 h12 l-2 3 c2 6 4 12 5 19 h-18 c1 -7 3 -13 5 -19 z',
  rook:'M10 6 h5 v4 h3 v-4 h4 v4 h3 v-4 h5 v9 l-4 3 v17 l3 8 h-20 l3 -8 v-17 l-4 -3 z',
  bishop:'M20 2 a3 3 0 1 1 0 6 a3 3 0 1 1 0 -6 z M20 9 c6 4 8 9 8 13 c0 4 -3 6 -5 7 l2 14 h-10 l2 -14 c-2 -1 -5 -3 -5 -7 c0 -4 2 -9 8 -13 z',
  knight:'M13 43 c0 -8 2 -12 6 -16 c-3 1 -6 3 -9 2 c-2 -1 -2 -3 0 -5 c3 -3 5 -8 6 -12 l2 -5 l2 4 c6 1 11 7 11 17 c0 6 -1 11 -2 15 z',
  queen:'M5 9 l5 20 h20 l5 -20 l-8 10 l-3 -14 l-4 13 l-4 -13 l-3 14 z M10 31 h20 l-2 12 h-16 z',
  king:'M18.5 1 h3 v4 h4 v3 h-4 v4 h-3 v-4 h-4 v-3 h4 z M12 14 c4 -2 12 -2 16 0 c3 4 1 10 -2 14 l2 15 h-16 l2 -15 c-3 -4 -5 -10 -2 -14 z'
};

function zkShade(hex, f){
  const v = parseInt(hex.slice(1), 16), c = [v >> 16, (v >> 8) & 255, v & 255];
  return '#' + c.map(x => Math.round(f > 0 ? x + (255 - x) * f : x * (1 + f)).toString(16).padStart(2, '0')).join('');
}
function zkRgba(hex, a){ const v = parseInt(hex.slice(1), 16); return 'rgba(' + (v >> 16) + ',' + ((v >> 8) & 255) + ',' + (v & 255) + ',' + a + ')'; }

// ---------- das Brett ----------
function zkSvgInner(z, sl, rot){
  const sel = z.sel, E = ZK_EL[sel - 1];
  const dead = z.dead;
  const CX = 688, CY = 352, R = 560, K = 0.42, P = 0.24, TH = 42, f = v => v.toFixed(1);
  const ang = n => 90 + (n - 1) * 30 - rot;
  const proj = (r, a) => { const t = a * Math.PI / 180, bx = r * Math.cos(t), by = r * Math.sin(t), s = 1 + (by / R) * P; return [CX + bx * s, CY + by * K * s, s]; };
  const ring = (r, a0, a1, dy, N) => { N = N || 72; const o = []; for(let i = 0; i <= N; i++){ const p = proj(r, a0 + (a1 - a0) * i / N); o.push(f(p[0]) + ' ' + f(p[1] + (dy || 0) * p[2])); } return o; };
  const disc = r => 'M' + ring(r, 0, 360).join(' L') + ' Z';
  const poly = (r0, r1, a0, a1) => 'M' + ring(r1, a0, a1, 0, 10).join(' L') + ' L' + ring(r0, a1, a0, 0, 10).join(' L') + ' Z';
  const lin = (id, c, l, dk, flip) => '<linearGradient id="' + id + '" x1="' + (flip ? 1 : 0) + '" y1="0" x2="' + (flip ? 0 : 1) + '" y2="1"><stop offset="0" stop-color="' + zkShade(c, l) + '"/><stop offset="0.5" stop-color="' + c + '"/><stop offset="1" stop-color="' + zkShade(c, dk) + '"/></linearGradient>';
  const pcol = c => '<linearGradient id="zkC' + c.slice(1) + '" x1="0" x2="1" y1="0" y2="0.25"><stop offset="0" stop-color="' + zkShade(c, 0.7) + '"/><stop offset="0.35" stop-color="' + c + '"/><stop offset="0.8" stop-color="' + zkShade(c, -0.55) + '"/><stop offset="1" stop-color="' + zkShade(c, 0.3) + '"/></linearGradient>';

  let d = '<defs><filter id="zkBlur" x="-50%" y="-50%" width="200%" height="200%"><feGaussianBlur stdDeviation="14"/></filter>' +
    '<filter id="zkGlow" x="-80%" y="-80%" width="260%" height="260%"><feGaussianBlur stdDeviation="9"/></filter>' +
    '<linearGradient id="zkSlab" x1="0" x2="0" y1="0" y2="1"><stop offset="0" stop-color="#2c2048"/><stop offset="0.5" stop-color="#160c2e"/><stop offset="1" stop-color="#07040f"/></linearGradient>' +
    '<radialGradient id="zkHub" cx="50%" cy="45%" r="60%"><stop offset="0" stop-color="#fff6d8"/><stop offset="0.45" stop-color="' + E.col + '"/><stop offset="1" stop-color="' + zkShade(E.col, -0.45) + '"/></radialGradient>' +
    '<linearGradient id="zkShaft" x1="0" x2="0" y1="0" y2="1"><stop offset="0" stop-color="' + E.col + '" stop-opacity="0"/><stop offset="0.75" stop-color="' + E.col + '" stop-opacity="0.3"/><stop offset="1" stop-color="' + E.col + '" stop-opacity="0"/></linearGradient>' +
    '<marker id="zkArr" viewBox="0 0 10 10" refX="8" refY="5" markerWidth="7" markerHeight="7" orient="auto-start-reverse"><path d="M0 0 L10 5 L0 10 z" fill="context-stroke"/></marker>' +
    lin('zkDeadT', '#4a4560', 0.25, -0.6) + pcol('#6a6480') + pcol('#a89568');
  ZK_EL.forEach(e => { const odd = e.n % 2; d += lin('zks' + e.n, e.col, 0.55, -0.6, !odd) + lin('zkp' + e.n, e.col, 0.5, -0.55, !odd) + lin('zkr' + e.n, e.col, 0.25, -0.7, !odd) + pcol(e.col); });
  d += '</defs>';

  const top = ring(R, 0, 180), bot = ring(R, 180, 0, TH);
  let s = d +
    '<ellipse cx="' + CX + '" cy="' + f(CY + R * K + 70) + '" rx="' + f(R * 1.05) + '" ry="' + f(R * K * 0.65) + '" fill="#000" fill-opacity="0.65" filter="url(#zkBlur)"/>' +
    '<path d="M' + top.join(' L') + ' L' + bot.join(' L') + ' Z" fill="url(#zkSlab)" stroke="#07040f" stroke-width="2"/>' +
    '<path d="M' + top.join(' L') + '" fill="none" stroke="#d9a24a" stroke-opacity="0.75" stroke-width="2"/>' +
    '<path d="M' + ring(R, 0, 180, TH * 0.5).join(' L') + '" fill="none" stroke="#a89568" stroke-opacity="0.35" stroke-width="1"/>' +
    '<path d="' + disc(R) + '" fill="#0d0820"/>';

  // Glasfelder
  ZK_EL.forEach(e => {
    const a = ang(e.n), on = e.n === sel, odd = e.n % 2, isDead = e.n === dead;
    [['s', 0.27, 0.56, odd ? 0.32 : 0.55], ['p', 0.56, 0.86, odd ? 0.62 : 0.34], ['r', 0.86, 0.99, 0.8]].forEach(([k, r0, r1, op]) => {
      const fill = isDead ? 'url(#zkDeadT)' : 'url(#zk' + k + e.n + ')';
      s += '<path class="zk-tile" data-zsel="' + e.n + '" d="' + poly(R * r0, R * r1, a - 15, a + 15) + '" fill="' + fill + '" fill-opacity="' + (on ? 1 : isDead ? 0.85 : op) + '" stroke="#07040f" stroke-width="' + (on ? 4 : 3) + '" stroke-linejoin="round"/>';
      s += '<path d="M' + ring(R * r1 - 6, a - 13, a + 13, 0, 12).filter((x, i) => i % 3 === 0).join(' L') + '" fill="none" stroke="#fff6dc" stroke-opacity="' + (on ? 0.75 : 0.22) + '" stroke-width="1.2" pointer-events="none"/>';
    });
    if(isDead){
      const o = proj(R * 0.71, a + 2);
      [[0.57, -12], [0.58, 9], [0.85, -13], [0.86, 5], [0.79, 14], [0.66, -4]].forEach(([rr, da], i) => {
        const mid = proj(R * (0.71 + (rr - 0.71) * 0.5), a + 2 + da * 0.5 + (i % 2 ? 3 : -3)), p = proj(R * rr, a + da);
        s += '<path d="M' + f(o[0]) + ' ' + f(o[1]) + ' L' + f(mid[0]) + ' ' + f(mid[1]) + ' L' + f(p[0]) + ' ' + f(p[1]) + '" fill="none" stroke="#e8e2ff" stroke-opacity="0.75" stroke-width="1.3" pointer-events="none"/>';
      });
    }
  });
  // Kreislauf
  s += '<path class="zk-flow" d="' + disc(R * 0.56) + '" fill="none" stroke="#fff6dc" stroke-opacity="0.55" stroke-width="2" stroke-dasharray="2 19" stroke-linecap="round" pointer-events="none"/>';
  s += '<path d="' + disc(R * 0.27) + '" fill="#07040f"/><path class="zk-hub" d="' + disc(R * 0.22) + '" fill="url(#zkHub)"/>';

  // Figuren
  const pieces = [], pos = {};
  ZK_EL.forEach(e => {
    const a = ang(e.n), on = e.n === sel;
    [['p', R * 0.71, a], ['s', R * 0.42, a + 10]].forEach(([r, rr, aa]) => {
      const p = proj(rr, aa), dd = (Math.sin(aa * Math.PI / 180) + 1) / 2, isP = r === 'p';
      const fig = z.figuren[e.n + '|' + r].fig, known = !!(fig && ZK_FIG[fig]), isDead = isP && e.n === dead;
      const col = isDead ? '#6a6480' : z.figuren[e.n + '|' + r].col;
      const big = isP && on && !isDead ? 1.45 : 1;
      const sc = (isP ? 60 : 36) * p[2] * (0.7 + 0.5 * dd) * big / 40;
      pos[e.n + '|' + r] = [p[0], p[1] - 30 * sc];
      let g = '<g class="zk-pc" data-zsel="' + e.n + '" data-zrole="' + r + '" transform="translate(' + f(p[0] - 20 * sc) + ' ' + f(p[1] - 51 * sc) + ') scale(' + sc.toFixed(3) + ')">';
      if(isP && on && !isDead) g += '<ellipse cx="20" cy="30" rx="18" ry="24" fill="' + col + '" fill-opacity="0.55" filter="url(#zkGlow)"/>';
      g += '<ellipse cx="20" cy="51" rx="16" ry="3.6" fill="#000" fill-opacity="0.6"/>';
      g += '<g' + (isDead ? ' transform="rotate(-80 20 51)"' : '') + (isP && on && !isDead ? ' class="zk-float"' : '') + ' opacity="' + (0.62 + 0.38 * dd).toFixed(2) + '">';
      const dpath = ZK_PATH[known ? ZK_FIG[fig][2] : 'pawn'] + ZK_BASE;
      if(known || isDead){
        g += '<path d="' + dpath + '" fill="url(#zkC' + col.slice(1) + ')" stroke="' + zkShade(col, -0.7) + '" stroke-width="0.8"/>' +
          '<path d="' + dpath + '" fill="none" stroke="#fff6dc" stroke-opacity="0.4" stroke-width="0.5" transform="translate(-0.6 -0.4)"/>';
      } else {
        g += '<path d="' + dpath + '" fill="#120a24" fill-opacity="0.8" stroke="' + col + '" stroke-width="0.9" stroke-dasharray="2 2"/>' +
          '<text x="20" y="36" text-anchor="middle" font-family="Cinzel Decorative, serif" font-size="12" fill="' + col + '">?</text>';
      }
      g += '</g><title>' + esc(z.figuren[e.n + '|' + r].title) + '</title></g>';
      pieces.push([p[1] + (isP && on ? 1000 : 0), g]);
    });
  });
  const pp = pos[sel + '|p'], sp = pos[sel + '|s'];
  // Lichtkegel und Lichtpfütze des gewählten Priors
  if(sel !== dead){
    const pb = proj(R * 0.71, ang(sel));
    s += '<polygon points="' + f(pb[0] - 34) + ',0 ' + f(pb[0] + 34) + ',0 ' + f(pb[0] + 170) + ',' + f(pb[1] + 30) + ' ' + f(pb[0] - 170) + ',' + f(pb[1] + 30) + '" fill="url(#zkShaft)" pointer-events="none"/>';
    s += '<ellipse cx="' + f(pb[0]) + '" cy="' + f(pb[1]) + '" rx="' + f(110 * pb[2]) + '" ry="' + f(34 * pb[2]) + '" fill="' + E.col + '" fill-opacity="0.7" filter="url(#zkBlur)" pointer-events="none"/>';
  }
  s += '<line x1="' + f(pp[0]) + '" y1="' + f(pp[1] + 26) + '" x2="' + f(sp[0]) + '" y2="' + f(sp[1] + 14) + '" stroke="' + z.figuren[sel + '|s'].col + '" stroke-width="2.5" stroke-dasharray="4 6" stroke-linecap="round" pointer-events="none"/>';
  // Beschriftung
  ZK_EL.forEach(e => {
    const a = ang(e.n), L = proj(R * 0.925, a), dd = (Math.sin(a * Math.PI / 180) + 1) / 2;
    s += '<text x="' + f(L[0]) + '" y="' + f(L[1] + 5) + '" text-anchor="middle" font-family="Cinzel Decorative, serif" font-size="' + f(11 + dd * 9) + '" fill="#fff6dc" stroke="#07040f" stroke-width="3.5" paint-order="stroke" fill-opacity="' + (0.45 + dd * 0.55).toFixed(2) + '" pointer-events="none">' + e.name + '</text>';
  });
  pieces.sort((a, b) => a[0] - b[0]).forEach(x => { s += x[1]; });
  // Spielleitung: wem dienen Prior und Sekundant wirklich?
  // Spielleitung: wem dienen Prior und Sekundant wirklich? (Pfeile kommen fertig vom Server)
  (z.pfeile || []).forEach(pf => {
    const to = pos[pf.to], from = pos[pf.from]; if(!to || !from) return;
    const mx = (from[0] + to[0]) / 2, my = Math.min(from[1], to[1]) - 90;
    s += '<path d="M' + f(from[0]) + ' ' + f(from[1]) + ' Q' + f(mx) + ' ' + f(my) + ' ' + f(to[0]) + ' ' + f(to[1]) + '" fill="none" stroke="' + (pf.bad ? '#e5606a' : '#e7d6a3') + '" stroke-width="2.5" stroke-dasharray="7 5" marker-end="url(#zkArr)" pointer-events="none"/>';
  });
  return s;
}


// ---------------------------------------------------------------------
// Drehen: Die letzte Stellung merkt sich der Browser, damit das Brett
// nach dem Neuladen von dort zur neuen Magie weiterdreht.
// ---------------------------------------------------------------------
const SPEICHER = 'hes-zk-rot';
const ziel = (D.sel - 1) * 30;
let rot = ziel;
try { const s = sessionStorage.getItem(SPEICHER); if (s !== null) rot = parseFloat(s); } catch (e) { }
function zeichnen(){ svg.innerHTML = zkSvgInner(D, false, rot); }
function merken(){ try { sessionStorage.setItem(SPEICHER, String(rot)); } catch (e) { } }

const dlt = ((ziel - rot) % 360 + 540) % 360 - 180;
if (reduce || !dlt) { rot = ziel; zeichnen(); merken(); }
else {
  const start = rot, t0 = performance.now(), dur = 950;
  const step = now => { const k = Math.min(1, (now - t0) / dur), e = 1 - Math.pow(1 - k, 3); rot = start + dlt * e; zeichnen(); if (k < 1) requestAnimationFrame(step); else { rot = ziel; merken(); } };
  zeichnen(); requestAnimationFrame(step);
}

// ---------------------------------------------------------------------
// Wenden: Vorderseite (Brett) / Rückseite (Prior und Sekundant)
// ---------------------------------------------------------------------
const flip = document.querySelector('.zk-flip');
function wenden(an){
  if (!flip) return;
  if (!an && flip.classList.contains('flat')) { flip.style.transition = 'none'; flip.classList.remove('flat'); void flip.offsetWidth; flip.style.transition = ''; }
  flip.classList.toggle('on', an);
  const back = flip.querySelector('.zk-back');
  if (back) { if (an) back.removeAttribute('aria-hidden'); else back.setAttribute('aria-hidden', 'true'); }
  if (an) { clearTimeout(wenden.t); wenden.t = setTimeout(() => { if (flip.classList.contains('on')) flip.classList.add('flat'); }, reduce ? 0 : 1350); }
}

const basis = svg.dataset.url;
svg.addEventListener('click', e => {
  const b = e.target.closest('[data-zsel]'); if (!b) return;
  const n = +b.dataset.zsel;
  if (n !== D.sel) { merken(); window.location.href = basis + '?nr=' + n; }
  else if (b.dataset.zrole) wenden(true);
});
document.querySelectorAll('[data-zunflip]').forEach(b => b.addEventListener('click', () => wenden(false)));
document.querySelectorAll('[data-zwenden]').forEach(b => b.addEventListener('click', () => wenden(true)));
document.addEventListener('keydown', e => { if (e.key === 'Escape' && flip && flip.classList.contains('on')) wenden(false); });
// Links zu einer anderen Magie (Pfeile, Chips): Stellung vorher merken
document.querySelectorAll('[data-zgo]').forEach(a => a.addEventListener('click', merken));

// ---------------------------------------------------------------------
// Sperren herunterzählen; ist eine abgelaufen, Seite neu laden
// ---------------------------------------------------------------------
const sperren = Array.from(document.querySelectorAll('[data-zlock]'));
if (sperren.length) {
  const ende = Date.now();
  setInterval(() => {
    let abgelaufen = false;
    sperren.forEach(el => {
      const rest = Math.ceil((+el.dataset.zlock * 1000 - (Date.now() - ende)) / 1000);
      if (rest <= 0) abgelaufen = true;
      else el.textContent = Math.floor(rest / 60) + ':' + String(rest % 60).padStart(2, '0');
    });
    if (abgelaufen) window.location.reload();
  }, 1000);
}

// ---------------------------------------------------------------------
// Erst nachfragen, dann ausführen (JS: mdArm)
// ---------------------------------------------------------------------
document.querySelectorAll('[data-zweistufig]').forEach(knopf => {
  const alt = knopf.textContent;
  knopf.addEventListener('click', e => {
    if (knopf.classList.contains('armed')) return;
    e.preventDefault();
    knopf.classList.add('armed');
    knopf.textContent = knopf.dataset.frage || 'Wirklich?';
    setTimeout(() => { knopf.classList.remove('armed'); knopf.textContent = alt; }, 2800);
  });
});
})();
