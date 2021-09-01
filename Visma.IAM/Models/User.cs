using System.Collections.Generic;

namespace Visma.IAM.Models
{
    public class User
    {        
        public string Name { get; set; }

        public string Email { get; set; }

        public string Department { get; set; }

        public string Password { get; set; } // should be a hash instead of plain password

        public List<IRole> AssignedRoles { get; set; } = new();
    }
}
