using System.Collections.Generic;

namespace DataLayer.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } //Collection of users
    }
}
