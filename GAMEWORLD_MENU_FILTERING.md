# GameWorld Menu Filtering - Dokumentation

## 🎯 Übersicht

Das Menü passt sich automatisch an die aktive GameWorld an.

---

## 🌍 Wiki-Menü Filterung

### **Sündenbock 1618** (Standard)

```
🌍 Wiki
  ├─ 📚 Glossar (gefiltert nach 1618 Characters)
  ├─ 👹 Bestiarium (keine Filterung - universell)
  ├─ 👤 Charaktere (nur 1618 Characters)
  ├─ 🏰 Gilden (nur 1618 - sichtbar)
  └─ ⚔️ Infanterien (nur 1618 - sichtbar)
```

### **1810 Season Zwei**

```
🌍 Wiki
  ├─ 📚 Glossar (gefiltert nach 1810 Characters)
  ├─ 👹 Bestiarium (keine Filterung - universell)
  └─ 👤 Charaktere (nur 1810 Characters)

  ❌ Gilden - NICHT SICHTBAR
  ❌ Infanterien - NICHT SICHTBAR
```

---

## 🔍 Filterlogik im Detail

### **1. Character Overview (`/Character/Overview`)**

**Code:** `CharacterController.cs - Overview()`

```csharp
// Nicht eingeloggt → "Sündenbock 1618"
// Spieler in 1618 → nur 1618 Characters
// Spieler in 1810 → nur 1810 Characters
// Gott → ALLE Characters
```

**Filterung:**
- Lädt Characters mit `CharacterGameWorlds`
- Filtert nach `effectiveGameWorld.Id`
- Gott-Bypass: Sieht alles

---

### **2. Glossar (`/Glossary/Browse`)**

**Code:** `GlossaryController.cs - Browse()`

**Filterung:**
- EntityType = "Character" → Prüft `CharacterGameWorlds`
- Andere EntityTypes (Rasse, Magie, etc.) → Keine Filterung
- Freie Einträge (kein EntityType) → Keine Filterung

**Beispiel:**
```
Glossar-Eintrag: "Hans Müller" (Character)
  → Prüfe: Ist Hans in aktueller GameWorld?
  → Ja → Anzeigen
  → Nein → Verstecken

Glossar-Eintrag: "Elben" (Rasse)
  → Immer anzeigen (universell)
```

---

### **3. Gilden & Infanterien - Menü-Sichtbarkeit**

**Code:** `_NavShared.cshtml`

```csharp
@if (is1618World)
{
    // Gilden-Link anzeigen
    // Infanterien-Link anzeigen
}
```

**Logik:**
- `is1618World = true` → Links sichtbar
- `is1618World = false` → Links **komplett versteckt**
- Default (wenn keine GameWorld): `is1618World = true`

**Wann ist `is1618World = true`?**
- Nicht eingeloggt → `true` (Standard)
- Spieler in "Sündenbock 1618" → `true`
- Spieler in "1810 Season Zwei" → `false`
- Gott → `true` (Standard)

---

## 🎮 User-Szenarien

### **Szenario 1: Nicht eingeloggt**
```
Aktive Welt: "Sündenbock 1618" (Default)

Wiki-Menü:
  ✅ Glossar (1618 Characters)
  ✅ Bestiarium (alle Monster)
  ✅ Charaktere (1618 Characters)
  ✅ Gilden (sichtbar)
  ✅ Infanterien (sichtbar)
```

---

### **Szenario 2: Spieler nur in 1618**
```
User hat Character "Hans" in "Sündenbock 1618"
Aktive Welt: "Sündenbock 1618"

Wiki-Menü:
  ✅ Glossar (1618 Characters)
  ✅ Bestiarium (alle Monster)
  ✅ Charaktere (1618 Characters)
  ✅ Gilden (sichtbar)
  ✅ Infanterien (sichtbar)
```

---

### **Szenario 3: Spieler nur in 1810**
```
User hat Character "Anna" in "1810 Season Zwei"
Aktive Welt: "1810 Season Zwei"

Wiki-Menü:
  ✅ Glossar (1810 Characters)
  ✅ Bestiarium (alle Monster)
  ✅ Charaktere (1810 Characters)
  ❌ Gilden (NICHT SICHTBAR!)
  ❌ Infanterien (NICHT SICHTBAR!)
```

---

### **Szenario 4: Spieler in BEIDEN Welten**
```
User hat:
  - Character "Hans" in "Sündenbock 1618"
  - Character "Anna" in "1810 Season Zwei"

=== Character "Hans" aktiv ===
Aktive Welt: "Sündenbock 1618" (priorität!)

Wiki-Menü:
  ✅ Glossar (1618 Characters)
  ✅ Charaktere (1618 Characters)
  ✅ Gilden (sichtbar)
  ✅ Infanterien (sichtbar)

=== Wechsel zu Character "Anna" ===
Aktive Welt: "1810 Season Zwei"

Wiki-Menü:
  ✅ Glossar (1810 Characters - GEÄNDERT!)
  ✅ Charaktere (1810 Characters - GEÄNDERT!)
  ❌ Gilden (VERSCHWUNDEN!)
  ❌ Infanterien (VERSCHWUNDEN!)
```

---

### **Szenario 5: Gott**
```
Gott ist eingeloggt
Aktive Welt: "Sündenbock 1618" (Default für Gott)

Wiki-Menü:
  ✅ Glossar (ALLE Einträge - kein Filter!)
  ✅ Bestiarium (alle Monster)
  ✅ Charaktere (ALLE Characters - kein Filter!)
  ✅ Gilden (sichtbar)
  ✅ Infanterien (sichtbar)

WICHTIG: Gott sieht ALLES, unabhängig von GameWorld!
```

---

## 🔧 Technische Details

### **ViewBag-Variablen**

Jeder Controller setzt automatisch (via `BaseController.OnActionExecutionAsync`):

```csharp
ViewBag.IsGott = IsGott();
ViewBag.CurrentCharacter = await GetCurrentCharacterAsync();
ViewBag.CurrentGameWorld = await GetCurrentGameWorldAsync();
ViewBag.EffectiveGameWorld = await GetEffectiveGameWorldAsync();
ViewBag.UserCharacters = await GetUserCharactersAsync();
```

**In Views verfügbar:**
```csharp
@using Suendenbock_App.Models.Domain

var currentGameWorld = ViewBag.CurrentGameWorld as GameWorld;
var is1618World = currentGameWorld?.Name == "Sündenbock 1618";
```

---

### **Prioritäts-Regel für GameWorld-Auswahl**

```csharp
// BaseController.cs - GetCurrentGameWorldAsync()

1. Prüfe: Hat Character CharacterGameWorlds?
   → Nein → return null

2. Prüfe: Ist Character in "Sündenbock 1618"?
   → Ja → return "Sündenbock 1618"

3. Fallback: return erste verfügbare GameWorld
   (z.B. nur in "1810 Season Zwei")
```

**Beispiel:**
```
Character "Peter Wolf" ist in BEIDEN Welten:
  - CharacterGameWorld 1: "Sündenbock 1618"
  - CharacterGameWorld 2: "1810 Season Zwei"

→ GetCurrentGameWorldAsync() gibt zurück: "Sündenbock 1618"
```

---

## 📋 Zusammenfassung

| Feature | 1618 | 1810 | Gott | Nicht eingeloggt |
|---------|------|------|------|------------------|
| **Glossar** | 1618 Characters | 1810 Characters | ALLE | 1618 Characters |
| **Charaktere** | 1618 Characters | 1810 Characters | ALLE | 1618 Characters |
| **Bestiarium** | Alle Monster | Alle Monster | Alle Monster | Alle Monster |
| **Gilden** | ✅ Sichtbar | ❌ Versteckt | ✅ Sichtbar | ✅ Sichtbar |
| **Infanterien** | ✅ Sichtbar | ❌ Versteckt | ✅ Sichtbar | ✅ Sichtbar |

---

## ✅ Implementierte Dateien

- ✅ `Controllers/BaseController.cs` - GameWorld-Logik
- ✅ `Controllers/CharacterController.cs` - Character-Filterung
- ✅ `Controllers/GlossaryController.cs` - Glossar-Filterung
- ✅ `Views/Shared/_NavShared.cshtml` - Menü-Filterung

---

**Stand:** Vollständig implementiert und bereit zum Testen!
