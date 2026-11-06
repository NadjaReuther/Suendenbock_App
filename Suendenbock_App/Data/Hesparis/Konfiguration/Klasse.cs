using Suendenbock_App.Models.Domain.Hesparis;
using Microsoft.EntityFrameworkCore;

namespace Suendenbock_App.Data.Hesparis;

public static partial class HesparisModell
{
    private static void KonfiguriereKlasse(ModelBuilder mb)
    {
        mb.Entity<Mitschueler>(e =>
        {
            e.Property(x => x.Version).IsRowVersion();
            e.Property(x => x.Code).HasMaxLength(20);
            e.HasOne(x => x.Kampagne).WithMany(k => k.Mitschueler).HasForeignKey(x => x.KampagneId);
            e.HasOne(x => x.Lieblingsfach).WithMany().HasForeignKey(x => x.LieblingsfachId);
            e.HasOne(x => x.Hassfach).WithMany().HasForeignKey(x => x.HassfachId);
            e.HasOne(x => x.Spielfigur).WithMany().HasForeignKey(x => x.SpielfigurId);
            e.HasIndex(x => new { x.KampagneId, x.Code }).IsUnique();
            // jede Spielfigur steht höchstens einmal in der Klasse
            e.HasIndex(x => x.SpielfigurId).IsUnique().HasFilter("[SpielfigurId] IS NOT NULL");
        });

        mb.Entity<MitschuelerMeinung>(e =>
        {
            e.ToTable("MitschuelerMeinungen");
            e.HasKey(x => new { x.MitschuelerId, x.Thema });
            e.Property(x => x.Thema).HasMaxLength(20);
            e.HasOne(x => x.Mitschueler).WithMany(m => m.Meinungen).HasForeignKey(x => x.MitschuelerId);
        });

        mb.Entity<MitschuelerAg>(e =>
        {
            e.ToTable("MitschuelerAgs");
            e.HasKey(x => new { x.MitschuelerId, x.AgCode });
            e.HasOne(x => x.Mitschueler).WithMany(m => m.Ags).HasForeignKey(x => x.MitschuelerId);
        });

        mb.Entity<Beziehung>(e =>
        {
            e.ToTable("Beziehungen");
            e.HasKey(x => new { x.MitschuelerId, x.SpielfigurId });
            e.HasOne(x => x.Mitschueler).WithMany(m => m.Beziehungen).HasForeignKey(x => x.MitschuelerId);
            e.OwnsOne(x => x.SpielfigurZuMitschueler, o =>
            {
                o.Property(p => p.Freundschaft).HasColumnName("Figur_Freundschaft");
                o.Property(p => p.Romantik).HasColumnName("Figur_Romantik");
                o.Property(p => p.Rivalitaet).HasColumnName("Figur_Rivalitaet");
                o.Property(p => p.Abneigung).HasColumnName("Figur_Abneigung");
                o.Property(p => p.Respekt).HasColumnName("Figur_Respekt");
            });
            e.OwnsOne(x => x.MitschuelerZuSpielfigur, o =>
            {
                o.Property(p => p.Freundschaft).HasColumnName("Mitschueler_Freundschaft");
                o.Property(p => p.Romantik).HasColumnName("Mitschueler_Romantik");
                o.Property(p => p.Rivalitaet).HasColumnName("Mitschueler_Rivalitaet");
                o.Property(p => p.Abneigung).HasColumnName("Mitschueler_Abneigung");
                o.Property(p => p.Respekt).HasColumnName("Mitschueler_Respekt");
            });
            e.Navigation(x => x.SpielfigurZuMitschueler).IsRequired();
            e.Navigation(x => x.MitschuelerZuSpielfigur).IsRequired();
        });

        mb.Entity<Lehrer>(e =>
        {
            e.ToTable("Lehrer");
            e.HasIndex(x => new { x.KampagneId, x.Name }).IsUnique();
        });

        mb.Entity<LehrerFach>(e =>
        {
            e.ToTable("LehrerFaecher");
            e.HasKey(x => new { x.LehrerId, x.FachId });
            e.HasOne(x => x.Lehrer).WithMany(l => l.Faecher).HasForeignKey(x => x.LehrerId);
        });

        mb.Entity<LehrerFakt>(e =>
        {
            e.ToTable("LehrerFakten");
            e.HasOne(x => x.Lehrer).WithMany(l => l.Fakten).HasForeignKey(x => x.LehrerId);
        });
    }
}
