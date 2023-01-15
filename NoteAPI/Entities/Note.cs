using System;
using System.ComponentModel.DataAnnotations;

namespace NoteAPI.Entities
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string NoteTitle { get; set; }
        public string NoteBody { get; set; }
        public bool IsImportant { get; set; }
        public int Color { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
