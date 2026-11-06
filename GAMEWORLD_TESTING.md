# Multi-GameWorld System - Test-Anleitung

## ✅ System erfolgreich implementiert!

Das Multi-Tenant GameWorld-System ist vollständig implementiert und bereit zum Testen.

---

## 🎯 Was wurde implementiert?

### 1. **Datenbank-Schema**
- `GameWorlds` - Tabelle für Spielwelten
- `CharacterGameWorlds` - Many-to-Many Zuordnung
- `ApplicationUser.CurrentCharacterId` - Aktiver Character pro User
- Migration erfolgreich durchgeführt ✅

### 2. **Berechtigungssystem**
- **Gott (Admin)**: Vollzugriff auf alle Characters und alle Spielwelten
- **Spieler**: Nur Zugriff auf eigene Characters
- **Character-Ownership Validation**: Spieler können keine fremden Characters bearbeiten
- **Automatische Berechtigungschecks** in BaseController

### 3. **Admin-Interface (nur Gott)**
- `/GameWorld/Index` - Spielwelten-Übersicht
- `/GameWorld/Create` - Neue Spielwelt erstellen
- `/GameWorld/Edit/{id}` - Spielwelt bearbeiten
- `/GameWorld/ManageCharacters/{id}` - Characters zu Welten zuordnen

### 4. **Spieler-Interface**
- **Character-Switcher** in Navigation (automatisch sichtbar)
- Umschalten zwischen Characters/Spielwelten
- Farbcodierung pro Spielwelt

---

## 🧪 TEST-SZENARIEN

### Test 1: Gott-Login und Spielwelt-Verwaltung

**Schritte:**
1. Starte die Anwendung: `dotnet run`
2. Logge dich als **Gott** ein
   - Email: `gott@suendenbock.lore`
   - Password: `Adrijan1618!`
3. Navigiere zu `/GameWorld/Index`

**Erwartetes Ergebnis:**
- ✅ Du siehst zwei Spielwelten:
  - "Sündenbock 1618" (braun)
  - "1810 Season Zwei" (grau/stahl)
- ✅ Anzahl Characters wird angezeigt
- ✅ Buttons: "Bearbeiten" und "Characters"

---

### Test 2: Character-Zuordnung zu Spielwelten

**Schritte:**
1. Als **Gott** eingeloggt
2. Gehe zu `/GameWorld/ManageCharacters/1` (Sündenbock 1618)
3. Prüfe welche Characters zugeordnet sind

**Erwartetes Ergebnis:**
- ✅ **ALLE bestehenden Characters** sind **beiden Welten** zugeordnet
- ✅ "Origin World" = Ja (für Sündenbock 1618)
- ✅ Du kannst Characters hinzufügen/entfernen

**Test: Character entfernen**
1. Wähle einen Character
2. Klicke "Entfernen"
3. Bestätige

**Erwartetes Ergebnis:**
- ✅ Character wird aus dieser Welt entfernt
- ✅ Character bleibt in der anderen Welt

---

### Test 3: Neue Spielwelt erstellen

**Schritte:**
1. Als **Gott** bei `/GameWorld/Index`
2. Klicke "Neue Spielwelt erstellen"
3. Ausfüllen:
   - Name: "Test-Welt"
   - Era: "2025"
   - Beschreibung: "Testbeschreibung"
   - ColorCode: Wähle eine Farbe
   - SortOrder: 3
   - IsActive: ✓
4. "Spielwelt erstellen"

**Erwartetes Ergebnis:**
- ✅ Neue Spielwelt erscheint in der Übersicht
- ✅ Erfolgs-Meldung wird angezeigt
- ✅ Farbe wird korrekt angezeigt

---

### Test 4: Spieler-Login und Character-Switcher

**Vorbereitung:**
1. Erstelle einen Test-User mit Rolle "Spieler"
2. Erstelle 2 Characters und weise sie diesem User zu
3. Weise Character 1 zu "Sündenbock 1618"
4. Weise Character 2 zu "1810 Season Zwei"

**Schritte:**
1. Logge dich als **Spieler** ein
2. Prüfe die Navigation (rechts oben)

**Erwartetes Ergebnis:**
- ✅ **Character-Switcher Dropdown** ist sichtbar
- ✅ Zeigt aktuellen Character + Spielwelt
- ✅ Dropdown zeigt alle Characters des Users
- ✅ Farbcodierung nach Spielwelt

**Test: Character wechseln**
1. Öffne Dropdown
2. Wähle anderen Character
3. Klicke darauf

**Erwartetes Ergebnis:**
- ✅ AJAX-Request wird gesendet
- ✅ Erfolgs-Notification erscheint
- ✅ Seite lädt neu
- ✅ Neuer Character ist aktiv

---

### Test 5: Berechtigungssystem - Spieler darf fremde Characters NICHT bearbeiten

**Schritte:**
1. Als **Spieler** eingeloggt
2. Notiere ID eines fremden Characters (z.B. ID 999)
3. Versuche direkt URL aufzurufen: `/Character/Form/999`

**Erwartetes Ergebnis:**
- ✅ Zugriff wird **verweigert** (Forbid)
- ✅ Fehlermeldung: "Du hast keinen Zugriff auf diesen Character"
- ✅ Redirect zu Player-Index

---

### Test 6: Gott sieht alles

**Schritte:**
1. Als **Gott** eingeloggt
2. Gehe zu `/Character/Overview`
3. Wähle einen beliebigen Character

**Erwartetes Ergebnis:**
- ✅ Gott kann **alle Characters** sehen
- ✅ Gott kann **alle Characters** bearbeiten
- ✅ Keine Berechtigungsfehler

---

### Test 7: Neuen Character nur für "1810 Season Zwei" erstellen

**Schritte:**
1. Als **Gott** eingeloggt
2. Erstelle einen neuen Character (Name: "Test Character 1810")
3. Nach Erstellung:
   - Gehe zu `/GameWorld/ManageCharacters/2` (1810 Season Zwei)
   - Füge den Character hinzu
   - Markiere "Origin World" = Ja
4. **NICHT** zu "Sündenbock 1618" hinzufügen

**Erwartetes Ergebnis:**
- ✅ Character existiert **nur** in "1810 Season Zwei"
- ✅ Character ist **nicht** in "Sündenbock 1618" sichtbar
- ✅ Das ist das gewünschte Verhalten für neue 1810-Characters!

---

### Test 8: Character-Switcher zeigt nur Spieler-Characters

**Vorbereitung:**
- User A hat Character 1 und 2
- User B hat Character 3

**Schritte:**
1. Als **User A** eingeloggt
2. Öffne Character-Switcher

**Erwartetes Ergebnis:**
- ✅ Nur Character 1 und 2 werden angezeigt
- ✅ Character 3 (von User B) ist **nicht** sichtbar

**Schritte:**
1. Als **User B** eingeloggt
2. Öffne Character-Switcher

**Erwartetes Ergebnis:**
- ✅ Nur Character 3 wird angezeigt
- ✅ Character 1 und 2 sind **nicht** sichtbar

---

## 🔒 Sicherheits-Features

### Implementierte Berechtigungschecks:

1. **`HasCharacterAccessAsync(characterId)`**
   - Gott: ✅ Zugriff
   - Eigener Character: ✅ Zugriff
   - Fremder Character: ❌ Kein Zugriff

2. **`ValidateCharacterAccessAsync(characterId)`**
   - Automatischer Forbid() wenn kein Zugriff
   - TempData Error-Message

3. **`RequireCurrentCharacterAsync()`**
   - Erzwingt Character-Auswahl
   - Redirect wenn kein Character aktiv

4. **`SwitchCharacterAsync(characterId)`**
   - Prüft Character-Ownership
   - Verhindert Wechsel zu fremden Characters

---

## 📂 URLs zum Testen

### Gott (Admin):
- `/GameWorld/Index` - Spielwelten-Übersicht
- `/GameWorld/Create` - Neue Spielwelt
- `/GameWorld/Edit/1` - Spielwelt bearbeiten
- `/GameWorld/ManageCharacters/1` - Character-Zuordnung für "Sündenbock 1618"
- `/GameWorld/ManageCharacters/2` - Character-Zuordnung für "1810 Season Zwei"

### Spieler:
- Navigation: Character-Switcher (automatisch sichtbar)
- `/Player/Index` - Player Dashboard
- `/Character/Form/1` - Character bearbeiten (nur eigene!)

---

## ⚠️ Bekannte Limitierungen

1. **Gott hat keinen Character-Switcher**
   - Gott agiert als Admin, nicht als Spieler
   - Gott sieht alle GameWorlds ohne Filter

2. **Character kann in mehreren Welten sein**
   - Das ist gewollt für bestehende Characters
   - Neue 1810-Characters sollten nur dort erstellt werden

3. **Aktuell kein GameWorld-Filter in Messages/Forum**
   - Das kann als nächstes implementiert werden
   - Erst System testen, dann erweitern

---

## 🐛 Fehlersuche

### Problem: "Character-Switcher nicht sichtbar"
**Lösung:**
- Prüfe: Bist du als **Spieler** eingeloggt? (nicht Gott)
- Prüfe: Hat dein User **Characters zugeordnet**?
- Prüfe: `ViewBag.UserCharacters` in Debug-Mode

### Problem: "Zugriff verweigert" beim Character-Bearbeiten
**Lösung:**
- Prüfe: Gehört der Character wirklich dir?
- Prüfe: `UserId` im Character-Model stimmt?
- Gott hat immer Zugriff - teste mit Gott-Account

### Problem: "Migration-Fehler"
**Lösung:**
```bash
dotnet ef database update
```

### Problem: "GameWorlds nicht sichtbar in Admin"
**Lösung:**
- Seeder läuft beim App-Start automatisch
- Prüfe Datenbank: `SELECT * FROM GameWorlds`
- Notfalls App neu starten

---

## 🚀 Nächste Schritte (nach erfolgreichem Test)

1. **GameWorld-Filtering in Features**
   - Messages nur von Characters der gleichen Welt
   - Forum-Posts nach Spielwelt filtern

2. **Themes pro Spielwelt**
   - Unterschiedliche CSS-Styles für 1618 vs. 1810
   - Layout-Anpassungen

3. **Gott-Übersicht über alle Welten**
   - Dashboard mit Statistiken pro Welt
   - Welt-übergreifende Suche

---

## ✅ System-Status

- ✅ Datenbank-Schema migriert
- ✅ Berechtigungssystem implementiert
- ✅ Admin-Interface funktionsfähig
- ✅ Character-Switcher implementiert
- ✅ Build erfolgreich (0 Fehler)
- ⏳ Bereit zum Testen!

---

**Viel Erfolg beim Testen! 🎮**
