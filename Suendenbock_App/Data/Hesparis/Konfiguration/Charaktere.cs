using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Models.Domain.Hesparis;

namespace Suendenbock_App.Data.Hesparis;

public static partial class HesparisModell
{
    private static void KonfiguriereCharaktere(ModelBuilder mb)
    {
        mb.Entity<Charakterart>(e =>
        {
            e.ToTable("Charakterarten", "stamm");
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Name).HasMaxLength(40);
            e.HasData(
                new Charakterart { Id = Charakterart.Spieler, Name = "Spieler" },
                new Charakterart { Id = Charakterart.Lehrkraft, Name = "Lehrkraft" },
                new Charakterart { Id = Charakterart.Mitschueler, Name = "Mitschüler" });
        });

        mb.Entity<HesparisCharakter>(e =>
        {
            e.ToTable("HesparisCharaktere");
            e.Property(x => x.Vorname).HasMaxLength(100);
            e.Property(x => x.Nachname).HasMaxLength(100);
            e.Property(x => x.Rufname).HasMaxLength(100);
            e.Property(x => x.BildPfad).HasMaxLength(400);
            e.Ignore(x => x.Anzeigename);
            e.Ignore(x => x.VollerName);
            e.HasOne(x => x.Charakterart).WithMany(a => a.Charaktere).HasForeignKey(x => x.CharakterartId);
            // Spieler (Benutzer). NO ACTION: beim Löschen eines Benutzers setzt die Benutzerverwaltung das Feld auf NULL.
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasIndex(x => x.UserId);
        });

        mb.Entity<CharakterVerknuepfung>(e =>
        {
            e.ToTable("CharakterVerknuepfungen");
            e.HasOne(x => x.HesparisCharakter).WithOne(h => h.Verknuepfung).HasForeignKey<CharakterVerknuepfung>(x => x.HesparisCharakterId);
            e.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId);
            // Jeder Charakter höchstens einmal verknüpft
            e.HasIndex(x => x.HesparisCharakterId).IsUnique();
            e.HasIndex(x => x.CharacterId).IsUnique();
        });
    }
}
