﻿using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<MagicClass> MagicClasses { get; set; } = new List<MagicClass>();
        public List<Guild> Guilds { get; set; } = new List<Guild>();
        public List<Character> recentCharacters { get; set; } = new List<Character>();
        public List<Character> Characters { get; set; } = new List<Character>();
        public Dictionary<string, int> MagicClassStats { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> GenderStats { get; set; } = new Dictionary<string, int>();
    }
}
