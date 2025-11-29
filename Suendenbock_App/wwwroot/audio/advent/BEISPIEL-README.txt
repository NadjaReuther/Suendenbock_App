========================================
ADVENTSKALENDER - AUDIO STRUKTUR
========================================

Dieser Ordner enthält Audio-Dateien für die Türchen.

ORDNER-STRUKTUR:
----------------
/audio/advent/
  ├── direct/          → Für DirectAudio-Türchen (Typ 2)
  │   ├── day3.mp3
  │   └── day7.mp3
  ├── emma/            → Emma-Audios für Choice-Türchen (Typ 1)
  │   ├── day2.mp3
  │   ├── day5.mp3
  │   └── day9.mp3
  └── kasimir/         → Kasimir-Audios für Choice-Türchen (Typ 1)
      ├── day2.mp3
      ├── day5.mp3
      └── day9.mp3

AUDIO-FORMATE:
--------------
- MP3 (empfohlen)
- OGG
- WAV

BEISPIEL-EINRICHTUNG:
---------------------

1) DirectAudio-Türchen (Typ 2):
   INSERT INTO AdventDoors (DayNumber, DoorType, AudioPath)
   VALUES (3, 2, '/audio/advent/direct/day3.mp3');

2) Choice-Türchen (Typ 1) - Emma vs Kasimir:
   INSERT INTO AdventDoors (DayNumber, DoorType, EmmaAudioPath, KasimirAudioPath)
   VALUES (2, 1, '/audio/advent/emma/day2.mp3', '/audio/advent/kasimir/day2.mp3');

WICHTIG:
--------
- Audiodateien sollten nicht zu groß sein (max. 10MB empfohlen)
- Dateinamen sollten eindeutig sein (z.B. day2.mp3, day5.mp3)
- Pfade in der DB müssen mit "/" beginnen (nicht "\")
