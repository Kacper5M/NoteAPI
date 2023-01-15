using System;

namespace NoteAPI.Models
{
    public class NoteDto
    {
        public int Id { get; set; }
        public string NoteTitle { get; set; }
        public string NoteBody { get; set; }
        public int Color { get; set; }
        public bool IsImportant { get; set; } 
        public DateTime CreatedDate { get; set; }
    }
}