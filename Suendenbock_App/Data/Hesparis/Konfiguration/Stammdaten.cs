using Suendenbock_App.Models.Domain.Hesparis;
using Microsoft.EntityFrameworkCore;

namespace Suendenbock_App.Data.Hesparis;

public static partial class HesparisModell
{
    private static void KonfiguriereStammdaten(ModelBuilder mb)
    {
        mb.Entity<Fach>(e =>
        {
            e.ToTable("Faecher", "stamm");
            e.Property(x => x.Id).ValueGeneratedNever();
            e.HasIndex(x => x.Name).IsUnique();
        });

        mb.Entity<FachStufe>(e =>
        {
            e.ToTable("FachStufen", "stamm");
            e.HasKey(x => new { x.FachId, x.Stufe });
            e.Property(x => x.Beschreibung).HasMaxLength(400);
            e.HasOne(x => x.Fach).WithMany(f => f.Stufen).HasForeignKey(x => x.FachId);
        });

        mb.Entity<Begabung>(e =>
        {
            e.ToTable("Begabungen", "stamm");
            e.HasKey(x => x.Code);
            e.Property(x => x.Code).HasMaxLength(1);
        });

        mb.Entity<Talent>(e =>
        {
            e.ToTable("Talente", "stamm");
            e.HasKey(x => x.Code);
            e.Property(x => x.Code).HasMaxLength(3);
            e.HasOne(x => x.Begabung).WithMany(b => b.Talente).HasForeignKey(x => x.BegabungCode);
            e.HasIndex(x => new { x.BegabungCode, x.Position }).IsUnique();
        });

        mb.Entity<MeisterschaftDefinition>(e =>
        {
            e.ToTable("Meisterschaften", "stamm");
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Beschreibung).HasMaxLength(400);
            e.HasOne(x => x.Talent).WithMany(t => t.Meisterschaften).HasForeignKey(x => x.TalentCode);
            e.HasIndex(x => new { x.TalentCode, x.Position }).IsUnique();
        });

        mb.Entity<Ag>(e =>
        {
            e.ToTable("Ags", "stamm");
            e.HasKey(x => x.Code);
            e.Property(x => x.Code).HasMaxLength(20);
            e.HasOne(x => x.BonusTalent).WithMany().HasForeignKey(x => x.BonusTalentCode);
        });

        mb.Entity<AkademieTeam>(e =>
        {
            e.ToTable("AkademieTeams", "stamm");
            e.HasKey(x => x.Code);
            e.Property(x => x.Code).HasMaxLength(20);
            e.Property(x => x.Kurz).HasMaxLength(5);
        });

        mb.Entity<Sonderzauber>(e =>
        {
            e.ToTable("Sonderzauber", "stamm");
            e.HasKey(x => x.Code);
            e.Property(x => x.Code).HasMaxLength(40);
        });

        mb.Entity<SonderzauberOption>(e =>
        {
            e.ToTable("SonderzauberOptionen", "stamm");
            e.HasKey(x => new { x.SonderzauberCode, x.Position });
            e.Property(x => x.Wirkung).HasMaxLength(600);
            e.HasOne(x => x.Sonderzauber).WithMany(s => s.Optionen).HasForeignKey(x => x.SonderzauberCode);
        });

        mb.Entity<Gericht>(e =>
        {
            e.ToTable("Gerichte", "stamm");
            e.HasKey(x => x.Code);
            e.Property(x => x.Code).HasMaxLength(40);
        });
    }
}
