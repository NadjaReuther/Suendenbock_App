# Angular Adventskalender - Integration in ASP.NET Core

## 📦 Was wurde erstellt?

Eine vollständige Angular-Anwendung für den Adventskalender mit:

### 1. **Komponenten**
- `AdventCalendarComponent`: Zeigt das 24-Türchen-Grid
- `DoorDetailComponent`: Zeigt den Inhalt eines geöffneten Türchens

### 2. **Services**
- `AdventCalendarService`: Handhabt alle API-Aufrufe zum Backend

### 3. **Models**
- TypeScript-Interfaces für alle API-Requests und Responses

### 4. **Routing**
- `/advent` → Hauptkalender-Ansicht
- `/advent/:day` → Detail-Ansicht für Türchen

## 🚀 Integration in ASP.NET Core

### Option 1: Als eigenständige SPA (empfohlen für Produktion)

#### Schritt 1: Angular-App in ASP.NET hosten

Erstellen Sie einen neuen Controller `AngularController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Suendenbock_App.Controllers
{
    [Authorize]
    public class AngularController : Controller
    {
        /// <summary>
        /// Hostet die Angular Advent Calendar App
        /// </summary>
        [HttpGet]
        public IActionResult AdventCalendar()
        {
            return PhysicalFile(
                Path.Combine(Directory.GetCurrentDirectory(),
                    "ClientApp/dist/client-app/browser/index.html"),
                "text/html"
            );
        }
    }
}
```

#### Schritt 2: Statische Dateien konfigurieren

In `Program.cs` oder `Startup.cs` die Angular-Build-Dateien als statische Dateien bereitstellen:

```csharp
// In Program.cs (nach var app = builder.Build();)

// Statische Dateien für Angular
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "ClientApp/dist/client-app/browser")),
    RequestPath = "/app"
});

// Fallback für Angular Routing
app.MapFallbackToFile("/app/{*path:nonfile}", "ClientApp/dist/client-app/browser/index.html");
```

#### Schritt 3: Navigation hinzufügen

In Ihrer Navbar einen Link einfügen:
```html
<a asp-controller="Angular" asp-action="AdventCalendar" class="nav-link">
    🎄 Adventskalender
</a>
```

### Option 2: Entwicklungsmodus mit Hot Reload

Für die Entwicklung können Sie die Angular-App separat laufen lassen:

```bash
# Terminal 1: ASP.NET Backend
dotnet run

# Terminal 2: Angular Frontend
cd ClientApp
npm start
```

Die Angular-App läuft dann auf `http://localhost:4200` und verbindet sich mit dem ASP.NET Backend.

Passen Sie dafür den `baseUrl` im Service an:
```typescript
// In advent-calendar.service.ts
private readonly baseUrl = 'http://localhost:5000/AdventCalendar';  // Ihre ASP.NET URL
```

## 🔐 Anti-Forgery Token Integration

Das Anti-Forgery Token wird bereits im Service behandelt. Sie müssen es nur in Ihrer Razor-View bereitstellen:

### Erstellen Sie eine View `Views/Angular/AdventCalendar.cshtml`:

```cshtml
@{
    ViewData["Title"] = "Adventskalender";
}

<!-- Anti-Forgery Token für POST Requests -->
@Html.AntiForgeryToken()

<!-- Angular App wird hier geladen -->
<app-root></app-root>

<!-- Angular Scripts -->
<script src="/app/main-WICA3JSE.js" type="module"></script>
<script src="/app/polyfills-FFHMD2TL.js" type="module"></script>
```

**WICHTIG**: Die Script-Namen (main-WICA3JSE.js) ändern sich bei jedem Build.
Sie können sie in `ClientApp/dist/client-app/browser/index.html` finden.

### Automatische Integration (Besser):

Erstellen Sie eine View, die die Angular index.html einbindet:

```cshtml
@{
    ViewData["Title"] = "Adventskalender";
    var indexPath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "ClientApp/dist/client-app/browser/index.html"
    );
    var indexContent = System.IO.File.ReadAllText(indexPath);
}

<!-- Anti-Forgery Token -->
@Html.AntiForgeryToken()

<!-- Angular App Content -->
@Html.Raw(indexContent)
```

## 🎨 Assets (SVG-Bilder) kopieren

Kopieren Sie Ihre Türchen-SVGs nach Angular:

```bash
# Kopieren Sie:
wwwroot/assets/spieler/Weihnachten/*
# Nach:
ClientApp/src/assets/spieler/Weihnachten/
```

Dann neu builden:
```bash
cd ClientApp
npm run build
```

## 📝 Backend-API Anpassungen

Der bestehende `AdventCalendarController` funktioniert bereits!

Nur eine kleine Anpassung für das Anti-Forgery Token bei POST-Requests:

```csharp
[HttpPost]
public async Task<IActionResult> SaveChoice([FromBody] SaveChoiceRequest request)
{
    // Anti-Forgery Token wird automatisch über Header geprüft
    // Wenn Sie den Token aus dem Header lesen wollen:
    var token = Request.Headers["RequestVerificationToken"].FirstOrDefault();

    // Rest bleibt gleich
    // ...
}
```

Falls der Token nicht automatisch validiert wird, fügen Sie dies in Program.cs hinzu:

```csharp
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});
```

## 🔄 Workflow für Updates

### Während der Entwicklung:
1. Änderungen in Angular machen
2. `npm start` nutzen für Hot Reload

### Für Deployment:
1. Angular builden: `cd ClientApp && npm run build`
2. ASP.NET deployen (inkl. ClientApp/dist Ordner)

## ⚡ Performance-Vorteile

Im Vergleich zur Razor-Version:

1. **Initiales Laden**: Ähnlich schnell (HTML + JS Bundle)
2. **Navigation**: **DEUTLICH schneller** - kein Server-Roundtrip
3. **Türchen öffnen**: **Instant** - nur API-Call für Daten
4. **Auswahl speichern**: **Smoother** - keine Page-Reloads
5. **Caching**: Browser cached JS-Bundle

## 📊 Bundle-Größe

- Total: ~80 KB (gzipped)
- Main: ~69 KB
- Polyfills: ~11 KB

**Sehr klein!** Die alte Razor-Version lädt bei jedem Klick das komplette HTML neu.

## 🎓 Lern-Ressourcen

Sie können jetzt lernen über:

1. **TypeScript**: Typsichere Programmierung
2. **RxJS Observables**: Reaktive Programmierung
3. **Angular Components**: Modulare UI-Komponenten
4. **Angular Services**: State Management & API-Calls
5. **Angular Router**: Client-side Routing

## 🐛 Troubleshooting

### Problem: "Anti-Forgery Token nicht gefunden"
**Lösung**: Stellen Sie sicher, dass `@Html.AntiForgeryToken()` in der View ist

### Problem: "Assets nicht gefunden"
**Lösung**: Assets nach `ClientApp/src/assets` kopieren und neu builden

### Problem: "CORS Fehler"
**Lösung**: Im Entwicklungsmodus CORS in ASP.NET aktivieren:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
app.UseCors();
```

## 📁 Projekt-Struktur

```
ClientApp/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   ├── advent-calendar/      # Hauptkalender
│   │   │   └── door-detail/          # Türchen-Details
│   │   ├── models/
│   │   │   └── advent-calendar.models.ts  # TypeScript Interfaces
│   │   ├── services/
│   │   │   └── advent-calendar.service.ts # API Service
│   │   ├── app-routing.module.ts     # Routes
│   │   └── app.module.ts            # App Configuration
│   └── assets/                      # Bilder, SVGs, etc.
└── dist/                           # Build Output (nach npm run build)
```

## ✅ Nächste Schritte

1. ☑️ Assets (SVGs) kopieren
2. ☑️ Angular Controller in ASP.NET erstellen
3. ☑️ View für Angular erstellen
4. ☑️ Statische Dateien konfigurieren
5. ☑️ Testen!

Viel Erfolg! 🚀
