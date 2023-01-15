using NoteAPI.Entities;

namespace NoteAPI
{
    public class RoleSeeder
    {
        private readonly DatabaseContext _dbContext;

        public RoleSeeder(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Seed()
        {
            if(_dbContext.Database.CanConnect())
            {
                if(!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
            }    
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>
            {
                new Role()
                {
                    Name = "User"
                },
                new Role()
                {
                    Name = "Admin"
                }
            };

            return roles;
        }
    }
}