using System;
using System.ComponentModel.DataAnnotations;

namespace NoteAPI.Models
{
    public class UpdateUserDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [MaxLength(15)]
        public string FirstName { get; set; }
        [MaxLength(15)]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
