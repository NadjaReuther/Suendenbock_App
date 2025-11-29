========================================
ADVENTSKALENDER - CONTENT STRUKTUR
========================================

Dieser Ordner enthält HTML-Dateien für Simple-Türchen.

BEISPIEL für ein Simple-Türchen (day1.html):
--------------------------------------------
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

WICHTIG:
--------
- Dateiname: day{1-24}.html (z.B. day1.html, day2.html, etc.)
- Bilder sollten in /wwwroot/assets/weihnachten/ gespeichert werden
- HTML kann beliebig gestaltet werden (h1-h6, p, img, ul, ol, etc.)
- CSS wird automatisch aus advent-calendar.css geladen

Einrichtung in der Datenbank:
-----------------------------
INSERT INTO AdventDoors (DayNumber, DoorType, HtmlContentPath)
VALUES (1, 0, '/content/advent/day1.html');

DoorType: 0 = Simple, 1 = Choice, 2 = DirectAudio
