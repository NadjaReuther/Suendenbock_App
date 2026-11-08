using Microsoft.EntityFrameworkCore;

namespace Suendenbock_App.Data
{
    /// <summary>
    /// Sündenbock ist das Hauptspiel: Der Hauptcharakter eines Spielers ist sein Character in der Originalwelt,
    /// nur der trägt den Spieler in Character.UserId. Characters in Nebenwelten (z.B. Hesparis) tragen den Spieler
    /// an der Zuordnung (CharacterGameWorld.UserId). So tauchen sie in Sündenbock nirgends als Spieler-Character auf.
    /// </summary>
    public static class Nebencharaktere
    {
        /// <summary>
        /// Zieht den Spieler von Nebencharakteren auf die Welt-Zuordnung um:
        /// Character mit UserId, der Welten zugeordnet ist, aber NICHT der Originalwelt → Nebencharakter.
        /// Characters ganz ohne Welt-Zuordnung bleiben unangetastet (gelten als Sündenbock).
        /// Kann beliebig oft laufen (Programmstart, nach dem Zuordnen, beim Öffnen von Hesparis).
        /// </summary>
        public static async Task UmziehenAsync(ApplicationDbContext context)
        {
            if (!await context.GameWorlds.AnyAsync(gw => gw.IsOriginWorld))
            {
                return;
            }

            var nebencharaktere = await context.Characters
                .Include(c => c.CharacterGameWorlds)
                .Where(c => c.UserId != null
                            && c.CharacterGameWorlds.Any()
                            && !c.CharacterGameWorlds.Any(cgw => cgw.GameWorld.IsOriginWorld))
                .ToListAsync();
            if (nebencharaktere.Count == 0)
            {
                return;
            }

            foreach (var character in nebencharaktere)
            {
                foreach (var zuordnung in character.CharacterGameWorlds)
                {
                    if (zuordnung.UserId == null)
                    {
                        zuordnung.UserId = character.UserId;
                    }
                }
                character.UserId = null;
            }
            await context.SaveChangesAsync();
        }
    }
}
