using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Suendenbock_App.Models.ViewModels
{
    // Für die Threadliste (Forum-Übersichtsseite)
    public class ForumPageViewModel
    {
        public List<ForumThreadItemViewModel> Threads { get; set; } = new();
        public List<ForumCategoryViewModel> Categories { get; set; } = new();
        public string? AktiveKategorie { get; set; }
        public string? Suche { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class ForumThreadItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorUserId { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public int ReplyCount { get; set; }
        public bool IsPinned { get; set; }
        public bool CanDelete { get; set; }
    }

    public class ForumCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int ThreadCount { get; set; }
    }

    // Für die Detailseite eines Threads
    public class ThreadDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorUserId { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public bool IsPinned { get; set; }
        public bool CanDelete { get; set; }
        public List<ReplyViewModel> Replies { get; set; } = new();
    }

    public class ReplyViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorUserId { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public bool CanDelete { get; set; }
    }

    // Für das "Neues Thema"-Formular
    public class CreateThreadViewModel
    {
        [Required(ErrorMessage = "Titel ist erforderlich")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Inhalt ist erforderlich")]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        // Für das Dropdown im Formular
        public List<ForumCategoryViewModel> Categories { get; set; } = new();
    }
}