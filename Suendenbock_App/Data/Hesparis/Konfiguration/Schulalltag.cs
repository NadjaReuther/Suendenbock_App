using Suendenbock_App.Models.Domain.Hesparis;
using Microsoft.EntityFrameworkCore;

namespace Suendenbock_App.Data.Hesparis;

public static partial class HesparisModell
{
    private static void KonfiguriereSchulalltag(ModelBuilder mb)
    {
        mb.Entity<KalenderTermin>(e =>
        {
            e.ToTable("KalenderTermine");
            e.Property(x => x.RatMonat).HasMaxLength(7);
            e.HasIndex(x => new { x.KampagneId, x.Datum });
        });

        mb.Entity<AgEintrag>(e =>
        {
            e.ToTable("AgEintraege");
            e.HasIndex(x => new { x.KampagneId, x.AgCode }).IsUnique();
        });

        mb.Entity<AgMitglied>(e =>
        {
            e.ToTable("AgMitglieder");
            e.HasKey(x => new { x.AgEintragId, x.SpielfigurId });
            e.HasOne(x => x.AgEintrag).WithMany(a => a.Mitglieder).HasForeignKey(x => x.AgEintragId);
        });

        mb.Entity<Klassenarbeit>(e => e.ToTable("Klassenarbeiten"));

        mb.Entity<KlassenarbeitSpielfigur>(e =>
        {
            e.ToTable("KlassenarbeitSpielfiguren");
            e.HasKey(x => new { x.KlassenarbeitId, x.SpielfigurId });
            e.HasOne(x => x.Klassenarbeit).WithMany(k => k.Spielfiguren).HasForeignKey(x => x.KlassenarbeitId);
        });

        mb.Entity<KlassenarbeitMitschueler>(e =>
        {
            e.ToTable("KlassenarbeitMitschueler");
            e.HasKey(x => new { x.KlassenarbeitId, x.MitschuelerId });
            e.HasOne(x => x.Klassenarbeit).WithMany(k => k.Mitschueler).HasForeignKey(x => x.KlassenarbeitId);
        });

        mb.Entity<Nachsitzen>(e => e.ToTable("Nachsitzen"));

        mb.Entity<SchuljahrAbschnitt>(e =>
        {
            e.ToTable("SchuljahrAbschnitte");
            e.HasKey(x => new { x.SpielfigurId, x.Abschnitt });
            e.Property(x => x.Abschnitt).HasMaxLength(20);
        });

        mb.Entity<SchuljahrBilanz>(e =>
        {
            e.ToTable("SchuljahrBilanzen");
            e.HasIndex(x => new { x.KampagneId, x.Jahr }).IsUnique();
        });

        mb.Entity<SchuljahrBilanzSpielfigur>(e =>
        {
            e.ToTable("SchuljahrBilanzSpielfiguren");
            e.HasKey(x => new { x.SchuljahrBilanzId, x.SpielfigurId });
            e.HasOne(x => x.SchuljahrBilanz).WithMany(b => b.Spielfiguren).HasForeignKey(x => x.SchuljahrBilanzId);
        });

        mb.Entity<SchuljahrZeugnisnote>(e =>
        {
            e.ToTable("SchuljahrZeugnisnoten");
            e.HasKey(x => new { x.SchuljahrBilanzId, x.SpielfigurId, x.FachId });
            e.HasOne(x => x.SchuljahrBilanz).WithMany(b => b.Zeugnis).HasForeignKey(x => x.SchuljahrBilanzId);
        });

        mb.Entity<KartenNotiz>(e =>
        {
            e.ToTable("KartenNotizen");
            e.Property(x => x.Ort).HasMaxLength(40);
            e.HasIndex(x => new { x.KampagneId, x.Ort }).IsUnique();
        });

        mb.Entity<KartenZeit>(e =>
        {
            e.ToTable("KartenZeiten");
            e.HasIndex(x => new { x.KampagneId, x.Schluessel }).IsUnique();
        });

        mb.Entity<SpeiseplanEintrag>(e =>
        {
            e.ToTable("SpeiseplanEintraege");
            e.HasIndex(x => new { x.KampagneId, x.Woche, x.Wochentag, x.Mahlzeit }).IsUnique();
        });

        mb.Entity<Essenskauf>(e =>
        {
            e.ToTable("Essenskaeufe");
            e.HasIndex(x => new { x.SpielfigurId, x.Datum, x.Mahlzeit }).IsUnique();
        });

        mb.Entity<Essenswirkung>(e => e.ToTable("Essenswirkungen"));

        mb.Entity<ProtokollEintrag>(e => e.ToTable("ProtokollEintraege"));

        mb.Entity<ProtokollBeteiligter>(e =>
        {
            e.ToTable("ProtokollBeteiligte");
            e.HasKey(x => new { x.ProtokollEintragId, x.SpielfigurId });
            e.HasOne(x => x.ProtokollEintrag).WithMany(p => p.Beteiligte).HasForeignKey(x => x.ProtokollEintragId);
        });

        mb.Entity<Aufgabe>(e => e.ToTable("Aufgaben"));

        mb.Entity<Mission>(e => e.ToTable("Missionen"));

        mb.Entity<MissionHinweis>(e =>
        {
            e.ToTable("MissionHinweise");
            e.HasOne(x => x.Mission).WithMany(m => m.Hinweise).HasForeignKey(x => x.MissionId);
        });
    }
}
