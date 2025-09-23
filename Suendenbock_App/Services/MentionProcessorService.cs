using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using System.Text.RegularExpressions;

namespace Suendenbock_App.Services
{
    public interface IMentionProcessorService
    {
        string ProcessMentions(string text);
        string GetPlainText(string processedText);
        string ProcessMentionsPreservingExisting(string newText, string existingProcessedText);
    }

    public class MentionProcessorService : IMentionProcessorService
    {
        private readonly ApplicationDbContext _context;
        private readonly char[] _mentionSymbols = { '@', '#', '§', '&', '%' };

        public MentionProcessorService(ApplicationDbContext context)
        {
            _context = context;
        }

        // NEUE ROBUSTE ProcessMentions Methode
        public string ProcessMentions(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // SCHRITT 1: Finde alle Mentions mit ihren Positionen
            var mentions = FindAllMentions(text);

            // SCHRITT 2: Sortiere nach Position (rückwärts für korrekte Ersetzung)
            mentions = mentions.OrderByDescending(m => m.StartIndex).ToList();

            // SCHRITT 3: Ersetze jede Mention einzeln
            foreach (var mention in mentions)
            {
                var replacement = ProcessSingleMention(mention);
                if (replacement != null)
                {
                    text = text.Substring(0, mention.StartIndex) +
                           replacement +
                           text.Substring(mention.StartIndex + mention.Length);
                }
            }

            return text;
        }

        private List<MentionMatch> FindAllMentions(string text)
        {
            var mentions = new List<MentionMatch>();

            for (int i = 0; i < text.Length; i++)
            {
                if (_mentionSymbols.Contains(text[i]))
                {
                    var mention = ExtractMentionAt(text, i);
                    if (mention != null)
                    {
                        mentions.Add(mention);
                        i = mention.StartIndex + mention.Length - 1; // Springe über diese Mention
                    }
                }
            }

            return mentions;
        }

        private MentionMatch ExtractMentionAt(string text, int startIndex)
        {
            if (startIndex >= text.Length) return null;

            var symbol = text[startIndex];
            var nameStart = startIndex + 1;
            var nameEnd = nameStart;

            // Überspringe Leerzeichen direkt nach dem Symbol
            while (nameEnd < text.Length && text[nameEnd] == ' ')
            {
                nameEnd++;
            }

            // Wenn nur Leerzeichen nach dem Symbol kommen
            if (nameEnd >= text.Length) return null;

            // Sammle den Entitätsnamen
            var nameBuilder = new System.Text.StringBuilder();
            bool lastCharWasSpace = false;

            while (nameEnd < text.Length)
            {
                var currentChar = text[nameEnd];

                // Gültige Zeichen für Namen
                if (char.IsLetterOrDigit(currentChar) || "äöüÄÖÜß".Contains(currentChar))
                {
                    nameBuilder.Append(currentChar);
                    lastCharWasSpace = false;
                    nameEnd++;
                }
                // Leerzeichen zwischen Wörtern
                else if (currentChar == ' ' && !lastCharWasSpace && nameBuilder.Length > 0)
                {
                    // Schaue voraus: Kommt nach dem Leerzeichen noch ein gültiger Buchstabe?
                    if (nameEnd + 1 < text.Length &&
                        (char.IsLetterOrDigit(text[nameEnd + 1]) || "äöüÄÖÜß".Contains(text[nameEnd + 1])))
                    {
                        nameBuilder.Append(currentChar);
                        lastCharWasSpace = true;
                        nameEnd++;
                    }
                    else
                    {
                        break; // Ende des Namens erreicht
                    }
                }
                else
                {
                    break; // Ungültiges Zeichen - Ende des Namens
                }
            }

            var entityName = nameBuilder.ToString().Trim();

            // Name muss mindestens 2 Zeichen haben
            if (string.IsNullOrEmpty(entityName) || entityName.Length < 2) return null;

            return new MentionMatch
            {
                StartIndex = startIndex,
                Length = nameEnd - startIndex,
                Symbol = symbol,
                EntityName = entityName,
                FullMatch = text.Substring(startIndex, nameEnd - startIndex)
            };
        }

        private string ProcessSingleMention(MentionMatch mention)
        {
            return mention.Symbol switch
            {
                '@' => ProcessCharacterMention(mention.EntityName),
                '#' => ProcessGuildMention(mention.EntityName),
                '§' => ProcessInfanterieMention(mention.EntityName),
                '&' => ProcessMonsterMention(mention.EntityName),
                '%' => ProcessMagicClassMention(mention.EntityName),
                _ => null
            };
        }

        // BESTEHENDE Mention-Verarbeitungsmethoden bleiben unverändert
        private string ProcessCharacterMention(string characterName)
        {
            var character = FindCharacterByName(characterName);
            return character != null
                ? $"<a href='/Character/CharacterSheet/{character.Id}' class='entity-mention character-mention' title='Charakter: {characterName}'>👤@{characterName}</a>"
                : null;
        }

        private string ProcessGuildMention(string guildName)
        {
            var guild = _context.Guilds
                .FirstOrDefault(g => EF.Functions.Like(g.Name, guildName));

            return guild != null
                ? $"<a href='/Guild/GuildSheet/{guild.Id}' class='entity-mention guild-mention' title='Gilde: {guildName}'>🏰#{guildName}</a>"
                : null;
        }

        private string ProcessInfanterieMention(string infanterieName)
        {
            var searchName = infanterieName.Replace(". Infanterie", "").Trim();

            var infanterie = _context.Infanterien
                .FirstOrDefault(i => EF.Functions.Like(i.Bezeichnung, searchName) ||
                                   EF.Functions.Like(i.Bezeichnung + ". Infanterie", infanterieName));

            return infanterie != null
                ? $"<a href='/Infanterie/InfanterieSheet/{infanterie.Id}' class='entity-mention infanterie-mention' title='Infanterie: {infanterieName}'>⚔️§{infanterieName}</a>"
                : null;
        }

        private string ProcessMonsterMention(string monsterName)
        {
            var monster = _context.MonsterTypes
                .FirstOrDefault(m => EF.Functions.Like(m.Name, monsterName));

            return monster != null
                ? $"<a href='/Monster/MonstertypSheet/{monster.Id}' class='entity-mention monster-mention' title='Monster: {monsterName}'>👹&{monsterName}</a>"
                : null;
        }

        private string ProcessMagicClassMention(string magicClassName)
        {
            var magicClass = _context.MagicClasses
                .FirstOrDefault(mc => EF.Functions.Like(mc.Bezeichnung, magicClassName));

            return magicClass != null
                ? $"<a href='/MagicClass/MagicClassSheet/{magicClass.Id}' class='entity-mention magicclass-mention' title='Magieklasse: {magicClassName}'>🔮%{magicClassName}</a>"
                : null;
        }

        private Character FindCharacterByName(string fullName)
        {
            var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length < 2) return null;

            var firstName = nameParts[0];
            var lastName = string.Join(" ", nameParts.Skip(1));

            return _context.Characters
                .FirstOrDefault(c => EF.Functions.Like(c.Vorname, firstName) &&
                               EF.Functions.Like(c.Nachname, lastName));
        }

        // INTELLIGENTE LINK-ERHALTUNG
        public string ProcessMentionsPreservingExisting(string newText, string existingProcessedText)
        {
            if (string.IsNullOrEmpty(newText)) return newText;

            // SCHRITT 1: Neue Beschreibung verarbeiten
            var newProcessed = ProcessMentions(newText);

            // SCHRITT 2: Wenn keine alte Beschreibung existiert
            if (string.IsNullOrEmpty(existingProcessedText))
                return newProcessed;

            // SCHRITT 3: Bestehende Links erhalten
            return PreserveExistingLinks(newProcessed, existingProcessedText);
        }

        private string PreserveExistingLinks(string newProcessedText, string existingProcessedText)
        {
            // Extrahiere alle bestehenden Links
            var existingLinks = ExtractExistingLinks(existingProcessedText);
            var result = newProcessedText;

            foreach (var existingLink in existingLinks.Values)
            {
                // Wenn die plain mention im neuen Text existiert, aber noch nicht verlinkt ist
                if (result.Contains(existingLink.PlainMention) && !result.Contains(existingLink.LinkHtml))
                {
                    result = result.Replace(existingLink.PlainMention, existingLink.LinkHtml);
                }
            }

            return result;
        }

        private Dictionary<string, EntityLink> ExtractExistingLinks(string processedText)
        {
            var links = new Dictionary<string, EntityLink>();

            var patterns = new Dictionary<string, string>
            {
                { "character", @"<a[^>]*class='[^']*character-mention[^']*'[^>]*>👤@([^<]+)</a>" },
                { "guild", @"<a[^>]*class='[^']*guild-mention[^']*'[^>]*>🏰#([^<]+)</a>" },
                { "infanterie", @"<a[^>]*class='[^']*infanterie-mention[^']*'[^>]*>⚔️§([^<]+)</a>" },
                { "monster", @"<a[^>]*class='[^']*monster-mention[^']*'[^>]*>👹&([^<]+)</a>" },
                { "magicclass", @"<a[^>]*class='[^']*magicclass-mention[^']*'[^>]*>🔮%([^<]+)</a>" }
            };

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(processedText, pattern.Value);
                foreach (Match match in matches)
                {
                    var entityName = match.Groups[1].Value;
                    var linkHtml = match.Value;
                    var symbol = GetSymbolForType(pattern.Key);

                    var key = $"{pattern.Key}_{entityName}";
                    if (!links.ContainsKey(key))
                    {
                        links[key] = new EntityLink
                        {
                            EntityName = entityName,
                            LinkHtml = linkHtml,
                            PlainMention = $"{symbol}{entityName}",
                            EntityType = pattern.Key
                        };
                    }
                }
            }

            return links;
        }

        private string GetSymbolForType(string entityType)
        {
            return entityType switch
            {
                "character" => "@",
                "guild" => "#",
                "infanterie" => "§",
                "monster" => "&",
                "magicclass" => "%",
                _ => "@"
            };
        }

        // BESTEHENDE GetPlainText Methode bleibt unverändert
        public string GetPlainText(string processedText)
        {
            if (string.IsNullOrEmpty(processedText)) return processedText;

            var patterns = new[]
            {
                @"<a[^>]*class='[^']*character-mention[^']*'[^>]*>👤@([^<]+)</a>",
                @"<a[^>]*class='[^']*guild-mention[^']*'[^>]*>🏰#([^<]+)</a>",
                @"<a[^>]*class='[^']*infanterie-mention[^']*'[^>]*>⚔️§([^<]+)</a>",
                @"<a[^>]*class='[^']*monster-mention[^']*'[^>]*>👹&([^<]+)</a>",
                @"<a[^>]*class='[^']*magicclass-mention[^']*'[^>]*>🔮%([^<]+)</a>"
            };

            var symbols = new[] { "@", "#", "§", "&", "%" };

            for (int i = 0; i < patterns.Length; i++)
            {
                processedText = Regex.Replace(processedText, patterns[i], $"{symbols[i]}$1");
            }

            return processedText;
        }

        // Hilfklassen
        private class MentionMatch
        {
            public int StartIndex { get; set; }
            public int Length { get; set; }
            public char Symbol { get; set; }
            public string EntityName { get; set; } = string.Empty;
            public string FullMatch { get; set; } = string.Empty;
        }

        private class EntityLink
        {
            public string EntityName { get; set; } = string.Empty;
            public string LinkHtml { get; set; } = string.Empty;
            public string PlainMention { get; set; } = string.Empty;
            public string EntityType { get; set; } = string.Empty;
        }
    }
}