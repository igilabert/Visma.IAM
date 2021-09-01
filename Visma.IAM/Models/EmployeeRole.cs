using System.Collections.Generic;
using System.Linq;

namespace Visma.IAM.Models
{
    public class EmployeeRole : IRole
    {
        public string Name { get; set; }

        public List<Permission> Permissions { get; set; } = new() { Permission.Read };

        public EmployeeRole() : this("Employee")
        {
        }

        private EmployeeRole(string name)
        {
            Name = name;            
        }

        protected EmployeeRole(string name, List<Permission> permissions)
        {
            Name = name;
            Permissions = GetGivenOrDefaultPermissions(permissions);
        }

        public List<Permission> GetGivenOrDefaultPermissions(List<Permission> permissions)
        {
            return permissions == null
                ? Permissions :
                permissions.Any() ? permissions : Permissions;
        }
    }
}
