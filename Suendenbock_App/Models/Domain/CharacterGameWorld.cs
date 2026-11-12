namespace Suendenbock_App.Models.Domain
{
    /// <summary>
    /// Many-to-Many Beziehung zwischen Characters und GameWorlds
    /// Ein Character kann in mehreren Welten existieren (z.B. alte Characters in beiden Welten)
    /// </summary>
    public class CharacterGameWorld
    {
        public int Id { get; set; }

        public int CharacterId { get; set; }
        public virtual Character Character { get; set; } = null!;

        public int GameWorldId { get; set; }
        public virtual GameWorld GameWorld { get; set; } = null!;

        /// <summary>
        /// Wann wurde dieser Character zu dieser Welt hinzugefügt
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Ist diese Welt die Originalwelt? Wird beim Zuordnen automatisch aus GameWorld.IsOriginWorld übernommen.
        /// </summary>
        public bool IsOriginWorld { get; set; } = false;

        /// <summary>
        /// Spieler dieses Characters IN DIESER WELT (nur für Nebenwelten wie Hesparis).
        /// Hauptcharaktere (Originalwelt Sündenbock) haben ihren Spieler weiter in Character.UserId.
        /// Nebencharaktere haben Character.UserId = null, damit sie in Sündenbock nirgends als Spieler-Character auftauchen.
        /// </summary>
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
