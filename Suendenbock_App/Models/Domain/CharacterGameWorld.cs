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
        /// Wurde dieser Character ursprünglich in dieser Welt erstellt?
        /// (wichtig für Logik: neue 1810-Characters existieren nur dort)
        /// </summary>
        public bool IsOriginWorld { get; set; } = false;
    }
}
