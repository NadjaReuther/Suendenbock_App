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
    }

    public class MentionProcessorService : IMentionProcessorService
    {
        private readonly ApplicationDbContext _context;

        public MentionProcessorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string ProcessMentions(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Verschiedene Mention-Patterns definieren
            var patterns = new Dictionary<string, Func<string, string>>
            {
                { @"@([A-Za-zÄÖÜäöüß\s]+)", ProcessCharacterMention },
                { @"#([A-Za-zÄÖÜäöüß\s]+)", ProcessGuildMention },
                { @"§([A-Za-zÄÖÜäöüß\s\d]+)", ProcessInfanterieMention },
                { @"&([A-Za-zÄÖÜäöüß\s]+)", ProcessMonsterMention },
                { @"%([A-Za-zÄÖÜäöüß\s]+)", ProcessMagicClassMention }
            };

            foreach (var pattern in patterns)
            {
                text = Regex.Replace(text, pattern.Key, match =>
                {
                    var entityName = match.Groups[1].Value.Trim();
                    return pattern.Value(entityName) ?? match.Value;
                });
            }

            return text;
        }

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

            // LÖSUNG: EF.Functions.Like für case-insensitive Suche
            return _context.Characters
                .FirstOrDefault(c => EF.Functions.Like(c.Vorname, firstName) &&
                               EF.Functions.Like(c.Nachname, lastName));
        }

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
    }
}