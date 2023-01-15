using System;
using System.ComponentModel.DataAnnotations;

namespace NoteAPI.Models
{
    public class UpdateNoteDto
    {
        [MaxLength(50)]
        public string NoteTitle { get; set; }
        [MaxLength(500)]
        public string NoteBody { get; set; }
        public int Color { get; set; }
    }
}