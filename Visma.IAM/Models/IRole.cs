using System.Collections.Generic;

namespace Visma.IAM.Models
{    
    public interface IRole
    {
        public string Name { get; set; }

        public List<Permission> Permissions { get; set; }
    }
}
