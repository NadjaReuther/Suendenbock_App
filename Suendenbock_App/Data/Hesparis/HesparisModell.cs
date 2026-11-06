using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Data.Hesparis;

/// <summary>
/// Datenbankmodell der Hesparis-Akademie innerhalb des ApplicationDbContext.
/// Die Konfiguration je Bereich steht in Data/Hesparis/Konfiguration/*.cs,
/// die festen Spielinhalte in StammdatenSeed.cs.
///
/// Wichtig: Alle Regeln hier (Textlängen, Löschregeln) gelten NUR für Klassen aus
/// Suendenbock_App.Models.Domain.Hesparis. Die bestehenden Sündenbock-Tabellen bleiben unverändert.
/// </summary>
public static partial class HesparisModell
{
    private const string HesparisNamespace = "Suendenbock_App.Models.Domain.Hesparis";

    /// <summary>Aufruf am Ende von ApplicationDbContext.OnModelCreating.</summary>
    public static void Konfigurieren(ModelBuilder mb)
    {
        KonfiguriereStammdaten(mb);
        KonfiguriereSpielfiguren(mb);
        KonfiguriereKlasse(mb);
        KonfiguriereSchulalltag(mb);
        KonfiguriereHandel(mb);
        KonfiguriereWettbewerbe(mb);
        KonfiguriereGlimrack(mb);
        KonfiguriereZirkelUndArchiv(mb);
        KonfiguriereSchuelerrat(mb);
        KonfiguriereProtokollhefte(mb);

        StammdatenSeed.Einspielen(mb);

        TextUndZahlRegelnAnwenden(mb);
        LoeschRegelnAnwenden(mb);
    }

    private static bool IstHesparis(Type t) => t.Namespace == HesparisNamespace;

    /// <summary>Eigenschaften mit diesen Namen dürfen beliebig lang sein (nvarchar(max)).</summary>
    private static readonly HashSet<string> LangeTexte = new()
    {
        "Text", "Notiz", "Beschreibung", "Wirkung", "Stil", "Ziel", "Auftrag", "Rueckmeldung",
        "Motiv", "SchwarzmarktRaetsel", "Sagt", "Eindruck", "Diagnose", "Behandlung", "Malus",
        "Grund", "Strafe", "Aufgaben", "Geheimnis", "WunderPunkt", "SoPunktetMan", "LadenNotiz",
    };

    /// <summary>
    /// Ersetzt die globalen Konventionen aus dem alten HesparisDbContext
    /// (Texte nvarchar(200), decimal(9,2)), aber nur für Hesparis-Tabellen.
    /// </summary>
    private static void TextUndZahlRegelnAnwenden(ModelBuilder mb)
    {
        foreach (var entity in mb.Model.GetEntityTypes().Where(e => IstHesparis(e.ClrType)))
        {
            var keyProps = entity.FindPrimaryKey()?.Properties.Select(p => p.Name).ToHashSet() ?? new();
            foreach (var prop in entity.GetProperties())
            {
                if (prop.ClrType == typeof(decimal) || prop.ClrType == typeof(decimal?))
                {
                    if (prop.GetPrecision() is null) prop.SetPrecision(9);
                    if (prop.GetScale() is null) prop.SetScale(2);
                    continue;
                }
                if (prop.ClrType != typeof(string)) continue;

                bool schluessel = keyProps.Contains(prop.Name) || prop.IsForeignKey();
                if (!schluessel && (LangeTexte.Contains(prop.Name) || prop.Name.EndsWith("Json")))
                    prop.SetMaxLength(null);
                else if (prop.GetMaxLength() is null)
                    prop.SetMaxLength(200);
            }
        }
    }

    // -----------------------------------------------------------------------
    // Löschregeln
    //
    // SQL Server erlaubt pro Tabelle nur einen kaskadierenden Löschpfad. Darum:
    //  * Kampagne löscht alles, was direkt an ihr hängt.
    //  * Ein Datensatz löscht seine eigenen Unterzeilen (Liste unten).
    //  * Alle anderen Verweise (z. B. Nachsitzen → Spielfigur, Spielfigur → Character) sind NO ACTION.
    // -----------------------------------------------------------------------
    private static readonly HashSet<(Type Kind, Type Eltern)> Kaskaden = new()
    {
        (typeof(FachStufe), typeof(Fach)),
        (typeof(Talent), typeof(Begabung)),
        (typeof(MeisterschaftDefinition), typeof(Talent)),
        (typeof(SonderzauberOption), typeof(Sonderzauber)),

        (typeof(SpielfigurFachStufe), typeof(Spielfigur)),
        (typeof(StufenWahl), typeof(Spielfigur)),
        (typeof(Schulnote), typeof(Spielfigur)),
        (typeof(Pruefungseintrag), typeof(Spielfigur)),
        (typeof(TalentWert), typeof(Spielfigur)),
        (typeof(TalentZiel), typeof(Spielfigur)),
        (typeof(SpielfigurMeisterschaft), typeof(Spielfigur)),
        (typeof(Gegenstand), typeof(Spielfigur)),
        (typeof(Geldbuchung), typeof(Spielfigur)),
        (typeof(Wirkung), typeof(Spielfigur)),
        (typeof(SchuljahrAbschnitt), typeof(Spielfigur)),
        (typeof(Essenskauf), typeof(Spielfigur)),
        (typeof(Essenswirkung), typeof(Spielfigur)),
        (typeof(SonderzauberAufgedeckt), typeof(SonderzauberStand)),

        (typeof(MitschuelerMeinung), typeof(Mitschueler)),
        (typeof(MitschuelerAg), typeof(Mitschueler)),
        (typeof(Beziehung), typeof(Mitschueler)),
        (typeof(LehrerFach), typeof(Lehrer)),
        (typeof(LehrerFakt), typeof(Lehrer)),

        (typeof(AgMitglied), typeof(AgEintrag)),
        (typeof(KlassenarbeitSpielfigur), typeof(Klassenarbeit)),
        (typeof(KlassenarbeitMitschueler), typeof(Klassenarbeit)),
        (typeof(SchuljahrBilanzSpielfigur), typeof(SchuljahrBilanz)),
        (typeof(SchuljahrZeugnisnote), typeof(SchuljahrBilanz)),
        (typeof(ProtokollBeteiligter), typeof(ProtokollEintrag)),
        (typeof(MissionHinweis), typeof(Mission)),
        (typeof(RezeptZutat), typeof(Rezept)),

        (typeof(WettbewerbPlatzierung), typeof(Wettbewerb)),
        (typeof(WettkampfZeugnisEintrag), typeof(WettkampfZeugnis)),
        (typeof(WettkampfNpcWert), typeof(WettkampfNpc)),

        (typeof(GlimTraining), typeof(GlimSpieler)),
        (typeof(GlimNachwuchsTraining), typeof(GlimNachwuchs)),
        (typeof(GlimSpielLogEintrag), typeof(GlimSpiel)),
        (typeof(GlimSpielStatistik), typeof(GlimSpiel)),
        (typeof(GlimKarriereCup), typeof(GlimKarriere)),

        (typeof(ArchivDokumentKategorie), typeof(ArchivDokument)),

        (typeof(RatUnterschrift), typeof(RatAntrag)),
        (typeof(RatGefallen), typeof(RatAntrag)),
        (typeof(RatStimme), typeof(RatAntrag)),

        (typeof(HeftSchwerpunkt), typeof(Protokollheft)),
        (typeof(HeftFehlend), typeof(Protokollheft)),
        (typeof(HeftNotiz), typeof(Protokollheft)),
        (typeof(HeftNotizUebernahme), typeof(HeftNotiz)),
        (typeof(HeftAbschnitt), typeof(Protokollheft)),
        (typeof(HeftFrage), typeof(Protokollheft)),
        (typeof(HeftAblauf), typeof(Protokollheft)),
        (typeof(HeftPerson), typeof(Protokollheft)),
        (typeof(HeftJederEintrag), typeof(Protokollheft)),
        (typeof(HeftCheck), typeof(Protokollheft)),
    };

    private static void LoeschRegelnAnwenden(ModelBuilder mb)
    {
        var fks = mb.Model.GetEntityTypes()
            .Where(e => IstHesparis(e.ClrType))
            .SelectMany(e => e.GetForeignKeys());

        foreach (var fk in fks)
        {
            if (fk.IsOwnership) continue;
            var kind = fk.DeclaringEntityType.ClrType;
            var eltern = fk.PrincipalEntityType.ClrType;

            bool kaskade = eltern == typeof(Kampagne) || Kaskaden.Contains((kind, eltern));
            fk.DeleteBehavior = kaskade ? DeleteBehavior.Cascade : DeleteBehavior.NoAction;
        }
    }
}
