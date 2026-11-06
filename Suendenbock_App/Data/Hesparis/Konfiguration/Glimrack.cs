using Suendenbock_App.Models.Domain.Hesparis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Suendenbock_App.Data.Hesparis;

public static partial class HesparisModell
{
    private static void KonfiguriereGlimrack(ModelBuilder mb)
    {
        mb.Entity<GlimSpieler>(e =>
        {
            e.ToTable("GlimSpieler");
            e.HasIndex(x => new { x.KampagneId, x.Code }).IsUnique();
            GlimWerteSpalten(e.OwnsOne(x => x.Werte));
            e.Navigation(x => x.Werte).IsRequired();
        });

        mb.Entity<GlimNachwuchs>(e =>
        {
            e.ToTable("GlimNachwuchs");
            e.Property(x => x.Slot).HasMaxLength(20);
            e.HasIndex(x => new { x.KampagneId, x.Slot }).IsUnique();
            GlimWerteSpalten(e.OwnsOne(x => x.Werte));
            e.Navigation(x => x.Werte).IsRequired();
        });

        mb.Entity<GlimTraining>(e =>
        {
            e.ToTable("GlimTraining");
            e.HasKey(x => new { x.GlimSpielerId, x.Wert });
            e.Property(x => x.Wert).HasMaxLength(10);
            e.HasOne(x => x.GlimSpieler).WithMany(s => s.Training).HasForeignKey(x => x.GlimSpielerId);
        });

        mb.Entity<GlimNachwuchsTraining>(e =>
        {
            e.ToTable("GlimNachwuchsTraining");
            e.HasKey(x => new { x.GlimNachwuchsId, x.Wert });
            e.Property(x => x.Wert).HasMaxLength(10);
            e.HasOne(x => x.GlimNachwuchs).WithMany(s => s.Training).HasForeignKey(x => x.GlimNachwuchsId);
        });

        mb.Entity<GlimSpiel>(e =>
        {
            e.ToTable("GlimSpiele");
            e.HasOne(x => x.GegnerTeam).WithMany().HasForeignKey(x => x.GegnerTeamCode);
        });

        mb.Entity<GlimSpielLogEintrag>(e =>
        {
            e.ToTable("GlimSpielLog");
            e.Property(x => x.Haelfte).HasMaxLength(10);
            e.Property(x => x.Seite).HasMaxLength(10);
            e.Property(x => x.Spielstand).HasMaxLength(10);
            e.HasOne(x => x.GlimSpiel).WithMany(s => s.Log).HasForeignKey(x => x.GlimSpielId);
        });

        mb.Entity<GlimSpielStatistik>(e =>
        {
            e.ToTable("GlimSpielStatistiken");
            e.HasOne(x => x.GlimSpiel).WithMany(s => s.Statistik).HasForeignKey(x => x.GlimSpielId);
        });

        mb.Entity<GlimCupArchiv>(e =>
        {
            e.ToTable("GlimCupArchiv");
            e.HasOne(x => x.MeisterTeam).WithMany().HasForeignKey(x => x.MeisterTeamCode);
        });

        mb.Entity<GlimTeamStaerke>(e =>
        {
            e.ToTable("GlimTeamStaerken");
            e.HasIndex(x => new { x.KampagneId, x.TeamCode }).IsUnique();
        });

        mb.Entity<GlimSaisonErgebnis>(e =>
        {
            e.ToTable("GlimSaisonErgebnisse");
            e.Property(x => x.Fixture).HasMaxLength(20);
            e.HasIndex(x => new { x.KampagneId, x.Fixture }).IsUnique();
        });

        mb.Entity<GlimPlayoff>(e =>
        {
            e.ToTable("GlimPlayoffs");
            e.Property(x => x.Runde).HasMaxLength(10);
            e.HasOne(x => x.HeimTeam).WithMany().HasForeignKey(x => x.HeimTeamCode);
            e.HasOne(x => x.GastTeam).WithMany().HasForeignKey(x => x.GastTeamCode);
            e.HasIndex(x => new { x.KampagneId, x.Runde }).IsUnique();
        });

        mb.Entity<GlimSaisonStatistik>(e =>
        {
            e.ToTable("GlimSaisonStatistiken");
            e.HasIndex(x => new { x.KampagneId, x.Name }).IsUnique();
        });

        mb.Entity<GlimKarriere>(e =>
        {
            e.ToTable("GlimKarrieren");
            e.HasIndex(x => new { x.KampagneId, x.Name }).IsUnique();
        });

        mb.Entity<GlimKarriereCup>(e =>
        {
            e.ToTable("GlimKarriereCups");
            e.HasKey(x => new { x.GlimKarriereId, x.CupNr });
            e.HasOne(x => x.GlimKarriere).WithMany(k => k.Cups).HasForeignKey(x => x.GlimKarriereId);
        });

        mb.Entity<GlimAufstellung>(e =>
        {
            e.ToTable("GlimAufstellung");
            e.Property(x => x.Slot).HasMaxLength(20);
            e.HasIndex(x => new { x.KampagneId, x.Slot }).IsUnique();
        });

        mb.Entity<GlimBankplatz>(e =>
        {
            e.ToTable("GlimBank");
            e.HasIndex(x => new { x.KampagneId, x.Position }).IsUnique();
        });

        mb.Entity<GlimTrikotnummer>(e =>
        {
            e.ToTable("GlimTrikotnummern");
            e.HasIndex(x => new { x.KampagneId, x.Mitglied }).IsUnique();
        });

        mb.Entity<GlimVerletzung>(e =>
        {
            e.ToTable("GlimVerletzungen");
            e.HasIndex(x => new { x.KampagneId, x.Name }).IsUnique();
        });
    }

    private static void GlimWerteSpalten<T>(OwnedNavigationBuilder<T, GlimWerte> o) where T : class
    {
        o.Property(x => x.Wurf).HasColumnName("Wurf");
        o.Property(x => x.Schlag).HasColumnName("Schlag");
        o.Property(x => x.Tempo).HasColumnName("Tempo");
        o.Property(x => x.Fang).HasColumnName("Fang");
        o.Property(x => x.Nerven).HasColumnName("Nerven");
    }
}
