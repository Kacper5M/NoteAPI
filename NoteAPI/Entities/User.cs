using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoteAPI.Entities
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public virtual List<Note> Notes { get; set; }

        internal static bool FindFirstValue(string nameIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
