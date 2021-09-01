using System.Collections.Generic;

namespace Visma.IAM.Models
{
    public class Role : EmployeeRole
    {
        public Role(string name, List<Permission> permissions = null) : 
            base(name, permissions)
        {
        }
    }
}
