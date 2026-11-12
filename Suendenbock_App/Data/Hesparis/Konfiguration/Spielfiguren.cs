using Suendenbock_App.Models.Domain.Hesparis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Suendenbock_App.Data.Hesparis;

public static partial class HesparisModell
{
    private static void KonfiguriereSpielfiguren(ModelBuilder mb)
    {
        mb.Entity<Kampagne>(e =>
        {
            e.Property(x => x.Version).IsRowVersion();
            e.Property(x => x.SchwarzmarktCode).HasMaxLength(20);
        });

        mb.Entity<Spielfigur>(e =>
        {
            e.Property(x => x.Version).IsRowVersion();
            e.HasOne(x => x.Kampagne).WithMany(k => k.Spielfiguren).HasForeignKey(x => x.KampagneId);
            e.HasIndex(x => new { x.KampagneId, x.Position }).IsUnique();

            // Spielwerte gehören zu genau einem Hesparis-Charakter (1:1). Wird der Charakter gelöscht,
            // verschwinden auch Talente, Noten, Tasche … (Kaskade, siehe HesparisModell.Kaskaden).
            e.HasOne(x => x.HesparisCharakter).WithOne(h => h.Spielfigur).HasForeignKey<Spielfigur>(x => x.HesparisCharakterId);
            e.HasIndex(x => x.HesparisCharakterId).IsUnique();

            // ALT: Verweis auf den Sündenbock-Character, nur noch für den Umzug (HesparisUmzug)
            e.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId);
        });

        mb.Entity<SpielfigurFachStufe>(e =>
        {
            e.HasKey(x => new { x.SpielfigurId, x.FachId });
            e.HasOne(x => x.Spielfigur).WithMany(s => s.FachStufen).HasForeignKey(x => x.SpielfigurId);
        });

        mb.Entity<StufenWahl>(e =>
        {
            e.HasKey(x => new { x.SpielfigurId, x.FachId, x.Stufe });
            e.HasOne(x => x.Spielfigur).WithMany(s => s.StufenWahlen).HasForeignKey(x => x.SpielfigurId);
        });

        mb.Entity<Schulnote>(e =>
        {
            e.HasOne(x => x.Spielfigur).WithMany(s => s.Noten).HasForeignKey(x => x.SpielfigurId);
            e.HasIndex(x => new { x.SpielfigurId, x.FachId, x.Position }).IsUnique();
        });

        mb.Entity<Pruefungseintrag>(e =>
        {
            e.HasKey(x => new { x.SpielfigurId, x.Abschnitt, x.Teil });
            e.Property(x => x.Abschnitt).HasMaxLength(20);
            e.Property(x => x.Teil).HasMaxLength(20);
            e.HasOne(x => x.Spielfigur).WithMany(s => s.Pruefungen).HasForeignKey(x => x.SpielfigurId);
        });

        mb.Entity<TalentWert>(e =>
        {
            e.HasKey(x => new { x.SpielfigurId, x.TalentCode });
            e.HasOne(x => x.Spielfigur).WithMany(s => s.Talente).HasForeignKey(x => x.SpielfigurId);
        });

        mb.Entity<TalentZiel>(e =>
        {
            e.HasKey(x => new { x.SpielfigurId, x.Schluessel });
            e.Property(x => x.Schluessel).HasMaxLength(20);
            e.HasOne(x => x.Spielfigur).WithMany(s => s.TalentZiele).HasForeignKey(x => x.SpielfigurId);
        });

        mb.Entity<SpielfigurMeisterschaft>(e =>
        {
            e.HasKey(x => new { x.SpielfigurId, x.MeisterschaftDefinitionId });
            e.HasOne(x => x.Spielfigur).WithMany(s => s.Meisterschaften).HasForeignKey(x => x.SpielfigurId);
        });

        mb.Entity<Gegenstand>(e =>
        {
            e.ToTable("Gegenstaende");
            e.HasOne(x => x.Spielfigur).WithMany(s => s.Gegenstaende).HasForeignKey(x => x.SpielfigurId);
            EffektSpalten(e.OwnsOne(x => x.Effekt));
            e.Navigation(x => x.Effekt).IsRequired();
        });

        mb.Entity<Geldbuchung>(e =>
        {
            e.HasOne(x => x.Spielfigur).WithMany(s => s.Geldbuchungen).HasForeignKey(x => x.SpielfigurId);
        });

        mb.Entity<Wirkung>(e =>
        {
            e.HasOne(x => x.Spielfigur).WithMany(s => s.Wirkungen).HasForeignKey(x => x.SpielfigurId);
            EffektSpalten(e.OwnsOne(x => x.Effekt));
            e.Navigation(x => x.Effekt).IsRequired();
        });

        mb.Entity<SonderzauberStand>(e =>
        {
            e.ToTable("SonderzauberStaende");
            e.HasIndex(x => new { x.KampagneId, x.SonderzauberCode }).IsUnique();
        });

        mb.Entity<SonderzauberAufgedeckt>(e =>
        {
            e.ToTable("SonderzauberAufgedeckt");
            e.HasKey(x => new { x.SonderzauberStandId, x.Position });
            e.HasOne(x => x.SonderzauberStand).WithMany(s => s.Aufgedeckt).HasForeignKey(x => x.SonderzauberStandId);
        });
    }

    /// <summary>Spalten der eingebetteten Wirkung: Effekt_Art, Effekt_TalentCode, …</summary>
    private static void EffektSpalten<T>(OwnedNavigationBuilder<T, Effekt> o) where T : class
    {
        o.Property(x => x.Art).HasColumnName("Effekt_Art");
        o.Property(x => x.TalentCode).HasColumnName("Effekt_TalentCode").HasMaxLength(3);
        o.Property(x => x.Wert).HasColumnName("Effekt_Wert");
        o.Property(x => x.VorteilAuf).HasColumnName("Effekt_VorteilAuf").HasMaxLength(60);
        o.Property(x => x.DauerArt).HasColumnName("Effekt_DauerArt");
        o.Property(x => x.DauerAnzahl).HasColumnName("Effekt_DauerAnzahl");
        o.Property(x => x.DauerText).HasColumnName("Effekt_DauerText").HasMaxLength(40);
        o.HasOne<Talent>().WithMany().HasForeignKey(x => x.TalentCode);
    }
}
