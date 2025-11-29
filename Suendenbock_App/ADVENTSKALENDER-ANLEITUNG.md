# Adventskalender - Anleitung

## Übersicht

Der Adventskalender unterstützt 3 verschiedene Türchen-Typen:

1. **Simple** (Typ 0): Einfache HTML-Seite mit Text und Bildern
2. **Choice** (Typ 1): Emma vs Kasimir Auswahl mit Audio nach der Wahl
3. **DirectAudio** (Typ 2): Direkt abspielbares Audio

## Ordnerstruktur

```
wwwroot/
├── content/
│   └── advent/
│       ├── day1.html
│       ├── day2.html
│       └── ...
├── audio/
│   └── advent/
│       ├── direct/
│       │   ├── day3.mp3
│       │   └── ...
│       ├── emma/
│       │   ├── day2.mp3
│       │   └── ...
│       └── kasimir/
│           ├── day2.mp3
│           └── ...
└── assets/
    └── advent/
        ├── emma.png
        ├── kasimir.png
        └── placeholder.png
```

## Türchen in der Datenbank anlegen

### 1. Simple-Türchen (HTML)

```sql
INSERT INTO AdventDoors (DayNumber, DoorType, HtmlContentPath)
VALUES (1, 0, '/content/advent/day1.html');
```

**Beispiel HTML-Datei** (`day1.html`):
```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
</head>
<body>
    <h2>🎄 Willkommen! 🎄</h2>
    <p>Dein Text hier...</p>
    <img src="/assets/weihnachten/bild.jpg" alt="Beschreibung">
</body>
</html>
```

### 2. Choice-Türchen (Emma vs Kasimir)

```sql
INSERT INTO AdventDoors (DayNumber, DoorType, EmmaAudioPath, KasimirAudioPath)
VALUES (2, 1, '/audio/advent/emma/day2.mp3', '/audio/advent/kasimir/day2.mp3');
```

**Wichtig:**
- User sieht Emma und Kasimir Bilder (aus `/assets/advent/emma.png` und `/assets/advent/kasimir.png`)
- Nach der Wahl wird das entsprechende Audio abgespielt
- Die Wahl wird in `UserAdventChoices` gespeichert (0 = Emma, 1 = Kasimir)
- User kann nur einmal wählen!

### 3. DirectAudio-Türchen

```sql
INSERT INTO AdventDoors (DayNumber, DoorType, AudioPath)
VALUES (3, 2, '/audio/advent/direct/day3.mp3');
```

## Testing-Modus (God-Rolle)

User mit der **"God"-Rolle** können alle Türchen unabhängig vom Datum öffnen:

```sql
-- User zur God-Rolle hinzufügen
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES ('user-id-hier', (SELECT Id FROM AspNetRoles WHERE Name = 'God'));
```

Der God-Status wird automatisch beim Laden der Seite geprüft und alle Türchen sind für Gods verfügbar.

## API-Endpunkte

### GetContent
```
GET /AdventCalendar/GetContent?day=1
```

Rückgabe je nach Türchen-Typ:
- **Simple**: `{ doorType: "simple", htmlContent: "..." }`
- **Choice** (nicht gewählt): `{ doorType: "choice", alreadyChosen: false, emmaAudioPath: "...", kasimirAudioPath: "..." }`
- **Choice** (gewählt): `{ doorType: "choice", alreadyChosen: true, choiceIndex: 0, choiceName: "Emma", audioPath: "..." }`
- **DirectAudio**: `{ doorType: "directAudio", audioPath: "..." }`

### SaveChoice
```
POST /AdventCalendar/SaveChoice
Content-Type: application/json

{
  "day": 2,
  "choiceIndex": 0
}
```

Rückgabe:
```json
{
  "success": true,
  "choiceIndex": 0,
  "audioPath": "/audio/advent/emma/day2.mp3",
  "message": "Auswahl gespeichert!"
}
```

### IsGod
```
GET /AdventCalendar/IsGod
```

Rückgabe:
```json
{
  "isGod": true
}
```

## Bilder für Emma & Kasimir

Die Bilder müssen hier gespeichert werden:
- `/wwwroot/assets/advent/emma.png`
- `/wwwroot/assets/advent/kasimir.png`
- `/wwwroot/assets/advent/placeholder.png` (Fallback)

## CSS Klassen

Für Custom-Styling in HTML-Dateien:
- `.advent-simple-content` - Container für Simple-Inhalte
- `.advent-audio-content` - Container für Audio
- `.advent-choice-content` - Container für Auswahlmöglichkeiten
- `.choice-card` - Emma/Kasimir Karten
- `.choice-result` - Ergebnis-Anzeige

## Troubleshooting

### Türchen öffnet sich nicht
- Prüfen ob Datum korrekt ist (nur im Dezember, nur wenn Tag erreicht)
- Als God-User testen
- Browser-Konsole auf Fehler prüfen

### Audio spielt nicht ab
- Pfad in Datenbank prüfen (muss mit "/" beginnen)
- Datei existiert in wwwroot?
- Format: MP3, OGG oder WAV

### HTML wird nicht angezeigt
- HtmlContentPath in DB prüfen
- Datei existiert in wwwroot/content/advent/?
- HTML-Syntax prüfen

### Auswahl kann nicht gespeichert werden
- CSRF-Token vorhanden? (sollte automatisch sein)
- User eingeloggt?
- Schon gewählt?
