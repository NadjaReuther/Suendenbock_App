using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Data;

/// <summary>
/// Hesparis-Teil des ApplicationDbContext: alle Tabellen der Akademie-Welt.
/// Die Konfiguration steht in Data/Hesparis/HesparisModell.cs.
/// </summary>
public partial class ApplicationDbContext
{
    // ---- Hesparis: Stammdaten (für alle Kampagnen gleich) ----
    public DbSet<Fach> Faecher => Set<Fach>();
    public DbSet<FachStufe> FachStufen => Set<FachStufe>();
    public DbSet<Begabung> Begabungen => Set<Begabung>();
    public DbSet<Talent> Talente => Set<Talent>();
    public DbSet<MeisterschaftDefinition> MeisterschaftDefinitionen => Set<MeisterschaftDefinition>();
    public DbSet<Ag> Ags => Set<Ag>();
    public DbSet<AkademieTeam> AkademieTeams => Set<AkademieTeam>();
    public DbSet<Sonderzauber> Sonderzauber => Set<Sonderzauber>();
    public DbSet<SonderzauberOption> SonderzauberOptionen => Set<SonderzauberOption>();
    public DbSet<Gericht> Gerichte => Set<Gericht>();

    // ---- Hesparis: Kampagne und Spielfiguren ----
    public DbSet<Kampagne> Kampagnen => Set<Kampagne>();
    public DbSet<Spielfigur> Spielfiguren => Set<Spielfigur>();
    public DbSet<SpielfigurFachStufe> SpielfigurFachStufen => Set<SpielfigurFachStufe>();
    public DbSet<StufenWahl> StufenWahlen => Set<StufenWahl>();
    public DbSet<Schulnote> Schulnoten => Set<Schulnote>();
    public DbSet<Pruefungseintrag> Pruefungseintraege => Set<Pruefungseintrag>();
    public DbSet<TalentWert> TalentWerte => Set<TalentWert>();
    public DbSet<TalentZiel> TalentZiele => Set<TalentZiel>();
    public DbSet<SpielfigurMeisterschaft> SpielfigurMeisterschaften => Set<SpielfigurMeisterschaft>();
    public DbSet<Gegenstand> Gegenstaende => Set<Gegenstand>();
    public DbSet<Geldbuchung> Geldbuchungen => Set<Geldbuchung>();
    public DbSet<Wirkung> Wirkungen => Set<Wirkung>();
    public DbSet<SonderzauberStand> SonderzauberStaende => Set<SonderzauberStand>();
    public DbSet<SonderzauberAufgedeckt> SonderzauberAufgedeckt => Set<SonderzauberAufgedeckt>();

    // ---- Hesparis: Klasse und Lehrkräfte ----
    public DbSet<Mitschueler> Mitschueler => Set<Mitschueler>();
    public DbSet<MitschuelerMeinung> MitschuelerMeinungen => Set<MitschuelerMeinung>();
    public DbSet<MitschuelerAg> MitschuelerAgs => Set<MitschuelerAg>();
    public DbSet<Beziehung> Beziehungen => Set<Beziehung>();
    public DbSet<Lehrer> Lehrer => Set<Lehrer>();
    public DbSet<LehrerFach> LehrerFaecher => Set<LehrerFach>();
    public DbSet<LehrerFakt> LehrerFakten => Set<LehrerFakt>();

    // ---- Hesparis: Schulalltag ----
    public DbSet<KalenderTermin> KalenderTermine => Set<KalenderTermin>();
    public DbSet<AgEintrag> AgEintraege => Set<AgEintrag>();
    public DbSet<AgMitglied> AgMitglieder => Set<AgMitglied>();
    public DbSet<Klassenarbeit> Klassenarbeiten => Set<Klassenarbeit>();
    public DbSet<KlassenarbeitSpielfigur> KlassenarbeitSpielfiguren => Set<KlassenarbeitSpielfigur>();
    public DbSet<KlassenarbeitMitschueler> KlassenarbeitMitschueler => Set<KlassenarbeitMitschueler>();
    public DbSet<Nachsitzen> Nachsitzen => Set<Nachsitzen>();
    public DbSet<SchuljahrAbschnitt> SchuljahrAbschnitte => Set<SchuljahrAbschnitt>();
    public DbSet<SchuljahrBilanz> SchuljahrBilanzen => Set<SchuljahrBilanz>();
    public DbSet<SchuljahrBilanzSpielfigur> SchuljahrBilanzSpielfiguren => Set<SchuljahrBilanzSpielfigur>();
    public DbSet<SchuljahrZeugnisnote> SchuljahrZeugnisnoten => Set<SchuljahrZeugnisnote>();
    public DbSet<KartenNotiz> KartenNotizen => Set<KartenNotiz>();
    public DbSet<KartenZeit> KartenZeiten => Set<KartenZeit>();
    public DbSet<SpeiseplanEintrag> SpeiseplanEintraege => Set<SpeiseplanEintrag>();
    public DbSet<Essenskauf> Essenskaeufe => Set<Essenskauf>();
    public DbSet<Essenswirkung> Essenswirkungen => Set<Essenswirkung>();
    public DbSet<ProtokollEintrag> ProtokollEintraege => Set<ProtokollEintrag>();
    public DbSet<ProtokollBeteiligter> ProtokollBeteiligte => Set<ProtokollBeteiligter>();
    public DbSet<Aufgabe> Aufgaben => Set<Aufgabe>();
    public DbSet<Mission> Missionen => Set<Mission>();
    public DbSet<MissionHinweis> MissionHinweise => Set<MissionHinweis>();

    // ---- Hesparis: Handel, Brauen, Krankenstation ----
    public DbSet<LadenArtikel> LadenArtikel => Set<LadenArtikel>();
    public DbSet<SchwarzmarktWare> SchwarzmarktWaren => Set<SchwarzmarktWare>();
    public DbSet<SchwarzmarktVerkauf> SchwarzmarktVerkaeufe => Set<SchwarzmarktVerkauf>();
    public DbSet<Rezept> Rezepte => Set<Rezept>();
    public DbSet<RezeptZutat> RezeptZutaten => Set<RezeptZutat>();
    public DbSet<Braueintrag> Braueintraege => Set<Braueintrag>();
    public DbSet<Waffenwurf> Waffenwuerfe => Set<Waffenwurf>();
    public DbSet<Krankheitsfall> Krankheitsfaelle => Set<Krankheitsfall>();
    public DbSet<KrankenGlimGemeldet> KrankenGlimGemeldet => Set<KrankenGlimGemeldet>();

    // ---- Hesparis: Wettbewerbe ----
    public DbSet<Wettbewerb> Wettbewerbe => Set<Wettbewerb>();
    public DbSet<WettbewerbPlatzierung> WettbewerbPlatzierungen => Set<WettbewerbPlatzierung>();
    public DbSet<TeamNotiz> TeamNotizen => Set<TeamNotiz>();
    public DbSet<WettkampfStand> WettkampfStaende => Set<WettkampfStand>();
    public DbSet<WettkampfTeilnehmer> WettkampfTeilnehmer => Set<WettkampfTeilnehmer>();
    public DbSet<DuellClubBilanz> DuellClubBilanzen => Set<DuellClubBilanz>();
    public DbSet<WettkampfErgebnis> WettkampfErgebnisse => Set<WettkampfErgebnis>();
    public DbSet<WettkampfZeugnis> WettkampfZeugnisse => Set<WettkampfZeugnis>();
    public DbSet<WettkampfZeugnisEintrag> WettkampfZeugnisEintraege => Set<WettkampfZeugnisEintrag>();
    public DbSet<WettkampfNpc> WettkampfNpcs => Set<WettkampfNpc>();
    public DbSet<WettkampfNpcWert> WettkampfNpcWerte => Set<WettkampfNpcWert>();

    // ---- Hesparis: Glimrack ----
    public DbSet<GlimSpieler> GlimSpieler => Set<GlimSpieler>();
    public DbSet<GlimNachwuchs> GlimNachwuchs => Set<GlimNachwuchs>();
    public DbSet<GlimTraining> GlimTraining => Set<GlimTraining>();
    public DbSet<GlimNachwuchsTraining> GlimNachwuchsTraining => Set<GlimNachwuchsTraining>();
    public DbSet<GlimSpiel> GlimSpiele => Set<GlimSpiel>();
    public DbSet<GlimSpielLogEintrag> GlimSpielLog => Set<GlimSpielLogEintrag>();
    public DbSet<GlimSpielStatistik> GlimSpielStatistiken => Set<GlimSpielStatistik>();
    public DbSet<GlimCupArchiv> GlimCupArchiv => Set<GlimCupArchiv>();
    public DbSet<GlimTeamStaerke> GlimTeamStaerken => Set<GlimTeamStaerke>();
    public DbSet<GlimSaisonErgebnis> GlimSaisonErgebnisse => Set<GlimSaisonErgebnis>();
    public DbSet<GlimPlayoff> GlimPlayoffs => Set<GlimPlayoff>();
    public DbSet<GlimSaisonStatistik> GlimSaisonStatistiken => Set<GlimSaisonStatistik>();
    public DbSet<GlimKarriere> GlimKarrieren => Set<GlimKarriere>();
    public DbSet<GlimKarriereCup> GlimKarriereCups => Set<GlimKarriereCup>();
    public DbSet<GlimAufstellung> GlimAufstellung => Set<GlimAufstellung>();
    public DbSet<GlimBankplatz> GlimBank => Set<GlimBankplatz>();
    public DbSet<GlimTrikotnummer> GlimTrikotnummern => Set<GlimTrikotnummer>();
    public DbSet<GlimVerletzung> GlimVerletzungen => Set<GlimVerletzung>();

    // ---- Hesparis: Zirkel und Archiv ----
    public DbSet<ZirkelPerson> ZirkelPersonen => Set<ZirkelPerson>();
    public DbSet<ZirkelFeld> ZirkelFelder => Set<ZirkelFeld>();
    public DbSet<ZirkelVersuch> ZirkelVersuche => Set<ZirkelVersuch>();
    public DbSet<ZirkelNotiz> ZirkelNotizen => Set<ZirkelNotiz>();
    public DbSet<ZirkelMordfall> ZirkelMordfaelle => Set<ZirkelMordfall>();
    public DbSet<ArchivKategorie> ArchivKategorien => Set<ArchivKategorie>();
    public DbSet<ArchivDokument> ArchivDokumente => Set<ArchivDokument>();
    public DbSet<ArchivDokumentKategorie> ArchivDokumentKategorien => Set<ArchivDokumentKategorie>();

    // ---- Hesparis: Schülerrat ----
    public DbSet<RatTagesordnungspunkt> RatTagesordnungspunkte => Set<RatTagesordnungspunkt>();
    public DbSet<RatAntrag> RatAntraege => Set<RatAntrag>();
    public DbSet<RatUnterschrift> RatUnterschriften => Set<RatUnterschrift>();
    public DbSet<RatGefallen> RatGefallen => Set<RatGefallen>();
    public DbSet<RatStimme> RatStimmen => Set<RatStimme>();
    public DbSet<RatRegel> RatRegeln => Set<RatRegel>();
    public DbSet<RatVerbrauchterAntrag> RatVerbrauchteAntraege => Set<RatVerbrauchterAntrag>();
    public DbSet<RatPlanung> RatPlanungen => Set<RatPlanung>();

    // ---- Hesparis: Protokollhefte ----
    public DbSet<Protokollheft> Protokollhefte => Set<Protokollheft>();
    public DbSet<HeftSchwerpunkt> HeftSchwerpunkte => Set<HeftSchwerpunkt>();
    public DbSet<HeftFehlend> HeftFehlend => Set<HeftFehlend>();
    public DbSet<HeftNotiz> HeftNotizen => Set<HeftNotiz>();
    public DbSet<HeftNotizUebernahme> HeftNotizUebernahmen => Set<HeftNotizUebernahme>();
    public DbSet<HeftAbschnitt> HeftAbschnitte => Set<HeftAbschnitt>();
    public DbSet<HeftFrage> HeftFragen => Set<HeftFrage>();
    public DbSet<HeftAblauf> HeftAblauf => Set<HeftAblauf>();
    public DbSet<HeftPerson> HeftPersonen => Set<HeftPerson>();
    public DbSet<HeftJederEintrag> HeftJederEintraege => Set<HeftJederEintrag>();
    public DbSet<HeftCheck> HeftChecks => Set<HeftCheck>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        base.ConfigureConventions(builder);

        // Hesparis-Enums als lesbarer Text statt als Zahl.
        // Betrifft nur diese Enum-Typen, die Sündenbock-Tabellen bleiben unverändert.
        builder.Properties<Note>().HaveConversion<string>().HaveMaxLength(1);
        builder.Properties<Termintyp>().HaveConversion<string>().HaveMaxLength(20);
        builder.Properties<Waffenart>().HaveConversion<string>().HaveMaxLength(10);
        builder.Properties<Mahlzeit>().HaveConversion<string>().HaveMaxLength(20);
        builder.Properties<Prioritaet>().HaveConversion<string>().HaveMaxLength(10);
        builder.Properties<Wettkampfart>().HaveConversion<string>().HaveMaxLength(20);
        builder.Properties<EffektArt>().HaveConversion<string>().HaveMaxLength(10);
        builder.Properties<EffektDauerArt>().HaveConversion<string>().HaveMaxLength(15);
        builder.Properties<ZirkelSeite>().HaveConversion<string>().HaveMaxLength(10);
    }
}
