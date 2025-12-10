# Achievement-System - Dokumentation

## 📋 Inhaltsverzeichnis

1. [Überblick](#überblick)
2. [Architektur](#architektur)
3. [Achievement-Struktur](#achievement-struktur)
4. [Ablauf: Achievement erstellen](#ablauf-achievement-erstellen)
5. [Ablauf: Achievement vergeben](#ablauf-achievement-vergeben)
6. [Beispiele](#beispiele)
7. [Verfügbare Achievements](#verfügbare-achievements)
8. [FAQ](#faq)

---

## 🎯 Überblick

Das Achievement-System ermöglicht es, Spieler und Gilden für bestimmte Aktionen und Erfolge zu belohnen.

### Kernfunktionen

- ✅ **Admin-Verwaltung** - Gott kann Achievements über Web-Interface erstellen/bearbeiten
- ✅ **Automatische Vergabe** - System prüft Bedingungen und vergibt Achievements automatisch
- ✅ **Zwei Scopes** - User-Achievements (für einzelne Spieler) und Guild-Achievements (für Gilden)
- ✅ **Kategorisierung** - Achievements sind in Kategorien gruppiert
- ✅ **Benachrichtigungen** - Toast-Popups beim Freischalten
- ✅ **Öffentliche Übersicht** - Alle können Fortschritte sehen
- ✅ **Persönliche Ansicht** - Spieler sehen ihre eigenen Achievements

---

## 🏗️ Architektur

### Wichtige Dateien

```
Suendenbock_App/
├── Controllers/
│   ├── AdminController.cs          # Achievement-Verwaltung (CRUD)
│   ├── HomeController.cs            # Öffentliche Achievement-Übersicht
│   ├── PlayerController.cs          # Persönliche Achievement-Ansicht
│   └── CharacterController.cs       # Ruft Achievement-Checks auf
│
├── Services/
│   ├── IAchievementService.cs       # Interface
│   └── AchievementService.cs        # ⭐ Kern-Logik für Achievement-Vergabe
│
├── Data/
│   └── AchievementSeeder.cs         # Initial-Achievements beim ersten Start
│
├── Models/
│   └── Domain/
│       ├── Achievement.cs           # Achievement-Model
│       ├── UserAchievement.cs       # Verknüpfung User ↔ Achievement
│       └── GuildAchievement.cs      # Verknüpfung Guild ↔ Achievement
│
└── Views/
    ├── Admin/
    │   ├── ManageAchievements.cshtml    # Übersicht für Gott
    │   ├── CreateAchievement.cshtml     # Formular: Neues Achievement
    │   └── EditAchievement.cshtml       # Formular: Achievement bearbeiten
    ├── Home/
    │   ├── Index.cshtml                 # Startseite mit Achievement-Widget
    │   └── AchievementOverview.cshtml   # Öffentliche Übersicht
    └── Player/
        └── Achievements.cshtml           # Persönliche Achievement-Ansicht
```

### Datenbank-Tabellen

- **Achievements** - Alle verfügbaren Achievements
- **UserAchievements** - Welcher User hat welches Achievement wann freigeschaltet
- **GuildAchievements** - Welche Gilde hat welches Achievement wann freigeschaltet

---

## 📊 Achievement-Struktur

Ein Achievement besteht aus folgenden Feldern:

### Pflichtfelder

| Feld | Typ | Beispiel | Beschreibung |
|------|-----|----------|--------------|
| **Id** | int | 1 | Automatische Datenbank-ID |
| **Key** | string | `character_basics_filled` | Eindeutiger technischer Bezeichner (nur Kleinbuchstaben + Unterstriche) |
| **Name** | string | "Grundlagen gelegt" | Anzeigename für Spieler |
| **Description** | string | "Basis-Felder des Charakterbogens ausgefüllt" | Beschreibung des Achievements |
| **Icon** | string | 📝 | Emoji-Icon |
| **Category** | enum | CharacterCompletion | Kategorie (siehe unten) |
| **Scope** | enum | User / Guild | Für wen ist das Achievement? |
| **Points** | int | 10 | Wie viele Punkte ist es wert? |

### Optionale Felder

| Feld | Typ | Beispiel | Beschreibung |
|------|-----|----------|--------------|
| **RequiredCount** | int? | 10 | Ziel-Anzahl für gestaffelte Achievements |
| **IsSecret** | bool | false | Versteckt bis zum Freischalten? |
| **EntityType** | string? | "Monstertyp" | Für spezifische Entitäten (fortgeschritten) |
| **EntityId** | int? | 5 | ID der spezifischen Entität (fortgeschritten) |

### Kategorien (AchievementCategory)

```csharp
public enum AchievementCategory
{
    CharacterCompletion,    // Charakterbogen-Vervollständigung
    CharacterRelations,     // Familie & Beziehungen
    GuildSize,              // Gilden-Größe
    Bestiary,               // Bestiarium
    BestiaryType,           // Typ-spezifische Monster
    AdventCalendar,         // Weihnachtsabenteuer
    Knowledge               // Wissens-Achievements
}
```

### Scope (AchievementScope)

```csharp
public enum AchievementScope
{
    User,   // Für einzelne Spieler
    Guild   // Für Gilden
}
```

---

## 🔨 Ablauf: Achievement erstellen

### Option 1: Über Admin-Interface (für Gott-Rolle)

#### Schritt 1: Zur Verwaltung navigieren
```
URL: /Admin/ManageAchievements
```

#### Schritt 2: "Neues Achievement erstellen" klicken

#### Schritt 3: Formular ausfüllen

**Beispiel:**
```
Key:              world_traveler
Name:             Weltenbummler
Description:      5 verschiedene Länder bereist
Icon:             🌍
Category:         Knowledge
Scope:            User
Points:           30
RequiredCount:    5
IsSecret:         ☐ (nicht aktiviert)
```

#### Schritt 4: Speichern

- Achievement wird in Datenbank gespeichert
- **WICHTIG:** Achievement wird noch NICHT automatisch vergeben!
- Vergabe-Logik muss separat programmiert werden (siehe nächster Abschnitt)

### Option 2: Initial-Seeding (beim ersten App-Start)

Achievements können auch in `AchievementSeeder.cs` vordefiniert werden:

```csharp
new Achievement
{
    Key = "world_traveler",
    Name = "Weltenbummler",
    Description = "5 verschiedene Länder bereist",
    Icon = "🌍",
    Category = AchievementCategory.Knowledge,
    Scope = AchievementScope.User,
    Points = 30,
    RequiredCount = 5
}
```

Diese werden beim ersten Start automatisch in die DB eingefügt.

---

## ⚙️ Ablauf: Achievement vergeben

### Schritt 1: Vergabe-Logik in AchievementService.cs erstellen

**Neue Methode hinzufügen:**

```csharp
public async Task CheckTravelAchievements(string userId)
{
    // 1. Daten aus Datenbank laden
    var visitedCountries = await _context.CharacterTravels
        .Where(ct => ct.Character.UserId == userId)
        .Select(ct => ct.CountryId)
        .Distinct()
        .CountAsync();

    // 2. Bedingung prüfen und Achievement vergeben
    await CheckAndAwardAchievement(
        userId,              // Wer bekommt es?
        "world_traveler",    // Welches Achievement? (Key!)
        visitedCountries >= 5 // Bedingung erfüllt?
    );
}
```

### Schritt 2: Service im Controller aufrufen

**Im passenden Controller** (z.B. `TravelController.cs`):

```csharp
public class TravelController : Controller
{
    private readonly IAchievementService _achievementService;

    // Dependency Injection im Constructor
    public TravelController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    [HttpPost]
    public async Task<IActionResult> VisitCountry(int countryId)
    {
        // ... Reise speichern ...
        await _context.SaveChangesAsync();

        // Achievement prüfen
        var userId = _userManager.GetUserId(User);
        await _achievementService.CheckTravelAchievements(userId);

        // Benachrichtigung vorbereiten
        await StoreNewAchievementsInTempData(userId);

        return RedirectToAction("Index");
    }

    // Helper-Methode für Benachrichtigung
    private async Task StoreNewAchievementsInTempData(string userId)
    {
        var newAchievements = await _achievementService
            .GetNewlyUnlockedAchievements(userId);

        if (newAchievements != null && newAchievements.Any())
        {
            var achievementsData = newAchievements.Select(a => new
            {
                a.Name,
                a.Description,
                a.Icon,
                a.Points
            }).ToList();

            TempData["NewAchievements"] =
                System.Text.Json.JsonSerializer.Serialize(achievementsData);
        }
    }
}
```

### Schritt 3: Fertig!

- Das Achievement wird automatisch vergeben, wenn die Bedingung erfüllt ist
- Spieler sieht Toast-Benachrichtigung
- Achievement erscheint in persönlicher Übersicht

---

## 💡 Beispiele

### Beispiel 1: Charakterbogen-Achievement (bereits implementiert)

**Achievement:**
```
Key: character_50_percent
Name: Detailverliebt
Beschreibung: 50% des Charakterbogens ausgefüllt
```

**Vergabe-Logik in AchievementService.cs:**

```csharp
public async Task CheckCharacterAchievements(string userId)
{
    // Character laden
    var character = await _context.Characters
        .Include(c => c.Details)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    if (character == null) return;

    // Vollständigkeit berechnen
    var completionPercentage = CalculateCharacterCompletionPercentage(character);

    // Achievement vergeben bei 50%
    await CheckAndAwardAchievement(
        userId,
        "character_50_percent",
        completionPercentage >= 50
    );
}

private int CalculateCharacterCompletionPercentage(Character character)
{
    int totalFields = 0;
    int filledFields = 0;

    // Basis-Felder zählen
    totalFields += 5;
    filledFields += 5;

    // Optionale Felder prüfen
    if (!string.IsNullOrWhiteSpace(character.Geburtsdatum)) filledFields++;
    totalFields++;

    // ... weitere Felder ...

    return (int)((filledFields * 100.0) / totalFields);
}
```

**Aufruf in CharacterController.cs:**

```csharp
[HttpPost]
public async Task<IActionResult> Step2(CharacterFormStep2 model)
{
    // ... Character speichern ...

    // Achievement prüfen
    var userId = _userManager.GetUserId(User);
    await _achievementService.CheckCharacterAchievements(userId);
    await StoreNewAchievementsInTempData(userId);

    return RedirectToAction("Step3");
}
```

---

### Beispiel 2: Gilden-Achievement (bereits implementiert)

**Achievement:**
```
Key: guild_10_members
Name: Wachsende Gemeinschaft
Beschreibung: Gilde hat 10 Mitglieder erreicht
```

**Vergabe-Logik in AchievementService.cs:**

```csharp
public async Task CheckGuildAchievements(int guildId)
{
    var guild = await _context.Guilds
        .FirstOrDefaultAsync(g => g.Id == guildId);

    if (guild == null) return;

    // ⚠️ Nur für Wolkenbruch-Gilde prüfen!
    if (guild.Name != "Wolkenbruch") return;

    // Mitglieder zählen
    var memberCount = await _context.CharacterAffiliations
        .CountAsync(ca => ca.GuildId == guildId);

    // Achievement vergeben
    await CheckAndAwardGuildAchievement(
        guildId,
        "guild_10_members",
        memberCount >= 10
    );
}
```

**Wichtig:** Nur die Gilde "Wolkenbruch" erhält Gilden-Achievements!

---

### Beispiel 3: Neues Achievement erstellen (Hypothetisch)

**Szenario:** Achievement für das Sammeln von magischen Artefakten

#### 1. Achievement über Admin-Interface erstellen

```
Key:              artifact_collector
Name:             Artefakt-Sammler
Description:      10 magische Artefakte gesammelt
Icon:             💎
Category:         Knowledge
Scope:            User
Points:           50
RequiredCount:    10
```

#### 2. Vergabe-Logik programmieren

**In AchievementService.cs:**

```csharp
public async Task CheckArtifactAchievements(string userId)
{
    // Artefakte des Users zählen
    var artifactCount = await _context.CharacterArtifacts
        .Where(ca => ca.Character.UserId == userId)
        .CountAsync();

    // Achievements vergeben
    await CheckAndAwardAchievement(userId, "artifact_collector", artifactCount >= 10);
    await CheckAndAwardAchievement(userId, "artifact_master", artifactCount >= 25);
}
```

**Im IAchievementService.cs Interface hinzufügen:**

```csharp
Task CheckArtifactAchievements(string userId);
```

#### 3. Im Controller aufrufen

**In ArtifactController.cs:**

```csharp
[HttpPost]
public async Task<IActionResult> CollectArtifact(int artifactId)
{
    // Artefakt dem Character zuweisen
    var userId = _userManager.GetUserId(User);
    var character = await _context.Characters
        .FirstOrDefaultAsync(c => c.UserId == userId);

    var characterArtifact = new CharacterArtifact
    {
        CharacterId = character.Id,
        ArtifactId = artifactId,
        CollectedAt = DateTime.Now
    };

    _context.CharacterArtifacts.Add(characterArtifact);
    await _context.SaveChangesAsync();

    // Achievement prüfen
    await _achievementService.CheckArtifactAchievements(userId);
    await StoreNewAchievementsInTempData(userId);

    return RedirectToAction("MyArtifacts");
}
```

---

## 🏆 Verfügbare Achievements

### Charakterbogen-Vervollständigung

| Key | Name | Beschreibung | Punkte |
|-----|------|--------------|--------|
| `character_basics_filled` | Grundlagen gelegt | Basis-Felder ausgefüllt (20%) | 10 |
| `character_50_percent` | Detailverliebt | 50% des Bogens ausgefüllt | 25 |
| `character_80_percent` | Perfektionist | 80% des Bogens ausgefüllt | 50 |

### Familie & Beziehungen

| Key | Name | Beschreibung | Punkte |
|-----|------|--------------|--------|
| `family_one_relation` | Familienbande | 1 Beziehung eingetragen | 15 |
| `family_complete` | Stammbaum | Alle 3 Beziehungen (Vater, Mutter, Partner) | 30 |

### Gilden-Größe (nur Wolkenbruch!)

| Key | Name | Beschreibung | Punkte |
|-----|------|--------------|--------|
| `guild_10_members` | Wachsende Gemeinschaft | 10 Mitglieder | 25 |
| `guild_25_members` | Große Gemeinschaft | 25 Mitglieder | 50 |
| `guild_50_members` | Sehr große Gilde | 50 Mitglieder | 100 |

### Bestiarium (nur Wolkenbruch!)

| Key | Name | Beschreibung | Punkte |
|-----|------|--------------|--------|
| `bestiary_first_encounter` | Erste Begegnung | 1 Monster freigeschaltet | 10 |
| `bestiary_10_monsters` | Monsterjäger | 10 Monster freigeschaltet | 30 |
| `bestiary_25_monsters` | Bestienkenner | 25 Monster freigeschaltet | 75 |
| `bestiary_all_monsters` | Meister des Bestiariums | Alle Monster freigeschaltet | 200 |

### Adventskalender

| Key | Name | Beschreibung | Punkte |
|-----|------|--------------|--------|
| `advent_first_door` | Türchen-Öffner | 1 Türchen geöffnet | 5 |
| `advent_12_doors` | Fleißiger Adventskalender-Leser | 12 Türchen geöffnet | 25 |
| `advent_all_doors` | Advent-Enthusiast | Alle 24 Türchen geöffnet | 50 |

---

## ❓ FAQ

### F: Kann ich ein Achievement nachträglich bearbeiten?

**A:** Ja! Als Gott kannst du unter `/Admin/ManageAchievements` jedes Achievement bearbeiten:
- Name, Beschreibung, Icon ändern
- Punkte anpassen
- Kategorie wechseln

**ACHTUNG:** Den **Key** solltest du NICHT ändern, da dieser im Code verwendet wird!

---

### F: Kann ich ein Achievement löschen?

**A:** Ja, aber nur wenn es noch nicht vergeben wurde:
- Noch nicht vergebene Achievements können gelöscht werden
- Bereits vergebene Achievements werden durch Safety-Check geschützt
- Fehlermeldung erscheint, falls Achievement bereits vergeben wurde

---

### F: Muss ich für jedes neue Achievement Code schreiben?

**A:** Ja! Das Admin-Interface erstellt nur den Datenbank-Eintrag. Die Vergabe-Logik (wann wird es vergeben?) muss immer programmiert werden.

**Workflow:**
1. Gott erstellt Achievement über Admin-Interface
2. Entwickler implementiert Vergabe-Logik in `AchievementService.cs`
3. Entwickler ruft die Check-Methode im passenden Controller auf

---

### F: Warum werden Gilden-Achievements nur für "Wolkenbruch" vergeben?

**A:** Das ist eine bewusste Einschränkung in `AchievementService.cs` (Zeile 57):

```csharp
if (guild.Name != "Wolkenbruch") return;
```

**Grund:** Nur die Haupt-Spieler-Gilde soll Achievements tracken.

**Ändern:** Entferne diese Zeile, um alle Gilden zu berücksichtigen, oder ändere den Namen.

---

### F: Wie kann ich die Vergabe-Logik testen?

**A:** Manuell im Code ausführen:

```csharp
// Im Controller
var userId = _userManager.GetUserId(User);
await _achievementService.CheckCharacterAchievements(userId);
```

Oder über Unit-Tests (nicht implementiert).

---

### F: Können Achievements mehrfach vergeben werden?

**A:** Nein! Die Methode `CheckAndAwardAchievement` prüft automatisch:

```csharp
var existingUserAchievement = await _context.UserAchievements
    .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievement.Id);

if (existingUserAchievement != null) return false; // ← Bereits vergeben!
```

Jedes Achievement wird nur **einmal** pro User/Gilde vergeben.

---

### F: Wo sehe ich, welche Achievements bereits vergeben wurden?

**A:** Mehrere Orte:

1. **Admin-Übersicht:** `/Home/AchievementOverview`
   - Zeigt alle Spieler/Gilden mit Achievement-Counts

2. **Persönliche Ansicht:** `/Player/Achievements`
   - Zeigt eigene freigeschaltete + gesperrte Achievements

3. **Datenbank:**
   - Tabelle `UserAchievements` für User-Achievements
   - Tabelle `GuildAchievements` für Guild-Achievements

---

### F: Wie funktionieren die Toast-Benachrichtigungen?

**A:** Über TempData:

1. Controller speichert Achievement-Daten in `TempData["NewAchievements"]`
2. Partial View `_AchievementNotification.cshtml` liest TempData
3. JavaScript zeigt Toast-Popup an
4. Auto-Dismiss nach 8 Sekunden

**Code:**
```csharp
TempData["NewAchievements"] = JsonSerializer.Serialize(achievementsData);
```

---

### F: Kann ich die Punkte-Werte nachträglich ändern?

**A:** Ja, über `/Admin/ManageAchievements` → "Bearbeiten":
- Neue Punkte werden sofort gespeichert
- Bereits vergebene Achievements behalten die neuen Punkte
- Punktestände werden dynamisch berechnet

---

### F: Was passiert, wenn ich ein Achievement mit falschem Key erstelle?

**A:** Das Achievement wird in der DB gespeichert, aber NIE vergeben:
- Der Code sucht nach dem exakten Key
- Falsche Schreibweise → kein Match → keine Vergabe
- Lösung: Key nachträglich korrigieren oder neues Achievement mit korrektem Key erstellen

---

## 🚀 Nächste Schritte

### Neue Achievements hinzufügen?

1. Überlege dir die Bedingung (z.B. "5 Duelle gewonnen")
2. Erstelle das Achievement über Admin-Interface
3. Implementiere die Check-Methode in `AchievementService.cs`
4. Rufe die Methode im passenden Controller auf
5. Teste die Vergabe

### Fragen?

Bei Problemen oder Unklarheiten:
- Dokumentation erneut lesen
- Code in `AchievementService.cs` anschauen
- Beispiele als Vorlage verwenden

---

**Viel Erfolg mit dem Achievement-System! 🎉**
