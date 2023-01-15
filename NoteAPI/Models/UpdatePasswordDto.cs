using System.ComponentModel.DataAnnotations;

namespace NoteAPI.Models
{
    public class UpdatePasswordDto
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}