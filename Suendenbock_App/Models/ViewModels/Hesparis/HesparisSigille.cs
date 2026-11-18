namespace Suendenbock_App.Models.ViewModels.Hesparis;

/// <summary>
/// Aussehen der Spielfiguren auf der Startseite (JS: PT_SIGIL, SONDER_COLORS):
/// Farbe und Symbol hängen am Sonderzauber der Figur (Code wie "elma-fische").
/// Reine Gestaltung, deshalb im Code und nicht in der Datenbank.
/// </summary>
public static class HesparisSigille
{
    public record Aussehen(string Farbe, string Rgb, string Svg);

    /// <summary>Für Figuren ohne Sonderzauber: Arkan-Violett, kein Symbol.</summary>
    public static readonly Aussehen Standard = new("#8a5cf0", "138,92,240", "");

    public static Aussehen Fuer(string? sonderzauberCode)
    {
        if (sonderzauberCode != null && Liste.TryGetValue(sonderzauberCode, out var a))
        {
            return a;
        }
        return Standard;
    }

    private static readonly Dictionary<string, Aussehen> Liste = new()
    {
        ["elma-fische"] = new("#49d9c9", "73,217,201",
            "<path d=\"M18 50 C30 30 58 28 74 42 L88 30 L84 50 L88 70 L74 58 C58 72 30 70 18 50 Z\" fill=\"currentColor\" fill-opacity=\"0.18\" stroke=\"currentColor\" stroke-width=\"2.5\" stroke-linejoin=\"round\"/><circle cx=\"34\" cy=\"47\" r=\"3.5\" fill=\"currentColor\"/><path d=\"M48 38 C54 44 54 56 48 62\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" opacity=\"0.7\"/><path d=\"M12 82 q8 -6 16 0 t16 0 t16 0 t16 0 t16 0\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" opacity=\"0.5\"/>"),
        ["levi-rauch"] = new("#e8894a", "232,137,74",
            "<path d=\"M50 10 C62 28 76 38 70 60 C66 76 58 84 50 84 C38 84 28 74 30 60 C31 50 39 44 42 34 C46 42 50 44 50 50 C56 42 56 26 50 10 Z\" fill=\"currentColor\" fill-opacity=\"0.2\" stroke=\"currentColor\" stroke-width=\"2.5\" stroke-linejoin=\"round\"/><path d=\"M50 84 C44 74 46 66 52 60 C54 68 60 70 58 78\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" opacity=\"0.7\"/><path d=\"M24 30 c-6 -6 -2 -14 4 -16 M78 24 c6 -6 2 -12 -4 -14\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" opacity=\"0.45\"/>"),
        ["darius-schleim"] = new("#8fd67a", "143,214,122",
            "<path d=\"M50 12 C62 32 76 46 76 62 C76 78 64 88 50 88 C36 88 24 78 24 62 C24 46 38 32 50 12 Z\" fill=\"currentColor\" fill-opacity=\"0.18\" stroke=\"currentColor\" stroke-width=\"2.5\"/><path d=\"M38 64 C38 54 46 50 52 54 C58 58 56 68 48 68\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2.2\"/><circle cx=\"62\" cy=\"46\" r=\"4\" fill=\"currentColor\" opacity=\"0.6\"/><circle cx=\"18\" cy=\"80\" r=\"3\" fill=\"currentColor\" opacity=\"0.4\"/><circle cx=\"84\" cy=\"30\" r=\"2.5\" fill=\"currentColor\" opacity=\"0.4\"/>"),
        ["svea-euphorie"] = new("#f08fbf", "240,143,191",
            "<path d=\"M50 10 L57 43 L90 50 L57 57 L50 90 L43 57 L10 50 L43 43 Z\" fill=\"currentColor\" fill-opacity=\"0.2\" stroke=\"currentColor\" stroke-width=\"2.5\" stroke-linejoin=\"round\"/><circle cx=\"50\" cy=\"50\" r=\"9\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\"/><path d=\"M22 22 l6 6 M78 22 l-6 6 M22 78 l6 -6 M78 78 l-6 -6\" stroke=\"currentColor\" stroke-width=\"2\" opacity=\"0.5\"/>"),
        ["rigel-kosmos"] = new("#ad8ff7", "173,143,247",
            "<path d=\"M60 14 C40 16 26 32 26 52 C26 72 42 86 62 86 C50 80 42 66 42 50 C42 34 50 20 60 14 Z\" fill=\"currentColor\" fill-opacity=\"0.2\" stroke=\"currentColor\" stroke-width=\"2.5\" stroke-linejoin=\"round\"/><path d=\"M72 30 l2.5 7 7 2.5 -7 2.5 -2.5 7 -2.5 -7 -7 -2.5 7 -2.5 Z\" fill=\"currentColor\"/><circle cx=\"80\" cy=\"64\" r=\"3\" fill=\"currentColor\" opacity=\"0.7\"/><circle cx=\"66\" cy=\"76\" r=\"2\" fill=\"currentColor\" opacity=\"0.5\"/><circle cx=\"18\" cy=\"24\" r=\"2\" fill=\"currentColor\" opacity=\"0.5\"/>"),
    };
}
