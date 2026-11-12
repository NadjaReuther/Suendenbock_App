using Suendenbock_App.Models.Domain.Hesparis;
using Microsoft.EntityFrameworkCore;

namespace Suendenbock_App.Data.Hesparis;

public static partial class HesparisModell
{
    private static void KonfiguriereZirkelUndArchiv(ModelBuilder mb)
    {
        mb.Entity<ZirkelPerson>(e =>
        {
            e.ToTable("ZirkelPersonen");
            e.HasIndex(x => new { x.KampagneId, x.Nr, x.Seite }).IsUnique();
        });

        mb.Entity<ZirkelFeld>(e =>
        {
            e.ToTable("ZirkelFelder");
            e.Property(x => x.Feld).HasMaxLength(10);
            e.HasIndex(x => new { x.KampagneId, x.Nr, x.Seite, x.Feld }).IsUnique();
        });

        mb.Entity<ZirkelVersuch>(e =>
        {
            e.ToTable("ZirkelVersuche");
            e.Property(x => x.Feld).HasMaxLength(10);
        });

        mb.Entity<ZirkelNotiz>(e => e.ToTable("ZirkelNotizen"));

        mb.Entity<ZirkelMordfall>(e =>
        {
            e.ToTable("ZirkelMordfaelle");
            e.HasIndex(x => x.KampagneId).IsUnique();
        });

        mb.Entity<ArchivKategorie>(e =>
        {
            e.ToTable("ArchivKategorien");
            e.Property(x => x.Code).HasMaxLength(20);
            e.HasIndex(x => new { x.KampagneId, x.Code }).IsUnique();
        });

        mb.Entity<ArchivDokument>(e => e.ToTable("ArchivDokumente"));

        mb.Entity<ArchivDokumentKategorie>(e =>
        {
            e.ToTable("ArchivDokumentKategorien");
            e.HasKey(x => new { x.ArchivDokumentId, x.ArchivKategorieId });
            e.HasOne(x => x.ArchivDokument).WithMany(d => d.Kategorien).HasForeignKey(x => x.ArchivDokumentId);
        });
    }

    private static void KonfiguriereSchuelerrat(ModelBuilder mb)
    {
        mb.Entity<RatTagesordnungspunkt>(e => e.ToTable("RatTagesordnungspunkte"));

        mb.Entity<RatAntrag>(e =>
        {
            e.ToTable("RatAntraege");
            e.Property(x => x.Monat).HasMaxLength(7);
            e.Property(x => x.Schluessel).HasMaxLength(40);
            e.HasIndex(x => new { x.KampagneId, x.Monat });
        });

        mb.Entity<RatUnterschrift>(e =>
        {
            e.ToTable("RatUnterschriften");
            e.HasKey(x => new { x.RatAntragId, x.MitschuelerId });
            e.HasOne(x => x.RatAntrag).WithMany(a => a.Unterschriften).HasForeignKey(x => x.RatAntragId);
        });

        mb.Entity<RatGefallen>(e =>
        {
            e.ToTable("RatGefallen");
            e.HasKey(x => new { x.RatAntragId, x.MitschuelerId });
            e.HasOne(x => x.RatAntrag).WithMany(a => a.Gefallen).HasForeignKey(x => x.RatAntragId);
        });

        mb.Entity<RatStimme>(e =>
        {
            e.ToTable("RatStimmen");
            e.HasKey(x => new { x.RatAntragId, x.Klasse });
            e.Property(x => x.Klasse).HasMaxLength(5);
            e.HasOne(x => x.RatAntrag).WithMany(a => a.Stimmen).HasForeignKey(x => x.RatAntragId);
        });

        mb.Entity<RatRegel>(e =>
        {
            e.ToTable("RatRegeln");
            e.Property(x => x.Schluessel).HasMaxLength(40);
        });

        mb.Entity<RatVerbrauchterAntrag>(e =>
        {
            e.ToTable("RatVerbrauchteAntraege");
            e.Property(x => x.Schluessel).HasMaxLength(40);
            e.HasIndex(x => new { x.KampagneId, x.Schluessel }).IsUnique();
        });

        mb.Entity<RatPlanung>(e =>
        {
            e.ToTable("RatPlanungen");
            e.Property(x => x.Monat).HasMaxLength(7);
            e.Property(x => x.Schluessel).HasMaxLength(40);
            e.HasIndex(x => new { x.KampagneId, x.Monat, x.Slot }).IsUnique();
        });
    }

    private static void KonfiguriereProtokollhefte(ModelBuilder mb)
    {
        mb.Entity<Protokollheft>(e =>
        {
            e.ToTable("Protokollhefte");
            e.HasOne(x => x.Begleiter).WithMany().HasForeignKey(x => x.BegleiterId);
            e.HasOne(x => x.SchreiberSpielfigur).WithMany().HasForeignKey(x => x.SchreiberSpielfigurId);
            e.HasOne(x => x.SchreiberMitschueler).WithMany().HasForeignKey(x => x.SchreiberMitschuelerId);
            e.HasOne(x => x.KalenderTermin).WithMany().HasForeignKey(x => x.KalenderTerminId);
        });

        mb.Entity<HeftSchwerpunkt>(e =>
        {
            e.ToTable("HeftSchwerpunkte");
            e.HasKey(x => new { x.ProtokollheftId, x.Text });
            e.Property(x => x.Text).HasMaxLength(100);
            e.HasOne(x => x.Protokollheft).WithMany(h => h.Schwerpunkte).HasForeignKey(x => x.ProtokollheftId);
        });

        mb.Entity<HeftFehlend>(e =>
        {
            e.ToTable("HeftFehlend");
            e.HasKey(x => new { x.ProtokollheftId, x.SpielfigurId });
            e.HasOne(x => x.Protokollheft).WithMany(h => h.Fehlend).HasForeignKey(x => x.ProtokollheftId);
        });

        mb.Entity<HeftNotiz>(e =>
        {
            e.ToTable("HeftNotizen");
            e.Property(x => x.Uhrzeit).HasMaxLength(10);
            e.HasOne(x => x.Protokollheft).WithMany(h => h.Notizen).HasForeignKey(x => x.ProtokollheftId);
        });

        mb.Entity<HeftNotizUebernahme>(e =>
        {
            e.ToTable("HeftNotizUebernahmen");
            e.HasKey(x => new { x.HeftNotizId, x.Abschnitt });
            e.Property(x => x.Abschnitt).HasMaxLength(20);
            e.HasOne(x => x.HeftNotiz).WithMany(n => n.Uebernommen).HasForeignKey(x => x.HeftNotizId);
        });

        mb.Entity<HeftAbschnitt>(e =>
        {
            e.ToTable("HeftAbschnitte");
            e.HasKey(x => new { x.ProtokollheftId, x.Abschnitt });
            e.Property(x => x.Abschnitt).HasMaxLength(20);
            e.HasOne(x => x.Protokollheft).WithMany(h => h.Abschnitte).HasForeignKey(x => x.ProtokollheftId);
        });

        mb.Entity<HeftFrage>(e =>
        {
            e.ToTable("HeftFragen");
            e.HasKey(x => new { x.ProtokollheftId, x.Abschnitt, x.Position });
            e.Property(x => x.Abschnitt).HasMaxLength(20);
            e.HasOne(x => x.Protokollheft).WithMany(h => h.Fragen).HasForeignKey(x => x.ProtokollheftId);
        });

        mb.Entity<HeftAblauf>(e =>
        {
            e.ToTable("HeftAblauf");
            e.Property(x => x.Uhrzeit).HasMaxLength(10);
            e.HasOne(x => x.Protokollheft).WithMany(h => h.Ablauf).HasForeignKey(x => x.ProtokollheftId);
        });

        mb.Entity<HeftPerson>(e =>
        {
            e.ToTable("HeftPersonen");
            e.HasOne(x => x.Protokollheft).WithMany(h => h.Personen).HasForeignKey(x => x.ProtokollheftId);
        });

        mb.Entity<HeftJederEintrag>(e =>
        {
            e.ToTable("HeftJederEintraege");
            e.HasKey(x => new { x.ProtokollheftId, x.Schluessel });
            e.Property(x => x.Schluessel).HasMaxLength(40);
            e.HasOne(x => x.Protokollheft).WithMany(h => h.JederEintraege).HasForeignKey(x => x.ProtokollheftId);
        });

        mb.Entity<HeftCheck>(e =>
        {
            e.ToTable("HeftChecks");
            e.HasKey(x => new { x.ProtokollheftId, x.Schluessel });
            e.Property(x => x.Schluessel).HasMaxLength(40);
            e.HasOne(x => x.Protokollheft).WithMany(h => h.Checks).HasForeignKey(x => x.ProtokollheftId);
        });
    }
}
