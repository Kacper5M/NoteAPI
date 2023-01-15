using System;
using System.ComponentModel.DataAnnotations;

namespace NoteAPI.Models
{
    public class CreateNoteDto
    {
        [MaxLength(50)]
        public string NoteTitle { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string NoteBody { get; set; }
        public bool IsImportant { get; set; } = false;
        public int Color { get; set; } = 1;
    }
}
