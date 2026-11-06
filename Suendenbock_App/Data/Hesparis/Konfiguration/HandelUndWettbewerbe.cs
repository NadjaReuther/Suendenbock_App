using Suendenbock_App.Models.Domain.Hesparis;
using Microsoft.EntityFrameworkCore;

namespace Suendenbock_App.Data.Hesparis;

public static partial class HesparisModell
{
    private static void KonfiguriereHandel(ModelBuilder mb)
    {
        mb.Entity<LadenArtikel>(e =>
        {
            e.ToTable("LadenArtikel");
            EffektSpalten(e.OwnsOne(x => x.Effekt));
            e.Navigation(x => x.Effekt).IsRequired();
        });

        mb.Entity<SchwarzmarktWare>(e =>
        {
            e.ToTable("SchwarzmarktWaren");
            EffektSpalten(e.OwnsOne(x => x.Effekt));
            e.Navigation(x => x.Effekt).IsRequired();
        });

        mb.Entity<SchwarzmarktVerkauf>(e => e.ToTable("SchwarzmarktVerkaeufe"));
        mb.Entity<Rezept>(e => e.ToTable("Rezepte"));
        mb.Entity<RezeptZutat>(e =>
        {
            e.ToTable("RezeptZutaten");
            e.HasOne(x => x.Rezept).WithMany(r => r.Zutaten).HasForeignKey(x => x.RezeptId);
        });
        mb.Entity<Braueintrag>(e => e.ToTable("Braueintraege"));
        mb.Entity<Waffenwurf>(e => e.ToTable("Waffenwuerfe"));
        mb.Entity<Krankheitsfall>(e => e.ToTable("Krankheitsfaelle"));
        mb.Entity<KrankenGlimGemeldet>(e =>
        {
            e.ToTable("KrankenGlimGemeldet");
            e.HasIndex(x => new { x.KampagneId, x.Name }).IsUnique();
        });
    }

    private static void KonfiguriereWettbewerbe(ModelBuilder mb)
    {
        mb.Entity<Wettbewerb>(e => e.ToTable("Wettbewerbe"));

        mb.Entity<WettbewerbPlatzierung>(e =>
        {
            e.ToTable("WettbewerbPlatzierungen");
            e.HasKey(x => new { x.WettbewerbId, x.TeamCode });
            e.HasOne(x => x.Wettbewerb).WithMany(w => w.Platzierungen).HasForeignKey(x => x.WettbewerbId);
        });

        mb.Entity<TeamNotiz>(e =>
        {
            e.ToTable("TeamNotizen");
            e.HasIndex(x => new { x.KampagneId, x.TeamCode }).IsUnique();
        });

        mb.Entity<WettkampfStand>(e =>
        {
            e.ToTable("WettkampfStaende");
            e.HasIndex(x => new { x.KampagneId, x.Art }).IsUnique();
        });

        mb.Entity<WettkampfTeilnehmer>(e =>
        {
            e.ToTable("WettkampfTeilnehmer");
            e.Property(x => x.Gruppe).HasMaxLength(10);
            e.Property(x => x.Typ).HasMaxLength(10);
        });

        mb.Entity<DuellClubBilanz>(e =>
        {
            e.ToTable("DuellClubBilanzen");
            e.Property(x => x.Typ).HasMaxLength(10);
        });

        mb.Entity<WettkampfErgebnis>(e =>
        {
            e.ToTable("WettkampfErgebnisse");
            e.HasOne(x => x.SiegerTeam).WithMany().HasForeignKey(x => x.SiegerTeamCode);
        });

        mb.Entity<WettkampfZeugnis>(e =>
        {
            e.ToTable("WettkampfZeugnisse");
            e.HasIndex(x => new { x.KampagneId, x.Stichtag }).IsUnique();
        });

        mb.Entity<WettkampfZeugnisEintrag>(e =>
        {
            e.ToTable("WettkampfZeugnisEintraege");
            e.HasOne(x => x.WettkampfZeugnis).WithMany(z => z.Eintraege).HasForeignKey(x => x.WettkampfZeugnisId);
        });

        mb.Entity<WettkampfNpc>(e =>
        {
            e.ToTable("WettkampfNpcs");
            e.HasIndex(x => new { x.KampagneId, x.Art, x.Code }).IsUnique();
        });

        mb.Entity<WettkampfNpcWert>(e =>
        {
            e.ToTable("WettkampfNpcWerte");
            e.HasKey(x => new { x.WettkampfNpcId, x.Schluessel });
            e.Property(x => x.Schluessel).HasMaxLength(40);
            e.HasOne(x => x.WettkampfNpc).WithMany(n => n.Werte).HasForeignKey(x => x.WettkampfNpcId);
        });
    }
}
