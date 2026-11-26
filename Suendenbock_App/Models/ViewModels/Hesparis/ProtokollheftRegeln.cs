using System.Text.RegularExpressions;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Models.ViewModels.Hesparis;

/// <summary>
/// Protokollhefte: feste Inhalte und Regeln aus der hesparis.html (PH_SEC, PH_GROUPS, PH_ACHTET, PH_CHECKS …).
/// Die JS-Namen stehen dabei, damit man sie im Original wiederfindet.
/// </summary>
public static class ProtokollheftRegeln
{
    /// <summary>Ein Abschnitt des Hefts (JS: PH_SEC). Art: text, kopf, ablauf, personen, jeder.</summary>
    public record Abschnitt(string Schluessel, string Titel, int Gruppe, string Art, int Minimum, string Rand,
                            string Einleitung, string[] Fragen, string Schlecht, string Gut, string Tipp);

    public static readonly Abschnitt[] Abschnitte =
    {
        new("kopf", "Kopfdaten", 0, "kopf", 0, "automatisch",
            "Wer, wann, wo und für wen. Das kommt automatisch vom Deckblatt, du musst hier nichts schreiben.",
            new[] { "Steht der Titel des Auftrags auf dem Deckblatt?", "Sind Lehrkraft, Ort und Datum eingetragen?", "Stimmt das Team, und ist der Schriftführer markiert?" },
            "„Ausflug, irgendwann im Herbst“",
            "„Sternwarte Norrvik · Fr. Hirschfelt · 30.–31. Oktober“",
            "Fehlt etwas? Dann über „Deckblatt bearbeiten“ nachtragen."),
        new("auftrag", "Auftrag in eigenen Worten", 0, "text", 40, "eigene Worte!",
            "Erkläre in zwei, drei Sätzen, was ihr herausfinden oder erledigen sollt. Schreib den Zettel nicht ab, sondern so, dass ein Erstklässler es versteht.",
            new[] { "Was genau sollt ihr herausfinden oder tun?", "Für wen macht ihr das, und warum ist es wichtig?", "Woran merkt ihr am Ende, dass der Auftrag erfüllt ist?" },
            "„Wir sollten zur Sternwarte.“",
            "„Wir sollten herausfinden, wie die Sternwarte Polarlichter vorhersagt, und es mit dem Unterricht vergleichen.“",
            "Beginne mit „Wir sollten …“ und nenne das Ziel in einem Satz."),
        new("vorb", "Vorbereitung", 0, "text", 60, "vorher!",
            "Was habt ihr vor dem Aufbruch geplant, nachgelesen und eingepackt?",
            new[] { "Was wusstet ihr schon über den Ort oder das Thema?", "Wo habt ihr nachgelesen, oder wen habt ihr gefragt?", "Was habt ihr mitgenommen, und warum?", "Wer hat welche Aufgabe übernommen?" },
            "„Wir haben uns vorbereitet.“",
            "„Elma hat in der Bibliothek zwei Bücher über Telegrafie gelesen. Levi hat Kreide und Kompass eingepackt.“",
            "Nenne Namen: Wer hat was gemacht?"),
        new("anreise", "Anreise und erster Eindruck", 1, "text", 60, "Sinne!",
            "Wie seid ihr hingekommen, und wie sah es dort aus, als ihr ankamt?",
            new[] { "Wann und womit seid ihr losgefahren?", "Wie lange hat die Reise gedauert?", "Wie sah der Ort aus, wie roch und klang es?", "Wer hat euch empfangen?" },
            "„Es war ein langer Weg und dann waren wir da.“",
            "„Um 8:15 Uhr fuhren wir mit der Postkutsche los. In Ostmark roch es nach Kohlenrauch.“",
            "Benutze mindestens zwei Sinne: sehen, hören, riechen."),
        new("ablauf", "Ablauf mit Uhrzeiten", 1, "ablauf", 6, "Uhrzeit!",
            "Der Ablauf ist das Rückgrat des Protokolls. Wer ihn liest, soll euren Einsatz Schritt für Schritt nachgehen können.",
            new[] { "Jeder Eintrag hat eine Uhrzeit, so genau wie möglich.", "Ein Satz pro Eintrag, in der Vergangenheit geschrieben.", "Nur was passiert ist. Warum, gehört ins Ergebnis.", "Bei jedem Eintrag steht, wer beteiligt war.", "Wichtige Momente sind mit ★ markiert." },
            "„Dann ist ganz viel passiert.“",
            "„21:40 · Kupferspule im Keller entdeckt, summt bei jedem Telegramm. (Rigel, Darius)“",
            "Mindestens 6 Einträge. Eure Notizen von unterwegs könnt ihr direkt übernehmen."),
        new("personen", "Begegnungen", 1, "personen", 1, "„Zitat“",
            "Legt für jede Person, die euch etwas Wichtiges gesagt oder gezeigt hat, einen Steckbrief an.",
            new[] { "Wie heißt die Person, und was macht sie?", "Was hat sie gesagt? Wörtlich in „Anführungszeichen“.", "Was hat sie getan oder gezeigt?", "Wie wichtig war sie für den Auftrag?" },
            "„Ein Mann hat uns was erzählt.“",
            "„Anselm Brauer, Turmwärter: ‚Seit dem Gewitter rauscht es jede Nacht.‘“",
            "Eure Eindrücke („wirkte nervös“) getrennt von dem, was die Person gesagt hat."),
        new("beob", "Beobachtungen und Fakten", 1, "text", 100, "nur Fakten!",
            "Schreibe auf, was ihr gesehen, gehört, gemessen und gespürt habt. Keine Meinung, keine Vermutung. Deutungen gehören ins Ergebnis.",
            new[] { "Was habt ihr mit eigenen Augen gesehen?", "Was habt ihr gehört, gerochen oder gespürt?", "Welche magischen Spuren gab es, und wer hat sie bemerkt?", "Gibt es Zahlen, Maße oder Uhrzeiten?", "Was war anders als erwartet?" },
            "„Die Spule war voll gruselig und bestimmt verflucht.“",
            "„Die Spule summte um 21:40 Uhr. Rigel spürte kalte Restmagie.“",
            "Ein Fakt pro Absatz, und dazuschreiben, wer es beobachtet hat."),
        new("magie", "Eingesetzte Magie", 1, "text", 60, "wer? was?",
            "Welche Zauber, Talente oder Tränke habt ihr eingesetzt, und was ist dabei passiert?",
            new[] { "Wer hat welchen Zauber oder welches Talent eingesetzt?", "Wofür genau?", "Hat es geklappt? Was war das Ergebnis?", "Gab es Nebenwirkungen oder Überraschungen?" },
            "„Wir haben gezaubert und es ging.“",
            "„Rigel setzte Magisches Gespür an der Spule ein und spürte kalte, fremde Restmagie.“",
            "Benutze die richtigen Namen der Zauber und Talente."),
        new("probleme", "Probleme und Lösungen", 2, "text", 60, "Problem → Lösung",
            "Was lief schief oder war schwierig, und wie seid ihr damit umgegangen?",
            new[] { "Welche Schwierigkeiten gab es?", "Wie habt ihr sie gelöst, oder warum nicht?", "Wer hatte die rettende Idee?", "Was würdet ihr nächstes Mal anders machen?" },
            "„Es gab ein paar Probleme.“",
            "„Der Keller war verschlossen. Darius fand den Schlüssel im Logbuch der Telegrafistin.“",
            "Schreibe immer paarweise: zuerst das Problem, dann die Lösung."),
        new("ergebnis", "Ergebnis", 2, "text", 80, "Antwort!",
            "Jetzt dürft ihr deuten. Beantwortet den Auftrag: Was habt ihr herausgefunden?",
            new[] { "Was ist die Antwort auf den Auftrag?", "Welche Beobachtungen belegen das?", "Was ist noch offen oder unklar?", "Ist der Auftrag ganz, teilweise oder nicht erfüllt?" },
            "„Wir haben es herausgefunden.“",
            "„Die Störung kommt von der Kupferspule. Belege: das Summen bei jedem Telegramm und die fremde Restmagie.“",
            "Beziehe dich auf eure ★-Momente aus dem Ablauf."),
        new("gelernt", "Was jeder gelernt hat", 2, "jeder", 20, "jeder selbst!",
            "Jedes Teammitglied schreibt hier selbst ein paar Sätze. Nicht abschreiben, jeder hat etwas anderes erlebt.",
            new[] { "Was weißt du jetzt, was du vorher nicht wusstest?", "Was hat dich überrascht?", "Was nimmst du für den Unterricht mit?" },
            "„Ich habe viel gelernt.“",
            "„Ich wusste nicht, dass Kupfer Resonanz verstärkt. Das erklärt unsere Übung in Resonanzkunde.“",
            "Mindestens 20 Wörter pro Person."),
        new("reflexion", "Reflexion und Ausblick", 2, "text", 60, "ehrlich!",
            "Wie lief die Zusammenarbeit, und was nehmt ihr als Gruppe mit?",
            new[] { "Wie hat die Zusammenarbeit im Team geklappt?", "Wie war es mit eurem Begleiter aus der Klasse?", "Was würdet ihr beim nächsten Einsatz besser machen?", "Was möchtet ihr noch herausfinden?" },
            "„War gut.“",
            "„Kian hat uns mit seinem Stadtwissen sehr geholfen. Nächstes Mal teilen wir die Notizen besser auf.“",
            "Ehrlich sein ist wichtiger als nett sein."),
    };

    /// <summary>Reiter am Heftrand (JS: PH_GROUPS): Name und Farbe.</summary>
    public static readonly (string Name, string Farbe)[] Gruppen =
    {
        ("Anfang", "#2a9d93"), ("Unterwegs", "#c9862b"), ("Auswertung", "#6f52c4"),
    };

    /// <summary>Worauf die Lehrkraft achten kann (JS: PH_ACHTET).</summary>
    public static readonly string[] Achtet =
    {
        "Genaue Uhrzeiten", "Fachbegriffe", "Zitate", "Skizzen", "Ausführlichkeit", "Rechtschreibung",
    };

    /// <summary>Haken vor der Abgabe, die man selbst setzt (JS: PH_CHECKS).</summary>
    public static readonly (string Schluessel, string Text)[] Checks =
    {
        ("vorgelesen", "Einmal laut vorgelesen und Fehler verbessert"),
        ("namen", "Alle Uhrzeiten und Namen geprüft"),
        ("hinweise", "Die Hinweise der Lehrkraft beachtet"),
    };

    /// <summary>Umschlagfarben, reihum nach Heftnummer (JS: PH_COVERS).</summary>
    private static readonly string[] Umschlaege =
        { "#3f5fa8", "#b9662d", "#3c7a52", "#2c7f7a", "#6a4a8c", "#7a3b3b", "#8a6a1f", "#2f6f95" };

    public static string Umschlag(int nummer) => Umschlaege[((Math.Max(1, nummer)) - 1) % Umschlaege.Length];

    /// <summary>Farbe des Stand-Stempels im Inhaltsverzeichnis (JS: PH_STCOL).</summary>
    public static string StatusFarbe(string status) => status switch
    {
        "unterwegs" => "#b9662d",
        "abgegeben" => "#2c7f7a",
        "benotet" => "#6a4a8c",
        _ => "#3f5fa8",
    };

    /// <summary>Noten zum Eintragen (JS: NOTEN).</summary>
    public static readonly string[] Noten = { "S", "A", "B", "C", "D", "E" };

    private static readonly Regex Wort = new(@"\S+", RegexOptions.Compiled);

    /// <summary>Wörter zählen (JS: phWords).</summary>
    public static int Woerter(string? text) => string.IsNullOrWhiteSpace(text) ? 0 : Wort.Matches(text.Trim()).Count;

    /// <summary>Note als Enum, null wenn keine gültige Note.</summary>
    public static Note? NoteAus(string? note) => Enum.TryParse<Note>(note, out var n) && Noten.Contains(note) ? n : null;

    // ---------------------------------------------------------------- Farben der Mitschüler (JS: klStColor / klColor)

    private static readonly string[] ElementFarben =
        { "#8b7ed8", "#4fa3e8", "#cfe4ff", "#f08fbf", "#8fd67a", "#e8894a", "#ffe08a", "#e5606a", "#c9a0ff", "#49d9c9", "#f0703a", "#d9a24a" };

    private static readonly string[] KlassenFarben =
        { "#49d9c9", "#e8894a", "#8fd67a", "#f08fbf", "#ad8ff7", "#e7c66a", "#6fb2f0", "#e58a98" };

    /// <summary>Tintenfarbe eines Mitschülers: Zirkel-Element (1–12), sonst aus der Magie errechnet.</summary>
    public static string MitschuelerFarbe(int? farbe, string? magie)
    {
        if (farbe is >= 1 and <= 12) return ElementFarben[farbe.Value - 1];
        var s = (magie ?? "").Trim().ToLowerInvariant();
        if (s.Length == 0) return "#a89568";
        uint h = 0;
        foreach (char c in s) h = unchecked(h * 31 + c);
        return KlassenFarben[h % (uint)KlassenFarben.Length];
    }

    /// <summary>Nachname zum Sortieren (JS: klSurname): das letzte Wort.</summary>
    public static string Nachname(string name)
    {
        var teile = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return teile.Length > 0 ? teile[^1] : name;
    }

    /// <summary>Datum kurz für Notizen: "3. Sep." (JS: phDate mit gekürztem Monat).</summary>
    public static string DatumKurz(DateOnly d) => HesparisRegeln.DatumKurz(d.ToDateTime(TimeOnly.MinValue));

    /// <summary>Wochentag und Datum für die Tageszeilen im Ablauf, z. B. "Freitag, 30. Oktober".</summary>
    public static string TagZeile(DateOnly d)
    {
        string[] wt = { "Sonntag", "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag" };
        return wt[(int)d.DayOfWeek] + ", " + HesparisRegeln.TagUndMonat(d);
    }

    // ---------------------------------------------------------------- kleine Zeichnungen (JS: phUl, phTick, phRing, phMeterHtml)

    /// <summary>Rote Unterstreichung unter Überschriften.</summary>
    public static string Unterstrich(double verzoegerung = 0) =>
        "<svg class=\"ph-ul\" viewBox=\"0 0 400 10\" preserveAspectRatio=\"none\" aria-hidden=\"true\"><path d=\"M2 6 C 100 1, 260 10, 398 4\"" +
        (verzoegerung > 0 ? " style=\"animation-delay:" + verzoegerung.ToString(System.Globalization.CultureInfo.InvariantCulture) + "s\"" : "") + "/></svg>";

    /// <summary>Kästchen, mit Haken wenn an; frisch = Haken wird gezeichnet.</summary>
    public static string Haken(bool an, bool frisch = false) =>
        "<span class=\"ph-box" + (frisch ? " ph-fresh" : "") + "\">" +
        (an ? "<svg viewBox=\"0 0 28 28\" aria-hidden=\"true\"><path d=\"M4 15 L11 22 L25 3\"/></svg>" : "") + "</span>";

    private static string Ring(int n, int min)
    {
        double p = Math.Min(1, min > 0 ? (double)n / min : 0), c = 100.5;
        var inv = System.Globalization.CultureInfo.InvariantCulture;
        return "<svg viewBox=\"0 0 40 40\" width=\"32\" height=\"32\" aria-hidden=\"true\"><circle cx=\"20\" cy=\"20\" r=\"16\" fill=\"none\" stroke=\"rgba(34,48,106,0.18)\" stroke-width=\"4\"/>" +
               "<circle cx=\"20\" cy=\"20\" r=\"16\" fill=\"none\" stroke=\"" + (p >= 1 ? "#2a8a3a" : "#c9862b") + "\" stroke-width=\"4\" stroke-linecap=\"round\" stroke-dasharray=\"" +
               (p * c).ToString("0.0", inv) + " " + c.ToString(inv) + "\" transform=\"rotate(-90 20 20)\"/></svg>";
    }

    /// <summary>Fortschrittsring mit "12 von 40 Wörtern" bzw. "✓ 41 Wörter, geschafft". Wird beim Tippen im Browser nachgeführt.</summary>
    public static string Zaehler(int n, int min, string einheit = "Wörtern")
    {
        bool fertig = n >= min;
        string text = fertig ? "✓ " + n + " " + einheit.Replace("Wörtern", "Wörter") + ", geschafft" : n + " von " + min + " " + einheit;
        return "<span class=\"ph-meter" + (fertig ? " done" : "") + "\" data-phmeter=\"" + min + "\" data-unit=\"" + einheit + "\">" + Ring(n, min) + "<span>" + text + "</span></span>";
    }
}
